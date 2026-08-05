<?php

namespace App\Search\Support;

/**
 * Turns the map query's OpenSearch hits into Google Maps pins (SRCH-9).
 *
 * A faithful port of the C# JobSearchQuery.GetMapPins
 * (WorkBC.ElasticSearch.Search/Queries/JobSearchQuery.cs): a job with a single
 * location plots that location; a job with multiple locations is pinned at the
 * one that best matches where the user is searching — the most frequent city in
 * the result set, falling back to the most frequent region, otherwise every one
 * of its locations. The whole list is then capped at {@see MAX_PINS} because
 * multi-location jobs can otherwise push it past the query's 5000-hit ceiling.
 *
 * Read-model only (Rule B): it copies coordinates the indexer already resolved;
 * it never geocodes or recomputes anything.
 *
 * Note on the index shape: the OpenSearch document stores City as an ARRAY, but
 * the C# model deserializes it to a comma-separated STRING (ListToCsvConverter)
 * and the selection logic reasons about that string — so this port first joins
 * the City array with ", " to reproduce the exact same behaviour.
 */
final class MapPins
{
    /** Matches the C# GetMapPins truncation (and jobsearch_googlemap.json size). */
    public const MAX_PINS = 5000;

    /** The C# convention for virtual jobs — never split their (single) city. */
    private const VIRTUAL_PREFIX = 'Virtual';

    /**
     * @param  array<int, array<string, mixed>>  $sources  the hits' `_source` docs
     *                                                      (JobId, City, Region, Location, Title)
     * @return array<int, array{JobId: string, Latitude: string, Longitude: string, Title: string}>
     */
    public static function fromSources(array $sources): array
    {
        $mostFrequentCity = self::mostFrequent(
            array_map(static fn (array $s): string => self::cityCsv($s['City'] ?? null), $sources),
            allowEmpty: true,
        );

        // Only jobs with exactly one region contribute a region key; the empty
        // placeholder for the rest is skipped when picking the winner.
        $mostFrequentRegion = self::mostFrequent(
            array_map(static function (array $s): string {
                $region = self::stringList($s['Region'] ?? null);

                return count($region) === 1 ? $region[0] : '';
            }, $sources),
            allowEmpty: false,
        );

        $pins = [];

        foreach ($sources as $source) {
            $locations = self::locations($source['Location'] ?? null);
            if ($locations === []) {
                continue;
            }

            $cityCsv = self::cityCsv($source['City'] ?? null);
            $regions = self::stringList($source['Region'] ?? null);
            $jobId = (string) ($source['JobId'] ?? '');
            $title = isset($source['Title']) ? (string) $source['Title'] : '';

            $bestIndex = self::bestLocationIndex($locations, $cityCsv, $regions, $mostFrequentCity, $mostFrequentRegion);

            foreach ($locations as $i => $geo) {
                // Plot the chosen location, or every location when none matched.
                if ($i === $bestIndex || $bestIndex === -1) {
                    $pins[] = [
                        'JobId' => $jobId,
                        'Latitude' => $geo['Lat'],
                        'Longitude' => $geo['Lon'],
                        'Title' => $title,
                    ];
                }
            }
        }

        // Multi-location jobs can push the list past the cap — truncate.
        return count($pins) > self::MAX_PINS ? array_slice($pins, 0, self::MAX_PINS) : $pins;
    }

    /**
     * Which of a job's locations to pin (or -1 to plot them all).
     *
     * @param  array<int, array{Lat: string, Lon: string}>  $locations
     * @param  string[]  $regions
     */
    private static function bestLocationIndex(
        array $locations,
        string $cityCsv,
        array $regions,
        ?string $mostFrequentCity,
        ?string $mostFrequentRegion,
    ): int {
        if (count($locations) <= 1) {
            return 0;
        }

        $bestIndex = -1;

        // Only a genuinely multi-city string (comma-separated, non-virtual) is a
        // parallel array we can align to the locations by index. A single city —
        // or a "Virtual…" city, which is never split — can't select one of several
        // locations, so we leave the index unresolved (-1 → plot every location)
        // unless a region matches. This mirrors the intent of the C# GetMapPins
        // city/location index pairing.
        if ($cityCsv !== '' && ! str_starts_with($cityCsv, self::VIRTUAL_PREFIX) && str_contains($cityCsv, ',')) {
            $cities = array_map('trim', explode(',', $cityCsv));
            foreach ($cities as $j => $city) {
                if ($city === $mostFrequentCity) {
                    $bestIndex = $j;
                }
            }
        }

        // No city match — try the regions instead.
        if ($bestIndex === -1 && count($regions) > 1) {
            foreach ($regions as $j => $region) {
                if ($region === $mostFrequentRegion) {
                    $bestIndex = $j;
                }
            }
        }

        return $bestIndex;
    }

    /**
     * The most frequently occurring value, preferring the first-encountered on a
     * tie (mirrors LINQ GroupBy → OrderByDescending(count) → FirstOrDefault). When
     * $allowEmpty is false, empty-string keys are ignored when choosing the winner
     * (but still counted, exactly like the C# Where filter after the ordering).
     *
     * @param  string[]  $values
     */
    private static function mostFrequent(array $values, bool $allowEmpty): ?string
    {
        $counts = [];
        $order = [];
        foreach ($values as $value) {
            if (! array_key_exists($value, $counts)) {
                $counts[$value] = 0;
                $order[] = $value;
            }
            $counts[$value]++;
        }

        $best = null;
        $bestCount = -1;
        foreach ($order as $key) {
            if (! $allowEmpty && $key === '') {
                continue;
            }
            if ($counts[$key] > $bestCount) {
                $bestCount = $counts[$key];
                $best = $key;
            }
        }

        return $best;
    }

    /**
     * Join the index's City array into the comma-separated string the selection
     * logic expects (ListToCsvConverter parity). Accepts an already-flat string too.
     */
    private static function cityCsv(mixed $value): string
    {
        if (is_string($value)) {
            return $value;
        }

        if (! is_array($value)) {
            return '';
        }

        $parts = array_values(array_filter(
            array_map(static fn (mixed $v): string => trim((string) $v), $value),
            static fn (string $v): bool => $v !== '',
        ));

        return implode(', ', $parts);
    }

    /**
     * @return string[]
     */
    private static function stringList(mixed $value): array
    {
        if (! is_array($value)) {
            return [];
        }

        return array_values(array_map(static fn (mixed $v): string => (string) $v, $value));
    }

    /**
     * @return array<int, array{Lat: string, Lon: string}>
     */
    private static function locations(mixed $value): array
    {
        if (! is_array($value)) {
            return [];
        }

        $out = [];
        foreach ($value as $point) {
            if (is_array($point) && isset($point['Lat'], $point['Lon'])) {
                $out[] = ['Lat' => (string) $point['Lat'], 'Lon' => (string) $point['Lon']];
            }
        }

        return $out;
    }
}
