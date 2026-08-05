<?php

namespace Tests\Feature\Api;

use PHPUnit\Framework\Attributes\DataProvider;
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

    /**
     * ASP.NET matched case-insensitively, so the .NET smoke tests call
     * `/api/Location/cities/...` with a capital L while contracts.md published
     * the lowercase form. Laravel matches case-sensitively, so both have to be
     * registered — see the comment in routes/api.php.
     */
    #[DataProvider('casingProvider')]
    public function test_it_accepts_every_path_casing_in_live_use(string $path): void
    {
        $response = $this->getJson($path);

        $response->assertOk();
        $response->assertExactJson(['Surrey', 'Surrey Village']);
    }

    public static function casingProvider(): array
    {
        return [
            'casing used by real callers (cases.txt)' => ['/api/Location/cities/Sur/true'],
            'casing published in our contracts.md' => ['/api/location/cities/Sur/true'],
        ];
    }
}
