<?php

declare(strict_types=1);

namespace WorkBC\JobImporter\Service;

use PDO;
use Monolog\Logger;
use WorkBC\JobImporter\Api\InnovibeApiClient;
use WorkBC\JobImporter\Config\AppConfig;

/**
 * Imports jobs from the Innovibe API into PostgreSQL.
 * Tables: "ImportedJobsWanted", "Jobs", "ExpiredJobs", "DeletedJobs", "JobIds"
 */
final class JobImportService
{
    private const JOB_SOURCE = 2;

    private int $fetched  = 0;
    private int $inserted = 0;
    private int $updated  = 0;
    private int $skipped  = 0;

    public function __construct(
        private readonly PDO              $db,
        private readonly InnovibeApiClient $api,
        private readonly AppConfig        $cfg,
        private readonly Logger           $log,
    ) {}

    public function run(): void
    {
        $this->log->info('IMPORTER STARTED');
        $this->log->info('Importing JSON data');
        $this->log->info('I = Inserted  U = Updated  S = Skipped  H = Duplicate hash');

        $seen = $this->importFromApi();
        $this->markSeen($seen);

        $this->log->info('Expiring jobs via API...');
        $this->expireJobsFromApi();

        $this->log->info('Importing to Jobs table...');
        $this->importNewJobs();

        $this->log->info('Updating Jobs table...');
        $this->updateExistingJobs();

        $this->log->info("IMPORTER FINISHED — fetched={$this->fetched} inserted={$this->inserted} updated={$this->updated} skipped={$this->skipped}");
    }

    // ── helpers ────────────────────────────────────────────────────

    private function computeHashId(string $json): int
    {
        return crc32($json);
    }

    // ── 1. API → "ImportedJobsWanted" ──────────────────────────────

    private function importFromApi(): array
    {
        $seen     = [];
        $progress = '';
        $pageNum  = 0;
        $pageJob  = 0;

        $chk     = $this->db->prepare('SELECT "ApiDate" FROM "ImportedJobsWanted" WHERE "JobId" = ?');
        $del     = $this->db->prepare('SELECT 1 FROM "DeletedJobs" WHERE "JobId" = ? LIMIT 1');
        $hashChk = $this->db->prepare('SELECT 1 FROM "ImportedJobsWanted" WHERE "HashId" = ? LIMIT 1');

        $upd = $this->db->prepare('
            UPDATE "ImportedJobsWanted" SET
                "JobPostEnglish" = ?, "ApiDate" = ?, "DateLastImported" = NOW(),
                "DateLastSeen" = NOW(), "ReIndexNeeded" = TRUE, "HashId" = ?
            WHERE "JobId" = ?
        ');

        $jidChk = $this->db->prepare('SELECT 1 FROM "JobIds" WHERE "Id" = ?');
        $jidIns = $this->db->prepare('INSERT INTO "JobIds" ("Id","DateFirstImported","JobSourceId") VALUES (?, NOW(), ?)');

        $ins = $this->db->prepare('
            INSERT INTO "ImportedJobsWanted"
                ("JobId","JobPostEnglish","DateFirstImported","DateLastImported","DateLastSeen","ApiDate","ReIndexNeeded","IsFederalOrWorkBc","HashId")
            VALUES (?, ?, NOW(), NOW(), NOW(), ?, TRUE, FALSE, ?)
        ');

        foreach ($this->api->fetchAllJobs($this->cfg->pageSize) as $job) {
            if ($pageJob % $this->cfg->pageSize === 0) {
                $pageNum++;
                if ($pageNum > 1) {
                    $progress .= "[pageindex={$pageNum}]";
                }
            }
            $pageJob++;
            $this->fetched++;

            $id = (string) $job['id'];
            if ($id === '') {
                continue;
            }

            // Skip jobs with no salary
            if (empty($job['salaryMin']) && empty($job['salaryMax']) && empty($job['salaryValue'])) {
                $this->skipped++;
                $progress .= 'S';
                continue;
            }

            $apiDate = date('Y-m-d H:i:s', strtotime($job['updatedAt'] ?? $job['createdAt']));
            $json    = json_encode($job, JSON_UNESCAPED_UNICODE | JSON_UNESCAPED_SLASHES);
            $hashId  = $this->computeHashId($json);

            // Skip deleted
            $del->execute([$id]);
            if ($del->fetchColumn()) {
                $this->skipped++;
                $progress .= 'S';
                continue;
            }

            // Duplicate hash check
            $hashChk->execute([$hashId]);
            if ($hashChk->fetchColumn()) {
                $chk->execute([$id]);
                $existingRow = $chk->fetch();
                if (!$existingRow) {
                    $this->skipped++;
                    $progress .= 'H';
                    $seen[] = $id;
                    continue;
                }
                $row = $existingRow;
            } else {
                $chk->execute([$id]);
                $row = $chk->fetch();
            }

            if ($row) {
                if (($row['ApiDate'] ?? '') !== $apiDate) {
                    $upd->execute([$json, $apiDate, $hashId, $id]);
                    $this->updated++;
                    $progress .= 'U';
                } else {
                    $this->skipped++;
                    $progress .= 'S';
                }
            } else {
                $jidChk->execute([$id]);
                if (!$jidChk->fetchColumn()) {
                    $jidIns->execute([$id, self::JOB_SOURCE]);
                }
                $ins->execute([$id, $json, $apiDate, $hashId]);
                $this->inserted++;
                $progress .= 'I';
            }

            $seen[] = $id;
        }

        $this->log->info("{$this->fetched} records returned by the API");
        if ($progress !== '') {
            $this->log->info($progress);
        }

        return $seen;
    }

    // ── 2. Mark seen ───────────────────────────────────────────────

    private function markSeen(array $ids): void
    {
        foreach (array_chunk($ids, 500) as $chunk) {
            $ph = implode(',', array_map(fn($id) => $this->db->quote($id), $chunk));
            $this->db->exec("UPDATE \"ImportedJobsWanted\" SET \"DateLastSeen\" = NOW() WHERE \"JobId\" IN ({$ph})");
        }
    }

    // ── 3. Expire jobs via Innovibe API ─────────────────────────────

    private function expireJobsFromApi(): void
    {
        $date = $this->api->getBcYesterdayDate();
        $this->log->info("Fetching expired job IDs for date: {$date}");

        $expiredIds = $this->api->fetchExpiredJobIds($date);

        if (empty($expiredIds)) {
            $this->log->info('No expired jobs returned by API');
            return;
        }

        $this->log->info(count($expiredIds) . ' expired jobs to process');
        $this->log->info('Expired job IDs: ' . implode(', ', $expiredIds));

        $expIns  = $this->db->prepare('
            INSERT INTO "ExpiredJobs" ("JobId","DateRemoved","RemovedFromElasticsearch")
            VALUES (?, NOW(), FALSE)
            ON CONFLICT ("JobId") DO UPDATE SET "DateRemoved" = NOW(), "RemovedFromElasticsearch" = FALSE
        ');
        $delStmt = $this->db->prepare('DELETE FROM "ImportedJobsWanted" WHERE "JobId" = ?');
        $deactStmt = $this->db->prepare('
            UPDATE "Jobs" SET "IsActive" = FALSE, "LastUpdated" = NOW()
            WHERE "JobId" = ? AND "IsActive" = TRUE
        ');

        $progressExpire = '';
        $deactivated    = 0;

        foreach ($expiredIds as $id) {
            $expIns->execute([$id]);
            $delStmt->execute([$id]);
            $deactStmt->execute([$id]);
            if ($deactStmt->rowCount() > 0) {
                $deactivated++;
            }
            $progressExpire .= 'E';
        }

        if ($progressExpire !== '') {
            $this->log->info($progressExpire);
        }

        $this->log->info("{$deactivated} jobs deactivated in Jobs table");

        // Mark all newly expired jobs as removed from Elasticsearch
        $this->db->exec('
            UPDATE "ExpiredJobs" SET "RemovedFromElasticsearch" = TRUE
            WHERE "RemovedFromElasticsearch" = FALSE
              AND "JobId" IN (SELECT "JobId" FROM "Jobs" WHERE "IsActive" = FALSE)
        ');
    }

    // ── 4. New jobs → "Jobs" ───────────────────────────────────────

    private function importNewJobs(): void
    {
        $rows = $this->db->query('
            SELECT ij."JobId", ij."JobPostEnglish", ij."DateFirstImported", ij."DateLastImported"
            FROM "ImportedJobsWanted" ij
            WHERE ij."IsFederalOrWorkBc" = FALSE
              AND NOT EXISTS (SELECT 1 FROM "Jobs" j WHERE j."JobId" = ij."JobId")
              AND NOT EXISTS (SELECT 1 FROM "DeletedJobs" d WHERE d."JobId" = ij."JobId")
        ')->fetchAll();

        $this->log->info(count($rows) . ' jobs found to import');

        $stmt = $this->db->prepare('
            INSERT INTO "Jobs" ("JobId","Title","City","IsActive","FullTime","PartTime","Permanent","Temporary",
                "OriginalSource","ExternalSourceUrl","DateFirstImported","DateLastImported","LastUpdated","JobSourceId",
                "PositionsAvailable","DatePosted","Casual","Seasonal","LeadingToFullTime","LocationId","ActualDatePosted",
                "Salary","SalarySummary")
            VALUES (?,?,?,TRUE,?,?,?,?,?,?,?,?,NOW(),?,?,?,?,?,?,?,?,?,?)
        ');

        $progress = '';
        foreach ($rows as $r) {
            $m = $this->map(json_decode($r['JobPostEnglish'], true) ?? []);
            $stmt->execute([
                $r['JobId'],
                $m['title'], $m['city'],
                $m['ft'], $m['pt'], $m['perm'], $m['temp'],
                $m['src'], $m['url'],
                $r['DateFirstImported'], $r['DateLastImported'],
                self::JOB_SOURCE,
                $m['positions'], $m['datePosted'],
                $m['casual'], $m['seasonal'], $m['leadingToFt'],
                $m['locationId'], $m['datePosted'],
                $m['salary'], $m['salarySummary'],
            ]);
            $progress .= 'I';
        }

        if ($progress !== '') {
            $this->log->info($progress);
        }
    }

    // ── 5. Update existing "Jobs" ──────────────────────────────────

    private function updateExistingJobs(): void
    {
        $rows = $this->db->query('
            SELECT ij."JobId", ij."JobPostEnglish", ij."DateLastImported"
            FROM "ImportedJobsWanted" ij
            INNER JOIN "Jobs" j ON j."JobId" = ij."JobId"
            WHERE ij."IsFederalOrWorkBc" = FALSE
              AND NOT EXISTS (SELECT 1 FROM "DeletedJobs" d WHERE d."JobId" = ij."JobId")
              AND (j."DateLastImported" <> ij."DateLastImported" OR j."IsActive" = FALSE)
        ')->fetchAll();

        $this->log->info(count($rows) . ' jobs found to update');

        $stmt = $this->db->prepare('
            UPDATE "Jobs" SET
                "Title"=?, "City"=?, "FullTime"=?, "PartTime"=?, "Permanent"=?, "Temporary"=?,
                "OriginalSource"=?, "ExternalSourceUrl"=?, "IsActive"=TRUE,
                "DateLastImported"=?, "LastUpdated"=NOW(),
                "Salary"=?, "SalarySummary"=?
            WHERE "JobId"=?
        ');

        $progress = '';
        foreach ($rows as $r) {
            $m = $this->map(json_decode($r['JobPostEnglish'], true) ?? []);
            $stmt->execute([
                $m['title'], $m['city'],
                $m['ft'], $m['pt'], $m['perm'], $m['temp'],
                $m['src'], $m['url'],
                $r['DateLastImported'],
                $m['salary'], $m['salarySummary'],
                $r['JobId'],
            ]);
            $progress .= 'U';
        }

        if ($progress !== '') {
            $this->log->info($progress);
        }
    }

    // ── Field mapping ──────────────────────────────────────────────

    private function map(array $j): array
    {
        $t          = strtolower(implode(' ', $j['employmentType'] ?? []));
        $city       = $j['jobLocations'][0]['city'] ?? '';
        $datePosted = date('Y-m-d H:i:s', strtotime($j['postedDate'] ?? $j['createdAt']));

        $salaryMin = $j['salaryMin'] ?? null;
        $salaryMax = $j['salaryMax'] ?? null;
        $salary    = $salaryMin ?? $j['salaryValue'] ?? $salaryMax;

        $salaryUnit = $j['salaryUnitText'] ?? '';
        if ($salaryMin !== null && $salaryMax !== null) {
            $salarySummary = '$' . number_format((float) $salaryMin) . ' - $' . number_format((float) $salaryMax);
        } else {
            $salarySummary = '$' . number_format((float) $salary);
        }
        if ($salaryUnit) {
            $salarySummary .= ' / ' . strtoupper($salaryUnit);
        }

        return [
            'title'         => mb_substr($j['title'], 0, 300),
            'city'          => mb_substr($city, 0, 120),
            'ft'            => (int) str_contains($t, 'full'),
            'pt'            => (int) str_contains($t, 'part'),
            'perm'          => (int) str_contains($t, 'permanent'),
            'temp'          => (int) (str_contains($t, 'contract') || str_contains($t, 'temporary')),
            'casual'        => (int) str_contains($t, 'casual'),
            'seasonal'      => (int) str_contains($t, 'seasonal'),
            'leadingToFt'   => 0,
            'src'           => mb_substr($j['source'] ?? $j['sourceDomain'] ?? '', 0, 100),
            'url'           => mb_substr($j['url'] ?? '', 0, 800),
            'positions'     => 1,
            'datePosted'    => $datePosted,
            'locationId'    => 0,
            'salary'        => $salary,
            'salarySummary' => mb_substr($salarySummary, 0, 60),
        ];
    }
}