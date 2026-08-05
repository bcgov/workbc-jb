<?php

namespace App\Console\Commands;

use Illuminate\Console\Command;
use OpenSearch\Client;
use Throwable;

/**
 * FND-1 health check: proves the app can reach the OpenSearch cluster and
 * reports the status of the two derived indexes it reads (jobs_en / jobs_fr).
 * Read-only — never creates or writes indexes (Rule B, ADR-001).
 */
class OpenSearchHealth extends Command
{
    protected $signature = 'opensearch:health';

    protected $description = 'Ping the OpenSearch cluster and report the jobs_en/jobs_fr index status';

    public function handle(Client $client): int
    {
        try {
            $health = $client->cluster()->health();
        } catch (Throwable $e) {
            $this->error('OpenSearch unreachable: '.$e->getMessage());

            return self::FAILURE;
        }

        $this->info(sprintf(
            'Cluster "%s": status=%s, nodes=%d',
            $health['cluster_name'] ?? '?',
            $health['status'] ?? '?',
            $health['number_of_nodes'] ?? 0,
        ));

        $allPresent = true;

        foreach (config('opensearch.indexes') as $locale => $index) {
            $exists = (bool) $client->indices()->exists(['index' => $index]);
            $count = $exists ? ($client->count(['index' => $index])['count'] ?? 0) : 0;

            $this->line(sprintf(
                '  %s [%s]: %s%s',
                $index,
                $locale,
                $exists ? '<info>present</info>' : '<comment>MISSING</comment>',
                $exists ? sprintf(' (%s docs)', number_format((int) $count)) : '',
            ));

            $allPresent = $allPresent && $exists;
        }

        if (! $allPresent) {
            $this->newLine();
            $this->warn('One or more indexes are missing. Load them from a TEST copy — the app does not build them (indexer is an external container).');
        }

        return self::SUCCESS;
    }
}
