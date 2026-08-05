<?php

namespace Tests\Feature\Api;

use Mockery;
use OpenSearch\Client;
use Tests\TestCase;

/**
 * SRCH-10 — GET /api/Search/gettotaljobs (contracts.md §2.2).
 */
class TotalJobsApiTest extends TestCase
{
    public function test_it_returns_the_active_job_count(): void
    {
        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')
            ->once()
            ->andReturn(['hits' => ['total' => ['value' => 37831], 'hits' => []]]);

        $this->app->instance(Client::class, $client);

        $response = $this->getJson('/api/Search/gettotaljobs');

        $response->assertOk();
        $response->assertExactJson(['count' => 37831]);
    }

    public function test_it_does_not_fetch_any_hit_documents(): void
    {
        $captured = [];
        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')
            ->andReturnUsing(function (array $params) use (&$captured): array {
                $captured = $params['body'];

                return ['hits' => ['total' => ['value' => 5], 'hits' => []]];
            });

        $this->app->instance(Client::class, $client);

        $this->getJson('/api/Search/gettotaljobs')->assertOk();

        $this->assertSame(0, $captured['size']);
        $this->assertTrue($captured['track_total_hits']);
    }

    protected function tearDown(): void
    {
        Mockery::close();
        parent::tearDown();
    }
}
