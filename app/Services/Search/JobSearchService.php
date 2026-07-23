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
     * SRCH-10 — `POST /api/Search/JobSearch` entry point. Runs the SAME filters
     * through {@see search()}, except for the contracts.md §2.1 profile-sidebar
     * heuristic: a NOC filter + `PageSize <= 10` + no source pinned returns
     * National Job Bank (federal) jobs first, falling back entirely to external
     * jobs when no federal jobs match. Mirrors the legacy WorkBC.Web
     * SearchController.JobSearch `runNjbFirst` branch exactly (an all-or-nothing
     * source switch, not a per-page mix).
     */
    public function searchForApi(JobSearchFilters $filters): SearchResult
    {
        if ($this->prefersFederalFirst($filters)) {
            return $this->searchFederalFirst($filters);
        }

        return $this->search($filters);
    }

    /**
     * Total active jobs (contracts.md §2.2 `gettotaljobs`) — the same base
     * query as an unfiltered {@see search()}, but with PageSize 0 so no hit
     * documents are fetched, only the `track_total_hits` count.
     */
    public function activeJobCount(): int
    {
        $filters = new JobSearchFilters;
        $filters->PageSize = 0;

        return $this->search($filters)->count;
    }

    private function prefersFederalFirst(JobSearchFilters $filters): bool
    {
        $noSourcePinned = $filters->SearchJobSource === '' || $filters->SearchJobSource === '0';
        if (! $noSourcePinned) {
            return false;
        }

        $hasNocFilter = ($filters->SearchNocField ?? '') !== '' || ($filters->NocCode ?? '') !== '';
        $looksLikeProfileSidebar = $hasNocFilter && $filters->PageSize > 0 && $filters->PageSize <= 10;

        return $filters->SearchNjbJobsFirst || $looksLikeProfileSidebar;
    }

    private function searchFederalFirst(JobSearchFilters $filters): SearchResult
    {
        $federal = clone $filters;
        $federal->SearchJobSource = '1';
        $result = $this->search($federal);

        if ($result->count > 0) {
            return $result;
        }

        $external = clone $filters;
        $external->SearchJobSource = '2';

        return $this->search($external);
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
