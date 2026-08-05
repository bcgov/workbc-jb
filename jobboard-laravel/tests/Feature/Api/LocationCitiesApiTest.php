<?php

namespace Tests\Feature\Api;

use Tests\Concerns\InteractsWithLocationsTable;
use Tests\TestCase;

/**
 * SRCH-10 — GET /api/location/cities/{cityName}/{includeRegion} (contracts.md
 * §2.3): city-name autocomplete, delegating to the same LocationService SRCH-2
 * uses, which reads the curated `Locations` table.
 */
class LocationCitiesApiTest extends TestCase
{
    use InteractsWithLocationsTable;

    protected function setUp(): void
    {
        parent::setUp();
        $this->createLocationsFixture();
    }

    protected function tearDown(): void
    {
        $this->dropLocationsFixture();
        parent::tearDown();
    }

    public function test_it_returns_matching_city_names(): void
    {
        $response = $this->getJson('/api/location/cities/Sur/true');

        $response->assertOk();
        $response->assertExactJson(['Surrey', 'Surrey Village']);
    }

    public function test_it_returns_an_empty_array_for_a_short_term(): void
    {
        $response = $this->getJson('/api/location/cities/S/true');

        $response->assertOk();
        $response->assertExactJson([]);
    }
}
