<?php

namespace Tests\Unit\Search;

use App\Search\Support\MapPins;
use PHPUnit\Framework\TestCase;

/**
 * SRCH-9 pin-selection tests — a faithful port of the C#
 * JobSearchQuery.GetMapPins behaviour: single-location jobs plot their one
 * point; multi-location jobs are pinned at the most-frequent city (then region),
 * otherwise every location; and the whole list is capped at 5000.
 */
class MapPinsTest extends TestCase
{
    /**
     * @param  array<int, array{Lat: string, Lon: string}>  $locations
     * @param  string[]  $city
     * @param  string[]  $region
     * @return array<string, mixed>
     */
    private function job(string $jobId, array $locations, array $city = [], array $region = [], string $title = ''): array
    {
        return [
            'JobId' => $jobId,
            'Title' => $title,
            'City' => $city,
            'Region' => $region,
            'Location' => $locations,
        ];
    }

    public function test_single_location_job_is_pinned_once(): void
    {
        $pins = MapPins::fromSources([
            $this->job('1', [['Lat' => '48.4', 'Lon' => '-123.3']], ['Victoria'], title: 'Baker'),
        ]);

        $this->assertCount(1, $pins);
        $this->assertSame([
            'JobId' => '1',
            'Latitude' => '48.4',
            'Longitude' => '-123.3',
            'Title' => 'Baker',
        ], $pins[0]);
    }

    public function test_jobs_with_no_location_are_skipped(): void
    {
        $pins = MapPins::fromSources([
            $this->job('1', [], ['Victoria']),
            $this->job('2', [['Lat' => '49.2', 'Lon' => '-123.1']], ['Vancouver']),
        ]);

        $this->assertCount(1, $pins);
        $this->assertSame('2', $pins[0]['JobId']);
    }

    public function test_multi_location_job_is_pinned_at_the_most_frequent_city(): void
    {
        // Vancouver is the most frequent city across the result set, so the
        // multi-location job (index 0) is pinned at its Vancouver coordinates.
        $pins = MapPins::fromSources([
            $this->job('a', [['Lat' => '49.2', 'Lon' => '-123.1']], ['Vancouver']),
            $this->job('b', [['Lat' => '49.2', 'Lon' => '-123.1']], ['Vancouver']),
            $this->job('multi',
                [
                    ['Lat' => '49.2', 'Lon' => '-123.1'], // Vancouver
                    ['Lat' => '48.4', 'Lon' => '-123.3'], // Victoria
                ],
                ['Vancouver', 'Victoria'],
            ),
        ]);

        $multiPins = array_values(array_filter($pins, static fn (array $p): bool => $p['JobId'] === 'multi'));
        $this->assertCount(1, $multiPins);
        $this->assertSame('49.2', $multiPins[0]['Latitude']);
        $this->assertSame('-123.1', $multiPins[0]['Longitude']);
    }

    public function test_multi_location_job_falls_back_to_most_frequent_region(): void
    {
        // No city matches, but the multi-region job shares "Mainland/Southwest"
        // with the most frequent single-region job → pin that region's location.
        $pins = MapPins::fromSources([
            $this->job('single', [['Lat' => '49.2', 'Lon' => '-123.1']], ['Vancouver'], ['Mainland/Southwest']),
            $this->job('multi',
                [
                    ['Lat' => '53.9', 'Lon' => '-122.7'], // Cariboo (index 0)
                    ['Lat' => '49.2', 'Lon' => '-123.1'], // Mainland/Southwest (index 1)
                ],
                ['Prince George', 'Squamish'],
                ['Cariboo', 'Mainland/Southwest'],
            ),
        ]);

        $multiPins = array_values(array_filter($pins, static fn (array $p): bool => $p['JobId'] === 'multi'));
        $this->assertCount(1, $multiPins);
        $this->assertSame('49.2', $multiPins[0]['Latitude']);
    }

    public function test_multi_location_job_with_no_match_plots_every_location(): void
    {
        // Distinct cities/regions with no shared "winner" → all locations plotted.
        $pins = MapPins::fromSources([
            $this->job('multi',
                [
                    ['Lat' => '53.9', 'Lon' => '-122.7'],
                    ['Lat' => '54.5', 'Lon' => '-128.6'],
                ],
                ['Prince George', 'Terrace'],
                ['Cariboo', 'North Coast'],
            ),
        ]);

        $this->assertCount(2, $pins);
        $this->assertSame('multi', $pins[0]['JobId']);
        $this->assertSame('multi', $pins[1]['JobId']);
    }

    public function test_virtual_city_is_not_split(): void
    {
        // A "Virtual" city string must never be comma-split; with two locations
        // and no city/region match, both are plotted.
        $pins = MapPins::fromSources([
            $this->job('v',
                [
                    ['Lat' => '49.2', 'Lon' => '-123.1'],
                    ['Lat' => '48.4', 'Lon' => '-123.3'],
                ],
                ['Virtual, Anywhere'],
            ),
        ]);

        $this->assertCount(2, $pins);
    }

    public function test_result_is_capped_at_five_thousand_pins(): void
    {
        // 3000 jobs each with 2 unmatched locations → 6000 raw pins, capped at 5000.
        $sources = [];
        for ($i = 0; $i < 3000; $i++) {
            $sources[] = $this->job((string) $i,
                [
                    ['Lat' => '50.0', 'Lon' => '-120.0'],
                    ['Lat' => '51.0', 'Lon' => '-121.0'],
                ],
                ['CityA'.$i, 'CityB'.$i],
                ['RegionA'.$i, 'RegionB'.$i],
            );
        }

        $pins = MapPins::fromSources($sources);

        $this->assertCount(MapPins::MAX_PINS, $pins);
    }

    public function test_city_may_be_a_plain_string_from_the_index(): void
    {
        // Robust to either the array (index) or CSV-string shape of City.
        $pins = MapPins::fromSources([
            $this->job('1', [['Lat' => '48.4', 'Lon' => '-123.3']]) + ['City' => 'Victoria'],
        ]);

        $this->assertCount(1, $pins);
        $this->assertSame('48.4', $pins[0]['Latitude']);
    }
}
