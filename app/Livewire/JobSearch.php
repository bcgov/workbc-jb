<?php

namespace App\Livewire;

use App\Search\Filters\JobSearchFilters;
use App\Search\Results\SearchResult;
use App\Services\Search\JobSearchService;
use App\Services\Search\LocationService;
use Livewire\Attributes\Layout;
use Livewire\Attributes\Title;
use Livewire\Attributes\Url;
use Livewire\Component;

/**
 * SRCH-1 / SRCH-2 — the public, server-rendered search results page.
 *
 * Server-rendered + crawlable (ADR-002): as a full-page Livewire component the
 * initial GET renders the results into the HTML (not injected client-side), and
 * each result links to the path-based job-detail URL. Keyword, scope, sort and
 * page are bound to the query string so results are shareable/deep-linkable.
 *
 * SRCH-2 adds the location facet: city/postal input is validated against the
 * index (via {@see LocationService}) and mapped to FND-7 location filters, with
 * radius search resolving coordinates through the Geocoder adapter. The
 * combobox's open/active view state lives in Alpine; this component owns the
 * suggestion data, validation and committed locations.
 *
 * No business logic here (copilot-instructions §6): the component maps its bound
 * state to a {@see JobSearchFilters} value object and delegates to
 * {@see JobSearchService}. Alpine handles pure view state elsewhere; this
 * component is the "data" side of the Alpine-vs-Livewire rule.
 */
#[Layout('components.layouts.app')]
#[Title('Find jobs — WorkBC Job Board')]
final class JobSearch extends Component
{
    #[Url(as: 'q', except: '')]
    public string $keyword = '';

    /** Search scope: all | title | employer | jobId. */
    #[Url(as: 'in', except: 'all')]
    public string $searchIn = 'all';

    /** SortOrder enum (contracts §1): 1..11, default 1 = DatePosted desc. */
    #[Url(except: 1)]
    public int $sort = 1;

    #[Url(except: 1)]
    public int $page = 1;

    public int $pageSize = 20;

    // --- Location facet (SRCH-2) -------------------------------------------

    /** The city/postal text currently being typed into the combobox. */
    public string $locationInput = '';

    /**
     * Committed locations, each a LocationField-shaped array ({City|Region|Postal}).
     *
     * @var array<int, array<string, string>>
     */
    public array $locations = [];

    /** SearchLocationDistance: -1 = exact, otherwise a radius in km. */
    public int $distance = -1;

    /** Validation message for the location input, surfaced via an ARIA-live region. */
    public ?string $locationError = null;

    /** @var string[] city-name autocomplete suggestions */
    public array $suggestions = [];

    /** @var string[] */
    private const SEARCH_IN = ['all', 'title', 'employer', 'jobId'];

    /** Submit the keyword/scope form: re-run from page 1. */
    public function applySearch(): void
    {
        $this->page = 1;
    }

    /** Changing sort restarts paging. */
    public function updatedSort(): void
    {
        $this->page = 1;
    }

    /** Changing the radius restarts paging. */
    public function updatedDistance(): void
    {
        $this->page = 1;
    }

    /** As the user types, refresh city suggestions (skip when it looks like a postal code). */
    public function updatedLocationInput(): void
    {
        $this->locationError = null;
        $input = trim($this->locationInput);
        $service = app(LocationService::class);

        if ($input === '' || $service->isPostalCode($input)) {
            $this->suggestions = [];

            return;
        }

        $this->suggestions = $service->suggestCities($input);
    }

    /** Commit a chosen city suggestion. */
    public function selectSuggestion(string $city): void
    {
        $this->commitLocation(['City' => $city]);
    }

    /**
     * Validate the typed input on blur/enter and commit it as a location, or
     * surface an accessible error. Postal codes are accepted on format; cities
     * must exist in the index.
     */
    public function addLocation(): void
    {
        $input = trim($this->locationInput);
        if ($input === '') {
            $this->suggestions = [];

            return;
        }

        $service = app(LocationService::class);

        if ($service->isPostalCode($input)) {
            // LocationField normalizes the postal (uppercase, no spaces) on read.
            $this->commitLocation(['Postal' => $input]);

            return;
        }

        if ($service->cityExists($input)) {
            $this->commitLocation(['City' => $input]);

            return;
        }

        $this->suggestions = [];
        $this->locationError = "We couldn't find \"{$input}\". Check the spelling, or try a nearby city or a postal code.";
    }

    public function removeLocation(int $index): void
    {
        unset($this->locations[$index]);
        $this->locations = array_values($this->locations);
        $this->page = 1;
    }

    /**
     * @param  array<string, string>  $location
     */
    private function commitLocation(array $location): void
    {
        // Ignore exact duplicates.
        foreach ($this->locations as $existing) {
            if ($existing === $location) {
                $this->resetLocationInput();

                return;
            }
        }

        $this->locations[] = $location;
        $this->resetLocationInput();
        $this->page = 1;
    }

    private function resetLocationInput(): void
    {
        $this->locationInput = '';
        $this->suggestions = [];
        $this->locationError = null;
    }

    public function gotoPage(int $page): void
    {
        $this->page = max(1, $page);
    }

    public function render()
    {
        try {
            $result = app(JobSearchService::class)->search($this->toFilters());
            $unavailable = false;
        } catch (\Throwable $e) {
            // Never leak an OpenSearch outage as a 500 on a public page; degrade to
            // an accessible "unavailable" message and log for ops.
            report($e);
            $result = new SearchResult(0, [], max(1, $this->page), $this->pageSize);
            $unavailable = true;
        }

        return view('livewire.job-search', [
            'result' => $result,
            'unavailable' => $unavailable,
            'sortOptions' => self::sortOptions(),
            'distanceOptions' => self::distanceOptions(),
        ]);
    }

    /**
     * Map the bound UI state to the shared filter value object.
     */
    public function toFilters(): JobSearchFilters
    {
        $searchIn = in_array($this->searchIn, self::SEARCH_IN, true) ? $this->searchIn : 'all';
        $sort = ($this->sort >= 1 && $this->sort <= 11) ? $this->sort : 1;

        return JobSearchFilters::fromArray([
            'Keyword' => $this->keyword !== '' ? $this->keyword : null,
            'SearchInField' => $searchIn,
            'SortOrder' => $sort,
            'Page' => max(1, $this->page),
            'PageSize' => $this->pageSize,
            'SearchLocations' => array_values($this->locations),
            'SearchLocationDistance' => $this->distance,
        ]);
    }

    /**
     * SearchLocationDistance options for the radius control (-1 = exact).
     *
     * @return array<int, string>
     */
    public static function distanceOptions(): array
    {
        return [
            -1 => 'Exact location',
            5 => 'Within 5 km',
            10 => 'Within 10 km',
            25 => 'Within 25 km',
            50 => 'Within 50 km',
            100 => 'Within 100 km',
        ];
    }

    /**
     * SortOrder enum (contracts §1) → human labels for the sort control.
     *
     * @return array<int, string>
     */
    public static function sortOptions(): array
    {
        return [
            1 => 'Date posted (newest first)',
            2 => 'Date posted (oldest first)',
            3 => 'Job title (A–Z)',
            4 => 'Job title (Z–A)',
            5 => 'City (A–Z)',
            6 => 'City (Z–A)',
            7 => 'Employer (A–Z)',
            8 => 'Employer (Z–A)',
            9 => 'Salary (low to high)',
            10 => 'Salary (high to low)',
            11 => 'Relevance',
        ];
    }
}
