<?php

namespace Tests\Feature\Search;

use App\Search\Filters\JobSearchFilters;
use App\Search\Queries\JobSearchQuery;
use App\Search\Results\SearchResult;
use OpenSearch\Client;
use Tests\TestCase;

/**
 * End-to-end check of the search foundation against the local jobs_en index
 * (the 500 fake docs from `php artisan dev:index-jobs --fresh`). The test skips
 * gracefully when OpenSearch is unreachable or the index is empty, so it stays
 * green in environments without a cluster while still validating the real query
 * path (structured body → OpenSearch → SearchResult projection) when one is up.
 *
 * Read-only (Rule B): this only reads jobs_en; it never writes or reindexes.
 */
class JobSearchIntegrationTest extends TestCase
{
    private const INDEX = 'jobs_en';

    private function reachableClient(): Client
    {
        /** @var Client $client */
        $client = app(Client::class);

        try {
            if (! $client->indices()->exists(['index' => self::INDEX])) {
                $this->markTestSkipped(self::INDEX.' index not present. Run: php artisan dev:index-jobs --fresh');
            }

            $count = $client->count(['index' => self::INDEX])['count'] ?? 0;
            if ($count === 0) {
                $this->markTestSkipped(self::INDEX.' is empty. Run: php artisan dev:index-jobs --fresh');
            }
        } catch (\Throwable $e) {
            $this->markTestSkipped('OpenSearch unreachable: '.$e->getMessage());
        }

        return $client;
    }

    /**
     * @param  array<string, mixed>  $overrides
     */
    private function search(Client $client, array $overrides = []): SearchResult
    {
        $filters = JobSearchFilters::fromArray($overrides);
        $body = (new JobSearchQuery($filters))->build();

        $response = $client->search(['index' => self::INDEX, 'body' => $body]);

        return SearchResult::fromOpenSearchResponse(
            $response,
            $filters->Page <= 0 ? 1 : $filters->Page,
            $filters->PageSize,
        );
    }

    public function test_default_search_returns_active_jobs(): void
    {
        $client = $this->reachableClient();

        $result = $this->search($client, ['PageSize' => 20]);

        $this->assertGreaterThan(0, $result->count, 'Expected active jobs in the fake corpus');
        $this->assertLessThanOrEqual(20, count($result->results));

        $first = $result->results[0]->toArray();
        $this->assertArrayHasKey('JobId', $first);
        $this->assertArrayHasKey('Title', $first);
    }

    public function test_page_size_is_respected(): void
    {
        $client = $this->reachableClient();

        $result = $this->search($client, ['PageSize' => 5]);

        $this->assertLessThanOrEqual(5, count($result->results));
        $this->assertSame(5, $result->pageSize);
    }

    public function test_federal_source_filter_returns_only_federal_jobs(): void
    {
        $client = $this->reachableClient();

        $result = $this->search($client, ['SearchJobSource' => '1', 'PageSize' => 25]);

        if ($result->count === 0) {
            $this->markTestSkipped('No federal jobs in the current corpus.');
        }

        foreach ($result->results as $job) {
            $this->assertTrue($job->toArray()['IsFederalJob'], 'Federal-source filter returned a non-federal job');
        }
    }

    public function test_excluding_placement_agencies_reduces_or_equals_total(): void
    {
        $client = $this->reachableClient();

        $all = $this->search($client, ['PageSize' => 1])->count;
        $filtered = $this->search($client, ['SearchExcludePlacementAgencyJobs' => true, 'PageSize' => 1])->count;

        $this->assertLessThanOrEqual($all, $filtered);
    }
}
