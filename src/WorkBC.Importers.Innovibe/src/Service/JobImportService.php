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

            $apiDate = date('Y-m-d H:i:s', strtotime($job['updatedAt'] ?? $job['createdAt'] ?? 'now'));
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

    // ── 3. Expire jobs via Innovibe API ────────────────────────────

    private function expireJobsFromApi(): void
    {
        $this->log->info('Fetching all expired job IDs from Innovibe...');

        $expiredIds = $this->api->fetchExpiredJobIds();

        if (empty($expiredIds)) {
            $this->log->info('No expired jobs returned by API');
            return;
        }

        $this->log->info(count($expiredIds) . ' total expired IDs returned by API');

        // Bulk-filter: only keep IDs that exist in our JobIds table
        $matchedIds = [];
        foreach (array_chunk($expiredIds, 500) as $chunk) {
            $ph   = implode(',', array_map(fn($id) => $this->db->quote($id), $chunk));
            $rows = $this->db->query("SELECT \"Id\" FROM \"JobIds\" WHERE \"Id\" IN ({$ph})")->fetchAll(PDO::FETCH_COLUMN);
            $matchedIds = array_merge($matchedIds, $rows);
        }

        $skipped = count($expiredIds) - count($matchedIds);
        $this->log->info(count($matchedIds) . " jobs to expire, {$skipped} skipped (not in system)");

        if (empty($matchedIds)) {
            return;
        }

        $expIns = $this->db->prepare('
            INSERT INTO "ExpiredJobs" ("JobId","DateRemoved","RemovedFromElasticsearch")
            VALUES (?, NOW(), FALSE)
            ON CONFLICT ("JobId") DO UPDATE SET "DateRemoved" = NOW(), "RemovedFromElasticsearch" = FALSE
        ');
        $delStmt   = $this->db->prepare('DELETE FROM "ImportedJobsWanted" WHERE "JobId" = ?');
        $deactStmt = $this->db->prepare('
            UPDATE "Jobs" SET "IsActive" = FALSE, "LastUpdated" = NOW()
            WHERE "JobId" = ? AND "IsActive" = TRUE
        ');

        $deactivated = 0;

        foreach ($matchedIds as $id) {
            try {
                $expIns->execute([$id]);
                $delStmt->execute([$id]);
                $deactStmt->execute([$id]);
                if ($deactStmt->rowCount() > 0) {
                    $deactivated++;
                }
            } catch (\Throwable $e) {
                $this->log->warning("Failed to expire job {$id}: {$e->getMessage()}");
            }
        }

        $this->log->info("{$deactivated} jobs deactivated in Jobs table");

        // Mark expired jobs as removed from Elasticsearch
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
                "Salary","SalarySummary","NocCodeId2021","ExpireDate","EmployerName")
            VALUES (?,?,?,TRUE,?,?,?,?,?,?,?,?,NOW(),?,?,?,?,?,?,?,?,?,?,?,?,?)
        ');

        $verStmt = $this->db->prepare('
            INSERT INTO "JobVersions" ("JobId","DateVersionStart","DatePosted","ActualDatePosted",
                "DateFirstImported","JobSourceId","IndustryId","NocCodeId","NocCodeId2021",
                "IsActive","PositionsAvailable","LocationId","IsCurrentVersion","VersionNumber")
            VALUES (?,?,?,?,?,?,NULL,NULL,?,TRUE,?,?,TRUE,1)
        ');

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
                $m['locationId'], $m['actualDatePosted'],
                $m['salary'], $m['salarySummary'],
                $m['nocCode2021'],
                $m['expireDate'],
                $m['employerName'],
            ]);

            // Create version 1 in JobVersions (mirrors C# JobsTableSyncService)
            $versionStart = $this->getVersion1StartDate($m['datePosted'], $r['DateFirstImported']);
            $verStmt->execute([
                $r['JobId'],
                $versionStart,
                $m['datePosted'], $m['actualDatePosted'],
                $versionStart,
                self::JOB_SOURCE,
                $m['nocCode2021'],
                $m['positions'], $m['locationId'],
            ]);
        }
    }

    // ── 5. Update existing "Jobs" ──────────────────────────────────

    private function updateExistingJobs(): void
    {
        $rows = $this->db->query('
            SELECT ij."JobId", ij."JobPostEnglish", ij."DateLastImported",
                   j."NocCodeId2021" AS "OldNoc2021", j."LocationId" AS "OldLocationId",
                   j."PositionsAvailable" AS "OldPositions", j."DatePosted" AS "OldDatePosted",
                   j."IsActive" AS "OldIsActive"
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
                "Casual"=?, "Seasonal"=?, "LeadingToFullTime"=?, "PositionsAvailable"=?,
                "OriginalSource"=?, "ExternalSourceUrl"=?, "IsActive"=TRUE,
                "DateLastImported"=?, "LastUpdated"=NOW(),
                "Salary"=?, "SalarySummary"=?,
                "NocCodeId2021"=?, "ExpireDate"=?, "LocationId"=?, "EmployerName"=?,
                "DatePosted"=?, "ActualDatePosted"=?
            WHERE "JobId"=?
        ');

        foreach ($rows as $r) {
            $m = $this->map(json_decode($r['JobPostEnglish'], true) ?? []);
            $stmt->execute([
                $m['title'], $m['city'],
                $m['ft'], $m['pt'], $m['perm'], $m['temp'],
                $m['casual'], $m['seasonal'], $m['leadingToFt'], $m['positions'],
                $m['src'], $m['url'],
                $r['DateLastImported'],
                $m['salary'], $m['salarySummary'],
                $m['nocCode2021'],
                $m['expireDate'], $m['locationId'],
                $m['employerName'],
                $m['datePosted'], $m['actualDatePosted'],
                $r['JobId'],
            ]);

            // Check if key fields changed → increment JobVersion
            $needsNewVersion = ($r['OldNoc2021'] != $m['nocCode2021'])
                || ((int)$r['OldLocationId'] !== $m['locationId'])
                || ((int)$r['OldPositions'] !== $m['positions'])
                || ($r['OldDatePosted'] !== $m['datePosted'])
                || !$r['OldIsActive'];

            if ($needsNewVersion) {
                $this->incrementJobVersion($r['JobId'], $m);
            }
        }
    }

    // ── Job version helpers ─────────────────────────────────────────

    /**
     * Returns min(datePosted, dateImported), capped at 24h before dateImported.
     * Mirrors C# JobsTableSyncServiceBase.GetVersion1StartDate().
     */
    private function getVersion1StartDate(string $datePosted, string $dateImported): string
    {
        $posted   = strtotime($datePosted);
        $imported = strtotime($dateImported);

        if ($imported <= $posted) {
            return $dateImported;
        }

        $cap = $imported - 86400; // 24 hours
        return $posted < $cap
            ? date('Y-m-d H:i:s', $cap)
            : $datePosted;
    }

    /**
     * Close the current version and create a new one.
     * Mirrors C# JobsTableSyncServiceBase.IncrementJobVersion().
     */
    private function incrementJobVersion(string $jobId, array $m): void
    {
        $now = date('Y-m-d H:i:s');

        // Find the current version
        $cur = $this->db->prepare('
            SELECT "Id", "VersionNumber", "DatePosted", "ActualDatePosted",
                   "DateFirstImported", "JobSourceId", "IndustryId", "NocCodeId"
            FROM "JobVersions"
            WHERE "JobId" = ? AND "IsCurrentVersion" = TRUE
            LIMIT 1
        ');
        $cur->execute([$jobId]);
        $old = $cur->fetch();

        if (!$old) {
            // Fallback: grab the highest version number
            $cur2 = $this->db->prepare('
                SELECT "Id", "VersionNumber", "DatePosted", "ActualDatePosted",
                       "DateFirstImported", "JobSourceId", "IndustryId", "NocCodeId"
                FROM "JobVersions"
                WHERE "JobId" = ?
                ORDER BY "VersionNumber" DESC
                LIMIT 1
            ');
            $cur2->execute([$jobId]);
            $old = $cur2->fetch();
        }

        if ($old) {
            // Close old version
            $close = $this->db->prepare('
                UPDATE "JobVersions" SET "IsCurrentVersion" = FALSE, "DateVersionEnd" = ?
                WHERE "Id" = ?
            ');
            $close->execute([$now, $old['Id']]);

            // Insert new version
            $ins = $this->db->prepare('
                INSERT INTO "JobVersions" ("JobId","DateVersionStart","DatePosted","ActualDatePosted",
                    "DateFirstImported","JobSourceId","IndustryId","NocCodeId","NocCodeId2021",
                    "IsActive","PositionsAvailable","LocationId","IsCurrentVersion","VersionNumber")
                VALUES (?,?,?,?,?,?,?,?,?,TRUE,?,?,TRUE,?)
            ');
            $ins->execute([
                $jobId, $now,
                $m['datePosted'], $m['actualDatePosted'],
                $old['DateFirstImported'],
                $old['JobSourceId'],
                $old['IndustryId'],
                $old['NocCodeId'],
                $m['nocCode2021'],
                $m['positions'], $m['locationId'],
                (int)$old['VersionNumber'] + 1,
            ]);
        } else {
            // No previous version exists — create version 1
            $ins = $this->db->prepare('
                INSERT INTO "JobVersions" ("JobId","DateVersionStart","DatePosted","ActualDatePosted",
                    "DateFirstImported","JobSourceId","IndustryId","NocCodeId","NocCodeId2021",
                    "IsActive","PositionsAvailable","LocationId","IsCurrentVersion","VersionNumber")
                VALUES (?,?,?,?,?,?,NULL,NULL,?,TRUE,?,?,TRUE,1)
            ');
            $ins->execute([
                $jobId, $now,
                $m['datePosted'], $m['actualDatePosted'],
                $now,
                self::JOB_SOURCE,
                $m['nocCode2021'],
                $m['positions'], $m['locationId'],
            ]);
        }
    }

    // ── Field mapping ──────────────────────────────────────────────

    private function map(array $j): array
    {
        $t                = strtolower(implode(' ', $j['employmentType'] ?? []));
        $actualDatePosted = date('Y-m-d H:i:s', strtotime($j['postedDate'] ?? $j['createdAt'] ?? 'now'));
        $datePosted       = date('Y-m-d H:i:s', strtotime($j['updatedAt'] ?? $j['postedDate'] ?? $j['createdAt'] ?? 'now'));
        $expireDate       = date('Y-m-d H:i:s', strtotime($datePosted . ' +90 days'));

        // Location: prefer a British Columbia location from the array
        $city = '';
        $locations = $j['jobLocations'] ?? [];
        if (!empty($locations)) {
            $loc = null;
            foreach ($locations as $candidate) {
                if (strcasecmp($candidate['state'] ?? '', 'British Columbia') === 0) {
                    $loc = $candidate;
                    break;
                }
            }
            $loc ??= $locations[0];
            $city = $loc['city'] ?? '';
            // Use province as fallback when city is empty (remote jobs)
            if ($city === '' && !empty($loc['state'])) {
                $city = $loc['state'];
            }
        }

        // Salary: convert everything to annual and format as "$XX,XXX annually"
        $salaryMin = $j['salaryMin'] ?? null;
        $salaryMax = $j['salaryMax'] ?? null;
        $salary    = $salaryMin ?? $j['salaryValue'] ?? $salaryMax;

        $salaryUnit = strtoupper($j['salaryUnitText'] ?? '');
        $annualMultiplier = match ($salaryUnit) {
            'HOUR'  => 2080,   // 40 hrs/week × 52 weeks
            'WEEK'  => 52,
            'MONTH' => 12,
            default => 1,      // YEAR or unknown
        };

        // Convert to annual
        $annualSalary = $salary !== null ? (float) $salary * $annualMultiplier : null;
        $annualMin    = $salaryMin !== null ? (float) $salaryMin * $annualMultiplier : null;
        $annualMax    = $salaryMax !== null ? (float) $salaryMax * $annualMultiplier : null;

        if ($annualMin !== null && $annualMax !== null && $annualMin != $annualMax) {
            $salarySummary = '$' . number_format($annualMin) . ' - $' . number_format($annualMax) . ' annually';
        } elseif ($annualSalary !== null) {
            $salarySummary = '$' . number_format($annualSalary) . ' annually';
        } else {
            $salarySummary = 'N/A';
        }

        // NOC 2021: pick the highest-scored match, validate against NocCodes2021
        $nocCode2021 = null;
        $nocMatches  = $j['nocMatches'] ?? [];
        if (!empty($nocMatches)) {
            usort($nocMatches, fn($a, $b) => ($b['score'] ?? 0) <=> ($a['score'] ?? 0));
            $code = $nocMatches[0]['code'] ?? null;
            if ($code !== null) {
                $candidate = (int) $code;
                if ($this->isValidNoc2021($candidate)) {
                    $nocCode2021 = $candidate;
                }
            }
        }

        // Employer name: extract from company.name, validate has English chars (matches C# regex)
        $employerName = '';
        $company = $j['company'] ?? null;
        if (is_array($company) && !empty($company['name'])) {
            $rawName = trim($company['name']);
            if (preg_match('/[a-zA-Z0-9$@!%*?&#^_.+\-]+/', $rawName)) {
                $employerName = $rawName;
            }
        }

        return [
            'title'         => mb_substr($j['title'] ?? '', 0, 300),
            'city'          => mb_substr($city ?? '', 0, 120),
            'ft'            => (int) str_contains($t, 'full'),
            'pt'            => (int) str_contains($t, 'part'),
            'perm'          => (int) str_contains($t, 'permanent'),
            'temp'          => (int) (str_contains($t, 'contract') || str_contains($t, 'freelance') || str_contains($t, 'temporary')),
            'casual'        => (int) str_contains($t, 'casual'),
            'seasonal'      => (int) str_contains($t, 'seasonal'),
            'leadingToFt'   => 0,
            'src'           => mb_substr($j['sourceDomain'] ?? '', 0, 100),
            'url'           => mb_substr($j['url'] ?? '', 0, 800),
            'positions'     => 1,
            'datePosted'       => $datePosted,
            'actualDatePosted' => $actualDatePosted,
            'locationId'    => $this->getBestAvailableLocationId($city),
            'salary'        => $salary,
            'salarySummary' => mb_substr($salarySummary ?? '', 0, 60),
            'nocCode2021'   => $nocCode2021,
            'expireDate'    => $expireDate,
            'employerName'  => mb_substr($employerName, 0, 100),
        ];
    }

    /**
     * Cache of valid NocCodes2021 IDs to avoid repeated DB lookups.
     */
    private ?array $validNocIds = null;

    private function isValidNoc2021(int $id): bool
    {
        if ($this->validNocIds === null) {
            $this->validNocIds = array_flip(
                $this->db->query('SELECT "Id" FROM "NocCodes2021"')->fetchAll(PDO::FETCH_COLUMN)
            );
            $this->log->info(count($this->validNocIds) . ' NOC 2021 codes loaded');
        }
        return isset($this->validNocIds[$id]);
    }

    // ── Location lookup ─────────────────────────────────────────────

    /**
     * Cache of Label => LocationId from the Locations table.
     * Mirrors C# JobsTableSyncServiceBase.LocationIdLookup.
     */
    private ?array $locationLookup = null;

    private function loadLocationLookup(): void
    {
        if ($this->locationLookup !== null) {
            return;
        }
        $rows = $this->db->query('
            SELECT "Label", "LocationId"
            FROM "Locations"
            WHERE "IsHidden" = FALSE
              AND ("IsDuplicate" = FALSE OR "FederalCityId" IS NOT NULL)
        ')->fetchAll();

        $this->locationLookup = [];
        foreach ($rows as $row) {
            $this->locationLookup[$row['Label']] = (int) $row['LocationId'];
        }
        $this->log->info(count($this->locationLookup) . ' locations loaded for lookup');
    }

    /**
     * Resolves a city name to a LocationId using cascading match strategies.
     * Mirrors C# JobsTableSyncServiceBase.GetBestAvailableLocationId().
     */
    private function getBestAvailableLocationId(string $city): int
    {
        $this->loadLocationLookup();

        $city = strtolower(trim($city));
        if ($city === '') {
            return 0;
        }

        // 1. Exact match
        foreach ($this->locationLookup as $label => $locId) {
            if (strtolower($label) === $city) {
                return $locId;
            }
        }

        // Don't fuzzy-match very short strings
        if (mb_strlen($city) < 5) {
            return 0;
        }

        // 2. City starts with a lookup label
        foreach ($this->locationLookup as $label => $locId) {
            if (str_starts_with($city, strtolower($label))) {
                return $locId;
            }
        }

        // 3. Lookup label starts with city
        foreach ($this->locationLookup as $label => $locId) {
            if (str_starts_with(strtolower($label), $city)) {
                return $locId;
            }
        }

        // 4. City contains a lookup label
        foreach ($this->locationLookup as $label => $locId) {
            if (str_contains($city, strtolower($label))) {
                return $locId;
            }
        }

        // 5. Lookup label contains city
        foreach ($this->locationLookup as $label => $locId) {
            if (str_contains(strtolower($label), $city)) {
                return $locId;
            }
        }

        return 0;
    }
}