<?php

namespace App\Livewire;

use App\Search\Filters\DateField;
use App\Search\Filters\JobSearchFilters;
use App\Search\Filters\LocationField;
use App\Search\Results\SearchResult;
use App\Search\Support\SalaryRangeHelper;
use App\Search\Url\FilterUrlSerializer;
use App\Services\Search\JobSearchService;
use App\Services\Search\LocationService;
use App\Support\JobSlug;
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
 * SRCH-4 adds the salary facet: a SalaryType unit selector, the fixed brackets
 * 1–5 and a custom range (bracket 6), an "unknown salary" toggle and the
 * benefit conditions. The component only maps state to filters — the
 * annualization/range maths live in {@see SalaryRangeHelper} and the query
 * groups in the shared JobSearchQuery (Rule B: the app only reads Salary).
 *
 * SRCH-5 adds the "More" facet: equity groups (→ the index's Is* terms),
 * posting language (English / English + French → federal jobs), a NOC 2021
 * code (→ Noc2021 term), the job source enum and the placement-agency
 * exclusion. As with the other facets it only maps state to the shared query.
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

    /**
     * SRCH-13 / ADR-006 — embed mode. When the search renders inside the Drupal
     * iframe (`?embed=1`) the host page already renders the H1/hero, so the
     * visible page title is suppressed (an sr-only heading is kept for AT). Read
     * once from the initial GET; deliberately NOT #[Url], so it never leaks into
     * shareable/alert links, and it persists in the component snapshot thereafter.
     */
    public bool $embed = false;

    /**
     * Results presentation (SRCH-9): 'list' (default) or 'map'. Bound to the URL
     * so a map view is shareable, and — crucially for a11y — the list stays a
     * full, always-available equivalent (the map is never the only way to reach
     * results).
     */
    #[Url(as: 'view', except: 'list')]
    public string $view = 'list';

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

    // --- Salary facet (SRCH-4) ---------------------------------------------

    /** SalaryType unit for the bracket labels + custom annualization (0 hourly … 4 annually). */
    public int $salaryType = 0;

    /** @var string[] selected fixed brackets ('1'…'5'); combined OR with the query's Salary ranges */
    public array $salaryBrackets = [];

    /** Custom range (bracket 6) toggle; reveals the min/max inputs. */
    public bool $salaryCustom = false;

    /** Custom range bounds in the selected SalaryType's units; '' = unset. */
    public string $salaryMin = '';

    public string $salaryMax = '';

    /** Include postings with no disclosed salary (SalarySort sentinel). */
    public bool $salaryUnknown = false;

    /** @var string[] selected benefit conditions (SalaryConditions.Description terms) */
    public array $salaryConditions = [];

    // --- More filters facet (SRCH-5) ---------------------------------------

    /** @var string[] selected equity groups (suffix of the SearchIs* flag) */
    public array $equityGroups = [];

    /** Job posting language: '1' English (default) | '2' English and French. */
    public string $postingLanguage = '1';

    /** Raw NOC entry (a 5-digit code or "NOC 12345 …"): NocCode context + Noc2021 term. */
    public string $nocCode = '';

    /** SearchJobSource enum (contracts §1): '0' any…'5' provincial. */
    public string $jobSource = '0';

    /** Exclude placement-agency employers (EmployerTypeId 1). */
    public bool $excludePlacementAgency = false;

    /** @var string[] */
    private const SEARCH_IN = ['all', 'title', 'employer', 'jobId'];

    /** Facet properties whose change should return to the first page of results. */
    private const FACET_PROPERTIES = [
        'hours', 'period', 'terms', 'workplace', 'industries',
        'educationLevels', 'dateSelection', 'startDate', 'endDate',
        'salaryType', 'salaryBrackets', 'salaryCustom', 'salaryMin',
        'salaryMax', 'salaryUnknown', 'salaryConditions',
        'equityGroups', 'postingLanguage', 'nocCode', 'jobSource',
        'excludePlacementAgency',
    ];

    /**
     * SRCH-6 — hydrate the facet state from a shareable/deep-linked URL.
     *
     * Keyword, scope, sort and page are bound to the query string by Livewire
     * (#[Url]); every other facet is reconstructed here from the canonical
     * parameters via {@see FilterUrlSerializer}, so loading a shared link (or an
     * old alert deep-link forwarded by the redirect shim) restores the exact
     * filters. Runs once on the initial GET.
     */
    public function mount(): void
    {
        $this->embed = request()->boolean('embed');
        $filters = app(FilterUrlSerializer::class)->fromQuery(request()->query());
        $this->hydrateFacets($filters);
    }

    /**
     * Map a reconstructed {@see JobSearchFilters} back onto the facet UI state
     * (the inverse of {@see toFilters()} for everything except the #[Url]-bound
     * keyword/scope/sort/page, which Livewire already restores).
     */
    private function hydrateFacets(JobSearchFilters $f): void
    {
        // Location facet.
        $this->locations = array_map(function (LocationField $loc): array {
            if ($loc->City !== null && $loc->City !== '') {
                return ['City' => $loc->City];
            }
            if ($loc->Region !== null && $loc->Region !== '') {
                return ['Region' => $loc->Region];
            }

            return ['Postal' => (string) $loc->getPostal()];
        }, $f->SearchLocations);
        // Only adopt a radius that maps to a real option; otherwise keep "exact".
        if (array_key_exists($f->SearchLocationDistance, self::distanceOptions())) {
            $this->distance = $f->SearchLocationDistance;
        }

        // Job-type facets (flags → the four checkbox groups).
        $this->hours = $this->selectedFlags($f, self::jobTypeHoursOptions(), 'SearchJobType');
        $this->period = $this->selectedFlags($f, self::jobTypePeriodOptions(), 'SearchJobType');
        $this->terms = $this->selectedFlags($f, self::jobTypeTermsOptions(), 'SearchJobType');
        $this->workplace = $this->selectedFlags($f, self::workplaceOptions(), 'SearchJobType');

        // Industry / education.
        $this->industries = $f->SearchIndustry;
        $this->educationLevels = $f->SearchJobEducationLevel;

        // Date facet.
        $this->dateSelection = $f->SearchDateSelection;
        $this->startDate = $f->StartDate !== null ? $this->dateFieldToInput($f->StartDate) : '';
        $this->endDate = $f->EndDate !== null ? $this->dateFieldToInput($f->EndDate) : '';

        // Salary facet.
        $this->salaryType = $f->SalaryType;
        $this->salaryBrackets = array_values(array_filter(
            array_map('strval', range(1, 5)),
            fn (string $n): bool => $f->{"SalaryBracket{$n}"},
        ));
        $this->salaryCustom = $f->SalaryBracket6;
        $this->salaryMin = $f->SalaryMin ?? '';
        $this->salaryMax = $f->SalaryMax ?? '';
        $this->salaryUnknown = $f->SearchSalaryUnknown;
        $this->salaryConditions = $f->SearchSalaryConditions;

        // "More" facet.
        $this->equityGroups = $this->selectedFlags($f, self::equityOptions(), 'SearchIs');
        $this->postingLanguage = $f->SearchIsPostingsInEnglishAndFrench ? '2' : '1';
        $this->nocCode = $f->NocCode ?? '';
        $this->jobSource = $f->SearchJobSource;
        $this->excludePlacementAgency = $f->SearchExcludePlacementAgencyJobs;
    }

    /**
     * The option keys whose "{$prefix}{Key}" boolean flag is set on the filters.
     *
     * @param  array<string, string>  $options
     * @return string[]
     */
    private function selectedFlags(JobSearchFilters $f, array $options, string $prefix): array
    {
        return array_values(array_filter(
            array_keys($options),
            fn (string $key): bool => (bool) $f->{"{$prefix}{$key}"},
        ));
    }

    /** DateField → the native date input's YYYY-MM-DD string. */
    private function dateFieldToInput(DateField $d): string
    {
        return sprintf('%04d-%02d-%02d', $d->Year, $d->Month, $d->Day);
    }

    /**
     * The canonical, shareable URL for the current filter state (SRCH-6). Kept
     * current on every render so a "copy link" affordance always reflects the
     * live facets, including those not bound to the query string by #[Url].
     */
    public function shareUrl(): string
    {
        return url(app(FilterUrlSerializer::class)->toUrl($this->toFilters()));
    }

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
        $this->salaryType = 0;
        $this->salaryBrackets = [];
        $this->salaryCustom = false;
        $this->salaryMin = '';
        $this->salaryMax = '';
        $this->salaryUnknown = false;
        $this->salaryConditions = [];
        $this->equityGroups = [];
        $this->postingLanguage = '1';
        $this->nocCode = '';
        $this->jobSource = '0';
        $this->excludePlacementAgency = false;
        $this->locations = [];
        $this->distance = -1;
        $this->resetLocationInput();
        $this->page = 1;
    }

    /**
     * SRCH-11 — remove a single active filter value (from the chips strip), then
     * return to the first page. `$value` selects which value within a multi-value
     * facet (or the location index); single-value facets ignore it and reset to
     * their default.
     */
    public function removeFilter(string $type, string $value = ''): void
    {
        switch ($type) {
            case 'location':
                $this->removeLocation((int) $value); // already resets the page

                return;
            case 'distance': $this->distance = -1; break;
            case 'hours': $this->hours = array_values(array_diff($this->hours, [$value])); break;
            case 'period': $this->period = array_values(array_diff($this->period, [$value])); break;
            case 'terms': $this->terms = array_values(array_diff($this->terms, [$value])); break;
            case 'workplace': $this->workplace = array_values(array_diff($this->workplace, [$value])); break;
            case 'industries':
                $this->industries = array_values(array_filter(
                    $this->industries,
                    static fn ($v): bool => (string) $v !== $value,
                ));
                break;
            case 'education': $this->educationLevels = array_values(array_diff($this->educationLevels, [$value])); break;
            case 'date':
                $this->dateSelection = '0';
                $this->startDate = '';
                $this->endDate = '';
                break;
            case 'salaryBracket': $this->salaryBrackets = array_values(array_diff($this->salaryBrackets, [$value])); break;
            case 'salaryCustom':
                $this->salaryCustom = false;
                $this->salaryMin = '';
                $this->salaryMax = '';
                break;
            case 'salaryUnknown': $this->salaryUnknown = false; break;
            case 'salaryCondition': $this->salaryConditions = array_values(array_diff($this->salaryConditions, [$value])); break;
            case 'equity': $this->equityGroups = array_values(array_diff($this->equityGroups, [$value])); break;
            case 'postingLanguage': $this->postingLanguage = '1'; break;
            case 'noc': $this->nocCode = ''; break;
            case 'jobSource': $this->jobSource = '0'; break;
            case 'excludeAgency': $this->excludePlacementAgency = false; break;
        }

        $this->page = 1;
    }

    /**
     * SRCH-11 — the active filters as removable chips, in facet order. Each entry
     * is { type, value, label }; type/value map to {@see removeFilter()}. Labels
     * reuse the facet option maps. Keyword/scope are excluded — they live in the
     * search band, not the filter strip.
     *
     * @return array<int, array{type: string, value: string, label: string}>
     */
    private function activeFilters(): array
    {
        $chips = [];
        $add = static function (string $type, string $value, string $label) use (&$chips): void {
            $chips[] = ['type' => $type, 'value' => $value, 'label' => $label];
        };

        // Location + radius.
        foreach ($this->locations as $i => $loc) {
            $add('location', (string) $i, $this->locationLabel($loc));
        }
        if ($this->distance !== -1 && $this->locations !== []) {
            $add('distance', '', self::distanceOptions()[$this->distance] ?? "Within {$this->distance} km");
        }

        // Job type (hours / period / terms / workplace).
        foreach ([
            ['hours', self::jobTypeHoursOptions(), $this->hours],
            ['period', self::jobTypePeriodOptions(), $this->period],
            ['terms', self::jobTypeTermsOptions(), $this->terms],
            ['workplace', self::workplaceOptions(), $this->workplace],
        ] as [$type, $map, $selected]) {
            foreach ($selected as $key) {
                if (isset($map[$key])) {
                    $add($type, (string) $key, $map[$key]);
                }
            }
        }

        // Industry.
        $industries = self::industryOptions();
        foreach ($this->industries as $id) {
            $id = (int) $id;
            if (isset($industries[$id])) {
                $add('industries', (string) $id, $industries[$id]);
            }
        }

        // Education (the value is itself the label).
        foreach ($this->educationLevels as $level) {
            if (in_array($level, self::educationOptions(), true)) {
                $add('education', $level, $level);
            }
        }

        // Date.
        if (in_array($this->dateSelection, ['1', '2', '3'], true)) {
            $label = self::dateOptions()[$this->dateSelection] ?? 'Date';
            if ($this->dateSelection === '3') {
                $label = $this->customDateLabel() ?: 'Custom date range';
            }
            $add('date', '', $label);
        }

        // Salary (brackets / custom / unknown / conditions).
        $brackets = $this->salaryBracketLabels();
        foreach ($this->salaryBrackets as $bracket) {
            if (isset($brackets[$bracket])) {
                $add('salaryBracket', (string) $bracket, $brackets[$bracket]);
            }
        }
        if ($this->salaryCustom) {
            $add('salaryCustom', '', $this->customSalaryLabel());
        }
        if ($this->salaryUnknown) {
            $add('salaryUnknown', '', 'Includes no salary listed');
        }
        foreach ($this->salaryConditions as $condition) {
            if (in_array($condition, self::salaryConditionOptions(), true)) {
                $add('salaryCondition', $condition, $condition);
            }
        }

        // More (equity / NOC / source / agency / language).
        $equity = self::equityOptions();
        foreach ($this->equityGroups as $key) {
            if (isset($equity[$key])) {
                $add('equity', (string) $key, $equity[$key]);
            }
        }
        if (trim($this->nocCode) !== '') {
            $add('noc', '', 'NOC '.trim($this->nocCode));
        }
        if ($this->jobSource !== '0' && isset(self::jobSourceOptions()[$this->jobSource])) {
            $add('jobSource', '', self::jobSourceOptions()[$this->jobSource]);
        }
        if ($this->excludePlacementAgency) {
            $add('excludeAgency', '', 'Excludes placement agencies');
        }
        if ($this->postingLanguage !== '1' && isset(self::postingLanguageOptions()[$this->postingLanguage])) {
            $add('postingLanguage', '', self::postingLanguageOptions()[$this->postingLanguage]);
        }

        return $chips;
    }

    /**
     * Human label for a committed location chip (City → Region → postal).
     *
     * @param  array<string, string>  $loc
     */
    private function locationLabel(array $loc): string
    {
        if (! empty($loc['City'])) {
            return $loc['City'];
        }
        if (! empty($loc['Region'])) {
            return $loc['Region'];
        }
        if (isset($loc['Postal'])) {
            return (string) LocationField::fromArray($loc)->getPostal();
        }

        return 'Location';
    }

    private function customDateLabel(): string
    {
        $start = trim($this->startDate);
        $end = trim($this->endDate);
        if ($start !== '' && $end !== '') {
            return "{$start} to {$end}";
        }
        if ($start !== '') {
            return "From {$start}";
        }
        if ($end !== '') {
            return "Until {$end}";
        }

        return '';
    }

    private function customSalaryLabel(): string
    {
        $unit = self::salaryTypeOptions()[$this->selectedSalaryType()] ?? '';
        $unit = $unit !== '' ? ' '.mb_strtolower($unit) : '';
        $min = trim($this->salaryMin);
        $max = trim($this->salaryMax);
        if ($min !== '' && $max !== '') {
            return "\${$min}–\${$max}{$unit}";
        }
        if ($min !== '') {
            return "From \${$min}{$unit}";
        }
        if ($max !== '') {
            return "Up to \${$max}{$unit}";
        }

        return 'Custom salary';
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

    /** Switch to the accessible list view (SRCH-9). */
    public function showListView(): void
    {
        $this->view = 'list';
    }

    /** Switch to the map view (SRCH-9); pins are built from the same filters. */
    public function showMapView(): void
    {
        $this->view = 'map';
    }

    public function render()
    {
        // Normalize the URL-bound view so a tampered value can't select anything
        // other than the two supported presentations.
        $this->view = $this->view === 'map' ? 'map' : 'list';

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

        // Only run the (heavier, up-to-5000-hit) map query when the map is shown.
        $mapPins = [];
        if ($this->view === 'map' && ! $unavailable) {
            try {
                $mapPins = $this->buildMapPins();
            } catch (\Throwable $e) {
                report($e);
            }
        }

        return view('livewire.job-search', [
            'result' => $result,
            'unavailable' => $unavailable,
            'activeFilters' => $this->activeFilters(),
            'shareUrl' => $this->shareUrl(),
            'view' => $this->view,
            'mapPins' => $mapPins,
            'mapApiKey' => (string) config('services.google_maps.js_key', ''),
            'sortOptions' => self::sortOptions(),
            'distanceOptions' => self::distanceOptions(),
            'jobTypeHoursOptions' => self::jobTypeHoursOptions(),
            'jobTypePeriodOptions' => self::jobTypePeriodOptions(),
            'jobTypeTermsOptions' => self::jobTypeTermsOptions(),
            'workplaceOptions' => self::workplaceOptions(),
            'industryOptions' => self::industryOptions(),
            'educationOptions' => self::educationOptions(),
            'dateOptions' => self::dateOptions(),
            'salaryTypeOptions' => self::salaryTypeOptions(),
            'salaryBracketLabels' => $this->salaryBracketLabels(),
            'salaryConditionOptions' => self::salaryConditionOptions(),
            'equityOptions' => self::equityOptions(),
            'jobSourceOptions' => self::jobSourceOptions(),
            'postingLanguageOptions' => self::postingLanguageOptions(),
        ]);
    }

    /**
     * Build the map pins for the current filters, each enriched with the
     * path-based detail URL used by its info window (SRCH-7/SRCH-9). The
     * pin-selection maths live in {@see JobSearchService::mapPins()}; this only
     * shapes the result for the browser component.
     *
     * @return array<int, array{id: string, lat: float, lng: float, title: string, url: string}>
     */
    private function buildMapPins(): array
    {
        $pins = app(JobSearchService::class)->mapPins($this->toFilters());

        return array_map(static function (array $pin): array {
            $title = $pin['Title'] ?? '';

            return [
                'id' => (string) $pin['JobId'],
                'lat' => (float) $pin['Latitude'],
                'lng' => (float) $pin['Longitude'],
                'title' => $title,
                'url' => route('jobs.show', ['job' => JobSlug::path((string) $pin['JobId'], $title !== '' ? $title : null)]),
            ];
        }, $pins);
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

        return JobSearchFilters::fromArray(
            $payload + $this->dateFilterPayload() + $this->salaryPayload() + $this->morePayload()
        );
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
     * Salary facet → filter payload. SalaryType is always sent (it drives the
     * bracket/custom annualization in the shared query); brackets, custom range,
     * unknown and conditions are added only when selected. All client values are
     * whitelisted so a tampered request can't inject an unknown field.
     *
     * @return array<string, mixed>
     */
    private function salaryPayload(): array
    {
        $payload = ['SalaryType' => $this->selectedSalaryType()];

        foreach ($this->selectedSalaryBrackets() as $bracket) {
            $payload["SalaryBracket{$bracket}"] = true;
        }

        if ($this->salaryCustom) {
            $payload['SalaryBracket6'] = true;
            if ($this->salaryMin !== '') {
                $payload['SalaryMin'] = $this->salaryMin;
            }
            if ($this->salaryMax !== '') {
                $payload['SalaryMax'] = $this->salaryMax;
            }
        }

        if ($this->salaryUnknown) {
            $payload['SearchSalaryUnknown'] = true;
        }

        $conditions = $this->selectedSalaryConditions();
        if ($conditions !== []) {
            $payload['SearchSalaryConditions'] = $conditions;
        }

        return $payload;
    }

    /** SalaryType, validated against the known units (defaults to 0 = hourly). */
    private function selectedSalaryType(): int
    {
        return array_key_exists($this->salaryType, self::salaryTypeOptions()) ? $this->salaryType : 0;
    }

    /**
     * Selected fixed brackets, cast to int and whitelisted to 1–5.
     *
     * @return int[]
     */
    private function selectedSalaryBrackets(): array
    {
        return array_values(array_filter(
            array_map('intval', $this->salaryBrackets),
            static fn (int $bracket): bool => $bracket >= 1 && $bracket <= 5,
        ));
    }

    /**
     * Selected benefit conditions, whitelisted against the known term list.
     *
     * @return string[]
     */
    private function selectedSalaryConditions(): array
    {
        return array_values(array_intersect($this->salaryConditions, self::salaryConditionOptions()));
    }

    /**
     * Labels for the fixed brackets (1–5), derived from the selected SalaryType's
     * boundaries so hourly/weekly/…/annually each read in their own units.
     *
     * @return array<int, string>
     */
    public function salaryBracketLabels(): array
    {
        [$b1, $b2, $b3, $b4] = SalaryRangeHelper::bracketBounds($this->selectedSalaryType());
        $money = static fn (float $n): string => '$' . number_format($n);

        return [
            1 => 'Under ' . $money($b1),
            2 => $money($b1) . ' – ' . $money($b2),
            3 => $money($b2) . ' – ' . $money($b3),
            4 => $money($b3) . ' – ' . $money($b4),
            5 => $money($b4) . ' or more',
        ];
    }

    /**
     * "More" facet (SRCH-5) → filter payload. Equity groups map to the index's
     * Is* boolean terms, posting language E+F flags the federal jobs, NOC maps
     * to the Noc2021 term, job source to the SearchJobSource enum, and the
     * placement-agency exclusion to a must_not. All client values are
     * whitelisted so a tampered request can't inject an unknown field.
     *
     * @return array<string, mixed>
     */
    private function morePayload(): array
    {
        $payload = [];

        // Equity groups: each selected suffix toggles its SearchIs{Suffix} flag.
        foreach ($this->selectedEquityKeys() as $key) {
            $payload["SearchIs{$key}"] = true;
        }

        // Posting language: '2' = English + French, which are the federal jobs.
        if ($this->postingLanguage === '2') {
            $payload['SearchIsPostingsInEnglish'] = false;
            $payload['SearchIsPostingsInEnglishAndFrench'] = true;
        }

        // Job source enum ('0'/any contributes no filter).
        if (array_key_exists($this->jobSource, self::jobSourceOptions()) && $this->jobSource !== '0') {
            $payload['SearchJobSource'] = $this->jobSource;
        }

        if ($this->excludePlacementAgency) {
            $payload['SearchExcludePlacementAgencyJobs'] = true;
        }

        return $payload + $this->nocPayload();
    }

    /**
     * Selected equity-group suffixes, whitelisted against the known list.
     *
     * @return string[]
     */
    private function selectedEquityKeys(): array
    {
        return array_values(array_intersect($this->equityGroups, array_keys(self::equityOptions())));
    }

    /**
     * NOC entry → payload. NocCode keeps the raw text (contract context); a
     * 5-digit code within it becomes SearchNocField (the Noc2021 term).
     *
     * @return array<string, string>
     */
    private function nocPayload(): array
    {
        $raw = trim($this->nocCode);
        if ($raw === '') {
            return [];
        }

        $payload = ['NocCode' => $raw];
        if (preg_match('/\d{5}/', $raw, $m) === 1) {
            $payload['SearchNocField'] = $m[0];
        }

        return $payload;
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
     * SalaryType units (contracts §1): int → label. Also the whitelist.
     *
     * @return array<int, string>
     */
    public static function salaryTypeOptions(): array
    {
        return [
            0 => 'Hourly',
            1 => 'Weekly',
            2 => 'Bi-weekly',
            3 => 'Monthly',
            4 => 'Annually',
        ];
    }

    /**
     * Benefit conditions (SalaryConditions.Description terms, contracts §1).
     * Values match the indexed keyword terms exactly.
     *
     * @return string[]
     */
    public static function salaryConditionOptions(): array
    {
        return [
            'As per collective agreement',
            'Bonus',
            'Commission',
            'Dental plan',
            'Disability benefits',
            'Gratuities',
            'Group insurance benefits',
            'Life insurance benefits',
            'Health care plan',
            'Mileage paid',
            'Pension plan benefits',
            'Piece work',
            'RESP benefits',
            'RRSP benefits',
            'Vision care benefits',
            'Other benefits',
        ];
    }

    /**
     * Equity groups (the "More" facet): key = SearchIs* flag suffix → label.
     * Also the whitelist for {@see selectedEquityKeys()}.
     *
     * @return array<string, string>
     */
    public static function equityOptions(): array
    {
        return [
            'Apprentice' => 'Apprentice',
            'Indigenous' => 'Indigenous person',
            'MatureWorkers' => 'Mature worker',
            'Newcomers' => 'Newcomer to B.C.',
            'PeopleWithDisabilities' => 'Person with a disability',
            'Students' => 'Student',
            'Veterans' => 'Veteran of the Canadian Armed Forces',
            'VisibleMinority' => 'Visible minority',
            'Youth' => 'Youth',
        ];
    }

    /**
     * SearchJobSource enum (contracts §1) → labels, in the C# dropdown order.
     * Also the whitelist for the job-source select.
     *
     * @return array<string, string>
     */
    public static function jobSourceOptions(): array
    {
        return [
            '0' => 'All sources',
            '1' => 'WorkBC',
            '5' => 'Provincial government',
            '4' => 'Municipal government',
            '3' => 'Federal government',
            '2' => 'External (other job boards)',
        ];
    }

    /**
     * Job posting language options (contracts §1).
     *
     * @return array<string, string>
     */
    public static function postingLanguageOptions(): array
    {
        return [
            '1' => 'English',
            '2' => 'English and French',
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
