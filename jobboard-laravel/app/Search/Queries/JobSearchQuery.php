<?php

namespace App\Search\Queries;

use App\Search\Contracts\Geocoder;
use App\Search\Filters\JobSearchFilters;
use App\Search\Support\GeoPoint;
use App\Search\Support\KeywordParsing;
use App\Search\Support\SalaryRangeHelper;

/**
 * Builds the OpenSearch request body for a job search as a STRUCTURED PHP ARRAY
 * (architecture.md §7: user input is data, never string-concatenated into a query
 * body). Faithful port of the C# JobSearchQuery
 * (WorkBC.ElasticSearch.Search/Queries/JobSearchQuery.cs +
 * PageableJobsQueryBase.cs + Resources/jobsearch_main.json).
 *
 * Read-model only (Rule B / ADR-001): the query READS the derived index. It never
 * writes it and never recomputes derived fields — Salary is already annualized and
 * ExpireDate already computed by the existing indexer; this only filters/sorts on them.
 *
 * The base filter always constrains to active jobs: ExpireDate >= now/d in
 * America/Vancouver (constraint #7 — all date logic is America/Vancouver).
 */
final class JobSearchQuery
{
    private const TIME_ZONE = 'America/Vancouver';

    /** simple_query_string field boosts (JobSearchBoost.cs). */
    private const BOOST_EMPLOYER_NAME = 4;
    private const BOOST_JOB_ID = 10;
    private const BOOST_TITLE = 5;
    private const BOOST_ALL_SKILLS = 1;
    private const BOOST_JOB_DESCRIPTION = 1;
    private const BOOST_CITY = 2;

    /** Sentinel the indexer writes to SalarySort.Descending for unknown salaries. */
    private const UNKNOWN_SALARY_SENTINEL = -99999999;

    /** WorkplaceType.Id for virtual jobs (always included alongside a location filter). */
    private const WORKPLACE_VIRTUAL = 15141;

    private const LOCATION_DISTANCE_EXACT = -1;

    /**
     * Max pins the map query returns (SRCH-9). Mirrors the C#
     * Resources/jobsearch_googlemap.json "size": 5000. Jobs with multiple
     * locations can still expand to more than this once pinned; MapPins applies
     * the same 5000 cap after expansion.
     */
    public const MAP_PIN_CAP = 5000;

    /**
     * The only fields the map query returns — the pin-selection logic needs
     * Location/City/Region/JobId (mirrors jobsearch_googlemap.json), and Title
     * is added for the SRCH-9 info-window content.
     *
     * @var string[]
     */
    private const MAP_SOURCE_FIELDS = ['Location', 'JobId', 'City', 'Region', 'Title'];

    public function __construct(
        private JobSearchFilters $filters,
        private ?Geocoder $geocoder = null,
    ) {}

    /**
     * Build the full OpenSearch _search request body.
     *
     * @return array<string, mixed>
     */
    public function build(): array
    {
        $pageSize = $this->filters->PageSize;
        $pageNumber = $this->filters->Page <= 0 ? 1 : $this->filters->Page;
        $skip = ($pageNumber - 1) * $pageSize;

        [$groups, $mustNot, $geoPoint] = $this->queryClauses();

        return [
            'track_total_hits' => true,
            'size' => $pageSize,
            'from' => $skip,
            'sort' => $this->sort($geoPoint),
            '_source' => $this->sourceFields(),
            'query' => [
                'bool' => [
                    'must' => $groups,
                    'must_not' => $mustNot,
                    'filter' => [
                        'range' => [
                            'ExpireDate' => [
                                'gte' => 'now/d',
                                'time_zone' => self::TIME_ZONE,
                            ],
                        ],
                    ],
                ],
            ],
        ];
    }

    /**
     * Build the OpenSearch body for the SRCH-9 map view — the SAME filters as
     * {@see build()} (a faithful port of Resources/jobsearch_googlemap.json), but
     * returning only the pin fields, capped at {@see MAP_PIN_CAP} hits, and
     * additionally requiring an indexed LocationGeo so only geo-located jobs are
     * plotted. Read-model only (Rule B): it reads the derived index, never writes
     * it. Pin selection itself happens in {@see \App\Search\Support\MapPins}.
     *
     * @return array<string, mixed>
     */
    public function buildMapQuery(): array
    {
        [$groups, $mustNot, $geoPoint] = $this->queryClauses();

        return [
            'size' => self::MAP_PIN_CAP,
            '_source' => self::MAP_SOURCE_FIELDS,
            'sort' => $this->sort($geoPoint),
            'query' => [
                'bool' => [
                    'must' => $groups,
                    'must_not' => $mustNot,
                    // Active jobs only, and only those the indexer geo-located
                    // (LocationGeo present) — a pin needs coordinates to plot.
                    'filter' => [
                        'bool' => [
                            'must' => [
                                ['range' => ['ExpireDate' => [
                                    'gte' => 'now/d',
                                    'time_zone' => self::TIME_ZONE,
                                ]]],
                                ['exists' => ['field' => 'LocationGeo']],
                            ],
                        ],
                    ],
                ],
            ],
        ];
    }

    /**
     * The shared query clauses — the faceted filters plus the placement-agency
     * exclusion — used by both the paged results {@see build()} and the map
     * {@see buildMapQuery()} so the two views always search identically.
     *
     * @return array{0: array<int, array<string, mixed>>, 1: array<int, array<string, mixed>>, 2: GeoPoint|null}
     *         [ must groups, must_not clauses, resolved geo point for sort ]
     */
    private function queryClauses(): array
    {
        /** @var array<int, array<string, mixed>> $groups each becomes a bool/should clause */
        $groups = [];
        $geoPoint = null;

        $this->addIfNotEmpty($groups, $this->dateGroup());
        $this->addIfNotEmpty($groups, $this->salaryGroup());
        $this->addIfNotEmpty($groups, $this->salaryConditionsGroup());
        $this->addIfNotEmpty($groups, $this->keywordGroup());
        $this->addIfNotEmpty($groups, $this->hoursOfWorkGroup());
        $this->addIfNotEmpty($groups, $this->periodOfEmploymentGroup());
        $this->addIfNotEmpty($groups, $this->employmentTermsGroup());
        $this->addIfNotEmpty($groups, $this->workplaceTypeGroup());
        $this->addIfNotEmpty($groups, $this->educationGroup());

        [$locationGroups, $geoPoint] = $this->locationGroups();
        foreach ($locationGroups as $group) {
            $groups[] = $this->should($group);
        }

        $this->addIfNotEmpty($groups, $this->industryGroup());
        $this->addIfNotEmpty($groups, $this->equityGroup());
        $this->addIfNotEmpty($groups, $this->postingLanguageGroup());
        $this->addIfNotEmpty($groups, $this->nocGroup());
        $this->addIfNotEmpty($groups, $this->jobSourceGroup());

        $mustNot = [];
        if ($this->filters->SearchExcludePlacementAgencyJobs) {
            // exclude placement-agency employers (EmployerTypeId 1)
            $mustNot[] = ['term' => ['EmployerTypeId' => 1]];
        }

        return [$groups, $mustNot, $geoPoint];
    }

    /**
     * @param  array<int, array<string, mixed>>  $groups
     * @param  array<int, array<string, mixed>>  $entries
     */
    private function addIfNotEmpty(array &$groups, array $entries): void
    {
        if ($entries !== []) {
            $groups[] = $this->should($entries);
        }
    }

    /**
     * Wrap a set of clauses as a bool/should group (matches "any of").
     *
     * @param  array<int, array<string, mixed>>  $entries
     * @return array<string, mixed>
     */
    private function should(array $entries): array
    {
        return ['bool' => ['should' => $entries]];
    }

    // ---------------------------------------------------------------------
    // Date
    // ---------------------------------------------------------------------

    /** @return array<int, array<string, mixed>> */
    private function dateGroup(): array
    {
        switch ($this->filters->SearchDateSelection) {
            case '1': // today
                return [['range' => ['DatePosted' => [
                    'gte' => 'now/d', 'lt' => 'now+1d/d', 'time_zone' => self::TIME_ZONE,
                ]]]];

            case '2': // past 3 days
                return [['range' => ['DatePosted' => [
                    'gte' => 'now-3d/d', 'lte' => 'now', 'time_zone' => self::TIME_ZONE,
                ]]]];

            case '3': // custom range
                $start = $this->filters->StartDate !== null && $this->filters->StartDate->Year > 0
                    ? (string) $this->filters->StartDate
                    : '1970-01-01';

                if ($this->filters->EndDate !== null && $this->filters->EndDate->Year > 0) {
                    // push to end-of-day so jobs posted on EndDate are included
                    $end = $this->filters->EndDate;
                    $end->Hour = 23;
                    $end->Minute = 59;
                    $end->Second = 59;
                    $end->Millisecond = 999;
                    $endValue = (string) $end;
                } else {
                    $endValue = '9999-12-31';
                }

                return [['range' => ['DatePosted' => [
                    'gte' => $start,
                    'lte' => $endValue,
                    // SRCH-3 / constraint #7: interpret the picked calendar dates in
                    // BC local time (the C# range omitted this, unlike today/past-3d).
                    'time_zone' => self::TIME_ZONE,
                ]]]];

            default:
                return [];
        }
    }

    // ---------------------------------------------------------------------
    // Salary
    // ---------------------------------------------------------------------

    /** @return array<int, array<string, mixed>> */
    private function salaryGroup(): array
    {
        $ranges = $this->salaryRanges();
        $entries = [];

        foreach ($ranges as [$min, $max]) {
            if ($max > 0) {
                $entries[] = ['range' => ['Salary' => ['gte' => $min, 'lte' => $max]]];
            } else {
                // "unlimited" upper bound (custom range with no max) → min only
                $entries[] = ['range' => ['Salary' => ['gte' => $min]]];
            }
        }

        if ($this->filters->SearchSalaryUnknown) {
            // can't match null; the indexer sets SalarySort.Descending to the sentinel
            $entries[] = ['range' => ['SalarySort.Descending' => ['lte' => self::UNKNOWN_SALARY_SENTINEL]]];
        }

        return $entries;
    }

    /**
     * Annualized [min, max] pairs from the selected salary brackets.
     *
     * @return array<int, array{0: float, 1: float}>
     */
    private function salaryRanges(): array
    {
        $type = $this->filters->SalaryType;
        $ranges = [];

        foreach ([1, 2, 3, 4, 5] as $bracket) {
            if ($this->filters->{"SalaryBracket{$bracket}"}) {
                $ranges[] = SalaryRangeHelper::getAnnualRange($type, $bracket);
            }
        }

        if ($this->filters->SalaryBracket6 && $this->filters->SalaryMin !== null && $this->filters->SalaryMin !== '') {
            $min = (float) $this->filters->SalaryMin;
            $max = (float) ($this->filters->SalaryMax ?? '');
            $multiplier = SalaryRangeHelper::multiplier($type);

            $ranges[] = [round($min * $multiplier, 0), round($max * $multiplier, 0)];
        }

        return $ranges;
    }

    /** @return array<int, array<string, mixed>> */
    private function salaryConditionsGroup(): array
    {
        $entries = [];
        foreach ($this->filters->SearchSalaryConditions as $condition) {
            $entries[] = ['term' => ['SalaryConditions.Description.keyword' => $condition]];
        }

        return $entries;
    }

    // ---------------------------------------------------------------------
    // Keyword
    // ---------------------------------------------------------------------

    /** @return array<int, array<string, mixed>> */
    private function keywordGroup(): array
    {
        if ($this->filters->Keyword === null || $this->filters->Keyword === '') {
            return [];
        }

        $queryString = KeywordParsing::buildSimpleQueryString($this->filters->Keyword);
        if ($queryString === '') {
            return [];
        }

        $fields = match (strtolower($this->filters->SearchInField)) {
            'employer' => ['EmployerName'],
            'jobid' => ['JobId'],
            'title' => ['Title'],
            default => [
                'EmployerName^'.self::BOOST_EMPLOYER_NAME,
                'JobId^'.self::BOOST_JOB_ID,
                'Title^'.self::BOOST_TITLE,
                'AllSkills^'.self::BOOST_ALL_SKILLS,
                'JobDescription^'.self::BOOST_JOB_DESCRIPTION,
                'City^'.self::BOOST_CITY,
            ],
        };

        return [[
            'simple_query_string' => [
                'query' => $queryString,
                'fields' => $fields,
                'default_operator' => 'AND',
                'quote_field_suffix' => '.exact',
            ],
        ]];
    }

    // ---------------------------------------------------------------------
    // Job type
    // ---------------------------------------------------------------------

    /** @return array<int, array<string, mixed>> */
    private function hoursOfWorkGroup(): array
    {
        return $this->termsFor('HoursOfWork.Description.keyword', [
            'SearchJobTypeFullTime' => 'Full-time',
            'SearchJobTypePartTime' => 'Part-time',
            'SearchJobTypeLeadingToFullTime' => 'Part-time leading to full-time',
        ]);
    }

    /** @return array<int, array<string, mixed>> */
    private function periodOfEmploymentGroup(): array
    {
        return $this->termsFor('PeriodOfEmployment.Description.keyword', [
            'SearchJobTypeTemporary' => 'Temporary',
            'SearchJobTypePermanent' => 'Permanent',
            'SearchJobTypeCasual' => 'Casual',
            'SearchJobTypeSeasonal' => 'Seasonal',
        ]);
    }

    /** @return array<int, array<string, mixed>> */
    private function employmentTermsGroup(): array
    {
        return $this->termsFor('EmploymentTerms.Description.keyword', [
            'SearchJobTypeDay' => 'Day',
            'SearchJobTypeEarly' => 'Early morning',
            'SearchJobTypeEvening' => 'Evening',
            'SearchJobTypeFlexible' => 'Flexible hours',
            'SearchJobTypeMorning' => 'Morning',
            'SearchJobTypeNight' => 'Night',
            'SearchJobTypeOnCall' => 'On call',
            'SearchJobTypeOvertime' => 'Overtime',
            'SearchJobTypeShift' => 'Shift',
            'SearchJobTypeTbd' => 'To be determined',
            'SearchJobTypeWeekend' => 'Weekend',
        ]);
    }

    /** @return array<int, array<string, mixed>> */
    private function workplaceTypeGroup(): array
    {
        $entries = [];
        $map = [
            'SearchJobTypeOnSite' => 0,
            'SearchJobTypeHybrid' => 100000,
            'SearchJobTypeTravelling' => 100001,
            'SearchJobTypeVirtual' => self::WORKPLACE_VIRTUAL,
        ];

        foreach ($map as $field => $id) {
            if ($this->filters->{$field}) {
                $entries[] = ['term' => ['WorkplaceType.Id' => $id]];
            }
        }

        return $entries;
    }

    /**
     * Build term clauses on $field for each truthy filter flag → value.
     *
     * @param  array<string, string>  $flagToValue
     * @return array<int, array<string, mixed>>
     */
    private function termsFor(string $field, array $flagToValue): array
    {
        $entries = [];
        foreach ($flagToValue as $flag => $value) {
            if ($this->filters->{$flag}) {
                $entries[] = ['term' => [$field => $value]];
            }
        }

        return $entries;
    }

    // ---------------------------------------------------------------------
    // Education
    // ---------------------------------------------------------------------

    /** @return array<int, array<string, mixed>> */
    private function educationGroup(): array
    {
        $entries = [];
        foreach ($this->filters->SearchJobEducationLevel as $level) {
            $entries[] = ['term' => ['EduLevel.keyword' => $level]];
        }

        return $entries;
    }

    // ---------------------------------------------------------------------
    // Location
    // ---------------------------------------------------------------------

    /**
     * @return array{0: array<int, array<int, array<string, mixed>>>, 1: GeoPoint|null}
     *         [ list of location units (each = clauses that get virtual-job OR'd in), resolved geo point for sort ]
     */
    private function locationGroups(): array
    {
        $locations = $this->filters->SearchLocations;
        if ($locations === [] || $locations[0] === null) {
            return [[], null];
        }

        $distance = $this->filters->SearchLocationDistance;
        $units = [];
        $geoPoint = null;

        if (count($locations) === 1) {
            $loc = $locations[0];
            $postal = $loc->getPostal();
            $city = $loc->City !== null ? trim($loc->City) : '';
            $region = $loc->Region !== null ? trim($loc->Region) : '';

            if ($postal !== null && $postal !== '') {
                if ($distance === self::LOCATION_DISTANCE_EXACT) {
                    $units[] = [['term' => ['PostalCode.keyword' => $postal]]];
                } else {
                    $resolved = $this->geocoder?->resolve("{$postal}, CANADA");
                    if ($resolved !== null) {
                        $geoPoint = $resolved;
                        $units[] = [$this->geoDistance($distance, $resolved)];
                    } else {
                        // invalid location → a 1km radius in the middle of the Pacific (no matches)
                        $units[] = [$this->geoDistance(1, new GeoPoint(0, 180))];
                    }
                }
            } elseif ($city !== '' || $region !== '') {
                if ($region !== '') {
                    $units[] = [['term' => ['Region.keyword' => $region]]];
                } elseif ($distance === self::LOCATION_DISTANCE_EXACT) {
                    if ($city !== '') {
                        $units[] = [['term' => ['City.normalize' => strtolower($city)]]];
                    }
                } elseif ($city !== '') {
                    $resolved = $this->geocoder?->resolve("{$city}, BC, CANADA");
                    if ($resolved !== null) {
                        $geoPoint = $resolved;
                        $units[] = [$this->geoDistance($distance, $resolved)];
                    } else {
                        // unresolvable city → 1km radius in the middle of the Pacific (no matches),
                        // matching the postal branch and current behaviour — never silently drop the filter
                        $units[] = [$this->geoDistance(1, new GeoPoint(0, 180))];
                    }
                }
            }
        } else {
            // multiple locations MUST be exact matches
            $entries = [];
            foreach ($locations as $loc) {
                $postal = $loc->getPostal();
                if ($postal !== null && $postal !== '') {
                    $entries[] = ['term' => ['PostalCode.keyword' => $postal]];
                }
                if ($loc->City !== null && $loc->City !== '') {
                    $entries[] = ['term' => ['City.normalize' => strtolower($loc->City)]];
                }
                if ($loc->Region !== null && $loc->Region !== '') {
                    $entries[] = ['term' => ['Region.keyword' => $loc->Region]];
                }
            }

            if ($entries !== []) {
                $units[] = $entries;
            }
        }

        // every location filter also includes virtual jobs (boosted to 0 so they don't skew relevance)
        $units = array_map(
            fn (array $entries): array => array_merge([
                ['term' => ['WorkplaceType.Id' => ['value' => self::WORKPLACE_VIRTUAL, 'boost' => 0]]],
            ], $entries),
            $units,
        );

        return [$units, $geoPoint];
    }

    /**
     * @return array<string, mixed>
     */
    private function geoDistance(int $km, GeoPoint $point): array
    {
        return ['geo_distance' => [
            'distance' => "{$km}km",
            'LocationGeo' => ['lat' => $point->lat, 'lon' => $point->lon],
        ]];
    }

    // ---------------------------------------------------------------------
    // Industry
    // ---------------------------------------------------------------------

    /** @return array<int, array<string, mixed>> */
    private function industryGroup(): array
    {
        $entries = [];
        foreach ($this->filters->SearchIndustry as $naicsId) {
            $entries[] = ['term' => ['NaicsId' => $naicsId]];
        }

        return $entries;
    }

    // ---------------------------------------------------------------------
    // More filters
    // ---------------------------------------------------------------------

    /** @return array<int, array<string, mixed>> */
    private function equityGroup(): array
    {
        // filter flag → the index's boolean field name
        $map = [
            'SearchIsApprentice' => 'IsApprentice',
            'SearchIsIndigenous' => 'IsAboriginal',
            'SearchIsMatureWorkers' => 'IsMatureWorker',
            'SearchIsNewcomers' => 'IsNewcomer',
            'SearchIsPeopleWithDisabilities' => 'IsDisability',
            'SearchIsStudents' => 'IsStudent',
            'SearchIsVeterans' => 'IsVeteran',
            'SearchIsVisibleMinority' => 'IsVismin',
            'SearchIsYouth' => 'IsYouth',
        ];

        $entries = [];
        foreach ($map as $flag => $indexField) {
            if ($this->filters->{$flag}) {
                $entries[] = ['term' => [$indexField => true]];
            }
        }

        return $entries;
    }

    /** @return array<int, array<string, mixed>> */
    private function postingLanguageGroup(): array
    {
        // English + French postings are the federal jobs
        if ($this->filters->SearchIsPostingsInEnglishAndFrench) {
            return [['term' => ['IsFederalJob' => true]]];
        }

        return [];
    }

    /** @return array<int, array<string, mixed>> */
    private function nocGroup(): array
    {
        if ($this->filters->SearchNocField === null || $this->filters->SearchNocField === '') {
            return [];
        }

        return [['term' => ['Noc2021' => $this->filters->SearchNocField]]];
    }

    /** @return array<int, array<string, mixed>> */
    private function jobSourceGroup(): array
    {
        $source = $this->filters->SearchJobSource;
        if ($source === '' || $source === '0') {
            return [];
        }

        return match ($source) {
            // National Job Bank / WorkBC (federal XML feed)
            '1' => [['term' => ['IsFederalJob' => true]]],
            // Other job posting websites (external API feed)
            '2' => [['term' => ['IsFederalJob' => false]]],
            // Federal government
            '3' => [$this->externalSourceUrl('https://emploisfp-psjobs.cfp-psc.gc.ca')],
            // Municipal government
            '4' => [
                ['term' => ['EmployerTypeId' => ['value' => '4']]],
                $this->externalSourceMatch('ExternalSource.Source.Source', ['CivicInfoBC', 'CivicJobs.ca']),
            ],
            // BC provincial government
            '5' => [$this->externalSourceUrl('https://bcpublicservice.hua.hrsmart.com')],
            default => [],
        };
    }

    /**
     * @return array<string, mixed>
     */
    private function externalSourceUrl(string $url): array
    {
        return $this->externalSourceMatch('ExternalSource.Source.Url', [$url]);
    }

    /**
     * A nested ExternalSource query matching any of $values on $field.
     *
     * @param  string[]  $values
     * @return array<string, mixed>
     */
    private function externalSourceMatch(string $field, array $values): array
    {
        return ['nested' => [
            'path' => 'ExternalSource',
            'query' => [
                'bool' => [
                    'should' => array_map(
                        static fn (string $value): array => ['match_phrase' => [$field => $value]],
                        $values,
                    ),
                ],
            ],
        ]];
    }

    // ---------------------------------------------------------------------
    // Sorting & source
    // ---------------------------------------------------------------------

    /**
     * @return array<int, array<string, mixed>>
     */
    private function sort(?GeoPoint $geoPoint): array
    {
        [$field, $direction] = $this->sortFieldAndDirection();

        $secondary = [
            ['DatePosted' => 'desc'],
            ['JobId.keyword' => 'asc'],
        ];

        if ($geoPoint !== null) {
            array_unshift($secondary, ['_geo_distance' => [
                'LocationGeo' => [$geoPoint->lon, $geoPoint->lat],
                'order' => 'asc',
                'mode' => 'min',
                'distance_type' => 'plane',
                'ignore_unmapped' => true,
            ]]);
        }

        $primary = $field === '' ? ['_score' => 'desc'] : [$field => $direction];

        return array_merge([$primary], $secondary);
    }

    /**
     * SortOrder enum → [field, direction]. Empty field == relevance (_score).
     *
     * @return array{0: string, 1: string}
     */
    private function sortFieldAndDirection(): array
    {
        return match ($this->filters->SortOrder) {
            2 => ['DatePosted', 'asc'],
            3 => ['Title.normalize', 'asc'],
            4 => ['Title.normalize', 'desc'],
            5 => ['City.normalize', 'asc'],
            6 => ['City.normalize', 'desc'],
            7 => ['EmployerName.normalize', 'asc'],
            8 => ['EmployerName.normalize', 'desc'],
            9 => ['SalarySort.Ascending', 'asc'],
            10 => ['SalarySort.Descending', 'desc'],
            11 => ['', 'asc'], // relevance
            default => ['DatePosted', 'desc'], // 1 and fallback
        };
    }

    /**
     * The index fields to return — the subset the §2.1 API projection needs.
     * (Filter-only fields such as EduLevel/NaicsId/LocationGeo/AllSkills are
     * queried but not returned — contracts §2.1.)
     *
     * @return string[]
     */
    private function sourceFields(): array
    {
        return [
            'JobId', 'Title', 'EmployerName', 'DatePosted', 'ExpireDate',
            'City', 'Province', 'Region', 'Location',
            'Noc2021', 'NocGroup', 'Salary', 'SalarySummary', 'IsFederalJob',
            'HoursOfWork', 'PeriodOfEmployment', 'EmploymentTerms', 'WorkplaceType',
            'WorkLangCd', 'WageClass', 'SalaryConditions', 'SkillCategories',
            'JobDescription', 'ExternalSource', 'ApplyWebsite',
        ];
    }
}
