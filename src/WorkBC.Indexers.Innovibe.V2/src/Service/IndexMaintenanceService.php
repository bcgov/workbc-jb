<?php

declare(strict_types=1);

namespace WorkBC\InnovibeJobIndexer\Service;

use Monolog\Logger;
use PDO;
use WorkBC\InnovibeJobIndexer\Config\AppConfig;
use WorkBC\InnovibeJobIndexer\Elastic\ElasticClient;

/**
 * Index-level maintenance: recreate indexes (--reindex) and close/reopen them
 * to reload synonyms (--reopen).
 *
 * Ports WorkBC.ElasticSearch.Indexing.Services.ReIndexingService.
 */
final class IndexMaintenanceService
{
    public function __construct(
        private readonly PDO $db,
        private readonly ElasticClient $elastic,
        private readonly AppConfig $config,
        private readonly Logger $log,
    ) {}

    /** @return list<string> */
    private function indexes(): array
    {
        return [$this->config->indexEn, $this->config->indexFr];
    }

    /**
     * Drop both indexes, recreate them from jobs_index.json.template, and flag every
     * staging row (Wanted + Federal) for re-indexing.
     */
    public function reCreateIndex(): void
    {
        $indexes = $this->indexes();

        $this->log->info('Removing index(es) ' . implode(',', $indexes));
        foreach ($indexes as $index) {
            $this->elastic->deleteIndex($index);
        }

        $this->log->info('Creating index(es) ' . implode(',', $indexes));
        foreach ($indexes as $index) {
            // The .NET ReIndexingService calls CreateIndexService.Create(false),
            // i.e. it uses the inline (predefined) synonyms rather than the
            // synonyms_path file. We match that so a reindex does not depend on
            // a synonym.txt being present on the ES node.
            if ($this->elastic->createIndex($index, false)) {
                $this->log->info("Index created - {$index}");
            } else {
                $this->log->error("Index NOT created - {$index}");
            }
        }

        $this->log->info('Updating jobs to re-index...');
        $this->flagAllJobsForReIndexing();
        $this->log->info('Done. Indexing will continue with the new index');
    }

    /** Close then reopen both indexes (reloads the synonym file). */
    public function reCloseAndReOpenIndexes(): void
    {
        foreach ($this->indexes() as $index) {
            $this->elastic->closeAndReopen($index);
        }
    }

    /**
     * Flags ALL rows in both staging tables. Mirrors the C#
     * ReIndexingService.FlagAllJobsForReIndexing (which deliberately touches
     * both Wanted and Federal so a shared reindex covers every source).
     */
    private function flagAllJobsForReIndexing(): void
    {
        $this->db->exec('UPDATE "ImportedJobsWanted" SET "ReIndexNeeded" = TRUE');
        $this->db->exec('UPDATE "ImportedJobsFederal" SET "ReIndexNeeded" = TRUE');
    }
}
