<?php

namespace Tests\Feature\Api;

use Mockery;
use OpenSearch\Client;
use PHPUnit\Framework\Attributes\DataProvider;
use Tests\TestCase;

/**
 * SRCH-10 — GET /api/Search/GetTotalJobs (contracts.md §2.2).
 *
 * These tests encode a contract that was originally recorded WRONG: the doc and
 * this file both claimed `{"count": n}`, while WorkBC.Web actually returns a
 * bare integer. Drupal's sitewide count rendered "Search NaN jobs in B.C."
 * as a result. The shape and the accepted path casings below were verified
 * against the .NET dev origin on 2026-08-05 — treat them as observed facts, not
 * preferences.
 */
class TotalJobsApiTest extends TestCase
{
    /**
     * Stub OpenSearch and record the search params it was called with.
     */
    private function fakeSearch(int $total, ?array &$captured = null): void
    {
        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')
            ->andReturnUsing(function (array $params) use ($total, &$captured): array {
                $captured = $params;

                return ['hits' => ['total' => ['value' => $total], 'hits' => []]];
            });

        $this->app->instance(Client::class, $client);
    }

    public function test_it_returns_the_active_job_count_as_a_bare_integer(): void
    {
        $this->fakeSearch(37831);

        $response = $this->getJson('/api/Search/GetTotalJobs');

        $response->assertOk();

        // The whole body is the number. Not ['count' => 37831] — a consumer
        // doing Number(body) on an object gets NaN.
        $this->assertSame(37831, $response->json());
        $this->assertSame('37831', $response->getContent());
    }

    /**
     * ASP.NET matched case-insensitively; Laravel does not. Both casings that
     * appear in live callers/docs must resolve to the same action.
     */
    #[DataProvider('casingProvider')]
    public function test_it_accepts_every_path_casing_in_live_use(string $path): void
    {
        $this->fakeSearch(42802);

        $response = $this->getJson($path);

        $response->assertOk();
        $this->assertSame(42802, $response->json());
    }

    public static function casingProvider(): array
    {
        return [
            'casing used by real callers (cases.txt)' => ['/api/Search/GetTotalJobs'],
            'casing published in our contracts.md' => ['/api/Search/gettotaljobs'],
        ];
    }

    public function test_it_reads_language_from_the_path_segment(): void
    {
        $this->fakeSearch(120, $captured);

        $this->getJson('/api/Search/GetTotalJobs/fr')->assertOk();

        $this->assertSame('jobs_fr', $captured['index']);
    }

    /**
     * `?language=fr` is exercised by the .NET smoke tests, so a caller may well
     * use it. Before the fix this silently returned the ENGLISH count.
     */
    public function test_it_reads_language_from_the_query_string(): void
    {
        $this->fakeSearch(120, $captured);

        $this->getJson('/api/Search/GetTotalJobs?language=fr')->assertOk();

        $this->assertSame('jobs_fr', $captured['index']);
    }

    public function test_it_defaults_to_the_english_index(): void
    {
        $this->fakeSearch(120, $captured);

        $this->getJson('/api/Search/GetTotalJobs')->assertOk();

        $this->assertSame('jobs_en', $captured['index']);
    }

    public function test_it_does_not_fetch_any_hit_documents(): void
    {
        $this->fakeSearch(5, $captured);

        $this->getJson('/api/Search/GetTotalJobs')->assertOk();

        $this->assertSame(0, $captured['body']['size']);
        $this->assertTrue($captured['body']['track_total_hits']);
    }

    protected function tearDown(): void
    {
        Mockery::close();
        parent::tearDown();
    }
}
