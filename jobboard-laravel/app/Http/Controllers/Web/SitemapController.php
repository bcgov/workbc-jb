<?php

namespace App\Http\Controllers\Web;

use App\Services\Sitemap\SitemapService;
use Illuminate\Http\Response;
use Illuminate\Support\Facades\Cache;

/**
 * SRCH-8 — serves the job sitemap.
 *
 * With ~35 k active jobs × 2 languages ≈ 70 k URLs the Google/sitemaps.org
 * 50 000-URL limit per file is exceeded, so two strategy:
 *   GET /sitemap.xml          — sitemap index listing EN + FR shards
 *   GET /sitemap-en.xml       — all active EN-index job URLs
 *   GET /sitemap-fr.xml       — all active FR-index job URLs
 *
 * Each shard is generated on first request and cached in Redis for
 * SITEMAP_CACHE_TTL seconds (default 4 hours via env). The index is tiny and
 * is never cached — it only contains the two shard <loc> entries.
 *
 * The shard query uses ExpireDate >= now/d (America/Vancouver), the same base
 * filter as every other search path, so expired jobs are excluded at the
 * OpenSearch level. The 4-hour TTL bounds the window in which a just-expired
 * job could still appear in the cached XML (acceptable for SEO crawlers).
 *
 * Rule B (ADR-001): reads the derived index only; never writes or recomputes
 * ExpireDate / LastUpdated.
 */
final class SitemapController
{
    /** Supported language shard keys (must match opensearch.indexes config keys). */
    private const SHARDS = ['en', 'fr'];

    /** Default cache TTL in seconds (4 hours). Override with SITEMAP_CACHE_TTL env. */
    private const DEFAULT_TTL = 4 * 60 * 60;

    public function __construct(private readonly SitemapService $service) {}

    /**
     * GET /sitemap.xml — sitemap index listing the EN and FR shard URLs.
     */
    public function index(): Response
    {
        $xml = view('sitemap.index', ['shards' => self::SHARDS])->render();

        return response($xml, 200, ['Content-Type' => 'text/xml; charset=utf-8']);
    }

    /**
     * GET /sitemap-{language}.xml — cached shard of all active job URLs for
     * the given language index. Cached in Redis (or the configured cache store).
     */
    public function shard(string $language): Response
    {
        $ttl = (int) env('SITEMAP_CACHE_TTL', self::DEFAULT_TTL);
        $indexKey = in_array($language, ['en', 'fr'], true) ? $language : 'en';
        $index = (string) config("opensearch.indexes.{$indexKey}", config('opensearch.indexes.en'));

        $xml = Cache::remember("sitemap.shard.{$language}", $ttl, function () use ($index, $language): string {
            $jobs = $this->service->activeJobs($index);

            return view('sitemap.shard', compact('jobs', 'language'))->render();
        });

        return response($xml, 200, ['Content-Type' => 'text/xml; charset=utf-8']);
    }
}
