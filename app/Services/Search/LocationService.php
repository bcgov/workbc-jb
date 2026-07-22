<?php

namespace App\Services\Search;

use OpenSearch\Client;

/**
 * Location autocomplete + validation against the derived OpenSearch read model
 * (Rule B — reads only). Suggestions come from a `City.keyword` aggregation so
 * the suggested strings match the exact city names the geocoder cache is keyed
 * on ("{city}, BC, CANADA"), and city validation checks the index rather than a
 * separate list.
 */
final class LocationService
{
    /** Canadian postal code (ANA NAN), optional single space; case-insensitive. */
    private const POSTAL_PATTERN = '/^[ABCEGHJ-NPRSTVXYabceghj-nprstvxy]\d[A-Za-z][ ]?\d[A-Za-z]\d$/';

    public function __construct(private Client $client) {}

    public function isPostalCode(string $input): bool
    {
        return (bool) preg_match(self::POSTAL_PATTERN, trim($input));
    }

    /**
     * Distinct city names whose name starts with $term (case-insensitive).
     *
     * @return string[]
     */
    public function suggestCities(string $term, int $limit = 8): array
    {
        $term = trim($term);
        if (mb_strlen($term) < 2) {
            return [];
        }

        $response = $this->client->search([
            'index' => $this->index(),
            'body' => [
                'size' => 0,
                'query' => ['match_phrase_prefix' => ['City' => $term]],
                'aggs' => ['cities' => ['terms' => ['field' => 'City.keyword', 'size' => 50]]],
            ],
        ]);

        $buckets = $response['aggregations']['cities']['buckets'] ?? [];
        $needle = mb_strtolower($term);

        $out = [];
        foreach ($buckets as $bucket) {
            $city = (string) ($bucket['key'] ?? '');
            if ($city !== '' && str_starts_with(mb_strtolower($city), $needle)) {
                $out[] = $city;
                if (count($out) >= $limit) {
                    break;
                }
            }
        }

        return $out;
    }

    /**
     * True when at least one active job is in the given city (exact, normalized).
     */
    public function cityExists(string $city): bool
    {
        $city = trim($city);
        if ($city === '') {
            return false;
        }

        $response = $this->client->search([
            'index' => $this->index(),
            'body' => [
                'size' => 0,
                'terminate_after' => 1,
                'query' => ['term' => ['City.normalize' => mb_strtolower($city)]],
            ],
        ]);

        return (int) ($response['hits']['total']['value'] ?? 0) > 0;
    }

    private function index(): string
    {
        $key = app()->getLocale() === 'fr' ? 'fr' : 'en';

        return (string) config("opensearch.indexes.{$key}", config('opensearch.indexes.en'));
    }
}
