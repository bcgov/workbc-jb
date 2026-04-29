<?php

declare(strict_types=1);

namespace WorkBC\FederalJobImporter\Service;

use Monolog\Logger;
use PDO;
use Throwable;
use WorkBC\FederalJobImporter\Api\FederalApiClient;
use WorkBC\FederalJobImporter\Config\AppConfig;
use WorkBC\FederalJobImporter\Xml\XmlJobMapper;

/**
 * Pipeline orchestrator. Mirrors the flow of the legacy C# entry point
 * (`Program.cs` + `XmlImportService` + `JobsTableSyncService` +
 * `JobsTableSyncServiceBase`):
 *
 *   1. Fetch the BC job index from the Federal feed.
 *   2. Insert new / update changed rows in "ImportedJobsFederal".
 *   3. Purge expired (DisplayUntil < now) and feed-vanished rows.
 *   4. Sync new staging rows into "Jobs" + "JobVersions".
 *   5. Refresh existing "Jobs" rows from staging; bump JobVersion when
 *      NOC / location / positions / DatePosted / IsActive change.
 *   6. Deactivate "Jobs" rows whose staging row no longer exists.
 *
 * The downstream WorkBC.Indexers.Federal still reads from
 * "ImportedJobsFederal" — this service is a drop-in replacement for the
 * legacy Stage-1 importer.
 */
final class JobImportService
{
    /** Matches WorkBC.Shared.Constants.JobSource.Federal. */
    private const JOB_SOURCE_FEDERAL = 1;
    /** Never purge more than this many rows in a single run (matches legacy 1000). */
    private const MAX_PURGE_PER_RUN = 1000;

    /** Flush the in-memory progress buffer through Monolog every N chars. */
    private const PROGRESS_FLUSH_CHARS = 500;

    private PDO $db;
    private FederalApiClient $api;
    private AppConfig $cfg;
    private Logger $log;
    private XmlJobMapper $mapper;

    private int $errorCount = 0;
    private int $insertedStaging = 0;
    private int $updatedStaging = 0;
    private int $skippedStaging = 0;
    private int $alreadyExpired = 0;
    private int $jobsImported = 0;
    private int $jobsUpdated = 0;
    private int $jobsDeactivated = 0;

    /** Accumulator for per-job progress chars; flushed via Monolog. */
    private string $progressBuf = '';

    public function __construct(PDO $db, FederalApiClient $api, AppConfig $cfg, Logger $logger)
    {
        $this->db = $db;
        $this->api = $api;
        $this->cfg = $cfg;
        $this->log = $logger;
        $this->mapper = new XmlJobMapper($db, $logger);
    }

    public function run(): void
    {
        $startedAt = microtime(true);
        $mode = $this->cfg->reImport ? 'reimport' : 'normal';
        $this->log->info("==== IMPORTER STARTED (mode={$mode}, maxJobs={$this->cfg->effectiveMaxJobs}) ====");

        if (!$this->cfg->reImport) {
            $this->log->info('-- Stage 1: fetching federal job index --');
            $index = $this->api->fetchJobIndex();
            if (!empty($index)) {
                $this->log->info('-- Stage 2: syncing staging table from feed --');
                $this->processStagingFromFeed($index);
            } else {
                $this->log->warning('Federal feed returned no jobs — skipping staging updates');
            }
        } else {
            $this->log->info('-- Stage 1+2 skipped (reimport in progress) --');
        }

        if ($this->cfg->reImport) {
            $this->log->info('-- Stage 3 skipped (reimport in progress) --');
        } else {
            $this->log->info('-- Stage 3: importing new rows into Jobs table --');
            $this->importNewJobs();
        }

        $this->log->info('-- Stage 4: updating existing rows in Jobs table --');
        $this->updateExistingJobs();

        $this->log->info('-- Stage 5: deactivating orphaned rows in Jobs table --');
        $this->deactivateOrphanedJobs();

        $active = (int) $this->db->query(
            'SELECT COUNT(*) FROM "Jobs" WHERE "JobSourceId" = ' . self::JOB_SOURCE_FEDERAL . ' AND "IsActive" = TRUE'
        )->fetchColumn();
        $this->log->info("{$active} active federal jobs in the Jobs table after import");

        $elapsed = number_format(microtime(true) - $startedAt, 1);
        $this->log->info(
            "==== IMPORTER FINISHED in {$elapsed}s — "
            . "stagingInserted={$this->insertedStaging} stagingUpdated={$this->updatedStaging} "
            . "alreadyExpired={$this->alreadyExpired} errors={$this->errorCount} "
            . "jobsInserted={$this->jobsImported} jobsUpdated={$this->jobsUpdated} "
            . "jobsDeactivated={$this->jobsDeactivated} ===="
        );
    }

    /**
     * Append a progress char and flush through the logger periodically.
     * Replaces the legacy `echo` calls so kubectl logs gets timestamped lines.
     */
    private function progress(string $marker): void
    {
        $this->progressBuf .= $marker;
        if (strlen($this->progressBuf) >= self::PROGRESS_FLUSH_CHARS) {
            $this->flushProgress();
        }
    }

    private function flushProgress(): void
    {
        if ($this->progressBuf === '') {
            return;
        }
        $this->log->info('progress: ' . $this->progressBuf);
        $this->progressBuf = '';
    }

    // ── Stage 1 — feed → "ImportedJobsFederal" ─────────────────────

    /**
     * @param array<int, array{id:string, fileUpdateDate:string}> $feed
     */
    private function processStagingFromFeed(array $feed): void
    {
        $existing = $this->loadStagingDates();

        $feedById = [];
        foreach ($feed as $row) {
            $feedById[$row['id']] = $row['fileUpdateDate'];
        }

        $toInsert = [];
        $toUpdate = [];
        foreach ($feedById as $id => $updated) {
            // PHP auto-coerces numeric string array keys to int; cast back to
            // string so downstream APIs (FederalApiClient::fetchJobXml takes
            // `string`, PDO param binds as text) receive the value the legacy
            // C# importer threaded through as System.String.
            $id = (string) $id;
            if (!isset($existing[$id])) {
                $toInsert[] = ['id' => $id, 'fileUpdateDate' => $updated];
                continue;
            }
            if (!$this->datesEqual($existing[$id], $updated)) {
                $toUpdate[] = ['id' => $id, 'fileUpdateDate' => $updated];
            }
        }

        $this->log->info(count($toInsert) . ' jobs found to insert into the ImportedJobsFederal table');
        if ($this->cfg->effectiveMaxJobs < count($toInsert)) {
            $this->log->warning("MaxJobs is {$this->cfg->effectiveMaxJobs}.");
        }

        try {
            $this->insertStagingRows($toInsert);
        } catch (Throwable $e) {
            $this->log->error('ERROR OCCURRED IMPORTING JOBS... SKIPPING AHEAD: ' . $e->getMessage());
        }

        $this->log->info(count($toUpdate) . ' jobs found to update in the ImportedJobsFederal table');
        if ($this->cfg->effectiveMaxJobs < count($toUpdate)) {
            $this->log->warning("MaxJobs is {$this->cfg->effectiveMaxJobs}.");
        }

        try {
            $this->updateStagingRows($toUpdate);
        } catch (Throwable $e) {
            $this->log->error('ERROR OCCURRED UPDATING JOBS... SKIPPING AHEAD: ' . $e->getMessage());
        }

        $this->purgeExpiredStaging();
        $this->purgeMissingStaging($feedById);

        $count = (int) $this->db->query('SELECT COUNT(*) FROM "ImportedJobsFederal"')->fetchColumn();
        $this->log->info("{$count} jobs in ImportedJobsFederal after import");
    }

    /** @return array<string, string>  jobId → ApiDate (stringified) */
    private function loadStagingDates(): array
    {
        $rows = $this->db->query('SELECT "JobId", "ApiDate" FROM "ImportedJobsFederal"')->fetchAll();
        $result = [];
        foreach ($rows as $row) {
            $result[(string) $row['JobId']] = (string) $row['ApiDate'];
        }
        return $result;
    }

    /**
     * @param array<int, array{id:string, fileUpdateDate:string}> $jobs
     */
    private function insertStagingRows(array $jobs): void
    {
        if (empty($jobs)) {
            return;
        }

        $this->log->info('Progress legend: I=inserted  X=already-expired  P=xml-problem  E=error');

        $jobIdCheck = $this->db->prepare('SELECT "DateFirstImported" FROM "JobIds" WHERE "Id" = ?');
        $jobIdInsert = $this->db->prepare(
            'INSERT INTO "JobIds" ("Id", "DateFirstImported", "JobSourceId") VALUES (?, ?, ?)'
        );
        $stagingInsert = $this->db->prepare(
            'INSERT INTO "ImportedJobsFederal"
                ("JobId", "JobPostEnglish", "JobPostFrench", "DateFirstImported",
                 "DateLastImported", "ApiDate", "ReIndexNeeded", "DisplayUntil")
             VALUES (?, ?, ?, ?, ?, ?, TRUE, ?)'
        );

        $cap = min(count($jobs), $this->cfg->effectiveMaxJobs);
        for ($i = 0; $i < $cap; $i++) {
            if ($this->errorCount >= $this->cfg->maxErrors) {
                break;
            }

            $job = $jobs[$i];
            try {
                $xml = $this->api->fetchJobXml($job['id']);
                if ($xml['english'] === null) {
                    $this->log->warning("No English XML returned for job {$job['id']} — skipping");
                    $this->progress('P');
                    continue;
                }

                $displayUntil = $this->api->extractDisplayUntil($xml['english']);
                $now = date('Y-m-d H:i:s');

                $jobIdCheck->execute([$job['id']]);
                $existingDate = $jobIdCheck->fetchColumn();
                if ($existingDate === false) {
                    $jobIdInsert->execute([$job['id'], $now, self::JOB_SOURCE_FEDERAL]);
                    $existingDate = $now;
                }

                if ($displayUntil === null || strtotime($displayUntil) === false || strtotime($displayUntil) <= time()) {
                    $this->progress('X');
                    $this->alreadyExpired++;
                    continue;
                }

                $stagingInsert->execute([
                    $job['id'],
                    $xml['english'],
                    $xml['french'] ?? '',
                    (string) $existingDate,
                    $now,
                    $job['fileUpdateDate'],
                    $displayUntil,
                ]);

                $this->progress('I');
                $this->insertedStaging++;
            } catch (Throwable $e) {
                $this->errorCount++;
                $this->progress('E');
                $this->log->warning("Insert failed for job {$job['id']}: " . $e->getMessage());
                if ($this->errorCount === $this->cfg->maxErrors) {
                    $this->log->error("MAX_ERRORS_{$this->cfg->maxErrors}_EXCEEDED — aborting insert phase");
                }
            }
        }

        $this->flushProgress();
        $skipped = max(0, $cap - $this->insertedStaging - $this->alreadyExpired);
        $this->log->info("{$this->insertedStaging} new jobs inserted into ImportedJobsFederal");
        $this->log->info("{$this->alreadyExpired} jobs were already expired");
        $this->log->info("{$skipped} other jobs were skipped");
    }

    /**
     * @param array<int, array{id:string, fileUpdateDate:string}> $jobs
     */
    private function updateStagingRows(array $jobs): void
    {
        if (empty($jobs)) {
            return;
        }

        $update = $this->db->prepare(
            'UPDATE "ImportedJobsFederal" SET
                "JobPostEnglish" = ?, "JobPostFrench" = ?,
                "ApiDate" = ?, "DateLastImported" = ?,
                "ReIndexNeeded" = TRUE, "DisplayUntil" = ?
             WHERE "JobId" = ?'
        );

        $cap = min(count($jobs), $this->cfg->effectiveMaxJobs);
        for ($i = 0; $i < $cap; $i++) {
            if ($this->errorCount >= $this->cfg->maxErrors) {
                break;
            }

            $job = $jobs[$i];
            try {
                $xml = $this->api->fetchJobXml($job['id']);
                if ($xml['english'] === null) {
                    $this->log->warning("No English XML returned for job {$job['id']} — skipping update");
                    $this->progress('P');
                    continue;
                }

                $displayUntil = $this->api->extractDisplayUntil($xml['english']);
                $now = date('Y-m-d H:i:s');

                $update->execute([
                    $xml['english'],
                    $xml['french'] ?? '',
                    $job['fileUpdateDate'],
                    $now,
                    $displayUntil,
                    $job['id'],
                ]);

                $this->progress('U');
                $this->updatedStaging++;
            } catch (Throwable $e) {
                $this->errorCount++;
                $this->progress('E');
                $this->log->warning("Update failed for job {$job['id']}: " . $e->getMessage());
                if ($this->errorCount === $this->cfg->maxErrors) {
                    $this->log->error("MAX_ERRORS_{$this->cfg->maxErrors}_EXCEEDED — aborting update phase");
                }
            }
        }

        $this->flushProgress();
        $this->log->info("{$this->updatedStaging} jobs updated in ImportedJobsFederal");
    }

    private function purgeExpiredStaging(): void
    {
        // Mirrors C# XmlImportService.PurgeJobs: load all expired rows, cap
        // at 1000 per pass. C# does not order — neither do we.
        $rows = $this->db->query(
            'SELECT "JobId" FROM "ImportedJobsFederal" WHERE "DisplayUntil" < NOW()
             LIMIT ' . self::MAX_PURGE_PER_RUN
        )->fetchAll(PDO::FETCH_COLUMN);

        $this->log->info(count($rows) . ' expired jobs found to purge from the ImportedJobsFederal table');
        $this->purgeAndArchive($rows);
    }

    /** @param array<string, string> $feedById */
    private function purgeMissingStaging(array $feedById): void
    {
        $stagingIds = $this->db->query('SELECT "JobId" FROM "ImportedJobsFederal"')
            ->fetchAll(PDO::FETCH_COLUMN);

        $missing = [];
        foreach ($stagingIds as $jobId) {
            if (!isset($feedById[(string) $jobId])) {
                $missing[] = (string) $jobId;
            }
            if (count($missing) >= self::MAX_PURGE_PER_RUN) {
                break;
            }
        }

        $this->log->info('Purging ' . count($missing) . ' jobs from the ImportedJobsFederal table because they are gone from the Federal JobBank');
        $this->purgeAndArchive($missing);
    }

    /** @param array<int, string|int> $jobIds */
    private function purgeAndArchive(array $jobIds): void
    {
        if (empty($jobIds)) {
            return;
        }

        $expCheck = $this->db->prepare('SELECT 1 FROM "ExpiredJobs" WHERE "JobId" = ? LIMIT 1');
        $expInsert = $this->db->prepare(
            'INSERT INTO "ExpiredJobs" ("JobId", "DateRemoved", "RemovedFromElasticsearch")
             VALUES (?, NOW(), FALSE)'
        );
        $expUpdate = $this->db->prepare(
            'UPDATE "ExpiredJobs" SET "DateRemoved" = NOW(), "RemovedFromElasticsearch" = FALSE
             WHERE "JobId" = ?'
        );
        $stagingDelete = $this->db->prepare('DELETE FROM "ImportedJobsFederal" WHERE "JobId" = ?');

        $purged = 0;
        $duplicates = 0;
        foreach ($jobIds as $jobId) {
            $jobId = (string) $jobId;
            try {
                $expCheck->execute([$jobId]);
                if ($expCheck->fetchColumn()) {
                    $expUpdate->execute([$jobId]);
                    $duplicates++;
                    $this->progress('d');
                } else {
                    $expInsert->execute([$jobId]);
                    $this->progress('D');
                }
                $stagingDelete->execute([$jobId]);
                $purged++;
            } catch (Throwable $e) {
                $this->progress('E');
                $this->log->warning("Could not purge job {$jobId}: " . $e->getMessage());
            }
        }
        $this->flushProgress();
        $this->log->info("Purged {$purged} rows from ImportedJobsFederal ({$duplicates} re-archived)");
    }

    // ── Stage 2 — "ImportedJobsFederal" → "Jobs" + "JobVersions" ───

    private function importNewJobs(): void
    {
        // Mirrors C# JobsTableSyncService.ImportJobs (Federal): no DeletedJobs
        // filter — federal import path does not reference that table.
        $rows = $this->db->query('
            SELECT ij."JobId", ij."JobPostEnglish",
                   ij."DateFirstImported", ij."DateLastImported"
            FROM "ImportedJobsFederal" ij
            WHERE ij."DisplayUntil" > NOW()
              AND NOT EXISTS (SELECT 1 FROM "Jobs" j WHERE j."JobId" = ij."JobId")
        ')->fetchAll();

        $this->log->info(count($rows) . ' jobs found to import');

        $insertJob = $this->db->prepare('
            INSERT INTO "Jobs" (
                "JobId", "Title", "City", "EmployerName",
                "NocCodeId", "NocCodeId2021", "IndustryId",
                "Salary", "SalarySummary", "PositionsAvailable",
                "DatePosted", "ActualDatePosted", "ExpireDate",
                "LastUpdated", "DateFirstImported", "DateLastImported",
                "IsActive", "FullTime", "PartTime", "LeadingToFullTime",
                "Permanent", "Temporary", "Casual", "Seasonal",
                "JobSourceId", "OriginalSource", "ExternalSourceUrl", "LocationId"
            ) VALUES (
                ?, ?, ?, ?,
                ?, ?, ?,
                ?, ?, ?,
                ?, ?, ?,
                NOW(), ?, ?,
                TRUE, ?, ?, ?,
                ?, ?, ?, ?,
                ?, ?, ?, ?
            )
        ');

        $insertVersion = $this->db->prepare('
            INSERT INTO "JobVersions" (
                "JobId", "DateVersionStart", "DatePosted", "ActualDatePosted",
                "DateFirstImported", "JobSourceId", "IndustryId", "NocCodeId",
                "NocCodeId2021", "IsActive", "PositionsAvailable", "LocationId",
                "IsCurrentVersion", "VersionNumber"
            ) VALUES (
                ?, ?, ?, ?,
                ?, ?, ?, ?,
                ?, TRUE, ?, ?,
                TRUE, 1
            )
        ');

        $imported = 0;
        $skippedUnmapped = 0;
        foreach ($rows as $row) {
            $mapped = $this->mapper->map((string) $row['JobPostEnglish']);
            if ($mapped === null) {
                $skippedUnmapped++;
                $this->progress('N');
                continue;
            }

            try {
                $this->db->beginTransaction();
                $insertJob->execute([
                    $row['JobId'],
                    $mapped['title'],
                    $mapped['city'],
                    $mapped['employerName'],
                    $mapped['nocCodeId'],
                    $mapped['nocCodeId2021'],
                    $mapped['industryId'],
                    $mapped['salary'],
                    $mapped['salarySummary'],
                    $mapped['positionsAvailable'],
                    $mapped['datePosted'],
                    $mapped['actualDatePosted'],
                    $mapped['expireDate'],
                    $row['DateFirstImported'],
                    $row['DateLastImported'],
                    $mapped['fullTime'] ? 'true' : 'false',
                    $mapped['partTime'] ? 'true' : 'false',
                    $mapped['leadingToFullTime'] ? 'true' : 'false',
                    $mapped['permanent'] ? 'true' : 'false',
                    $mapped['temporary'] ? 'true' : 'false',
                    $mapped['casual'] ? 'true' : 'false',
                    $mapped['seasonal'] ? 'true' : 'false',
                    self::JOB_SOURCE_FEDERAL,
                    'Federal Job Bank',
                    '',
                    $mapped['locationId'],
                ]);

                $version1Start = $this->getVersion1StartDate($mapped['datePosted'], (string) $row['DateFirstImported']);
                $insertVersion->execute([
                    $row['JobId'],
                    $version1Start,
                    $mapped['datePosted'],
                    $mapped['actualDatePosted'],
                    $version1Start,
                    self::JOB_SOURCE_FEDERAL,
                    $mapped['industryId'],
                    $mapped['nocCodeId'],
                    $mapped['nocCodeId2021'],
                    $mapped['positionsAvailable'],
                    $mapped['locationId'],
                ]);

                $this->db->commit();
                $this->progress('I');
                $imported++;
            } catch (Throwable $e) {
                if ($this->db->inTransaction()) {
                    $this->db->rollBack();
                }
                $this->progress('E');
                $this->log->warning("Failed to import job {$row['JobId']}: " . $e->getMessage());
            }
        }

        $this->flushProgress();
        $this->jobsImported = $imported;
        $this->log->info(
            "Imported {$imported} new rows into Jobs"
            . ($skippedUnmapped > 0 ? " ({$skippedUnmapped} unmappable XML rows skipped)" : '')
        );
    }

    private function updateExistingJobs(): void
    {
        // Mirrors C# JobsTableSyncService.UpdateJobs (Federal): with --reimport
        // every joined row is selected; otherwise only those whose
        // DateLastImported drifted or that are inactive.
        $where = $this->cfg->reImport
            ? '1 = 1'
            : '(j."DateLastImported" <> ij."DateLastImported" OR j."IsActive" = FALSE)';

        $rows = $this->db->query('
            SELECT ij."JobId", ij."JobPostEnglish", ij."DateLastImported",
                   j."NocCodeId", j."NocCodeId2021", j."IndustryId",
                   j."LocationId", j."PositionsAvailable", j."DatePosted",
                   j."ActualDatePosted", j."IsActive"
            FROM "ImportedJobsFederal" ij
            INNER JOIN "Jobs" j ON j."JobId" = ij."JobId"
            WHERE ' . $where . '
        ')->fetchAll();

        $this->log->info(count($rows) . ' jobs found to update');

        // C# UpdateJobs sets:
        //   job.LastUpdated = elasticJob.LastUpdated ?? importedJob.DateLastImported
        // where elasticJob.LastUpdated comes from <file_update_date>. We bind it
        // explicitly rather than `LastUpdated = NOW()` to keep the timestamp the
        // indexer/reports already see consistent.
        $update = $this->db->prepare('
            UPDATE "Jobs" SET
                "Title" = ?, "City" = ?, "EmployerName" = ?,
                "NocCodeId" = ?, "NocCodeId2021" = ?, "IndustryId" = ?,
                "Salary" = ?, "SalarySummary" = ?, "PositionsAvailable" = ?,
                "DatePosted" = ?, "ActualDatePosted" = ?, "ExpireDate" = ?,
                "LastUpdated" = ?, "DateLastImported" = ?, "IsActive" = TRUE,
                "FullTime" = ?, "PartTime" = ?, "LeadingToFullTime" = ?,
                "Permanent" = ?, "Temporary" = ?, "Casual" = ?, "Seasonal" = ?,
                "LocationId" = ?
            WHERE "JobId" = ?
        ');

        $updated = 0;
        foreach ($rows as $row) {
            $mapped = $this->mapper->map((string) $row['JobPostEnglish']);
            if ($mapped === null) {
                continue;
            }

            $needsNewVersion = $this->jobChanged($row, $mapped);
            $lastUpdated = $mapped['lastUpdated'] ?? (string) $row['DateLastImported'];

            try {
                $this->db->beginTransaction();
                $update->execute([
                    $mapped['title'],
                    $mapped['city'],
                    $mapped['employerName'],
                    $mapped['nocCodeId'],
                    $mapped['nocCodeId2021'],
                    $mapped['industryId'],
                    $mapped['salary'],
                    $mapped['salarySummary'],
                    $mapped['positionsAvailable'],
                    $mapped['datePosted'],
                    $mapped['actualDatePosted'],
                    $mapped['expireDate'],
                    $lastUpdated,
                    $row['DateLastImported'],
                    $mapped['fullTime'] ? 'true' : 'false',
                    $mapped['partTime'] ? 'true' : 'false',
                    $mapped['leadingToFullTime'] ? 'true' : 'false',
                    $mapped['permanent'] ? 'true' : 'false',
                    $mapped['temporary'] ? 'true' : 'false',
                    $mapped['casual'] ? 'true' : 'false',
                    $mapped['seasonal'] ? 'true' : 'false',
                    $mapped['locationId'],
                    $row['JobId'],
                ]);

                if ($needsNewVersion) {
                    $this->incrementJobVersion((string) $row['JobId'], $mapped);
                }

                $this->db->commit();
                $this->progress('U');
                $updated++;
            } catch (Throwable $e) {
                if ($this->db->inTransaction()) {
                    $this->db->rollBack();
                }
                $this->progress('E');
                $this->log->warning("Failed to update job {$row['JobId']}: " . $e->getMessage());
            }
        }

        $this->flushProgress();
        $this->jobsUpdated = $updated;
        $this->log->info("Updated {$updated} rows in Jobs");
    }

    private function deactivateOrphanedJobs(): void
    {
        $rows = $this->db->query('
            SELECT j."JobId" FROM "Jobs" j
            WHERE j."JobSourceId" = ' . self::JOB_SOURCE_FEDERAL . '
              AND j."IsActive" = TRUE
              AND NOT EXISTS (SELECT 1 FROM "ImportedJobsFederal" ij WHERE ij."JobId" = j."JobId")
        ')->fetchAll(PDO::FETCH_COLUMN);

        $this->log->info(count($rows) . ' jobs found to deactivate');

        $deactivate = $this->db->prepare(
            'UPDATE "Jobs" SET "IsActive" = FALSE, "LastUpdated" = NOW() WHERE "JobId" = ?'
        );

        $deactivated = 0;
        foreach ($rows as $jobId) {
            $jobId = (string) $jobId;
            try {
                $this->db->beginTransaction();
                $this->closeCurrentVersionAsInactive($jobId);
                $deactivate->execute([$jobId]);
                $this->db->commit();
                $this->progress('D');
                $deactivated++;
            } catch (Throwable $e) {
                if ($this->db->inTransaction()) {
                    $this->db->rollBack();
                }
                $this->progress('E');
                $this->log->warning("Failed to deactivate job {$jobId}: " . $e->getMessage());
            }
        }

        $this->flushProgress();
        $this->jobsDeactivated = $deactivated;
        $this->log->info("Deactivated {$deactivated} orphaned rows in Jobs");
    }

    /**
     * @param array<string,mixed> $row
     * @param array<string,mixed> $mapped
     */
    private function jobChanged(array $row, array $mapped): bool
    {
        return $this->intDiff($row['NocCodeId'], $mapped['nocCodeId'])
            || $this->intDiff($row['NocCodeId2021'], $mapped['nocCodeId2021'])
            || $this->intDiff($row['IndustryId'], $mapped['industryId'])
            || (int) $row['LocationId'] !== (int) $mapped['locationId']
            || (int) $row['PositionsAvailable'] !== (int) $mapped['positionsAvailable']
            || ! $this->datesEqual((string) $row['DatePosted'], (string) $mapped['datePosted'])
            || ! $this->datesEqual((string) $row['ActualDatePosted'], (string) $mapped['actualDatePosted'])
            || ! $this->boolish($row['IsActive']);
    }

    private function intDiff(mixed $a, mixed $b): bool
    {
        $aVal = $a === null ? null : (int) $a;
        $bVal = $b === null ? null : (int) $b;
        return $aVal !== $bVal;
    }

    private function boolish(mixed $value): bool
    {
        if (is_bool($value)) {
            return $value;
        }
        if (is_int($value)) {
            return $value !== 0;
        }
        return in_array(strtolower((string) $value), ['1', 't', 'true', 'yes', 'on'], true);
    }

    /** @param array<string,mixed> $mapped */
    private function incrementJobVersion(string $jobId, array $mapped): void
    {
        $now = date('Y-m-d H:i:s');

        $find = $this->db->prepare('
            SELECT "Id", "VersionNumber", "DateFirstImported", "JobSourceId",
                   "IndustryId", "NocCodeId", "NocCodeId2021"
            FROM "JobVersions"
            WHERE "JobId" = ? AND "IsCurrentVersion" = TRUE
            LIMIT 1
        ');
        $find->execute([$jobId]);
        $current = $find->fetch();

        if (!$current) {
            $fallback = $this->db->prepare('
                SELECT "Id", "VersionNumber", "DateFirstImported", "JobSourceId",
                       "IndustryId", "NocCodeId", "NocCodeId2021"
                FROM "JobVersions"
                WHERE "JobId" = ?
                ORDER BY "VersionNumber" DESC
                LIMIT 1
            ');
            $fallback->execute([$jobId]);
            $current = $fallback->fetch();
        }

        if ($current) {
            $close = $this->db->prepare(
                'UPDATE "JobVersions" SET "IsCurrentVersion" = FALSE, "DateVersionEnd" = ? WHERE "Id" = ?'
            );
            $close->execute([$now, $current['Id']]);

            // NOTE: matches the C# `JobsTableSyncServiceBase.IncrementJobVersion`
            // which leaves NocCodeId2021 unset on the new version when an
            // existing version is closed. Looks like a C# oversight (only the
            // brand-new-v1 branch sets it), but V2 mirrors the existing
            // production behavior so historical query results stay consistent.
            $insert = $this->db->prepare('
                INSERT INTO "JobVersions" (
                    "JobId", "DateVersionStart", "DatePosted", "ActualDatePosted",
                    "DateFirstImported", "JobSourceId", "IndustryId", "NocCodeId",
                    "NocCodeId2021", "IsActive", "PositionsAvailable", "LocationId",
                    "IsCurrentVersion", "VersionNumber"
                ) VALUES (
                    ?, ?, ?, ?,
                    ?, ?, ?, ?,
                    NULL, TRUE, ?, ?,
                    TRUE, ?
                )
            ');
            $insert->execute([
                $jobId,
                $now,
                $mapped['datePosted'],
                $mapped['actualDatePosted'],
                $current['DateFirstImported'],
                $current['JobSourceId'],
                $mapped['industryId'],
                $mapped['nocCodeId'],
                $mapped['positionsAvailable'],
                $mapped['locationId'],
                ((int) $current['VersionNumber']) + 1,
            ]);
            return;
        }

        // No prior version → create version 1 fresh
        $insert = $this->db->prepare('
            INSERT INTO "JobVersions" (
                "JobId", "DateVersionStart", "DatePosted", "ActualDatePosted",
                "DateFirstImported", "JobSourceId", "IndustryId", "NocCodeId",
                "NocCodeId2021", "IsActive", "PositionsAvailable", "LocationId",
                "IsCurrentVersion", "VersionNumber"
            ) VALUES (
                ?, ?, ?, ?,
                ?, ?, ?, ?,
                ?, TRUE, ?, ?,
                TRUE, 1
            )
        ');
        $insert->execute([
            $jobId,
            $now,
            $mapped['datePosted'],
            $mapped['actualDatePosted'],
            $now,
            self::JOB_SOURCE_FEDERAL,
            $mapped['industryId'],
            $mapped['nocCodeId'],
            $mapped['nocCodeId2021'],
            $mapped['positionsAvailable'],
            $mapped['locationId'],
        ]);
    }

    private function closeCurrentVersionAsInactive(string $jobId): void
    {
        $find = $this->db->prepare('
            SELECT "Id", "VersionNumber", "DatePosted", "ActualDatePosted",
                   "DateFirstImported", "JobSourceId", "IndustryId", "NocCodeId",
                   "PositionsAvailable", "LocationId"
            FROM "JobVersions"
            WHERE "JobId" = ? AND "IsCurrentVersion" = TRUE
            LIMIT 1
        ');
        $find->execute([$jobId]);
        $current = $find->fetch();
        if (!$current) {
            return;
        }

        $now = date('Y-m-d H:i:s');
        $close = $this->db->prepare(
            'UPDATE "JobVersions" SET "IsCurrentVersion" = FALSE, "DateVersionEnd" = ? WHERE "Id" = ?'
        );
        $close->execute([$now, $current['Id']]);

        // NOTE: matches C# `JobsTableSyncServiceBase.DeactivateJobs` which does
        // NOT carry NocCodeId2021 forward to the closing version (the field is
        // left null). Mirrors existing production behavior — see comment above.
        $insert = $this->db->prepare('
            INSERT INTO "JobVersions" (
                "JobId", "DateVersionStart", "DatePosted", "ActualDatePosted",
                "DateFirstImported", "JobSourceId", "IndustryId", "NocCodeId",
                "NocCodeId2021", "IsActive", "PositionsAvailable", "LocationId",
                "IsCurrentVersion", "VersionNumber"
            ) VALUES (
                ?, ?, ?, ?,
                ?, ?, ?, ?,
                NULL, FALSE, ?, ?,
                TRUE, ?
            )
        ');
        $insert->execute([
            $jobId,
            $now,
            $current['DatePosted'],
            $current['ActualDatePosted'],
            $current['DateFirstImported'],
            $current['JobSourceId'],
            $current['IndustryId'],
            $current['NocCodeId'],
            $current['PositionsAvailable'],
            $current['LocationId'],
            ((int) $current['VersionNumber']) + 1,
        ]);
    }

    /**
     * Returns min(DatePosted, DateImported) capped at 24 hours before
     * DateImported. Mirrors `JobsTableSyncServiceBase.GetVersion1StartDate`.
     */
    private function getVersion1StartDate(string $datePosted, string $dateImported): string
    {
        $posted = strtotime($datePosted);
        $imported = strtotime($dateImported);
        if ($posted === false || $imported === false) {
            return $dateImported ?: date('Y-m-d H:i:s');
        }
        if ($imported <= $posted) {
            return $dateImported;
        }
        $cap = $imported - 86400;
        return $posted < $cap ? date('Y-m-d H:i:s', $cap) : $datePosted;
    }

    private function datesEqual(string $a, string $b): bool
    {
        if ($a === $b) {
            return true;
        }
        $tsA = strtotime($a);
        $tsB = strtotime($b);
        return $tsA !== false && $tsB !== false && $tsA === $tsB;
    }
}
