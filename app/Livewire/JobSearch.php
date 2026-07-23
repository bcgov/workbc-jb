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

    // --- Standard filter facets (SRCH-3) -----------------------------------

    /** @var string[] selected hours keys (FullTime|PartTime|LeadingToFullTime) */
    public array $hours = [];

    /** @var string[] selected employment-period keys (Permanent|Temporary|Casual|Seasonal) */
    public array $period = [];

    /** @var string[] selected employment-terms keys (Day…Weekend) */
    public array $terms = [];

    /** @var string[] selected workplace keys (OnSite|Hybrid|Travelling|Virtual) */
    public array $workplace = [];

    /** @var array<int, int|string> selected NAICS industry ids */
    public array $industries = [];

    /** @var string[] selected education levels (EduLevel.keyword values) */
    public array $educationLevels = [];

    /** Date facet: 0 any | 1 today | 2 past-3-days | 3 custom range. */
    public string $dateSelection = '0';

    /** Custom-range bounds as YYYY-MM-DD (from native date inputs); '' = unset. */
    public string $startDate = '';

    public string $endDate = '';

    /** @var string[] */
    private const SEARCH_IN = ['all', 'title', 'employer', 'jobId'];

    /** Facet properties whose change should return to the first page of results. */
    private const FACET_PROPERTIES = [
        'hours', 'period', 'terms', 'workplace', 'industries',
        'educationLevels', 'dateSelection', 'startDate', 'endDate',
    ];

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

    /**
     * Generic Livewire hook: any standard-facet change (SRCH-3) returns to the
     * first page. Scoped to FACET_PROPERTIES so it never fights the keyword,
     * sort, distance or location hooks.
     */
    public function updated(string $name): void
    {
        if (in_array(strtok($name, '.'), self::FACET_PROPERTIES, true)) {
            $this->page = 1;
        }
    }

    /** Clear every filter facet (referenced by the empty-state hint). */
    public function clearFilters(): void
    {
        $this->hours = [];
        $this->period = [];
        $this->terms = [];
        $this->workplace = [];
        $this->industries = [];
        $this->educationLevels = [];
        $this->dateSelection = '0';
        $this->startDate = '';
        $this->endDate = '';
        $this->locations = [];
        $this->distance = -1;
        $this->resetLocationInput();
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
            'jobTypeHoursOptions' => self::jobTypeHoursOptions(),
            'jobTypePeriodOptions' => self::jobTypePeriodOptions(),
            'jobTypeTermsOptions' => self::jobTypeTermsOptions(),
            'workplaceOptions' => self::workplaceOptions(),
            'industryOptions' => self::industryOptions(),
            'educationOptions' => self::educationOptions(),
            'dateOptions' => self::dateOptions(),
        ]);
    }

    /**
     * Map the bound UI state to the shared filter value object.
     */
    public function toFilters(): JobSearchFilters
    {
        $searchIn = in_array($this->searchIn, self::SEARCH_IN, true) ? $this->searchIn : 'all';
        $sort = ($this->sort >= 1 && $this->sort <= 11) ? $this->sort : 1;

        $payload = [
            'Keyword' => $this->keyword !== '' ? $this->keyword : null,
            'SearchInField' => $searchIn,
            'SortOrder' => $sort,
            'Page' => max(1, $this->page),
            'PageSize' => $this->pageSize,
            'SearchLocations' => array_values($this->locations),
            'SearchLocationDistance' => $this->distance,
            'SearchIndustry' => $this->selectedIndustries(),
            'SearchJobEducationLevel' => $this->selectedEducationLevels(),
        ];

        // Job-type facets: each selected key toggles its SearchJobType{Key} flag.
        // Whitelisted against the known option keys so a tampered request can't
        // inject an unknown field (JobSearchFilters rejects those with a 400).
        foreach ($this->selectedJobTypeKeys() as $key) {
            $payload["SearchJobType{$key}"] = true;
        }

        return JobSearchFilters::fromArray($payload + $this->dateFilterPayload());
    }

    /**
     * Selected job-type keys (hours + period + terms + workplace), whitelisted.
     *
     * @return string[]
     */
    private function selectedJobTypeKeys(): array
    {
        $allowed = array_merge(
            array_keys(self::jobTypeHoursOptions()),
            array_keys(self::jobTypePeriodOptions()),
            array_keys(self::jobTypeTermsOptions()),
            array_keys(self::workplaceOptions()),
        );
        $selected = array_merge($this->hours, $this->period, $this->terms, $this->workplace);

        return array_values(array_intersect($selected, $allowed));
    }

    /**
     * Selected NAICS ids, cast to int and whitelisted against the known list.
     *
     * @return int[]
     */
    private function selectedIndustries(): array
    {
        $allowed = array_keys(self::industryOptions());

        return array_values(array_filter(
            array_map('intval', $this->industries),
            static fn (int $id): bool => in_array($id, $allowed, true),
        ));
    }

    /**
     * Selected education levels, whitelisted against the known EduLevel values.
     *
     * @return string[]
     */
    private function selectedEducationLevels(): array
    {
        return array_values(array_intersect($this->educationLevels, self::educationOptions()));
    }

    /**
     * Date facet → filter payload. Only a valid range contributes Start/End.
     *
     * @return array<string, mixed>
     */
    private function dateFilterPayload(): array
    {
        $selection = in_array($this->dateSelection, ['0', '1', '2', '3'], true) ? $this->dateSelection : '0';

        if ($selection !== '3') {
            return ['SearchDateSelection' => $selection];
        }

        $payload = ['SearchDateSelection' => '3'];
        if (($start = $this->parseDate($this->startDate)) !== null) {
            $payload['StartDate'] = $start;
        }
        if (($end = $this->parseDate($this->endDate)) !== null) {
            // The query pushes EndDate to end-of-day (23:59:59.999), so callers
            // only supply the calendar date.
            $payload['EndDate'] = $end;
        }

        return $payload;
    }

    /**
     * Parse a YYYY-MM-DD string into a DateField-shaped array, or null.
     *
     * @return array<string, int>|null
     */
    private function parseDate(string $value): ?array
    {
        if (preg_match('/^(\d{4})-(\d{2})-(\d{2})$/', trim($value), $m) !== 1) {
            return null;
        }

        return ['Year' => (int) $m[1], 'Month' => (int) $m[2], 'Day' => (int) $m[3]];
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
     * Job-type hours options: key → SearchJobType{key} flag → HoursOfWork term.
     *
     * @return array<string, string>
     */
    public static function jobTypeHoursOptions(): array
    {
        return [
            'FullTime' => 'Full-time',
            'PartTime' => 'Part-time',
            'LeadingToFullTime' => 'Part-time leading to full-time',
        ];
    }

    /**
     * Employment-period options.
     *
     * @return array<string, string>
     */
    public static function jobTypePeriodOptions(): array
    {
        return [
            'Permanent' => 'Permanent',
            'Temporary' => 'Temporary',
            'Casual' => 'Casual',
            'Seasonal' => 'Seasonal',
        ];
    }

    /**
     * Employment-terms (shift) options.
     *
     * @return array<string, string>
     */
    public static function jobTypeTermsOptions(): array
    {
        return [
            'Day' => 'Day',
            'Early' => 'Early morning',
            'Evening' => 'Evening',
            'Flexible' => 'Flexible hours',
            'Morning' => 'Morning',
            'Night' => 'Night',
            'OnCall' => 'On call',
            'Overtime' => 'Overtime',
            'Shift' => 'Shift',
            'Tbd' => 'To be determined',
            'Weekend' => 'Weekend',
        ];
    }

    /**
     * Workplace-type options.
     *
     * @return array<string, string>
     */
    public static function workplaceOptions(): array
    {
        return [
            'OnSite' => 'On-site',
            'Hybrid' => 'Hybrid',
            'Travelling' => 'Travelling',
            'Virtual' => 'Virtual',
        ];
    }

    /**
     * Education-level options (value = EduLevel.keyword term, contracts §1).
     *
     * @return string[]
     */
    public static function educationOptions(): array
    {
        return [
            'University',
            'College or apprenticeship',
            'Secondary school or job-specific training',
            'No education',
        ];
    }

    /**
     * Industry options: NAICS id → sector name (edm_naics.json, enabled sectors,
     * excluding "All Industries" id 0 and the disabled id 7).
     *
     * @return array<int, string>
     */
    public static function industryOptions(): array
    {
        return [
            1 => 'Accommodation and Food Services',
            2 => 'Agriculture and Fishing',
            3 => 'Business, Building and Other Support Services',
            4 => 'Construction',
            5 => 'Educational Services',
            6 => 'Finance, Insurance and Real Estate',
            8 => 'Forestry and Logging with Support Activities',
            9 => 'Health Care and Social Assistance',
            10 => 'Information, Culture and Recreation',
            11 => 'Manufacturing',
            12 => 'Utilities',
            13 => 'Mining and Oil and Gas Extraction',
            14 => 'Repair, Personal and Non-Profit Services',
            15 => 'Professional, Scientific, and Technical Services',
            16 => 'Public Administration',
            17 => 'Transportation and Warehousing',
            18 => 'Wholesale Trade',
            19 => 'Retail Trade',
        ];
    }

    /**
     * Date-selection options (SearchDateSelection enum, contracts §1).
     *
     * @return array<string, string>
     */
    public static function dateOptions(): array
    {
        return [
            '0' => 'Any time',
            '1' => 'Today',
            '2' => 'Past 3 days',
            '3' => 'Custom range',
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
