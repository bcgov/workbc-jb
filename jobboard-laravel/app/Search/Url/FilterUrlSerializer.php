<?php

namespace App\Search\Url;

use App\Search\Filters\DateField;
use App\Search\Filters\JobSearchFilters;

/**
 * SRCH-6 — the shareable-URL contract for {@see JobSearchFilters}.
 *
 * This is the single source of truth for turning a filter value object into a
 * query string and back (LARAVEL-MIGRATION-PLAN risk #3, "URL contract"). It is
 * deliberately framework-independent (no Livewire, no request()) so it can be
 * unit-tested for a lossless round-trip and reused by the saved-alerts feature
 * to regenerate {@see JobAlerts.UrlParameters} from the stored filter JSON.
 *
 * Contract goals (docs/epics/EPIC-SEARCH.md SRCH-6):
 *   - {@see toQuery()} → {@see fromQuery()} reconstructs the EXACT filters for
 *     every facet (round-trip is lossless).
 *   - Only non-default facets are emitted, so URLs stay short and readable.
 *   - All decoded values are whitelisted against the known option sets, so a
 *     tampered link can never inject an unknown field (which JobSearchFilters
 *     would reject with a 400).
 *
 * Versioning (docs/contracts.md §1): JobAlerts.JobSearchFiltersVersion is 0 or 1
 * — the same shape today. {@see fromQuery()} accepts an optional `v` param and
 * defers to {@see JobSearchFilters::fromArray()} for the version guard.
 *
 * Alert compatibility (SRCH-6 AC): the authoritative migration regenerates the
 * canonical URL from each alert's stored JobSearchFilters JSON via
 * {@see toQuery()}. For already-sent alert emails (whose deep-links use the old
 * Angular matrix-param format under a hash fragment), {@see fromLegacy()} maps
 * those legacy params onto a JobSearchFilters so a redirect shim can forward
 * them to the canonical `/jobs?…` URL.
 */
final class FilterUrlSerializer
{
    /** SearchJobType{Suffix} flag suffixes (hours + period + terms + workplace). */
    private const JOB_TYPE_SUFFIXES = [
        'FullTime', 'PartTime', 'LeadingToFullTime',
        'Permanent', 'Temporary', 'Casual', 'Seasonal',
        'Day', 'Early', 'Evening', 'Flexible', 'Morning', 'Night',
        'OnCall', 'Overtime', 'Shift', 'Tbd', 'Weekend',
        'OnSite', 'Hybrid', 'Travelling', 'Virtual',
    ];

    /** SearchIs{Suffix} equity-group flag suffixes ("More" facet). */
    private const EQUITY_SUFFIXES = [
        'Apprentice', 'Veterans', 'Indigenous', 'MatureWorkers', 'Newcomers',
        'PeopleWithDisabilities', 'Students', 'VisibleMinority', 'Youth',
    ];

    /** Known EduLevel.keyword values (docs/contracts.md §1). */
    private const EDUCATION_VALUES = [
        'University',
        'College or apprenticeship',
        'Secondary school or job-specific training',
        'No education',
    ];

    /** Known SalaryConditions.Description values (docs/contracts.md §1). */
    private const SALARY_CONDITIONS = [
        'As per collective agreement', 'Bonus', 'Commission', 'Dental plan',
        'Disability benefits', 'Gratuities', 'Group insurance benefits',
        'Life insurance benefits', 'Health care plan', 'Mileage paid',
        'Pension plan benefits', 'Piece work', 'RESP benefits', 'RRSP benefits',
        'Vision care benefits', 'Other benefits',
    ];

    /** Enabled NAICS sector ids (edm_naics.json; excludes "All" 0 and disabled 7). */
    /** Real Industries.Id values the search UI exposes (mirror of JobSearch::industryOptions keys). */
    private const INDUSTRY_IDS = [1, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 34, 35, 36, 37, 39, 40, 41, 42, 43, 44, 45, 46];

    /** Valid keyword scopes. */
    private const SEARCH_IN = ['all', 'title', 'employer', 'jobId'];

    /** Valid SearchJobSource enum values. */
    private const JOB_SOURCES = ['0', '1', '2', '3', '4', '5'];

    /**
     * Serialize filters to a compact, order-stable set of query parameters.
     *
     * Only facets that differ from their default are emitted. List facets are
     * joined with a comma; committed locations use typed `type:value` tokens
     * (`c` city, `r` region, `p` postal) joined with `;` to preserve order.
     *
     * @return array<string, string>
     */
    public function toQuery(JobSearchFilters $f): array
    {
        $q = [];

        // Keyword / scope / sort / paging.
        if ($f->Keyword !== null && $f->Keyword !== '') {
            $q['q'] = $f->Keyword;
        }
        if ($f->SearchInField !== 'all') {
            $q['in'] = $f->SearchInField;
        }
        if ($f->SortOrder !== 1) {
            $q['sort'] = (string) $f->SortOrder;
        }
        if ($f->Page !== 1) {
            $q['page'] = (string) $f->Page;
        }
        if ($f->PageSize !== 20) {
            $q['size'] = (string) $f->PageSize;
        }

        // Location facet.
        $tokens = [];
        foreach ($f->SearchLocations as $loc) {
            if ($loc->City !== null && $loc->City !== '') {
                $tokens[] = 'c:' . $loc->City;
            } elseif ($loc->Region !== null && $loc->Region !== '') {
                $tokens[] = 'r:' . $loc->Region;
            } elseif ($loc->getPostal() !== null && $loc->getPostal() !== '') {
                $tokens[] = 'p:' . $loc->getPostal();
            }
        }
        if ($tokens !== []) {
            $q['loc'] = implode(';', $tokens);
        }
        if ($f->SearchLocationDistance !== 0) {
            $q['radius'] = (string) $f->SearchLocationDistance;
        }

        // Job-type flags → suffix list.
        $jobTypes = [];
        foreach (self::JOB_TYPE_SUFFIXES as $suffix) {
            if ($f->{"SearchJobType{$suffix}"}) {
                $jobTypes[] = $suffix;
            }
        }
        if ($jobTypes !== []) {
            $q['jt'] = implode(',', $jobTypes);
        }

        // Industry / education.
        if ($f->SearchIndustry !== []) {
            $q['ind'] = implode(',', $f->SearchIndustry);
        }
        if ($f->SearchJobEducationLevel !== []) {
            $q['edu'] = implode(',', $f->SearchJobEducationLevel);
        }

        // Date facet.
        if ($f->SearchDateSelection !== '0') {
            $q['date'] = $f->SearchDateSelection;
        }
        if ($f->StartDate !== null) {
            $q['from'] = $this->dateToParam($f->StartDate);
        }
        if ($f->EndDate !== null) {
            $q['to'] = $this->dateToParam($f->EndDate);
        }

        // Salary facet.
        if ($f->SalaryType !== 0) {
            $q['salarytype'] = (string) $f->SalaryType;
        }
        $brackets = [];
        for ($n = 1; $n <= 6; $n++) {
            if ($f->{"SalaryBracket{$n}"}) {
                $brackets[] = (string) $n;
            }
        }
        if ($brackets !== []) {
            $q['salary'] = implode(',', $brackets);
        }
        if ($f->SearchSalaryUnknown) {
            $q['salaryunknown'] = '1';
        }
        if ($f->SalaryMin !== null && $f->SalaryMin !== '') {
            $q['salarymin'] = $f->SalaryMin;
        }
        if ($f->SalaryMax !== null && $f->SalaryMax !== '') {
            $q['salarymax'] = $f->SalaryMax;
        }
        if ($f->SearchSalaryConditions !== []) {
            $q['benefits'] = implode(',', $f->SearchSalaryConditions);
        }

        // "More" facet.
        $groups = [];
        foreach (self::EQUITY_SUFFIXES as $suffix) {
            if ($f->{"SearchIs{$suffix}"}) {
                $groups[] = $suffix;
            }
        }
        if ($groups !== []) {
            $q['groups'] = implode(',', $groups);
        }
        if ($f->SearchIsPostingsInEnglish === false) {
            $q['en'] = '0';
        }
        if ($f->SearchIsPostingsInEnglishAndFrench) {
            $q['enf'] = '1';
        }
        if ($f->NocCode !== null && $f->NocCode !== '') {
            $q['noc'] = $f->NocCode;
        }
        if ($f->SearchNocField !== null && $f->SearchNocField !== '') {
            $q['nocf'] = $f->SearchNocField;
        }
        if ($f->SearchJobSource !== '0') {
            $q['source'] = $f->SearchJobSource;
        }
        if ($f->SearchExcludePlacementAgencyJobs) {
            $q['noagency'] = '1';
        }
        if ($f->SearchNjbJobsFirst) {
            $q['njb'] = '1';
        }

        return $q;
    }

    /**
     * Reconstruct the exact filters from a query-parameter map.
     *
     * Unknown parameters are ignored (so unrelated tracking/query params never
     * break a shared link), and every value is whitelisted against the known
     * option sets before it reaches {@see JobSearchFilters::fromArray()}.
     *
     * @param  array<string, mixed>  $query
     */
    public function fromQuery(array $query): JobSearchFilters
    {
        $version = $this->readVersion($query);

        return JobSearchFilters::fromArray($this->decode($query), $version);
    }

    /**
     * Build the canonical `/jobs?…` URL string for a filter set.
     */
    public function toUrl(JobSearchFilters $f, string $path = '/jobs'): string
    {
        $query = $this->toQuery($f);

        return $query === [] ? $path : $path . '?' . http_build_query($query);
    }

    /**
     * Decode a query map into the JobSearchFilters payload (PascalCase keys).
     *
     * @param  array<string, mixed>  $query
     * @return array<string, mixed>
     */
    private function decode(array $query): array
    {
        $payload = [];
        $get = static fn (string $key): ?string => isset($query[$key]) && is_scalar($query[$key])
            ? trim((string) $query[$key])
            : null;

        if (($q = $get('q')) !== null && $q !== '') {
            $payload['Keyword'] = $q;
        }
        if (($in = $get('in')) !== null && in_array($in, self::SEARCH_IN, true)) {
            $payload['SearchInField'] = $in;
        }
        if (($sort = $get('sort')) !== null && ctype_digit($sort)) {
            $sortInt = (int) $sort;
            if ($sortInt >= 1 && $sortInt <= 11) {
                $payload['SortOrder'] = $sortInt;
            }
        }
        if (($page = $get('page')) !== null && ctype_digit($page)) {
            $payload['Page'] = max(1, (int) $page);
        }
        if (($size = $get('size')) !== null && ctype_digit($size)) {
            $payload['PageSize'] = (int) $size;
        }

        // Location tokens (`c:` city | `r:` region | `p:` postal).
        if (($loc = $get('loc')) !== null && $loc !== '') {
            $locations = [];
            foreach (explode(';', $loc) as $token) {
                $parts = explode(':', $token, 2);
                if (count($parts) !== 2 || $parts[1] === '') {
                    continue;
                }
                [$type, $value] = $parts;
                $locations[] = match ($type) {
                    'c' => ['City' => $value],
                    'r' => ['Region' => $value],
                    'p' => ['Postal' => $value],
                    default => null,
                };
            }
            $locations = array_values(array_filter($locations));
            if ($locations !== []) {
                $payload['SearchLocations'] = $locations;
            }
        }
        if (($radius = $get('radius')) !== null && $this->isInt($radius)) {
            $payload['SearchLocationDistance'] = (int) $radius;
        }

        // Job-type flags.
        foreach ($this->decodeList($get('jt'), self::JOB_TYPE_SUFFIXES) as $suffix) {
            $payload["SearchJobType{$suffix}"] = true;
        }

        // Industry ids.
        if (($ind = $get('ind')) !== null && $ind !== '') {
            $ids = array_values(array_filter(
                array_map('intval', explode(',', $ind)),
                static fn (int $id): bool => in_array($id, self::INDUSTRY_IDS, true),
            ));
            if ($ids !== []) {
                $payload['SearchIndustry'] = $ids;
            }
        }

        // Education levels.
        $edu = $this->decodeList($get('edu'), self::EDUCATION_VALUES);
        if ($edu !== []) {
            $payload['SearchJobEducationLevel'] = $edu;
        }

        // Date facet.
        if (($date = $get('date')) !== null && in_array($date, ['0', '1', '2', '3'], true)) {
            $payload['SearchDateSelection'] = $date;
        }
        if (($from = $this->paramToDate($get('from'))) !== null) {
            $payload['StartDate'] = $from;
        }
        if (($to = $this->paramToDate($get('to'))) !== null) {
            $payload['EndDate'] = $to;
        }

        // Salary facet.
        if (($salaryType = $get('salarytype')) !== null && ctype_digit($salaryType)) {
            $typeInt = (int) $salaryType;
            if ($typeInt >= 0 && $typeInt <= 4) {
                $payload['SalaryType'] = $typeInt;
            }
        }
        if (($salary = $get('salary')) !== null && $salary !== '') {
            foreach (explode(',', $salary) as $n) {
                $n = (int) $n;
                if ($n >= 1 && $n <= 6) {
                    $payload["SalaryBracket{$n}"] = true;
                }
            }
        }
        if ($this->isTrue($get('salaryunknown'))) {
            $payload['SearchSalaryUnknown'] = true;
        }
        if (($min = $get('salarymin')) !== null && $min !== '') {
            $payload['SalaryMin'] = $min;
        }
        if (($max = $get('salarymax')) !== null && $max !== '') {
            $payload['SalaryMax'] = $max;
        }
        $benefits = $this->decodeList($get('benefits'), self::SALARY_CONDITIONS);
        if ($benefits !== []) {
            $payload['SearchSalaryConditions'] = $benefits;
        }

        // "More" facet.
        foreach ($this->decodeList($get('groups'), self::EQUITY_SUFFIXES) as $suffix) {
            $payload["SearchIs{$suffix}"] = true;
        }
        if ($get('en') === '0') {
            $payload['SearchIsPostingsInEnglish'] = false;
        }
        if ($this->isTrue($get('enf'))) {
            $payload['SearchIsPostingsInEnglishAndFrench'] = true;
        }
        if (($noc = $get('noc')) !== null && $noc !== '') {
            $payload['NocCode'] = $noc;
        }
        if (($nocf = $get('nocf')) !== null && $nocf !== '') {
            $payload['SearchNocField'] = $nocf;
        }
        if (($source = $get('source')) !== null && in_array($source, self::JOB_SOURCES, true)) {
            $payload['SearchJobSource'] = $source;
        }
        if ($this->isTrue($get('noagency'))) {
            $payload['SearchExcludePlacementAgencyJobs'] = true;
        }
        if ($this->isTrue($get('njb'))) {
            $payload['SearchNjbJobsFirst'] = true;
        }

        return $payload;
    }

    /**
     * Map a legacy Angular alert deep-link (matrix params) onto filters so a
     * redirect shim can forward already-sent emails to the canonical URL.
     *
     * Accepts either an associative array of already-parsed params or the raw
     * `;key=value;…` (or `key=value;…`) matrix string stored in
     * JobAlerts.UrlParameters / carried in the email hash fragment.
     *
     * Note: the legacy `industry` and `region` params use an Angular-specific id
     * taxonomy that does not map 1:1 onto this rewrite's NAICS/region ids, so
     * those two facets are intentionally NOT decoded here — alerts carrying them
     * are migrated authoritatively from their stored JobSearchFilters JSON via
     * {@see toQuery()}. Every other facet is mapped faithfully.
     *
     * @param  array<string, mixed>|string  $legacy
     */
    public function fromLegacy(array|string $legacy): JobSearchFilters
    {
        $params = is_string($legacy) ? $this->parseMatrix($legacy) : $this->lowercaseKeys($legacy);

        $payload = [];
        $get = static fn (string $key): ?string => isset($params[$key]) && is_scalar($params[$key])
            ? trim((string) $params[$key])
            : null;

        // Keyword + scope.
        foreach (['search' => 'all', 'title' => 'title', 'employer' => 'employer', 'job' => 'jobId'] as $key => $scope) {
            if (($value = $get($key)) !== null && $value !== '') {
                $payload['Keyword'] = $value;
                $payload['SearchInField'] = $scope;
                break;
            }
        }

        // Location (city / postal; region uses a legacy id map and is skipped).
        $locations = [];
        if (($city = $get('city')) !== null && $city !== '') {
            foreach (explode(',', $city) as $name) {
                if (trim($name) !== '') {
                    $locations[] = ['City' => trim($name)];
                }
            }
        }
        if (($postal = $get('postal')) !== null && $postal !== '') {
            foreach (explode(',', $postal) as $code) {
                if (trim($code) !== '') {
                    $locations[] = ['Postal' => trim($code)];
                }
            }
        }
        if ($locations !== []) {
            $payload['SearchLocations'] = $locations;
        }
        if (($radius = $get('radius')) !== null && $this->isInt($radius)) {
            $payload['SearchLocationDistance'] = (int) $radius;
        }

        // Paging / sort.
        if (($sort = $get('sortby')) !== null && ctype_digit($sort)) {
            $sortInt = (int) $sort;
            if ($sortInt >= 1 && $sortInt <= 11) {
                $payload['SortOrder'] = $sortInt;
            }
        }
        if (($page = $get('page')) !== null && ctype_digit($page)) {
            $payload['Page'] = max(1, (int) $page);
        }
        if (($size = $get('pagesize')) !== null && ctype_digit($size)) {
            $payload['PageSize'] = (int) $size;
        }

        // Job-type checkbox groups (legacy id → suffix).
        $this->applyLegacyIdList($payload, $get('hoursofwork'), self::LEGACY_JOB_HOURS, 'SearchJobType%s', true);
        $this->applyLegacyIdList($payload, $get('periodofemployment'), self::LEGACY_JOB_PERIOD, 'SearchJobType%s', true);
        $this->applyLegacyIdList($payload, $get('employmentterms'), self::LEGACY_JOB_TERMS, 'SearchJobType%s', true);
        $this->applyLegacyIdList($payload, $get('workplacetype'), self::LEGACY_JOB_WORKPLACE, 'SearchJobType%s', true);

        // Education / benefits / equity checkbox groups.
        $edu = $this->mapLegacyIds($get('education'), self::LEGACY_EDUCATION);
        if ($edu !== []) {
            $payload['SearchJobEducationLevel'] = array_values($edu);
        }
        $benefits = $this->mapLegacyIds($get('benefits'), self::LEGACY_BENEFITS);
        if ($benefits !== []) {
            $payload['SearchSalaryConditions'] = array_values($benefits);
        }
        $this->applyLegacyIdList($payload, $get('employmentgroups'), self::LEGACY_EQUITY, 'SearchIs%s', true);

        // Salary.
        if (($interval = $get('salaryinterval')) !== null && ctype_digit($interval)) {
            $typeInt = (int) $interval;
            if ($typeInt >= 0 && $typeInt <= 4) {
                $payload['SalaryType'] = $typeInt;
            }
        }
        if (($range = $get('salaryrange')) !== null && $range !== '') {
            foreach (explode(',', $range) as $n) {
                $n = (int) $n;
                if ($n >= 1 && $n <= 6) {
                    $payload["SalaryBracket{$n}"] = true;
                } elseif ($n === 7) {
                    $payload['SearchSalaryUnknown'] = true;
                }
            }
        }
        if (($min = $get('salaryrangemin')) !== null && $min !== '' && $min !== '0') {
            $payload['SalaryMin'] = $min;
        }
        if (($max = $get('salaryrangemax')) !== null && $max !== '' && $max !== '0') {
            $payload['SalaryMax'] = $max;
        }

        // Date.
        if (($dateType = $get('datetype')) !== null && in_array($dateType, ['0', '1', '2', '3'], true)) {
            $payload['SearchDateSelection'] = $dateType;
        }
        if (($start = $this->legacyDate($get('startdate'))) !== null) {
            $payload['StartDate'] = $start;
        }
        if (($end = $this->legacyDate($get('enddate'))) !== null) {
            $payload['EndDate'] = $end;
        }

        // NOC / source / flags.
        if (($noc = $get('noc')) !== null && ctype_digit($noc)) {
            $code = substr('00000' . $noc, -5);
            $payload['NocCode'] = $code;
            $payload['SearchNocField'] = $code;
        }
        if (($source = $get('jobsource')) !== null && in_array($source, self::JOB_SOURCES, true)) {
            $payload['SearchJobSource'] = $source;
        }
        if ($get('placementagency') !== null) {
            $payload['SearchExcludePlacementAgencyJobs'] = true;
        }
        if ($get('language') !== null) {
            $payload['SearchIsPostingsInEnglish'] = false;
            $payload['SearchIsPostingsInEnglishAndFrench'] = true;
        }

        return JobSearchFilters::fromArray($payload);
    }

    /** Legacy Angular checkbox id → job-type suffix maps (checkboxinfo.service.ts). */
    private const LEGACY_JOB_HOURS = [
        '1' => 'FullTime', '2' => 'PartTime', '3' => 'LeadingToFullTime',
    ];

    private const LEGACY_JOB_PERIOD = [
        '1' => 'Permanent', '2' => 'Temporary', '3' => 'Casual', '4' => 'Seasonal',
    ];

    private const LEGACY_JOB_TERMS = [
        '1' => 'Day', '2' => 'Early', '3' => 'Evening', '4' => 'Flexible', '5' => 'Morning',
        '6' => 'Night', '7' => 'OnCall', '8' => 'Overtime', '9' => 'Shift', '10' => 'Tbd', '12' => 'Weekend',
    ];

    private const LEGACY_JOB_WORKPLACE = [
        '0' => 'OnSite', '100000' => 'Hybrid', '100001' => 'Travelling', '15141' => 'Virtual',
    ];

    private const LEGACY_EDUCATION = [
        '1' => 'University',
        '3' => 'College or apprenticeship',
        '2' => 'Secondary school or job-specific training',
        '4' => 'No education',
    ];

    private const LEGACY_BENEFITS = [
        '1' => 'As per collective agreement', '2' => 'Bonus', '3' => 'Commission', '4' => 'Dental plan',
        '5' => 'Disability benefits', '6' => 'Gratuities', '7' => 'Group insurance benefits',
        '8' => 'Life insurance benefits', '9' => 'Health care plan', '10' => 'Mileage paid',
        '11' => 'Pension plan benefits', '12' => 'Piece work', '13' => 'RESP benefits',
        '14' => 'RRSP benefits', '15' => 'Vision care benefits', '16' => 'Other benefits',
    ];

    private const LEGACY_EQUITY = [
        '1' => 'Apprentice', '2' => 'Indigenous', '3' => 'MatureWorkers', '4' => 'Newcomers',
        '5' => 'PeopleWithDisabilities', '6' => 'Students', '7' => 'Veterans', '8' => 'VisibleMinority', '9' => 'Youth',
    ];

    /**
     * Read the (optional) shape version; unrecognised values fall back to the
     * current version so a stray `v` never 500s a public link.
     *
     * @param  array<string, mixed>  $query
     */
    private function readVersion(array $query): int
    {
        if (! isset($query['v']) || ! is_scalar($query['v']) || ! ctype_digit((string) $query['v'])) {
            return JobSearchFilters::CURRENT_VERSION;
        }

        $version = (int) $query['v'];

        return in_array($version, JobSearchFilters::SUPPORTED_VERSIONS, true)
            ? $version
            : JobSearchFilters::CURRENT_VERSION;
    }

    /**
     * Split a comma list and keep only values present in the whitelist.
     *
     * @param  string[]  $allowed
     * @return string[]
     */
    private function decodeList(?string $value, array $allowed): array
    {
        if ($value === null || $value === '') {
            return [];
        }

        return array_values(array_intersect(
            array_map('trim', explode(',', $value)),
            $allowed,
        ));
    }

    /**
     * Map a comma list of legacy ids through a lookup, dropping unknown ids.
     *
     * @param  array<string, string>  $map
     * @return array<int, string>
     */
    private function mapLegacyIds(?string $value, array $map): array
    {
        if ($value === null || $value === '') {
            return [];
        }

        $out = [];
        foreach (explode(',', $value) as $id) {
            $id = trim($id);
            if (isset($map[$id]) && ! in_array($map[$id], $out, true)) {
                $out[] = $map[$id];
            }
        }

        return $out;
    }

    /**
     * Set boolean flags on the payload for each mapped legacy id.
     *
     * @param  array<string, mixed>  $payload
     * @param  array<string, string>  $map
     */
    private function applyLegacyIdList(array &$payload, ?string $value, array $map, string $flagFormat, bool $flag): void
    {
        foreach ($this->mapLegacyIds($value, $map) as $suffix) {
            $payload[sprintf($flagFormat, $suffix)] = $flag;
        }
    }

    private function dateToParam(DateField $d): string
    {
        return sprintf('%04d-%02d-%02d', $d->Year, $d->Month, $d->Day);
    }

    /**
     * Parse a canonical `YYYY-MM-DD` date param into a DateField-shaped array.
     *
     * @return array<string, int>|null
     */
    private function paramToDate(?string $value): ?array
    {
        if ($value === null || preg_match('/^(\d{4})-(\d{2})-(\d{2})$/', $value, $m) !== 1) {
            return null;
        }

        return ['Year' => (int) $m[1], 'Month' => (int) $m[2], 'Day' => (int) $m[3]];
    }

    /**
     * Parse a legacy `YYYYMMDD` date param into a DateField-shaped array.
     *
     * @return array<string, int>|null
     */
    private function legacyDate(?string $value): ?array
    {
        if ($value === null || preg_match('/^(\d{4})(\d{2})(\d{2})$/', $value, $m) !== 1) {
            return null;
        }

        return ['Year' => (int) $m[1], 'Month' => (int) $m[2], 'Day' => (int) $m[3]];
    }

    private function isInt(string $value): bool
    {
        return preg_match('/^-?\d+$/', $value) === 1;
    }

    private function isTrue(?string $value): bool
    {
        return $value !== null && in_array(strtolower($value), ['1', 'true', 'on', 'yes'], true);
    }

    /**
     * Parse a `;key=value;…` matrix string into a lowercase-keyed map.
     *
     * @return array<string, string>
     */
    private function parseMatrix(string $matrix): array
    {
        // Tolerate a leading `#/job-search` fragment and surrounding separators.
        $matrix = preg_replace('~^[#/]+[a-z0-9/-]*~i', '', trim($matrix)) ?? $matrix;
        $params = [];
        foreach (preg_split('/[;&]/', trim($matrix, ';&')) ?: [] as $pair) {
            if ($pair === '' || ! str_contains($pair, '=')) {
                continue;
            }
            [$key, $value] = explode('=', $pair, 2);
            $key = strtolower(trim($key));
            if ($key !== '') {
                $params[$key] = urldecode($value);
            }
        }

        return $params;
    }

    /**
     * @param  array<string, mixed>  $params
     * @return array<string, mixed>
     */
    private function lowercaseKeys(array $params): array
    {
        $out = [];
        foreach ($params as $key => $value) {
            $out[strtolower((string) $key)] = $value;
        }

        return $out;
    }
}
