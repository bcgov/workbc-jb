<?php

declare(strict_types=1);

namespace WorkBC\InnovibeJobIndexer\Json;

use Monolog\Logger;
use PDO;

/**
 * Converts an Innovibe job JSON payload into the Elasticsearch document
 * (the PascalCase shape defined by ElasticSearchJob.cs + jobs_index.json).
 *
 * Faithful port of the JSON branch of
 * WorkBC.ElasticSearch.Indexing.Services.XmlParsingServiceWanted
 * (ConvertJsonToElasticJob) plus the shared XmlParsingServiceBase helpers.
 *
 * Innovibe is the only source of external (Wanted) jobs and its payloads are
 * always JSON — the legacy TalentNeuron XML branch is intentionally not ported.
 *
 * Null-valued fields are stripped just before serialization (mirrors the C#
 * NullValueHandling.Ignore), so this mapper leaves nulls in place.
 */
final class InnovibeDocumentMapper
{
    /** Sentinel coordinates the feed uses for "unknown"; remapped to BC centroid. */
    private const SENTINEL_LAT = '54.5000992';
    private const SENTINEL_LON = '-125.1159973';

    /** @var array<int,array{title:?string,frenchTitle:?string}>|null */
    private ?array $nocGroups2021 = null;
    /** @var array<string,string>|null  lower-case city name → region label */
    private ?array $uniqueCities = null;
    /** @var list<array{LocationId:int,Label:string,City:string,FederalCityId:?int}>|null */
    private ?array $duplicateCities = null;
    /** @var array<string,true>|null */
    private ?array $duplicateCityNames = null;

    /** Mirrors XmlManualOverRides.AlternateCityNames. Keys MUST be lowercase. */
    private const ALTERNATE_CITY_NAMES = [
        'one hundred mile house' => '100 Mile House',
        'queen charlotte' => 'Daajing Giids ( Queen Charlotte City )',
        'queen charlotte city' => 'Daajing Giids ( Queen Charlotte City )',
        'cowichan valley a' => 'Lake Cowichan',
        'cowichan' => 'Lake Cowichan',
        'denny island' => 'Bella Bella',
        'barrière' => 'Barriere',
        'garden' => 'Garden Village',
    ];

    public function __construct(
        private readonly PDO $db,
        private readonly Logger $log,
        private readonly int $wantedJobExpiryDays = 90,
    ) {}

    /**
     * @return array<string,mixed>|null  null when the payload is empty, not JSON,
     *         or has no usable id (the caller skips a null result).
     */
    public function convertToElasticJob(?string $jobData): ?array
    {
        if ($jobData === null || ltrim($jobData) === '') {
            return null;
        }
        if (!str_starts_with(ltrim($jobData), '{')) {
            // Innovibe payloads are always JSON; anything else is unexpected.
            $this->log->warning('Non-JSON Wanted payload skipped');
            return null;
        }

        try {
            $j = json_decode($jobData, true, 512, JSON_THROW_ON_ERROR);
        } catch (\Throwable $e) {
            $this->log->error('Exception in convertToElasticJob() (decode): ' . $e->getMessage());
            return null;
        }
        if (!is_array($j)) {
            return null;
        }

        try {
            return $this->buildDocument($j);
        } catch (\Throwable $e) {
            $this->log->error('Exception in convertToElasticJob(): ' . $e->getMessage());
            return null;
        }
    }

    /**
     * @param array<string,mixed> $j
     * @return array<string,mixed>|null
     */
    private function buildDocument(array $j): ?array
    {
        $jobId = (string) ($j['id'] ?? '');
        if (trim($jobId) === '') {
            // C# returns an empty ElasticSearchJob (JobId null) for these and the
            // service skips a null JobId. Returning null is equivalent here.
            return null;
        }

        $employmentTypes = is_array($j['employmentType'] ?? null) ? $j['employmentType'] : [];
        $empTypeStr = strtolower(implode(' ', array_map('strval', $employmentTypes)));

        // Dates
        $postedDateStr = $j['postedDate'] ?? $j['createdAt'] ?? null;
        $postedDate = $this->parseDateUtc($postedDateStr) ?? new \DateTimeImmutable('now', new \DateTimeZone('UTC'));
        $updatedAtStr = $j['updatedAt'] ?? $postedDateStr;
        $refreshedDate = $this->parseDateUtc($updatedAtStr) ?? $postedDate;

        // Location: prefer a BC location
        $city = '';
        $province = '';
        $lat = '';
        $lon = '';
        $locations = is_array($j['jobLocations'] ?? null) ? $j['jobLocations'] : [];
        if (!empty($locations)) {
            $loc = null;
            foreach ($locations as $candidate) {
                if (strcasecmp((string) ($candidate['state'] ?? ''), 'British Columbia') === 0) {
                    $loc = $candidate;
                    break;
                }
            }
            $loc ??= $locations[0];
            $city = (string) ($loc['city'] ?? '');
            $province = (string) ($loc['state'] ?? $loc['province'] ?? '');
            $lats = $loc['lats'] ?? null;
            $lngs = $loc['lngs'] ?? null;
            if (is_array($lats) && count($lats) > 0) {
                $lat = $this->numberToString($lats[0]);
            }
            if (is_array($lngs) && count($lngs) > 0) {
                $lon = $this->numberToString($lngs[0]);
            }
        }

        if (trim($city) === '' && trim($province) !== '') {
            $city = $province;
        }
        $city = $this->cleanUpCityName($city);

        $co = is_array($j['company'] ?? null) ? $j['company'] : null;
        $source = (string) ($j['sourceDomain'] ?? '');
        $sourceUrl = (string) ($j['url'] ?? '');
        $description = (string) ($j['description'] ?? '');
        $employerName = (string) (($co['name'] ?? null) ?? $j['companyName'] ?? '');
        $industry = (string) (($j['industry'] ?? null) ?? ($co['linkedinOrgIndustry'] ?? null) ?? '');
        $function = (string) ($j['function'] ?? $j['category'] ?? '');
        $title = (string) ($j['title'] ?? '');

        $job = [
            'JobId' => $jobId,
            'DatePosted' => $this->formatNaive($refreshedDate),
            'LastUpdated' => $this->formatNaive($refreshedDate),
            'Lang' => 'en',
            'SkillCategories' => [],
            'City' => [$this->getJobCity($city)],
            'Location' => [],
            'Province' => trim($province),
            'Region' => [$this->getJobRegion($city)],
            'HoursOfWork' => ['Description' => []],
            'PeriodOfEmployment' => ['Description' => []],
            // EmploymentTerms.Description stays null for Wanted jobs, so Newtonsoft
            // serializes the object as {} (an empty object, not []).
            'EmploymentTerms' => new \stdClass(),
            'IsFederalJob' => false,
            'ExpireDate' => $this->formatNaive($refreshedDate->add(new \DateInterval('P' . $this->wantedJobExpiryDays . 'D'))),
            'Industry' => $industry,
            'JobDescription' => $description,
            'Occupation' => '',
            'Function' => $function,
            // Federal-only value-type fields. The shared C# ElasticSearchJob class
            // leaves these at their defaults for Wanted jobs, and Newtonsoft emits
            // them (NullValueHandling.Ignore does not drop 0 / false).
            'WorkHours' => 0.0,
            'EmployerTypeId' => 0,
            'IsVariousLocation' => false,
            'IsAboriginal' => false,
            'IsApprentice' => false,
            'IsStudent' => false,
            'IsNewcomer' => false,
            'IsVeteran' => false,
            'IsVismin' => false,
            'IsYouth' => false,
            'IsMatureWorker' => false,
            'IsDisability' => false,
        ];

        // ── Employer name (must contain an English-ish word) ──────────
        $job['EmployerName'] = $this->filterEmployerName($employerName);

        // ── Salary ────────────────────────────────────────────────────
        $salaryMin = $this->parseDecimal($j['salaryMin'] ?? null);
        $salaryMax = $this->parseDecimal($j['salaryMax'] ?? null);
        $salaryValue = $this->parseDecimal($j['salaryValue'] ?? null);
        $salaryUnit = strtoupper((string) ($j['salaryUnitText'] ?? ''));
        $salary = $salaryMin ?? $salaryValue ?? $salaryMax;

        if ($salary !== null && $salary >= 0.01) {
            $multiplier = match ($salaryUnit) {
                'HOUR' => 2080.0,
                'WEEK' => 52.0,
                'MONTH' => 12.0,
                default => 1.0,
            };
            // C# computes salary in `decimal` (exact). Innovibe salary fields
            // carry up to 4 decimal places and the multiplier is an integer, so
            // the true annual value has at most 4 decimals. Rounding there
            // reproduces the decimal output while removing PHP float noise
            // (e.g. 84275.15199999999 → 84275.152).
            $annualSalary = $salary * $multiplier;
            $job['Salary'] = round($annualSalary, 4);

            if ($salaryMin !== null && $salaryMax !== null && $salaryMin != $salaryMax) {
                $annualMin = $salaryMin * $multiplier;
                $annualMax = $salaryMax * $multiplier;
                $job['SalarySummary'] = '$' . number_format($annualMin) . ' - $' . number_format($annualMax) . ' annually';
            } else {
                $job['SalarySummary'] = '$' . number_format($annualSalary) . ' annually';
            }
        } else {
            $job['Salary'] = null;
            $job['SalarySummary'] = 'N/A';
        }

        // ── Hours of work ─────────────────────────────────────────────
        $hours = [];
        if (str_contains($empTypeStr, 'full')) {
            $hours[] = 'Full-time';
        }
        if (str_contains($empTypeStr, 'part')) {
            $hours[] = 'Part-time';
        }
        if (str_contains($empTypeStr, 'casual')) {
            $hours[] = 'Casual';
        }
        if (str_contains($empTypeStr, 'on-call') || str_contains($empTypeStr, 'on_call')) {
            $hours[] = 'On-call';
        }
        $job['HoursOfWork'] = ['Description' => $hours];

        // ── Period of employment ──────────────────────────────────────
        $period = [];
        if (str_contains($empTypeStr, 'contract')) {
            $period[] = 'Contract';
        }
        if (str_contains($empTypeStr, 'temporary') || str_contains($empTypeStr, 'freelance')) {
            $period[] = 'Temporary';
        }
        if (str_contains($empTypeStr, 'seasonal')) {
            $period[] = 'Seasonal';
        }
        if (str_contains($empTypeStr, 'intern')) {
            $period[] = 'Intern';
        }
        if (str_contains($empTypeStr, 'student')) {
            $period[] = 'Student';
        }
        $job['PeriodOfEmployment'] = ['Description' => $period];

        // ── Location geo ──────────────────────────────────────────────
        if ($lat !== '' && $lon !== '') {
            $location = $this->locationOrNull($lat, $lon);
            if ($location !== null) {
                if (str_starts_with($location['Lat'], '999') && str_starts_with($location['Lon'], '999')) {
                    $location = ['Lat' => self::SENTINEL_LAT, 'Lon' => self::SENTINEL_LON];
                }
                $job['Location'] = [$location];
                $job['LocationGeo'] = [$location['Lat'] . ',' . $location['Lon']];
            }
        }

        // ── Education (3 fallback strategies) ──────────────────────────
        $eduLevel = null;
        $educationLevel = (string) ($j['educationLevel'] ?? '');
        if ($educationLevel !== '') {
            $eduLevel = $this->mapEducationLevelText($educationLevel);
        }
        if (($eduLevel === null || $eduLevel === '')
            && is_array($j['educationRequirementsDetailed'] ?? null)
            && count($j['educationRequirementsDetailed']) > 0
        ) {
            $first = $j['educationRequirementsDetailed'][0];
            $categoryName = (string) ($first['categoryName'] ?? '');
            if ($categoryName !== '') {
                $mapped = $this->mapEducationCategoryName($categoryName);
                if ($mapped !== null && $mapped !== '') {
                    $eduLevel = $mapped;
                }
            }
            if (($eduLevel === null || $eduLevel === '')) {
                $name = (string) ($first['name'] ?? '');
                if ($name !== '') {
                    $eduLevel = $this->mapEducationLevelText($name);
                }
            }
        }
        if (($eduLevel === null || $eduLevel === '')
            && is_array($j['educationRequirements'] ?? null)
            && count($j['educationRequirements']) > 0
        ) {
            $firstReq = (string) ($j['educationRequirements'][0] ?? '');
            if ($firstReq !== '') {
                $eduLevel = $this->mapEducationLevelText($firstReq);
            }
        }
        if ($eduLevel !== null && $eduLevel !== '') {
            $job['EduLevel'] = $eduLevel;
        }

        // ── Apply fields ──────────────────────────────────────────────
        $job['ApplyEmailAddress'] = '';
        $job['ApplyPhoneNumber'] = '';
        $job['ApplyWebsite'] = $sourceUrl;
        $job['PositionsAvailable'] = 1;
        $job['NaicsId'] = null;

        // ── NOC 2021 (from highest-scored nocMatches) ─────────────────
        $nocStr = '';
        $nocLabel = '';
        $nocMatches = is_array($j['nocMatches'] ?? null) ? $j['nocMatches'] : [];
        if (count($nocMatches) > 0) {
            $best = $nocMatches[0];
            $bestScore = (float) ($best['score'] ?? 0);
            foreach ($nocMatches as $m) {
                $s = (float) ($m['score'] ?? 0);
                if ($s > $bestScore) {
                    $best = $m;
                    $bestScore = $s;
                }
            }
            $nocStr = (string) ($best['code'] ?? '');
            $nocLabel = (string) ($best['title'] ?? '');
        }

        $noc2021Int = 0;
        if ($nocStr !== '') {
            $noc2021Int = (int) $nocStr;
            if (!$this->isValidNoc2021($noc2021Int)) {
                $noc2021Int = 0;
            }
        }
        $job['Noc2021'] = $noc2021Int === 0 ? null : $noc2021Int;

        if ($noc2021Int > 0) {
            $job['NocGroup'] = $this->getNocGroup2021($noc2021Int);
            $job['NocJobTitle'] = str_replace("\u{200B}", '', $nocLabel);
        }

        // Legacy Noc (2016) intentionally left null for Innovibe jobs.
        $job['Noc'] = null;

        if ($nocLabel !== '') {
            $job['Occupation'] = $nocLabel;
        }

        // ── Job title cleanup ─────────────────────────────────────────
        // C# uses String.Trim() which strips all Unicode whitespace (incl. the
        // NBSP that some feed titles carry); PHP trim() only handles ASCII.
        if ($this->unicodeTrim($title) !== '') {
            $job['Title'] = str_replace("\u{200B}", '', $this->unicodeTrim($title));
        }
        $job['Title'] = $this->cleanTitle($job['Title'] ?? null, $job['NocJobTitle'] ?? null);

        // ── External source ───────────────────────────────────────────
        $sources = [];
        if ($source !== '' || $sourceUrl !== '') {
            $sources[] = ['Url' => $sourceUrl, 'Source' => $source];
        }
        $job['ExternalSource'] = ['Source' => $sources];

        $job['SalarySort'] = $this->salarySort($job['Salary'] ?? null, $job['SalarySummary'] ?? null);

        return $job;
    }

    // ── Title cleanup ──────────────────────────────────────────────

    private function cleanTitle(?string $title, ?string $nocJobTitle): string
    {
        $title ??= '';
        // substitute NocJobTitle if Title has no letters
        if (preg_match('/[a-zA-Z]/', $title) !== 1) {
            $title = ($nocJobTitle === null || $nocJobTitle === '')
                ? 'No job title provided'
                : $nocJobTitle;
        }
        // all-caps → lowercase (CSS title-cases via text-transform)
        if (!preg_match('/\p{Ll}/u', $title)) {
            $title = mb_strtolower($title);
        }
        // always capitalize PT / FT
        $title = preg_replace('/\bpt\b/i', 'PT', $title) ?? $title;
        $title = preg_replace('/\bft\b/i', 'FT', $title) ?? $title;
        return $title;
    }

    private function filterEmployerName(string $employerName): string
    {
        $employerName = trim($employerName);
        if ($employerName === '') {
            return '';
        }
        // Mirrors the C# regex new Regex("[a-zA-Z0-9$@!%*?&#^_.+-]+").
        return preg_match('/[a-zA-Z0-9$@!%*?&#^_.+\-]+/', $employerName) === 1 ? $employerName : '';
    }

    // ── Education maps ─────────────────────────────────────────────

    private function mapEducationLevelText(string $text): ?string
    {
        $t = strtolower(trim($text));
        return match ($t) {
            'less than high school', 'no education' => 'No education',
            "some college", "associate's degree", 'postsecondary', 'college', 'apprenticeship' => 'College or apprenticeship',
            'doctoral', "master's degree", "bachelor's degree", 'university' => 'University',
            'high school', 'high school diploma', 'high school diploma or equivalent' => 'Secondary school or job-specific training',
            default => null,
        };
    }

    private function mapEducationCategoryName(string $categoryName): ?string
    {
        $c = strtolower(trim($categoryName));
        return match ($c) {
            'no education' => 'No education',
            'college', 'college or apprenticeship', 'apprenticeship' => 'College or apprenticeship',
            'university' => 'University',
            'secondary school or job-specific training', 'secondary school', 'high school' => 'Secondary school or job-specific training',
            default => null,
        };
    }

    // ── Location ───────────────────────────────────────────────────

    /**
     * @return array{Lat:string,Lon:string}|null
     */
    private function locationOrNull(string $lat, string $lon): ?array
    {
        if (!is_numeric($lat) || !is_numeric($lon)) {
            return null;
        }
        $dLat = (float) $lat;
        $dLon = (float) $lon;
        if ($dLat > 48.2 && $dLat < 60 && $dLon < -114 && $dLon > -139) {
            return ['Lat' => $lat, 'Lon' => $lon];
        }
        return null;
    }

    // ── City / region disambiguation (XmlParsingServiceBase) ───────

    private function cleanUpCityName(string $cityName): string
    {
        if (str_contains($cityName, '&apos;')) {
            $cityName = str_replace('&apos;', "'", $cityName);
        }
        $key = mb_strtolower($cityName);
        return self::ALTERNATE_CITY_NAMES[$key] ?? $cityName;
    }

    private function getJobCity(string $cityName, int $cityId = 0): string
    {
        $this->loadDuplicateCities();
        $lower = mb_strtolower($cityName);
        if (!isset($this->duplicateCityNames[$lower])) {
            return $cityName;
        }

        if ($cityId !== 0) {
            foreach ($this->duplicateCities as $row) {
                if ($row['FederalCityId'] === $cityId) {
                    return $row['Label'];
                }
            }
        } else {
            foreach ($this->duplicateCities as $row) {
                if (mb_strtolower($row['City']) === $lower) {
                    return $row['Label'];
                }
            }
        }

        foreach ($this->duplicateCities as $row) {
            if (mb_strtolower($row['City']) === $lower && $row['FederalCityId'] === null) {
                return $row['Label'];
            }
        }

        return $cityName;
    }

    private function getJobRegion(string $cityName, int $cityId = 0): string
    {
        $this->loadUniqueCities();
        $this->loadDuplicateCities();
        $lower = mb_strtolower($cityName);

        if (isset($this->uniqueCities[$lower])) {
            return $this->uniqueCities[$lower];
        }

        if ($cityId !== 0) {
            foreach ($this->duplicateCities as $row) {
                if ($row['FederalCityId'] === $cityId) {
                    return $this->stripCityFromLabel($row['Label'], $row['City']);
                }
            }
        }

        foreach ($this->duplicateCities as $row) {
            if (mb_strtolower($row['City']) === $lower) {
                return $this->stripCityFromLabel($row['Label'], $row['City']);
            }
        }

        return '';
    }

    private function stripCityFromLabel(string $label, string $city): string
    {
        $needle = $city . ' - ';
        $pos = strpos($label, $needle);
        if ($pos === false) {
            return $label;
        }
        return substr_replace($label, '', $pos, strlen($needle));
    }

    // ── NOC ────────────────────────────────────────────────────────

    private function getNocGroup2021(?int $nocId, bool $isFrench = false): string
    {
        if ($nocId === null || $nocId === 0) {
            return '';
        }
        $this->loadNocGroups2021();
        $codeStr = str_pad((string) $nocId, 5, '0', STR_PAD_LEFT);
        if (!isset($this->nocGroups2021[$nocId])) {
            return $codeStr;
        }
        $group = $this->nocGroups2021[$nocId];
        $title = $isFrench ? $group['frenchTitle'] : $group['title'];
        if ($title !== null && $title !== '') {
            return "{$title} ({$codeStr})";
        }
        return $codeStr;
    }

    // ── Salary sort ────────────────────────────────────────────────

    /**
     * @return array{Ascending:float,Descending:float}
     */
    private function salarySort(?float $salary, ?string $salarySummary): array
    {
        if ($salarySummary === 'N/A') {
            return ['Ascending' => 99999999.0, 'Descending' => -99999999.0];
        }
        $isZero = ($salary ?? 0.0) < 0.01;
        return [
            'Ascending' => $isZero ? 88888888.0 : $salary,
            'Descending' => $isZero ? -88888888.0 : $salary,
        ];
    }

    // ── DB lookups (lazy, cached) ──────────────────────────────────

    private function isValidNoc2021(int $id): bool
    {
        $this->loadNocGroups2021();
        return isset($this->nocGroups2021[$id]);
    }

    private function loadNocGroups2021(): void
    {
        if ($this->nocGroups2021 !== null) {
            return;
        }
        $rows = $this->db->query('SELECT "Id","Title","FrenchTitle" FROM "NocCodes2021"')?->fetchAll() ?: [];
        $this->nocGroups2021 = [];
        foreach ($rows as $row) {
            $this->nocGroups2021[(int) $row['Id']] = [
                'title' => $row['Title'] !== null ? (string) $row['Title'] : null,
                'frenchTitle' => $row['FrenchTitle'] !== null ? (string) $row['FrenchTitle'] : null,
            ];
        }
        $this->log->info(count($this->nocGroups2021) . ' NOC 2021 codes loaded');
    }

    private function loadUniqueCities(): void
    {
        if ($this->uniqueCities !== null) {
            return;
        }
        $rows = $this->db->query('
            SELECT l."City", r."Name" AS "Region"
            FROM "Locations" l
            INNER JOIN "Regions" r ON r."Id" = l."RegionId"
            WHERE l."IsDuplicate" = FALSE
              AND l."IsHidden" = FALSE
              AND l."RegionId" IS NOT NULL
              AND (r."Id" = 0 OR r."IsHidden" = FALSE)
        ')?->fetchAll() ?: [];

        $this->uniqueCities = [];
        foreach ($rows as $row) {
            $city = trim((string) ($row['City'] ?? ''));
            if ($city === '') {
                continue;
            }
            $this->uniqueCities[mb_strtolower($city)] = (string) ($row['Region'] ?? '');
        }
        $this->log->info(count($this->uniqueCities) . ' unique cities loaded for region lookup');
    }

    private function loadDuplicateCities(): void
    {
        if ($this->duplicateCities !== null) {
            return;
        }
        $rows = $this->db->query('
            SELECT "LocationId", "Label", "City", "FederalCityId"
            FROM "Locations"
            WHERE "IsDuplicate" = TRUE AND "IsHidden" = FALSE
        ')?->fetchAll() ?: [];

        $this->duplicateCities = [];
        $this->duplicateCityNames = [];
        foreach ($rows as $row) {
            $cityName = trim((string) ($row['City'] ?? ''));
            $this->duplicateCities[] = [
                'LocationId' => (int) $row['LocationId'],
                'Label' => trim((string) ($row['Label'] ?? '')),
                'City' => $cityName,
                'FederalCityId' => $row['FederalCityId'] === null ? null : (int) $row['FederalCityId'],
            ];
            if ($cityName !== '') {
                $this->duplicateCityNames[mb_strtolower($cityName)] = true;
            }
        }
        $this->log->info(count($this->duplicateCities) . ' duplicate cities loaded for disambiguation');
    }

    // ── parsing helpers ────────────────────────────────────────────

    private function parseDateUtc(mixed $raw): ?\DateTimeImmutable
    {
        if ($raw === null) {
            return null;
        }
        $raw = (string) $raw;
        if (trim($raw) === '') {
            return null;
        }
        try {
            return new \DateTimeImmutable($raw, new \DateTimeZone('UTC'));
        } catch (\Throwable) {
            return null;
        }
    }

    /**
     * Formats as a naive ISO 8601 timestamp WITHOUT an offset, e.g.
     * "2026-04-27T03:11:06". The .NET Wanted indexer's dates come from
     * Convert.ToDateTime(jsonString) with DateTimeKind.Unspecified, which
     * Newtonsoft serializes without a "Z" or offset. We keep the same wall-clock
     * instant (parsed as UTC) and drop the offset to match byte-for-byte.
     */
    private function formatNaive(\DateTimeImmutable $dt): string
    {
        return $dt->setTimezone(new \DateTimeZone('UTC'))->format('Y-m-d\TH:i:s');
    }

    private function parseDecimal(mixed $raw): ?float
    {
        if ($raw === null || $raw === '') {
            return null;
        }
        if (is_int($raw) || is_float($raw)) {
            return (float) $raw;
        }
        $clean = str_replace([',', ' '], '', (string) $raw);
        return is_numeric($clean) ? (float) $clean : null;
    }

    /**
     * Strips ALL Unicode whitespace including U+00A0 (NBSP) to match C#
     * String.Trim(). PHP trim() only handles ASCII whitespace.
     */
    private function unicodeTrim(string $value): string
    {
        if ($value === '') {
            return '';
        }
        return preg_replace('/^[\pZ\pC]+|[\pZ\pC]+$/u', '', $value) ?? $value;
    }

    /**
     * Stringifies a JSON-decoded number the way .NET's JToken.ToString() does:
     * the shortest round-trippable representation. PHP's (string) cast uses the
     * `precision` ini (14) and would drop digits (49.26403579999999 →
     * 49.2640358); json_encode honours `serialize_precision = -1` and produces
     * the exact round-trip form, matching the .NET output.
     */
    private function numberToString(mixed $value): string
    {
        if (is_string($value)) {
            return $value;
        }
        if (is_int($value) || is_float($value)) {
            return json_encode($value);
        }
        return (string) $value;
    }
}
