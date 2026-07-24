<?php

namespace App\Services\Search;

use Illuminate\Support\Facades\DB;

/**
 * Location autocomplete + validation against the curated `Locations` reference
 * table (map, don't create — data-model.md §0; reads only — Rule B). This mirrors
 * the production `LocationController`/`GetCitiesAsync`, which query `Locations`
 * (a clean geography list) rather than aggregating the job documents' raw `City`
 * field — so suggestions are canonical (one "Victoria", not the source-feed's
 * mis-cased duplicates) and validation accepts any real B.C. location, not only
 * cities that happen to have an active posting.
 */
final class LocationService
{
    /** Canadian postal code (ANA NAN), optional single space; case-insensitive. */
    private const POSTAL_PATTERN = '/^[ABCEGHJ-NPRSTVXYabceghj-nprstvxy]\d[A-Za-z][ ]?\d[A-Za-z]\d$/';

    public function isPostalCode(string $input): bool
    {
        return (bool) preg_match(self::POSTAL_PATTERN, trim($input));
    }

    /**
     * Distinct, visible city names matching $term — a prefix match, or (for 2+
     * chars) a word-start match anywhere in the name. Prefix matches sort first,
     * then alphabetically. Mirrors the .NET `GetCitiesAsync`
     * (`ILIKE 'term%' OR ILIKE '% term%'`, distinct, prefix-first ordering).
     *
     * @return string[]
     */
    public function suggestCities(string $term, int $limit = 8): array
    {
        $term = trim($term);
        if (mb_strlen($term) < 2) {
            return [];
        }

        $lower = mb_strtolower($term);

        // lower("City") LIKE … keeps it case-insensitive on both Postgres (prod)
        // and SQLite (tests) without relying on Postgres-only ILIKE.
        $cities = DB::table('Locations')
            ->where('IsHidden', false)
            ->where('LocationId', '>', 0)
            ->where(function ($q) use ($lower): void {
                $q->whereRaw('lower("City") like ?', [$lower.'%'])
                    ->orWhereRaw('lower("City") like ?', ['% '.$lower.'%']);
            })
            ->distinct()
            ->pluck('City')
            ->filter(static fn ($c): bool => $c !== null && $c !== '')
            ->values()
            ->all();

        usort($cities, static function (string $a, string $b) use ($lower): int {
            $aStarts = str_starts_with(mb_strtolower($a), $lower);
            $bStarts = str_starts_with(mb_strtolower($b), $lower);
            if ($aStarts !== $bStarts) {
                return $aStarts ? -1 : 1;
            }

            return strcasecmp($a, $b);
        });

        return array_slice($cities, 0, $limit);
    }

    /**
     * True when $city is a known, visible B.C. location (exact name, case-insensitive).
     */
    public function cityExists(string $city): bool
    {
        $city = trim($city);
        if ($city === '') {
            return false;
        }

        return DB::table('Locations')
            ->where('IsHidden', false)
            ->where('LocationId', '>', 0)
            ->whereRaw('lower("City") = ?', [mb_strtolower($city)])
            ->exists();
    }
}
