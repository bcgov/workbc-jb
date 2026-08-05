<?php

namespace App\Services\Sitemap;

use OpenSearch\Client;

/**
 * SRCH-8 — iterates ALL active jobs from a given derived OpenSearch index using
 * search_after pagination (Rule B / ADR-001: read-only, never recompute derived
 * fields). Each page requests BATCH_SIZE documents sorted by JobId.keyword so
 * the cursor is stable across calls.
 *
 * Serving strategy (SRCH-8): the sitemap routes cache the rendered XML in Redis
 * (TTL: SITEMAP_CACHE_TTL env, default 4 hours). Expired jobs are excluded at
 * the query level via ExpireDate >= now/d (America/Vancouver), the same base
 * filter used by every other search path (architecture.md §5.3, Rule B).
 *
 * No artisan command or queued job is required — the Redis cache absorbs repeat
 * traffic; the 4-hour TTL bounds the window in which a newly-expired job might
 * still appear. If a scheduled regeneration is preferred in future, a k8s
 * CronJob → artisan command pattern (ADR-004) can be layered on top by calling
 * Cache::forget('sitemap.shard.en') + Cache::forget('sitemap.shard.fr') before
 * priming the cache with a warm-up request.
 */
final class SitemapService
{
    /** Documents fetched per OpenSearch request. */
    private const BATCH_SIZE = 1000;

    private const TIME_ZONE = 'America/Vancouver';

    /** Only the fields the sitemap needs — Title for the slug, LastUpdated for <lastmod>. */
    private const SOURCE_FIELDS = ['JobId', 'Title', 'LastUpdated'];

    public function __construct(private readonly Client $client) {}

    /**
     * Yield every active job from $index as a small associative array.
     * Callers (Blade views) iterate this generator; no full result set is built
     * in memory at once.
     *
     * @return iterable<array{jobId: string, title: string|null, lastUpdated: string|null}>
     */
    public function activeJobs(string $index): iterable
    {
        /** @var array<mixed>|null $searchAfter sort values from the last hit */
        $searchAfter = null;

        do {
            $response = $this->client->search([
                'index' => $index,
                'body' => $this->queryBody($searchAfter),
            ]);

            $hits = $response['hits']['hits'] ?? [];

            if ($hits === []) {
                break;
            }

            foreach ($hits as $hit) {
                $source = $hit['_source'] ?? [];
                yield [
                    'jobId' => (string) ($source['JobId'] ?? ''),
                    'title' => isset($source['Title']) ? (string) $source['Title'] : null,
                    'lastUpdated' => isset($source['LastUpdated']) ? (string) $source['LastUpdated'] : null,
                ];
            }

            // Advance the cursor using the sort values on the last hit.
            $last = end($hits);
            $searchAfter = $last['sort'] ?? null;

        } while (count($hits) === self::BATCH_SIZE);
    }

    /**
     * @param  array<mixed>|null  $searchAfter
     * @return array<string, mixed>
     */
    private function queryBody(?array $searchAfter): array
    {
        $body = [
            '_source' => self::SOURCE_FIELDS,
            'size' => self::BATCH_SIZE,
            // Stable sort by keyword JobId — required for deterministic search_after pagination.
            'sort' => [['JobId.keyword' => 'asc']],
            'query' => [
                'bool' => [
                    'filter' => [
                        'range' => [
                            'ExpireDate' => [
                                'gte' => 'now/d',
                                'time_zone' => self::TIME_ZONE,
                            ],
                        ],
                    ],
                ],
            ],
        ];

        if ($searchAfter !== null) {
            $body['search_after'] = $searchAfter;
        }

        return $body;
    }
}
