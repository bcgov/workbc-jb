<?php

namespace Tests\Feature\Integration;

use App\Services\Integration\CachedGeocoder;
use App\Search\Support\GeoPoint;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;
use Tests\TestCase;
use Throwable;

/**
 * Adapter logic for {@see CachedGeocoder}: cache-first resolution from
 * GeocodedLocationCache. This exercises the real Eloquent model against a
 * throwaway in-memory SQLite table (never the production PascalCase schema —
 * map, don't create). The live seeded dev cache is verified separately.
 */
class CachedGeocoderTest extends TestCase
{
    private bool $ready = false;

    protected function setUp(): void
    {
        parent::setUp();

        config([
            'database.default' => 'geocache_mem',
            'database.connections.geocache_mem' => [
                'driver' => 'sqlite',
                'database' => ':memory:',
                'prefix' => '',
                'foreign_key_constraints' => false,
            ],
        ]);

        try {
            Schema::create('GeocodedLocationCache', function ($table): void {
                $table->integer('Id');
                $table->string('Name');
                $table->string('Latitude')->nullable();
                $table->string('Longitude')->nullable();
                $table->string('City')->nullable();
                $table->string('FrenchCity')->nullable();
                $table->string('Province')->nullable();
                $table->dateTime('DateGeocoded')->nullable();
                $table->boolean('IsPermanent')->nullable();
            });
            $this->ready = true;
        } catch (Throwable $e) {
            $this->markTestSkipped('SQLite not available for the geocoder cache test: '.$e->getMessage());
        }
    }

    private function seedRow(string $name, ?string $lat, ?string $lon): void
    {
        DB::table('GeocodedLocationCache')->insert([
            'Id' => 1,
            'Name' => $name,
            'Latitude' => $lat,
            'Longitude' => $lon,
            'IsPermanent' => true,
        ]);
    }

    public function test_it_resolves_coordinates_for_a_cached_location(): void
    {
        $this->seedRow('VICTORIA, BC, CANADA', '48.4284', '-123.3656');

        $point = (new CachedGeocoder())->resolve('VICTORIA, BC, CANADA');

        $this->assertInstanceOf(GeoPoint::class, $point);
        $this->assertEqualsWithDelta(48.4284, $point->lat, 0.0001);
        $this->assertEqualsWithDelta(-123.3656, $point->lon, 0.0001);
    }

    public function test_it_returns_null_on_a_cache_miss(): void
    {
        $this->assertNull((new CachedGeocoder())->resolve('ATLANTIS, BC, CANADA'));
    }

    public function test_it_returns_null_when_stored_coordinates_are_not_numeric(): void
    {
        $this->seedRow('BROKEN, BC, CANADA', '', null);

        $this->assertNull((new CachedGeocoder())->resolve('BROKEN, BC, CANADA'));
    }
}
