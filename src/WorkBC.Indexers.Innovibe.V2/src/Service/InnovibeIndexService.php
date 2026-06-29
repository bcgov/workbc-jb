<?php

declare(strict_types=1);

namespace WorkBC\InnovibeJobIndexer\Service;

use Monolog\Logger;
use PDO;
use WorkBC\InnovibeJobIndexer\Config\AppConfig;
use WorkBC\InnovibeJobIndexer\Elastic\ElasticClient;
use WorkBC\InnovibeJobIndexer\Json\InnovibeDocumentMapper;

/**
 * Innovibe (Wanted) indexer orchestrator. Reads ImportedJobsWanted rows flagged
 * for re-indexing (non-federal only), builds the Elasticsearch document, PUTs it
 * into jobs_en, clears the flag, then purges expired and orphaned docs.
 *
 * Faithful port of WorkBC.Indexers.Wanted.Services.WantedIndexService. Unlike the
 * federal indexer there is a single index and a data-quality gate: jobs missing
 * the fields the UI search relies on are parked in ExpiredJobs + flipped inactive
 * in Jobs instead of being indexed.
 */
final class InnovibeIndexService
{
    private const FLUSH_THRESHOLD = 100;
    private const PURGE_BATCH = 100;
    private const JOB_SOURCE_WANTED = 2;

    /** @var list<string> JobIds whose ReIndexNeeded flag is pending clear */
    private array $pendingIds = [];
    /** @var list<string> JobIds rejected by the publishable gate, pending FlushRejected */
    private array $rejectedIds = [];

    public function __construct(
        private readonly PDO $db,
        private readonly ElasticClient $elastic,
        private readonly InnovibeDocumentMapper $mapper,
        private readonly AppConfig $config,
        private readonly Logger $log,
    ) {}

    // ── Index ──────────────────────────────────────────────────────

    public function indexJobs(): void
    {
        $rows = $this->db->query('
            SELECT "JobId", "JobPostEnglish"
            FROM "ImportedJobsWanted"
            WHERE "ReIndexNeeded" = TRUE AND "IsFederalOrWorkBc" = FALSE
        ')->fetchAll();

        $this->log->info(count($rows) . ' wanted jobs to index');

        $processed = 0;
        $indexed = 0;
        $rejected = 0;
        foreach ($rows as $row) {
            $jobId = (string) $row['JobId'];
            $doc = $this->mapper->convertToElasticJob($row['JobPostEnglish']);

            // A null document (or one without a JobId) is a job from the feds /
            // WorkBC; just clear its flag (mirrors the C# else-branch).
            if ($doc === null || ($doc['JobId'] ?? null) === null) {
                $this->pendingIds[] = $jobId;
                $this->maybeFlush();
                $processed++;
                continue;
            }

            // Data-quality gate: unpublishable rows are parked, not indexed.
            if (!$this->isPublishable($doc)) {
                $this->rejectedIds[] = $doc['JobId'];
                $this->pendingIds[] = $doc['JobId'];
                $rejected++;
                $this->maybeFlush();
                $processed++;
                continue;
            }

            $json = $this->serialize($doc);
            $result = $this->putWithRetry($doc['JobId'], $json);

            if ($result === 'error') {
                // Problem with Elasticsearch — most likely down. Stop here so we
                // don't churn the whole batch (mirrors the C# CRITICAL ERROR break).
                $this->log->error('CRITICAL ERROR talking to Elasticsearch — stopping index loop');
                break;
            }

            if ($result === 'ok' || $result === 'created' || $result === 'updated') {
                $this->pendingIds[] = $jobId;
                $indexed++;
            }

            $this->maybeFlush();
            $processed++;
            if ($processed % 1000 === 0) {
                $this->log->info("Processed {$processed} jobs...");
            }
        }

        $this->updateIds();
        $this->flushRejected();
        $this->log->info("Indexing complete — processed={$processed} indexed={$indexed} rejected={$rejected}");
    }

    private function maybeFlush(): void
    {
        if (count($this->rejectedIds) >= self::FLUSH_THRESHOLD) {
            $this->flushRejected();
        }
        if (count($this->pendingIds) > self::FLUSH_THRESHOLD) {
            $this->updateIds();
        }
    }

    /**
     * Data-quality gate. Mirrors WantedIndexService.IsPublishable: a job must
     * have a Title, an EmployerName, a valid Noc2021, and a non-empty City[0].
     *
     * @param array<string,mixed> $doc
     */
    private function isPublishable(array $doc): bool
    {
        if (trim((string) ($doc['Title'] ?? '')) === '') {
            return false;
        }
        if (trim((string) ($doc['EmployerName'] ?? '')) === '') {
            return false;
        }
        $noc = $doc['Noc2021'] ?? null;
        if ($noc === null || (int) $noc === 0) {
            return false;
        }
        $city = $doc['City'] ?? null;
        if (!is_array($city) || count($city) === 0 || trim((string) ($city[0] ?? '')) === '') {
            return false;
        }
        return true;
    }

    private function putWithRetry(string $jobId, ?string $json): string
    {
        if ($json === null || $json === 'null') {
            return 'error';
        }
        $result = $this->elastic->putDocument($this->config->indexEn, $jobId, $json);
        if ($result === 'error') {
            if ($this->config->retryDelayMs > 0) {
                usleep($this->config->retryDelayMs * 1000);
            }
            $result = $this->elastic->putDocument($this->config->indexEn, $jobId, $json);
        }
        return $result;
    }

    /**
     * Clear ReIndexNeeded for the buffered JobIds. The Wanted indexer correctly
     * targets ImportedJobsWanted (the table it reads), unlike the federal indexer.
     */
    private function updateIds(): void
    {
        if (empty($this->pendingIds)) {
            return;
        }
        try {
            $placeholders = implode(',', array_fill(0, count($this->pendingIds), '?'));
            $stmt = $this->db->prepare(
                "UPDATE \"ImportedJobsWanted\" SET \"ReIndexNeeded\" = FALSE WHERE \"JobId\" IN ({$placeholders})"
            );
            $stmt->execute(array_values($this->pendingIds));
        } catch (\Throwable $e) {
            $this->log->error('ERROR: Could not update SQL column `ReIndexNeeded`. Reason: ' . $e->getMessage());
        }
        $this->pendingIds = [];
    }

    /**
     * Park rejected jobs: upsert into ExpiredJobs and flip Jobs.IsActive = FALSE
     * so admin counts stop tracking them. Mirrors WantedIndexService.FlushRejected.
     */
    private function flushRejected(): void
    {
        if (empty($this->rejectedIds)) {
            return;
        }
        try {
            $placeholders = implode(',', array_fill(0, count($this->rejectedIds), '?'));
            $ids = array_values($this->rejectedIds);

            $expire = $this->db->prepare(
                "INSERT INTO \"ExpiredJobs\" (\"JobId\", \"DateRemoved\", \"RemovedFromElasticsearch\")
                 SELECT v, NOW(), FALSE FROM (SELECT unnest(ARRAY[{$placeholders}]::varchar[]) AS v) s
                 ON CONFLICT (\"JobId\") DO UPDATE
                 SET \"DateRemoved\" = NOW(), \"RemovedFromElasticsearch\" = FALSE"
            );
            $expire->execute($ids);

            $deactivate = $this->db->prepare(
                "UPDATE \"Jobs\" SET \"IsActive\" = FALSE, \"LastUpdated\" = NOW()
                 WHERE \"JobId\" IN ({$placeholders}) AND \"IsActive\" = TRUE"
            );
            $deactivate->execute($ids);

            $this->rejectedIds = [];
        } catch (\Throwable $e) {
            $this->log->error('ERROR: Could not write rejected jobs to ExpiredJobs/Jobs. Reason: ' . $e->getMessage());
        }
    }

    // ── Purge ──────────────────────────────────────────────────────

    public function purgeJobs(): void
    {
        $expired = $this->db->query('
            SELECT "JobId" FROM "ExpiredJobs" WHERE "RemovedFromElasticsearch" = FALSE
        ')->fetchAll(PDO::FETCH_COLUMN);

        $markRemoved = $this->db->prepare(
            'UPDATE "ExpiredJobs" SET "RemovedFromElasticsearch" = TRUE WHERE "JobId" = ?'
        );

        $counter = 0;
        foreach ($expired as $jobId) {
            $jobId = (string) $jobId;
            $this->elastic->deleteDocument($this->config->indexEn, $jobId);
            $markRemoved->execute([$jobId]);
            $counter++;
            if ($counter % self::PURGE_BATCH === 0) {
                $this->log->info("Purged {$counter} expired jobs...");
            }
        }

        // Sweep orphans: non-federal docs in ES whose JobId is no longer in staging.
        $stagingIds = $this->db->query('
            SELECT "JobId" FROM "ImportedJobsWanted" WHERE "IsFederalOrWorkBc" = FALSE
        ')->fetchAll(PDO::FETCH_COLUMN);
        $stagingLookup = array_flip(array_map('strval', $stagingIds));

        $orphans = 0;
        foreach ($this->elastic->getJobIdsBySource($this->config->indexEn, false) as $jobId) {
            if (!isset($stagingLookup[$jobId])) {
                $this->elastic->deleteDocument($this->config->indexEn, $jobId);
                $orphans++;
            }
        }

        $this->log->info("Purge complete — expired={$counter} orphans={$orphans}");
    }

    // ── Debug ──────────────────────────────────────────────────────

    public function debug(): void
    {
        $elasticIds = $this->elastic->getJobIdsBySource($this->config->indexEn, false, true);

        $sqlIds = $this->db->query('
            SELECT "JobId" FROM "Jobs"
            WHERE "JobSourceId" = ' . self::JOB_SOURCE_WANTED . '
              AND "ExpireDate" > NOW()
              AND "IsActive" = TRUE
        ')->fetchAll(PDO::FETCH_COLUMN);
        $sqlIds = array_map('strval', $sqlIds);

        $elasticSet = array_flip($elasticIds);
        $sqlSet = array_flip($sqlIds);

        echo 'Active jobs found in Elasticsearch: ' . count($elasticIds) . PHP_EOL;
        echo 'Active jobs found in the Jobs table: ' . count($sqlIds) . PHP_EOL;
        echo PHP_EOL;
        echo 'JobIds found in Elasticsearch but not in the Jobs table:' . PHP_EOL;
        foreach ($elasticIds as $id) {
            if (!isset($sqlSet[$id])) {
                echo $id . PHP_EOL;
            }
        }
        echo PHP_EOL;
        echo 'JobIds found in the Jobs table but not in Elasticsearch:' . PHP_EOL;
        foreach ($sqlIds as $id) {
            if (!isset($elasticSet[$id])) {
                echo $id . PHP_EOL;
            }
        }
    }

    // ── Serialization ──────────────────────────────────────────────

    private function serialize(?array $document): ?string
    {
        if ($document === null) {
            return null;
        }
        return json_encode($this->stripNulls($document), JSON_UNESCAPED_UNICODE | JSON_UNESCAPED_SLASHES);
    }

    /**
     * @param array<mixed> $value
     * @return array<mixed>
     */
    private function stripNulls(array $value): array
    {
        $out = [];
        foreach ($value as $key => $item) {
            if ($item === null) {
                continue;
            }
            // Objects (e.g. EmploymentTerms = stdClass for an empty {}) pass
            // through untouched so json_encode keeps them as {} rather than [].
            $out[$key] = is_array($item) ? $this->stripNulls($item) : $item;
        }
        return $out;
    }
}
