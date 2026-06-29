<?php

declare(strict_types=1);

namespace WorkBC\FederalJobIndexer\Service;

use Monolog\Logger;
use PDO;
use WorkBC\FederalJobIndexer\Config\AppConfig;
use WorkBC\FederalJobIndexer\Elastic\ElasticClient;
use WorkBC\FederalJobIndexer\Xml\FederalDocumentMapper;

/**
 * Federal indexer orchestrator. Reads ImportedJobsFederal rows flagged for
 * re-indexing, builds the EN + FR Elasticsearch documents, PUTs them into
 * jobs_en / jobs_fr, clears the flag, then purges expired and orphaned docs.
 *
 * Faithful port of WorkBC.Indexers.Federal.Services.FederalIndexService.
 */
final class FederalIndexService
{
    private const FLUSH_THRESHOLD = 100;
    private const PURGE_BATCH = 100;
    private const JOB_SOURCE_FEDERAL = 1;

    /** @var list<string> JobIds whose flag is pending clear */
    private array $pendingIds = [];

    public function __construct(
        private readonly PDO $db,
        private readonly ElasticClient $elastic,
        private readonly FederalDocumentMapper $mapper,
        private readonly AppConfig $config,
        private readonly Logger $log,
    ) {}

    // ── Index ──────────────────────────────────────────────────────

    /**
     * Index every federal staging row flagged ReIndexNeeded.
     */
    public function indexJobs(): void
    {
        $rows = $this->db->query('
            SELECT "JobId", "JobPostEnglish", "JobPostFrench"
            FROM "ImportedJobsFederal"
            WHERE "ReIndexNeeded" = TRUE
        ')->fetchAll();

        $this->log->info(count($rows) . ' federal jobs to index');

        $processed = 0;
        foreach ($rows as $row) {
            $jobId = (string) $row['JobId'];

            $english = $this->mapper->convertToElasticJob($row['JobPostEnglish'], false);
            $french = $this->mapper->convertToElasticJob($row['JobPostFrench'], true);

            $jsonEnglish = $this->serialize($english);
            $jsonFrench = $this->serialize($french);

            if ($english === null) {
                // Corrupt / unavailable English XML — nothing to index.
                $this->log->debug("[Missing Or Corrupt English XML {$jobId}]");
                continue;
            }

            $englishOk = $this->createJob($this->config->indexEn, $english['JobId'], $jsonEnglish);

            // French is best-effort: a missing French translation still lets the
            // English document (and the flag clear) go through.
            if ($englishOk) {
                $frenchOk = $this->createJob($this->config->indexFr, $english['JobId'], $jsonFrench);
                if ($frenchOk) {
                    $this->pendingIds[] = $jobId;
                }
            }

            if (count($this->pendingIds) > self::FLUSH_THRESHOLD) {
                $this->updateIds();
            }

            $processed++;
            if ($processed % 500 === 0) {
                $this->log->info("Indexed {$processed} jobs...");
            }
        }

        // Flush whatever is left below the threshold.
        $this->updateIds();
        $this->log->info("Indexing complete — {$processed} jobs processed");
    }

    /**
     * PUT a single document, tolerating a missing French translation and
     * retrying once (after a pause) on a transport-level failure.
     */
    private function createJob(string $index, string $jobId, ?string $json): bool
    {
        if ($json === null || $json === 'null') {
            // A few federal jobs have no French translation. We mostly care
            // about English for WorkBC, so treat a missing French doc as OK.
            if ($index === $this->config->indexFr) {
                $this->log->debug("[Missing French XML {$jobId}]");
                return true;
            }
            $this->log->warning("[Missing Or Corrupt English XML {$jobId}]");
            return false;
        }

        $result = $this->elastic->putDocument($index, $jobId, $json);
        if ($result === 'error') {
            // ElasticSearch occasionally stops accepting requests during Java
            // garbage collection; wait and retry once (mirrors the C# WebException branch).
            if ($this->config->retryDelayMs > 0) {
                usleep($this->config->retryDelayMs * 1000);
            }
            $result = $this->elastic->putDocument($index, $jobId, $json);
        }

        return $result === 'ok' || $result === 'created' || $result === 'updated';
    }

    /**
     * Clear ReIndexNeeded for the buffered JobIds, then empty the buffer.
     *
     * NOTE: this intentionally updates "ImportedJobsWanted" — NOT
     * "ImportedJobsFederal" — to preserve byte-for-byte parity with the legacy
     * C# FederalIndexService.UpdateIds(), which targets the wrong table. The
     * federal ReIndexNeeded flag is therefore reset in practice only by a full
     * --reindex. Do not "fix" this without an explicit decision: the federal
     * indexer's observable behaviour (re-emitting every flagged row each run)
     * must stay identical to the .NET implementation it replaces.
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

    // ── Purge ──────────────────────────────────────────────────────

    /**
     * Remove expired and orphaned federal documents from both indexes.
     * Ports FederalIndexService.PurgeJobs().
     */
    public function purgeJobs(): void
    {
        // 1. Drain ExpiredJobs not yet removed from Elasticsearch.
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
            $this->elastic->deleteDocument($this->config->indexFr, $jobId);
            $markRemoved->execute([$jobId]);
            $counter++;
            if ($counter % self::PURGE_BATCH === 0) {
                $this->log->info("Purged {$counter} expired jobs...");
            }
        }

        // 2. Sweep orphans: docs in ES whose JobId is no longer in staging.
        $stagingIds = $this->db->query('SELECT "JobId" FROM "ImportedJobsFederal"')
            ->fetchAll(PDO::FETCH_COLUMN);
        $stagingLookup = array_flip(array_map('strval', $stagingIds));

        $englishOrphans = 0;
        foreach ($this->elastic->getFederalJobIds($this->config->indexEn) as $jobId) {
            if (!isset($stagingLookup[$jobId])) {
                $this->elastic->deleteDocument($this->config->indexEn, $jobId);
                $englishOrphans++;
            }
        }

        $frenchOrphans = 0;
        foreach ($this->elastic->getFederalJobIds($this->config->indexFr) as $jobId) {
            if (!isset($stagingLookup[$jobId])) {
                $this->elastic->deleteDocument($this->config->indexFr, $jobId);
                $frenchOrphans++;
            }
        }

        $this->log->info(
            sprintf(
                'Purge complete — expired=%d englishOrphans=%d frenchOrphans=%d',
                $counter,
                $englishOrphans,
                $frenchOrphans
            )
        );
    }

    // ── Debug ──────────────────────────────────────────────────────

    /**
     * Prints the diff between active federal jobs in Elasticsearch and the
     * Jobs table. Ports FederalIndexService.Debug().
     */
    public function debug(): void
    {
        $elasticIds = $this->elastic->getFederalJobIds($this->config->indexEn, true);

        $sqlIds = $this->db->query('
            SELECT "JobId" FROM "Jobs"
            WHERE "JobSourceId" = ' . self::JOB_SOURCE_FEDERAL . '
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

    /**
     * Serialize a document array to JSON, omitting null values to mirror the
     * C# JsonSerializerSettings { NullValueHandling = Ignore }. Empty arrays,
     * empty strings, false and 0 are preserved (Newtonsoft keeps those).
     */
    private function serialize(?array $document): ?string
    {
        if ($document === null) {
            return null;
        }
        $clean = $this->stripNulls($document);
        return json_encode($clean, JSON_UNESCAPED_UNICODE | JSON_UNESCAPED_SLASHES);
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
            if (is_array($item)) {
                $out[$key] = $this->stripNulls($item);
            } else {
                $out[$key] = $item;
            }
        }
        return $out;
    }
}
