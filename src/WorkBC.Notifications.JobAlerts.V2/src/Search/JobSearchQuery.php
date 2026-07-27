<?php

declare(strict_types=1);

namespace WorkBC\JobAlertNotifier\Search;

/**
 * Port of WorkBC.ElasticSearch.Search.Queries.JobSearchQuery (plus the
 * paging/sorting logic it inherits from PageableJobsQueryBase).
 *
 * The C# builds the Elasticsearch request by string concatenation into the
 * jobsearch_main.json template; this port reproduces the exact same output
 * string (including spacing and trailing-comma cleanup) so the queries the
 * job-alert task sends are identical to the ones the .NET task sent.
 */
final class JobSearchQuery
{
    // DateType enum
    private const DATE_ANY = 0;
    private const DATE_TODAY = 1;
    private const DATE_PAST_THREE_DAYS = 2;
    private const DATE_RANGE = 3;
    private const DATE_NONE = 4;

    private const EXACT_MATCH = -1;

    // JobSearchBoost constants
    private const BOOST_EMPLOYER_NAME = 4;
    private const BOOST_JOB_ID = 10;
    private const BOOST_TITLE = 5;
    private const BOOST_ALL_SKILLS = 1;
    private const BOOST_JOB_DESCRIPTION = 1;
    private const BOOST_CITY = 2;

    private int $dateSearchType = self::DATE_NONE;
    private string $startDate = '';
    private string $endDate = '';

    /** @var list<array{0: string, 1: string}> [minAnnual, maxAnnual] strings */
    private array $salaryRanges = [];
    private int $salarySearchType = SalaryRangeHelper::NONE;

    private int $requestedPageSize = 0;
    private int $pageSize = 0;
    private int $pageNumber = 0;
    private string $sortField = '';
    private string $sortDirection = 'ASC';

    public function __construct(
        private readonly GeocodingInterface $geocodingService,
        private readonly JobSearchFilters $filters,
    ) {
        $this->setSortFilters();
        $this->setFilters();
    }

    /**
     * Build the JSON request body from the resource template.
     */
    public function toJson(string $template): string
    {
        $json = $template;
        $filterGroups = [];
        $geoLocation = null;
        $f = $this->filters;

        // ── Date search ─────────────────────────────────────────────
        switch ($this->dateSearchType) {
            case self::DATE_RANGE:
                $filterGroups[] = '{ "range": { "DatePosted": { "gte": "' . $this->startDate
                    . '", "lte": "' . $this->endDate . '" } } }';
                break;
            case self::DATE_PAST_THREE_DAYS:
                $filterGroups[] = '{ "range": { "DatePosted": { "gte": "now-3d/d", "lte": "now", "time_zone": "America/Vancouver" } } }';
                break;
            case self::DATE_TODAY:
                $filterGroups[] = '{ "range": { "DatePosted": { "gte": "now/d", "lt": "now+1d/d", "time_zone": "America/Vancouver" } } }';
                break;
        }

        // ── Salary search ───────────────────────────────────────────
        if ($f->searchSalaryUnknown
            || ($this->salarySearchType !== SalaryRangeHelper::NONE && count($this->salaryRanges) > 0)) {
            $jsonSalaryFilter = '';

            $k = 0;
            foreach ($this->salaryRanges as [$minSalary, $maxSalary]) {
                if ($k > 0) {
                    $jsonSalaryFilter .= ',';
                }

                if ((float) $maxSalary > 0) {
                    $jsonSalaryFilter .= '{ "range": { "Salary": { "gte": ' . $minSalary . ', "lte": ' . $maxSalary . ' } } }';
                } else {
                    // "unlimited" max — only use the min salary value
                    $jsonSalaryFilter .= '{ "range": { "Salary": { "gte": ' . $minSalary . ' } } }';
                }

                $k++;
            }

            if ($f->searchSalaryUnknown) {
                if ($k > 0) {
                    $jsonSalaryFilter .= ',';
                }

                // You can't search for null so we search in SalarySort.Descending instead.
                // The indexer sets this field to -99999999 for jobs with no salary.
                $jsonSalaryFilter .= '{ "range": { "SalarySort.Descending": { "lte": -99999999 } } }';
            }

            $filterGroups[] = $jsonSalaryFilter;
        }

        if ($f->searchSalaryConditions !== null && count($f->searchSalaryConditions) > 0) {
            $jsonBenefits = '';
            foreach ($f->searchSalaryConditions as $condition) {
                $jsonBenefits .= '{ "term": { "SalaryConditions.Description.keyword": "' . $condition . '" } },';
            }
            $filterGroups[] = substr($jsonBenefits, 0, -1);
        }

        // ── Keywords search ─────────────────────────────────────────
        if ($f->keyword !== null && $f->keyword !== '') {
            $queryString = KeywordParsing::buildSimpleQueryString($f->keyword);

            if ($queryString !== '') {
                if ($f->searchInField === null) {
                    // C# calls SearchInField.ToLower() unconditionally and throws on
                    // null, which the per-alert catch turns into a skipped alert.
                    throw new \RuntimeException('SearchInField is null');
                }

                switch (strtolower($f->searchInField)) {
                    case 'employer':
                        $fields = '"EmployerName"';
                        break;
                    case 'jobid':
                        $fields = '"JobId"';
                        break;
                    case 'title':
                        $fields = '"Title"';
                        break;
                    default:
                        $fields = sprintf(
                            '"EmployerName^%d","JobId^%d","Title^%d","AllSkills^%d","JobDescription^%d","City^%d"',
                            self::BOOST_EMPLOYER_NAME,
                            self::BOOST_JOB_ID,
                            self::BOOST_TITLE,
                            self::BOOST_ALL_SKILLS,
                            self::BOOST_JOB_DESCRIPTION,
                            self::BOOST_CITY
                        );
                        break;
                }

                // json escape quotation marks
                $queryString = str_replace('"', '\\"', $queryString);

                $filterGroups[] = '{ "simple_query_string": { "query": "' . $queryString
                    . '", "fields": [' . $fields . '], "default_operator": "AND", "quote_field_suffix": ".exact" } }';
            }
        }

        // ── Job Type ────────────────────────────────────────────────
        // Hours of Work (Full-time, Part-time, Part-time leading to full-time)
        if ($f->searchJobTypeFullTime || $f->searchJobTypePartTime || $f->searchJobTypeLeadingToFullTime) {
            $hoursOfWork = '';
            if ($f->searchJobTypeFullTime) {
                $hoursOfWork .= '{ "term": { "HoursOfWork.Description.keyword": "Full-time" } },';
            }
            if ($f->searchJobTypePartTime) {
                $hoursOfWork .= '{ "term": { "HoursOfWork.Description.keyword": "Part-time" } },';
            }
            if ($f->searchJobTypeLeadingToFullTime) {
                $hoursOfWork .= '{ "term": { "HoursOfWork.Description.keyword": "Part-time leading to full-time" } },';
            }
            $filterGroups[] = substr($hoursOfWork, 0, -1);
        }

        // Period of Employment (Temporary, Permanent, Casual, Seasonal)
        if ($f->searchJobTypeTemporary || $f->searchJobTypePermanent || $f->searchJobTypeSeasonal || $f->searchJobTypeCasual) {
            $periodOfEmployment = '';
            if ($f->searchJobTypeTemporary) {
                $periodOfEmployment .= '{ "term": { "PeriodOfEmployment.Description.keyword": "Temporary" } },';
            }
            if ($f->searchJobTypePermanent) {
                $periodOfEmployment .= '{ "term": { "PeriodOfEmployment.Description.keyword": "Permanent" } },';
            }
            if ($f->searchJobTypeCasual) {
                $periodOfEmployment .= '{ "term": { "PeriodOfEmployment.Description.keyword": "Casual" } },';
            }
            if ($f->searchJobTypeSeasonal) {
                $periodOfEmployment .= '{ "term": { "PeriodOfEmployment.Description.keyword": "Seasonal" } },';
            }
            $filterGroups[] = substr($periodOfEmployment, 0, -1);
        }

        // Employment Terms
        if ($f->searchJobTypeDay || $f->searchJobTypeEarly || $f->searchJobTypeEvening || $f->searchJobTypeFlexible
            || $f->searchJobTypeMorning || $f->searchJobTypeShift || $f->searchJobTypeNight || $f->searchJobTypeOnCall
            || $f->searchJobTypeOvertime || $f->searchJobTypeTbd || $f->searchJobTypeWeekend) {
            $employmentTerms = '';
            if ($f->searchJobTypeDay) {
                $employmentTerms .= '{ "term": { "EmploymentTerms.Description.keyword": "Day" } },';
            }
            if ($f->searchJobTypeEarly) {
                $employmentTerms .= '{ "term": { "EmploymentTerms.Description.keyword": "Early morning" } },';
            }
            if ($f->searchJobTypeEvening) {
                $employmentTerms .= '{ "term": { "EmploymentTerms.Description.keyword": "Evening" } },';
            }
            if ($f->searchJobTypeFlexible) {
                $employmentTerms .= '{ "term": { "EmploymentTerms.Description.keyword": "Flexible hours" } },';
            }
            if ($f->searchJobTypeMorning) {
                $employmentTerms .= '{ "term": { "EmploymentTerms.Description.keyword": "Morning" } },';
            }
            if ($f->searchJobTypeNight) {
                $employmentTerms .= '{ "term": { "EmploymentTerms.Description.keyword": "Night" } },';
            }
            if ($f->searchJobTypeOnCall) {
                $employmentTerms .= '{ "term": { "EmploymentTerms.Description.keyword": "On call" } },';
            }
            if ($f->searchJobTypeOvertime) {
                $employmentTerms .= '{ "term": { "EmploymentTerms.Description.keyword": "Overtime" } },';
            }
            if ($f->searchJobTypeShift) {
                $employmentTerms .= '{ "term": { "EmploymentTerms.Description.keyword": "Shift" } },';
            }
            if ($f->searchJobTypeTbd) {
                $employmentTerms .= '{ "term": { "EmploymentTerms.Description.keyword": "To be determined" } },';
            }
            if ($f->searchJobTypeWeekend) {
                $employmentTerms .= '{ "term": { "EmploymentTerms.Description.keyword": "Weekend" } },';
            }
            $filterGroups[] = substr($employmentTerms, 0, -1);
        }

        // ── Workplace Type ──────────────────────────────────────────
        if ($f->searchJobTypeOnSite || $f->searchJobTypeHybrid || $f->searchJobTypeTravelling || $f->searchJobTypeVirtual) {
            $workplaceType = '';
            if ($f->searchJobTypeOnSite) {
                $workplaceType .= '{ "term": { "WorkplaceType.Id": 0 } },';
            }
            if ($f->searchJobTypeHybrid) {
                $workplaceType .= '{ "term": { "WorkplaceType.Id": 100000 } },';
            }
            if ($f->searchJobTypeTravelling) {
                $workplaceType .= '{ "term": { "WorkplaceType.Id": 100001 } },';
            }
            if ($f->searchJobTypeVirtual) {
                $workplaceType .= '{ "term": { "WorkplaceType.Id": 15141 } },';
            }
            $filterGroups[] = substr($workplaceType, 0, -1);
        }

        // ── Education ───────────────────────────────────────────────
        if ($f->searchJobEducationLevel !== null && count($f->searchJobEducationLevel) > 0) {
            $jsonEducationLevel = '';
            foreach ($f->searchJobEducationLevel as $k => $level) {
                if ($k > 0) {
                    $jsonEducationLevel .= ',';
                }
                $jsonEducationLevel .= '{ "term": { "EduLevel.keyword": "' . $level . '" } }';
            }
            $filterGroups[] = $jsonEducationLevel;
        }

        // ── City / Postal Code (Location) ───────────────────────────
        $locations = $f->searchLocations ?? [];
        $locationDistance = $f->searchLocationDistance;

        if (count($locations) > 0 && $locations[0] !== null) {
            $locationFilters = [];

            if (count($locations) === 1) {
                $postal = $locations[0]->postal();
                $city = $locations[0]->city;
                $region = $locations[0]->region;

                if ($postal !== null && $postal !== '') {
                    // POSTAL CODE
                    if ($locationDistance === self::EXACT_MATCH) {
                        // Exact match (trailing comma matches the C# literal)
                        $locationFilters[] = '{ "term": { "PostalCode.keyword": "' . $postal . '" } },';
                    } else {
                        $geoLocation = $this->geocodingService->getLocation("{$postal}, CANADA");

                        if ($geoLocation !== null) {
                            $locationFilters[] = '{ "geo_distance" : { "distance" : "' . $locationDistance
                                . 'km", "LocationGeo" : { "lat" : ' . $geoLocation['Latitude']
                                . ',"lon" : ' . $geoLocation['Longitude'] . ' } } }';
                        } else {
                            // invalid location.  Use a lat/lon in the middle of the pacific ocean.
                            $locationFilters[] = '{ "geo_distance" : { "distance" : "1km", "LocationGeo" : { "lat" : 0,"lon" : 180 } } }';
                        }
                    }
                } elseif (($city !== null && $city !== '') || ($region !== null && $region !== '')) {
                    // CITY
                    $cityName = $city === null ? '' : trim($city);
                    $regionName = $region === null ? '' : trim($region);

                    if ($regionName !== '') {
                        $locationFilters[] = '{ "term": { "Region.keyword": "' . $regionName . '" } },';
                    } elseif ($locationDistance === self::EXACT_MATCH) {
                        // Exact match
                        if ($cityName !== '') {
                            $locationFilters[] = '{ "term": { "City.normalize": "' . mb_strtolower($cityName) . '" } }';
                        }
                    } else {
                        // Apply radius for city
                        if ($cityName !== '') {
                            $geoLocation = $this->geocodingService->getLocation("{$cityName}, BC, CANADA");

                            if ($geoLocation !== null) {
                                $locationFilters[] = '{ "geo_distance" : { "distance" : "' . $locationDistance
                                    . 'km", "LocationGeo" : { "lat" : ' . $geoLocation['Latitude']
                                    . ',"lon" : ' . $geoLocation['Longitude'] . ' } } }';
                            }
                        }
                    }
                }
            } else {
                // Multiple locations selected — MUST be exact match
                $jsonLocations = '';

                foreach ($locations as $location) {
                    $postal = $location->postal();
                    if ($postal !== null && $postal !== '') {
                        $jsonLocations .= '{ "term": { "PostalCode.keyword": "' . $postal . '" } },';
                    }
                    if ($location->city !== null && $location->city !== '') {
                        $jsonLocations .= '{ "term": { "City.normalize": "' . mb_strtolower($location->city) . '" } },';
                    }
                    if ($location->region !== null && $location->region !== '') {
                        $jsonLocations .= '{ "term": { "Region.keyword": "' . $location->region . '" } },';
                    }
                }

                if ($jsonLocations !== '') {
                    $locationFilters[] = substr($jsonLocations, 0, -1);
                }
            }

            // include all virtual jobs when there is a location filter
            foreach ($locationFilters as $loc) {
                $filterGroups[] = '{"term":{"WorkplaceType.Id":{"value":15141,"boost":0}}},' . $loc;
            }
        }

        // ── Industry filter ─────────────────────────────────────────
        if ($f->searchIndustry !== null && count($f->searchIndustry) > 0) {
            $jsonIndustry = '';
            foreach ($f->searchIndustry as $k => $industry) {
                if ($k > 0) {
                    $jsonIndustry .= ',';
                }
                $jsonIndustry .= '{ "term": { "NaicsId": ' . $industry . ' } }';
            }
            $filterGroups[] = $jsonIndustry;
        }

        // ── More filters ────────────────────────────────────────────
        // Employment groups
        if ($f->searchIsApprentice || $f->searchIsIndigenous || $f->searchIsMatureWorkers
            || $f->searchIsNewcomers || $f->searchIsPeopleWithDisabilities || $f->searchIsStudents
            || $f->searchIsVeterans || $f->searchIsVisibleMinority || $f->searchIsYouth) {
            $jsonEmploymentGroups = '';

            if ($f->searchIsApprentice) {
                $jsonEmploymentGroups .= '{ "term": { "IsApprentice": true } },';
            }
            if ($f->searchIsIndigenous) {
                $jsonEmploymentGroups .= '{ "term": { "IsAboriginal": true } },';
            }
            if ($f->searchIsMatureWorkers) {
                $jsonEmploymentGroups .= '{ "term": { "IsMatureWorker": true } },';
            }
            if ($f->searchIsNewcomers) {
                $jsonEmploymentGroups .= '{ "term": { "IsNewcomer": true } },';
            }
            if ($f->searchIsPeopleWithDisabilities) {
                $jsonEmploymentGroups .= '{ "term": { "IsDisability": true } },';
            }
            if ($f->searchIsStudents) {
                $jsonEmploymentGroups .= '{ "term": { "IsStudent": true } },';
            }
            if ($f->searchIsVeterans) {
                $jsonEmploymentGroups .= '{ "term": { "IsVeteran": true } },';
            }
            if ($f->searchIsVisibleMinority) {
                $jsonEmploymentGroups .= '{ "term": { "IsVismin": true } },';
            }
            if ($f->searchIsYouth) {
                $jsonEmploymentGroups .= '{ "term": { "IsYouth": true } },';
            }

            if ($jsonEmploymentGroups !== '') {
                $filterGroups[] = substr($jsonEmploymentGroups, 0, -1);
            }
        }

        // Job posting language (English & French = federal jobs only)
        if ($f->searchIsPostingsInEnglishAndFrench) {
            $filterGroups[] = '{ "term": { "IsFederalJob": true } }';
        }

        // NOC 2021 field
        if ($f->searchNocField !== null && $f->searchNocField !== '') {
            $filterGroups[] = '{ "term": { "Noc2021": "' . $f->searchNocField . '" } }';
        }

        // Job source
        if ($f->searchJobSource !== null && $f->searchJobSource !== '' && $f->searchJobSource !== '0') {
            $jsonJobSource = '';
            switch ($f->searchJobSource) {
                case '1':
                    // National Job Bank/WorkBC (federal XML feed)
                    $jsonJobSource = '{ "term": { "IsFederalJob": true } }';
                    break;
                case '2':
                    // Other job posting websites (external API feed)
                    $jsonJobSource = '{ "term": { "IsFederalJob": false } }';
                    break;
                case '3':
                    // federal government
                    $jsonJobSource = '{"nested": {"path": "ExternalSource","query": {"bool": {"should": [{"match_phrase": {"ExternalSource.Source.Url":  "https://emploisfp-psjobs.cfp-psc.gc.ca"}}]}}}}';
                    break;
                case '4':
                    // municipal government
                    $jsonJobSource = '{ "term": {"EmployerTypeId": {"value": "4"}}},{"nested": {"path": "ExternalSource","query": {"bool": {"should": [{"match_phrase": {"ExternalSource.Source.Source": "CivicInfoBC"}},{"match_phrase": {"ExternalSource.Source.Source": "CivicJobs.ca"}}]}}}}';
                    break;
                case '5':
                    // BC provincial government
                    $jsonJobSource = '{"nested": {"path": "ExternalSource","query": {"bool": {"should": [{"match_phrase": {"ExternalSource.Source.Url":  "https://bcpublicservice.hua.hrsmart.com"}}]}}}}';
                    break;
            }

            if ($jsonJobSource !== '') {
                $filterGroups[] = $jsonJobSource;
            }
        }

        if ($f->searchExcludePlacementAgencyJobs) {
            // Exclude placement agency (employer type id 1) via must_not
            $json = str_replace(
                '##QUERY_MUST_NOT##',
                '{ "term": { "EmployerTypeId": 1 } }, ##QUERY_MUST_NOT##',
                $json
            );
        }

        // ── Sorting and Pagination ──────────────────────────────────
        $json = $this->setPagingAndSorting($json, $geoLocation);

        // if one of the filter group options was selected update the query
        if (count($filterGroups) > 0) {
            $groupFilter = '';
            foreach ($filterGroups as $filterGroup) {
                $groupFilter .= '{ "bool": { "should" : [ ' . $filterGroup . ' ] } },';
            }
            $groupFilter = rtrim($groupFilter, ',');
            $json = str_replace('##QUERY_MUST##', $groupFilter, $json);
        }

        // ── Cleanup ─────────────────────────────────────────────────
        $json = str_replace(['##SORT##', '##QUERY_MUST##', '##QUERY_MUST_NOT##'], '', $json);

        // Remove unnecessary commas in arrays that result in invalid JSON
        $json = preg_replace('/}\s*,+\s*]/', '} ]', $json);
        $json = preg_replace('/}\s*,+\s*}/', '} }', $json);

        return $json;
    }

    /**
     * Port of PageableJobsQueryBase.SetPagingAndSorting.
     *
     * @param array{Latitude: string, Longitude: string}|null $geoLocation
     */
    private function setPagingAndSorting(string $json, ?array $geoLocation): string
    {
        $secondarySort = '{"DatePosted":"desc"},{"JobId.keyword":"asc"}';

        if ($geoLocation !== null) {
            $geoSort = '{"_geo_distance":{"LocationGeo":['
                . $geoLocation['Longitude'] . ',' . $geoLocation['Latitude']
                . '],"order":"asc","mode":"min","distance_type":"plane","ignore_unmapped": true}}';
            $secondarySort = $geoSort . ',' . $secondarySort;
        }

        if ($this->sortField !== '') {
            $jsonSort = '"sort":[{"' . $this->sortField . '":"' . $this->sortDirection . '"},' . $secondarySort . '],';
        } else {
            $jsonSort = '"sort":[{"_score":"desc"},' . $secondarySort . '],';
        }

        $json = str_replace('##SORT##', $jsonSort, $json);
        $json = str_replace('##PAGESIZE##', (string) $this->pageSize, $json);
        $json = str_replace('##SKIP##', (string) $this->skip(), $json);

        return $json;
    }

    private function skip(): int
    {
        return $this->pageNumber === 1 ? 0 : ($this->pageNumber - 1) * $this->requestedPageSize;
    }

    /** Port of PageableJobsQueryBase.SetSortFilters. */
    private function setSortFilters(): void
    {
        $this->requestedPageSize = $this->filters->pageSize;
        $this->pageSize = $this->filters->pageSize;
        $this->pageNumber = $this->filters->page === 0 ? 1 : $this->filters->page;

        switch ($this->filters->sortOrder) {
            case 1:
                $this->sortField = 'DatePosted';
                $this->sortDirection = 'DESC';
                break;
            case 2:
                $this->sortField = 'DatePosted';
                $this->sortDirection = 'ASC';
                break;
            case 3:
                $this->sortField = 'Title.normalize';
                $this->sortDirection = 'ASC';
                break;
            case 4:
                $this->sortField = 'Title.normalize';
                $this->sortDirection = 'DESC';
                break;
            case 5:
                $this->sortField = 'City.normalize';
                $this->sortDirection = 'ASC';
                break;
            case 6:
                $this->sortField = 'City.normalize';
                $this->sortDirection = 'DESC';
                break;
            case 7:
                $this->sortField = 'EmployerName.normalize';
                $this->sortDirection = 'ASC';
                break;
            case 8:
                $this->sortField = 'EmployerName.normalize';
                $this->sortDirection = 'DESC';
                break;
            case 9:
                $this->sortField = 'SalarySort.Ascending';
                $this->sortDirection = 'ASC';
                break;
            case 10:
                $this->sortField = 'SalarySort.Descending';
                $this->sortDirection = 'DESC';
                break;
            case 11:
                // Relevance
                $this->sortField = '';
                $this->sortDirection = 'ASC';
                break;
            default:
                // by default sort by date posted desc
                $this->sortField = 'DatePosted';
                $this->sortDirection = 'DESC';
                break;
        }
    }

    /** Port of JobSearchQuery.SetFilters (the parts not already in properties). */
    private function setFilters(): void
    {
        $f = $this->filters;

        // SetFilters re-assigns paging verbatim (without the Page==0→1 fix-up
        // applied in SetSortFilters) — Skip stays 0 either way for PageSize 0.
        $this->requestedPageSize = $f->pageSize;
        $this->pageSize = $f->pageSize;
        $this->pageNumber = $f->page;

        // Date type
        switch ($f->searchDateSelection) {
            case '0':
                $this->dateSearchType = self::DATE_NONE;
                break;
            case '1':
                $this->dateSearchType = self::DATE_TODAY;
                break;
            case '2':
                $this->dateSearchType = self::DATE_PAST_THREE_DAYS;
                break;
            case '3':
                $this->dateSearchType = self::DATE_RANGE;

                $this->startDate = ($f->startDate !== null && $f->startDate->year > 0)
                    ? (string) $f->startDate
                    : '1970-01-01';

                if ($f->endDate !== null && $f->endDate->year > 0) {
                    // set the time component to the end-of-day so
                    // the search includes jobs posted on the EndDate
                    $f->endDate->hour = 23;
                    $f->endDate->minute = 59;
                    $f->endDate->second = 59;
                    $f->endDate->millisecond = 999;
                    $this->endDate = (string) $f->endDate;
                } else {
                    // DateTime.MaxValue.ToString("yyyy-MM-dd")
                    $this->endDate = '9999-12-31';
                }
                break;
        }

        // Salary brackets 1–5 come from the shared annual-range table
        $this->salaryRanges = [];
        foreach ([1 => $f->salaryBracket1, 2 => $f->salaryBracket2, 3 => $f->salaryBracket3,
                  4 => $f->salaryBracket4, 5 => $f->salaryBracket5] as $bracket => $selected) {
            if ($selected) {
                $this->salaryRanges[] = SalaryRangeHelper::getAnnualRange($f->salaryType, $bracket);
            }
        }

        // Bracket 6 = custom range typed by the user
        if ($f->salaryBracket6 && $f->salaryMin !== null && $f->salaryMin !== '') {
            $salaryMin = self::parseDecimal($f->salaryMin);
            $salaryMax = self::parseDecimal($f->salaryMax ?? '');

            if ($f->salaryType === SalaryRangeHelper::HOURLY) {
                $salaryMin *= 2080;
                $salaryMax *= 2080;
            } elseif ($f->salaryType === SalaryRangeHelper::WEEKLY) {
                $salaryMin *= 52;
                $salaryMax *= 52;
            } elseif ($f->salaryType === SalaryRangeHelper::BI_WEEKLY) {
                $salaryMin *= 26;
                $salaryMax *= 26;
            }

            if ($f->salaryType === SalaryRangeHelper::MONTHLY) {
                $salaryMin *= 12;
                $salaryMax *= 12;
            }

            // decimal.Round defaults to banker's rounding
            $this->salaryRanges[] = [
                number_format(round($salaryMin, 0, PHP_ROUND_HALF_EVEN), 0, '.', ''),
                number_format(round($salaryMax, 0, PHP_ROUND_HALF_EVEN), 0, '.', ''),
            ];
        }

        $this->salarySearchType = match ($f->salaryType) {
            0 => SalaryRangeHelper::HOURLY,
            1 => SalaryRangeHelper::WEEKLY,
            2 => SalaryRangeHelper::BI_WEEKLY,
            3 => SalaryRangeHelper::MONTHLY,
            4 => SalaryRangeHelper::ANNUALLY,
            default => SalaryRangeHelper::NONE,
        };
    }

    /** decimal.TryParse semantics: thousands separators allowed, failure → 0. */
    private static function parseDecimal(string $value): float
    {
        $value = str_replace(',', '', trim($value));
        return is_numeric($value) ? (float) $value : 0.0;
    }
}
