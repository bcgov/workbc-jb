<?php

namespace Tests\Feature\Search;

use App\Services\Search\LocationService;
use PHPUnit\Framework\Attributes\DataProvider;
use Tests\Concerns\InteractsWithLocationsTable;
use Tests\TestCase;

/**
 * LocationService reads the curated `Locations` reference table (mirroring the
 * production LocationController), so suggestions are canonical and validation
 * accepts any known B.C. place — not only cities with an active posting.
 */
class LocationServiceTest extends TestCase
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

    /**
     * @return array<string, array{0: string, 1: bool}>
     */
    public static function postalCodes(): array
    {
        return [
            'valid no space' => ['V8W1P6', true],
            'valid with space' => ['V8W 1P6', true],
            'valid lowercase' => ['v8w 1p6', true],
            'city name' => ['Victoria', false],
            'invalid leading D' => ['D1A 1A1', false],
            'too short' => ['V8W 1P', false],
            'empty' => ['', false],
        ];
    }

    #[DataProvider('postalCodes')]
    public function test_is_postal_code_recognises_canadian_postal_codes(string $input, bool $expected): void
    {
        $this->assertSame($expected, (new LocationService())->isPostalCode($input));
    }

    public function test_suggest_cities_prefix_matches_sort_alphabetically(): void
    {
        $out = (new LocationService())->suggestCities('Vic');

        $this->assertSame(['Victoria', 'Victoria Harbour'], $out);
    }

    public function test_suggest_cities_matches_word_start_but_ranks_prefix_first(): void
    {
        // "van": Vancouver prefix-matches; North Vancouver word-start-matches.
        $out = (new LocationService())->suggestCities('van');

        $this->assertSame(['Vancouver', 'North Vancouver'], $out);
    }

    public function test_suggest_cities_is_case_insensitive(): void
    {
        $this->assertSame(['Surrey', 'Surrey Village'], (new LocationService())->suggestCities('sur'));
    }

    public function test_suggest_cities_excludes_hidden_and_nonpositive_ids(): void
    {
        $this->assertSame([], (new LocationService())->suggestCities('Hidden'));
        $this->assertSame([], (new LocationService())->suggestCities('Zero'));
    }

    public function test_suggest_cities_short_circuits_below_two_characters(): void
    {
        $this->assertSame([], (new LocationService())->suggestCities('V'));
    }

    public function test_city_exists_is_a_case_insensitive_exact_match_on_visible_locations(): void
    {
        $service = new LocationService();

        $this->assertTrue($service->cityExists('Victoria'));
        $this->assertTrue($service->cityExists('victoria'));   // case-insensitive
        $this->assertFalse($service->cityExists('Vic'));       // not an exact match
        $this->assertFalse($service->cityExists('Hidden Town')); // hidden
        $this->assertFalse($service->cityExists('Nowherightsville'));
        $this->assertFalse($service->cityExists(''));
    }
}
