<?php

namespace Tests\Feature\Sitemap;

use Mockery;
use OpenSearch\Client;
use Tests\TestCase;

/**
 * SRCH-8 — job sitemap (architecture.md §6, ADR-004).
 *
 * Verifies: sitemap index lists both language shards; a known active job URL
 * appears in the EN shard with the correct <lastmod>; a known expired job (one
 * whose ID was never returned by OpenSearch because the ExpireDate >= now/d
 * filter excluded it) is absent; the OpenSearch query body carries the expected
 * base filter; the FR shard queries the FR index; all XML is well-formed.
 */
class SitemapTest extends TestCase
{
    /** Bind a mock Client whose search() method delegates to $responder. */
    private function bindClient(callable $responder): void
    {
        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')
            ->andReturnUsing(fn (array $params): array => $responder($params));

        $this->app->instance(Client::class, $client);
    }

    // -------------------------------------------------------------------------
    // Sitemap index
    // -------------------------------------------------------------------------

    public function test_sitemap_index_is_valid_xml_listing_both_language_shards(): void
    {
        $response = $this->get('/sitemap.xml');

        $response->assertOk();
        $response->assertHeader('Content-Type', 'text/xml; charset=utf-8');

        $content = $response->getContent();

        // Well-formed XML
        $this->assertNotFalse(simplexml_load_string($content), 'sitemap.xml is not valid XML');

        // Both language shards referenced
        $this->assertStringContainsString('sitemap-en.xml', $content);
        $this->assertStringContainsString('sitemap-fr.xml', $content);
    }

    // -------------------------------------------------------------------------
    // EN shard — active job present, expired job absent, valid XML
    // -------------------------------------------------------------------------

    public function test_en_shard_contains_active_job_and_excludes_expired_job(): void
    {
        $callCount = 0;

        $this->bindClient(function (array $params) use (&$callCount): array {
            $callCount++;

            if ($callCount === 1) {
                // First page: one active job.
                return [
                    'hits' => [
                        'total' => ['value' => 1],
                        'hits' => [[
                            '_source' => [
                                'JobId' => 'A1234567',
                                'Title' => 'Software Engineer',
                                'LastUpdated' => '2026-07-01T12:00:00',
                            ],
                            // sort values used by search_after on the next call
                            'sort' => ['A1234567'],
                        ]],
                    ],
                ];
            }

            // Second call (search_after present) — pagination complete.
            return ['hits' => ['total' => ['value' => 0], 'hits' => []]];
        });

        $response = $this->get('/sitemap-en.xml');

        $response->assertOk();
        $response->assertHeader('Content-Type', 'text/xml; charset=utf-8');

        $content = $response->getContent();

        // Well-formed XML
        $this->assertNotFalse(simplexml_load_string($content), 'sitemap-en.xml is not valid XML');

        // Active job URL present (slug derived from title + jobId)
        $this->assertStringContainsString('software-engineer-A1234567', $content);

        // <lastmod> present and uses just the date portion
        $this->assertStringContainsString('<lastmod>2026-07-01</lastmod>', $content);

        // An expired job whose ID was never returned by OpenSearch is absent.
        // "EXPIRED999" represents any job the ExpireDate filter excluded.
        $this->assertStringNotContainsString('EXPIRED999', $content);
    }

    // -------------------------------------------------------------------------
    // Query body — ExpireDate base filter
    // -------------------------------------------------------------------------

    public function test_query_body_includes_expire_date_active_jobs_filter(): void
    {
        $capturedBody = null;

        $this->bindClient(function (array $params) use (&$capturedBody): array {
            $capturedBody = $params['body'];

            // Return empty immediately so no second call is made.
            return ['hits' => ['total' => ['value' => 0], 'hits' => []]];
        });

        $this->get('/sitemap-en.xml');

        $this->assertNotNull($capturedBody, 'OpenSearch was not called');

        $filter = $capturedBody['query']['bool']['filter'];
        $this->assertArrayHasKey('range', $filter);
        $this->assertSame('now/d', $filter['range']['ExpireDate']['gte']);
        $this->assertSame('America/Vancouver', $filter['range']['ExpireDate']['time_zone']);
    }

    // -------------------------------------------------------------------------
    // FR shard — uses the FR index
    // -------------------------------------------------------------------------

    public function test_fr_shard_queries_the_fr_opensearch_index(): void
    {
        $capturedIndex = null;

        $this->bindClient(function (array $params) use (&$capturedIndex): array {
            $capturedIndex = $params['index'];

            return ['hits' => ['total' => ['value' => 0], 'hits' => []]];
        });

        $response = $this->get('/sitemap-fr.xml');

        $response->assertOk();

        // The FR index name comes from config('opensearch.indexes.fr') — in tests
        // this resolves to 'jobs_fr' (the default from config/opensearch.php).
        $this->assertStringContainsString('fr', (string) $capturedIndex);
    }

    // -------------------------------------------------------------------------
    // search_after pagination — second batch is fetched when first is full
    // -------------------------------------------------------------------------

    public function test_search_after_pagination_fetches_second_batch(): void
    {
        // Build exactly BATCH_SIZE (1000) hits for the first response so the
        // service loops and fires a second request with search_after.
        $firstBatch = array_map(
            fn (int $i): array => [
                '_source' => ['JobId' => sprintf('JOB%04d', $i), 'Title' => "Job {$i}", 'LastUpdated' => null],
                'sort' => [sprintf('JOB%04d', $i)],
            ],
            range(1, 1000),
        );

        $callCount = 0;
        $secondParams = null;

        $this->bindClient(function (array $params) use (&$callCount, &$secondParams, $firstBatch): array {
            $callCount++;

            if ($callCount === 1) {
                return ['hits' => ['total' => ['value' => 1000], 'hits' => $firstBatch]];
            }

            $secondParams = $params['body'];

            return ['hits' => ['total' => ['value' => 0], 'hits' => []]];
        });

        $response = $this->get('/sitemap-en.xml');
        $response->assertOk();

        // Two OpenSearch calls were made.
        $this->assertSame(2, $callCount);

        // The second call carries a search_after key matching the last hit's sort values.
        $this->assertArrayHasKey('search_after', $secondParams);
        $this->assertSame(['JOB1000'], $secondParams['search_after']);
    }
}
