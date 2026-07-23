<?php

namespace Tests\Feature\Api;

use Mockery;
use OpenSearch\Client;
use Tests\TestCase;

/**
 * SRCH-10 — GET /api/location/cities/{cityName}/{includeRegion} (contracts.md
 * §2.3): city-name autocomplete, delegating to the same LocationService SRCH-2 uses.
 */
class LocationCitiesApiTest extends TestCase
{
    public function test_it_returns_matching_city_names(): void
    {
        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')->andReturn([
            'aggregations' => ['cities' => ['buckets' => [
                ['key' => 'Surrey'],
                ['key' => 'Surrey Village'],
            ]]],
        ]);

        $this->app->instance(Client::class, $client);

        $response = $this->getJson('/api/location/cities/Sur/true');

        $response->assertOk();
        $response->assertExactJson(['Surrey', 'Surrey Village']);
    }

    public function test_it_returns_an_empty_array_for_a_short_term(): void
    {
        $client = Mockery::mock(Client::class);
        $client->shouldNotReceive('search');
        $this->app->instance(Client::class, $client);

        $response = $this->getJson('/api/location/cities/S/true');

        $response->assertOk();
        $response->assertExactJson([]);
    }

    protected function tearDown(): void
    {
        Mockery::close();
        parent::tearDown();
    }
}
