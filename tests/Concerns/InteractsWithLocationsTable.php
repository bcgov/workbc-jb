<?php

namespace Tests\Concerns;

use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;
use Throwable;

/**
 * A minimal `Locations` reference table + fixtures for tests that exercise
 * {@see \App\Services\Search\LocationService} (city autocomplete + validation now
 * read this table, mirroring production — not the OpenSearch job documents).
 */
trait InteractsWithLocationsTable
{
    protected function createLocationsFixture(): void
    {
        try {
            Schema::create('Locations', function ($table): void {
                $table->integer('LocationId')->primary();
                $table->string('City')->nullable();
                $table->string('Label')->nullable();
                $table->boolean('IsHidden')->default(false);
            });

            DB::table('Locations')->insert([
                ['LocationId' => 1, 'City' => 'Victoria', 'Label' => 'Victoria', 'IsHidden' => false],
                ['LocationId' => 2, 'City' => 'Victoria Harbour', 'Label' => 'Victoria Harbour', 'IsHidden' => false],
                ['LocationId' => 3, 'City' => 'Vancouver', 'Label' => 'Vancouver', 'IsHidden' => false],
                ['LocationId' => 4, 'City' => 'North Vancouver', 'Label' => 'North Vancouver', 'IsHidden' => false],
                ['LocationId' => 5, 'City' => 'Nanaimo', 'Label' => 'Nanaimo', 'IsHidden' => false],
                ['LocationId' => 6, 'City' => 'Surrey', 'Label' => 'Surrey', 'IsHidden' => false],
                ['LocationId' => 7, 'City' => 'Surrey Village', 'Label' => 'Surrey Village', 'IsHidden' => false],
                // Excluded from results: hidden, and the LocationId = 0 sentinel row.
                ['LocationId' => 8, 'City' => 'Hidden Town', 'Label' => 'Hidden Town', 'IsHidden' => true],
                ['LocationId' => 0, 'City' => 'Zeroville', 'Label' => 'Zeroville', 'IsHidden' => false],
            ]);
        } catch (Throwable $e) {
            $this->markTestSkipped('DB not available for the Locations fixture: '.$e->getMessage());
        }
    }

    protected function dropLocationsFixture(): void
    {
        Schema::dropIfExists('Locations');
    }
}
