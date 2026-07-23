<?php

namespace App\Services\Search;

use App\Search\Contracts\Geocoder;
use App\Search\Filters\JobSearchFilters;
use App\Search\Queries\JobSearchQuery;
use App\Search\Results\SearchResult;
use App\Search\Support\MapPins;
use OpenSearch\Client;

/**
 * Executes a job search against the derived OpenSearch read model (ADR-001,
 * Rule B): it READS the jobs_en / jobs_fr index only — never writes or
 * recomputes derived fields.
 *
 * Dependencies flow Http/Livewire → Service → Search/Adapters
 * (copilot-instructions §6). The Livewire component and any controller call
 * this service; the query body itself is built by the FND-7
 * {@see JobSearchQuery} as a structured array (never string-concatenated).
 * Radius searches resolve coordinates through the injected {@see Geocoder}
 * adapter.
 */
final class JobSearchService
{
    public function __construct(
        private Client $client,
        private Geocoder $geocoder,
    ) {}

    /**
     * Run the filters against OpenSearch and project the hits to the §2.1 DTO.
     */
    public function search(JobSearchFilters $filters): SearchResult
    {
        $body = (new JobSearchQuery($filters, $this->geocoder))->build();

        $response = $this->client->search([
            'index' => $this->index(),
            'body' => $body,
        ]);

        $pageNumber = $filters->Page <= 0 ? 1 : $filters->Page;

        return SearchResult::fromOpenSearchResponse($response, $pageNumber, $filters->PageSize);
    }

    /**
     * Run the SAME filters through the map query path (SRCH-9) and reduce the
     * hits to Google Maps pins. The map query returns only the pin fields for up
     * to {@see JobSearchQuery::MAP_PIN_CAP} geo-located jobs; {@see MapPins}
     * applies the current pin-selection behaviour (most-frequent city/region,
     * multi-location handling, 5000 cap).
     *
     * @return array<int, array{JobId: string, Latitude: string, Longitude: string, Title: string}>
     */
    public function mapPins(JobSearchFilters $filters): array
    {
        $body = (new JobSearchQuery($filters, $this->geocoder))->buildMapQuery();

        $response = $this->client->search([
            'index' => $this->index(),
            'body' => $body,
        ]);

        $sources = array_map(
            static fn (array $hit): array => $hit['_source'] ?? [],
            $response['hits']['hits'] ?? [],
        );

        return MapPins::fromSources($sources);
    }

    /**
     * The index to query for the active locale (contracts: en/fr are the two
     * derived indexes). Defaults to the English index.
     */
    private function index(): string
    {
        $key = app()->getLocale() === 'fr' ? 'fr' : 'en';

        return (string) config("opensearch.indexes.{$key}", config('opensearch.indexes.en'));
    }
}
