<?php

declare(strict_types=1);

namespace WorkBC\FederalJobImporter\Xml;

use DOMDocument;
use DOMElement;
use DOMNode;
use DOMXPath;
use Monolog\Logger;
use PDO;

/**
 * Parses a Federal Job-Bank XML body into the field set required by the
 * canonical "Jobs" / "JobVersions" tables.
 *
 * This is a deliberately *narrow* port of the C# `XmlParsingServiceFederal`
 * — it only extracts the subset of fields that the legacy
 * `JobsTableSyncService` writes back to SQL. The exhaustive
 * Elasticsearch-document construction (skill categories, geocoding, apply
 * methods, workplace-type strings, etc.) still lives in
 * `WorkBC.Indexers.Federal` (C#), which reads from the same staging table.
 *
 * Special cases preserved from the C# implementation:
 *   - NOC 2021 codes 00011..00015 → 00018  (NOC-387 special case)
 *   - Workplace type "Virtual"            → LocationId = -4
 *   - More than one BC city               → LocationId = -5
 *   - City list filtered to BC province only and sorted alphabetically
 *     (mirrors `FederalXmlLocations`)
 *   - Title / City / EmployerName / SalarySummary truncations match the
 *     SQL column lengths from `WorkBC.Data.Model.JobBoard.Job`
 *     (300 / 120 / 100 / 60).
 */
final class XmlJobMapper
{
    private const SPECIAL_NOC_2021_CODES = [11, 12, 13, 14, 15];
    private const SPECIAL_NOC_2021_REPLACEMENT = 18;

    /**
     * Mirrors WorkBC.Shared.Constants.WorkplaceTypeId. The numeric values
     * are *not* sequential — they match the actual `option_name id` values
     * that appear in the Federal feed under skill_category id="100000".
     */
    private const WORKPLACE_TYPE_ON_SITE = 0;
    private const WORKPLACE_TYPE_HYBRID = 100000;
    private const WORKPLACE_TYPE_TRAVELLING = 100001;
    private const WORKPLACE_TYPE_VIRTUAL = 15141;
    /** XML <skill_category id="100000"> = Workplace information section. */
    private const WORKPLACE_INFO_SKILL_CATEGORY_ID = 100000;

    private const VIRTUAL_LOCATION_ID = -4;
    private const MULTIPLE_LOCATIONS_ID = -5;

    private const WEEKLY_WORK_HOURS = 40.0;
    private const MAX_HOURLY_RATE = 2500.0;
    private const MAX_WEEKLY_SALARY = 100000.0;
    private const MAX_YEARLY_SALARY = 5000000.0;
    private const MINIMUM_WAGE_FALLBACK = 17.40;

    /** Matches WorkBC.Shared.Constants.General.DefaultWantedJobExpiryDays. */
    private const DEFAULT_EXPIRY_DAYS = 90;

    /** English-only label used when a virtual posting has no resolvable city. */
    private const VIRTUAL_JOB_BASED_IN_EN = 'Virtual job based in';

    private PDO $db;
    private Logger $log;

    /** @var array<int, true>|null  validated NOC 2021 IDs, lazy-loaded */
    private ?array $validNoc2021 = null;
    /** @var array<int, true>|null  validated NOC 2016 IDs, lazy-loaded */
    private ?array $validNoc = null;
    /** @var array<string, int>|null  Locations.Label (lower-cased) → LocationId */
    private ?array $locationLookup = null;
    /**
     * @var list<array{LocationId:int, Label:string, City:string, FederalCityId:?int}>|null
     * Cached duplicate-cities rows (Locations.IsDuplicate = TRUE AND !IsHidden).
     */
    private ?array $duplicateCities = null;
    /** @var array<string, true>|null  lower-case city names that appear in DuplicateCities */
    private ?array $duplicateCityNames = null;
    private ?float $minimumWage = null;

    /**
     * Mirrors WorkBC.ElasticSearch.Indexing.XmlParsingHelpers.XmlManualOverRides.
     * Keys MUST be lowercase.
     */
    private const ALTERNATE_CITY_NAMES = [
        'one hundred mile house' => '100 Mile House',
        'queen charlotte'        => 'Daajing Giids ( Queen Charlotte City )',
        'queen charlotte city'   => 'Daajing Giids ( Queen Charlotte City )',
        'cowichan valley a'      => 'Lake Cowichan',
        'cowichan'               => 'Lake Cowichan',
        'denny island'           => 'Bella Bella',
        'barrière'               => 'Barriere',
        'garden'                 => 'Garden Village',
    ];

    public function __construct(PDO $db, Logger $logger)
    {
        $this->db = $db;
        $this->log = $logger;
    }

    /**
     * Maps a job-posting XML body into the field set expected by the
     * "Jobs" / "JobVersions" tables. Returns `null` if the document is
     * malformed or contains zero `<Document>` entries.
     *
     * @return array<string,mixed>|null
     */
    public function map(string $xml): ?array
    {
        $doc = $this->loadXml($xml);
        if ($doc === null) {
            return null;
        }

        $xpath = new DOMXPath($doc);
        $found = (int) ($xpath->evaluate('string(/SolrResponse/Header/numFound)') ?: '0');
        if ($found !== 1) {
            return null;
        }

        $job = $xpath->query('/SolrResponse/Documents/Document')->item(0);
        if (!$job instanceof DOMElement) {
            return null;
        }

        $now = date('Y-m-d H:i:s');

        $jobId = $this->text($job, 'jobs_id');
        if ($jobId === '') {
            return null;
        }

        $title = $this->text($job, 'title');
        $employerName = $this->text($job, 'employer_name_string');
        $datePosted = $this->parseDate($this->text($job, 'date_posted')) ?? $now;
        $actualDatePosted = $datePosted;
        $expireDate = $this->parseDate($this->text($job, 'display_until'));
        $lastUpdated = $this->parseDate($this->text($job, 'file_update_date'));
        $positions = max(1, (int) ($this->text($job, 'num_positions') ?: '1'));
        $naicsId = (int) ($this->text($job, 'naics_id') ?: '0');
        $industryId = $naicsId === 0 ? null : $naicsId;

        $noc = $this->resolveNoc2016($this->text($job, 'noc2016'));
        $noc2021 = $this->resolveNoc2021($this->text($job, 'noc2021'));

        [$workplaceTypeId] = $this->resolveWorkplaceType($job, $xpath);

        if ($workplaceTypeId === self::WORKPLACE_TYPE_VIRTUAL) {
            $cityCsv = $this->resolveVirtualCity($job);
            $locationId = self::VIRTUAL_LOCATION_ID;
        } else {
            [$cityCsv, $cityCount] = $this->resolveCities($job, $xpath);
            // For non-virtual jobs the post-disambiguation labels feed the
            // location lookup just like the BC-cities CSV did before.
            $locationId = $this->resolveLocationId($cityCount, $this->firstCity($cityCsv));
        }

        [$salary, $salarySummary] = $this->resolveSalary($job, $xpath);

        [$ft, $pt, $leadingToFt] = $this->resolveWorkPeriod($this->text($job, 'work_period_cd'));
        [$permanent, $temporary, $casual, $seasonal] = $this->resolveWorkTerm($this->text($job, 'work_term_cd'));

        $expireFallback = date('Y-m-d H:i:s', strtotime('+' . self::DEFAULT_EXPIRY_DAYS . ' days'));

        return [
            'jobId'             => $jobId,
            'title'             => $this->mbTruncate($title, 300),
            'city'              => $this->mbTruncate($cityCsv, 120),
            'employerName'      => $this->mbTruncate($employerName, 100),
            'nocCodeId'         => $noc,
            'nocCodeId2021'     => $noc2021,
            'industryId'        => $industryId,
            'salary'            => $salary,
            'salarySummary'     => $this->mbTruncate($salarySummary, 60),
            'positionsAvailable'=> $positions,
            'datePosted'        => $datePosted,
            'actualDatePosted'  => $actualDatePosted,
            'expireDate'        => $expireDate ?? $expireFallback,
            'lastUpdated'       => $lastUpdated,
            'locationId'        => $locationId,
            'fullTime'          => $ft,
            'partTime'          => $pt,
            'leadingToFullTime' => $leadingToFt,
            'permanent'         => $permanent,
            'temporary'         => $temporary,
            'casual'            => $casual,
            'seasonal'          => $seasonal,
            'workplaceTypeId'   => $workplaceTypeId,
        ];
    }

    /**
     * Mirrors the C# `XmlParsingServiceFederal` virtual-location flow.
     *
     *   1. Look up `GeocodedLocationCache.Name = '{postalCode}, CANADA'`.
     *   2. On miss with a 6-char postal code, retry with the 3-char prefix.
     *   3. On hit: format as "Virtual job based in {City}, {Province}"
     *      (City missing → "Virtual job based in {Province}", everything
     *      missing → fall through to postal-prefix → province table).
     *   4. On miss with no usable cache row: "Virtual job based in {postalCode}".
     *
     * Note: V2 does *not* call Google Maps — the C# `GeocodingService` does on
     * cache miss, but we need PostgreSQL-only behaviour here. New postal codes
     * unseen by the cache will produce the postal-code form (matches the C#
     * miss branch).
     */
    private function resolveVirtualCity(DOMElement $job): string
    {
        $postalCode = $this->text($job, 'employer_postal_code');
        if ($postalCode === '') {
            return '';
        }

        $cache = $this->lookupGeocodedLocation($postalCode . ', CANADA');
        if ($cache === null && strlen($postalCode) === 6) {
            $cache = $this->lookupGeocodedLocation(substr($postalCode, 0, 3) . ', CANADA');
        }

        if ($cache === null) {
            return self::VIRTUAL_JOB_BASED_IN_EN . ' ' . $postalCode;
        }

        $city = isset($cache['City']) ? trim((string) $cache['City']) : '';
        $province = isset($cache['Province']) ? trim((string) $cache['Province']) : '';

        if ($city === '' && $province === '') {
            $byPrefix = $this->virtualProvinceByPostalPrefix($postalCode);
            return $byPrefix !== null
                ? self::VIRTUAL_JOB_BASED_IN_EN . ' ' . $byPrefix
                : self::VIRTUAL_JOB_BASED_IN_EN . ' ' . $postalCode;
        }

        if ($city === '') {
            return self::VIRTUAL_JOB_BASED_IN_EN . ' ' . $province;
        }

        return self::VIRTUAL_JOB_BASED_IN_EN . ' ' . $city . ($province !== '' ? ", {$province}" : '');
    }

    /** @return array<string,mixed>|null */
    private function lookupGeocodedLocation(string $name): ?array
    {
        if (!isset($this->geocodingStmt)) {
            $this->geocodingStmt = $this->db->prepare(
                'SELECT "City", "Province", "FrenchCity" FROM "GeocodedLocationCache" WHERE "Name" = ? LIMIT 1'
            );
        }
        $this->geocodingStmt->execute([$name]);
        $row = $this->geocodingStmt->fetch();
        return $row === false ? null : $row;
    }

    private \PDOStatement $geocodingStmt;

    /**
     * Mirrors the C# `XmlParsingServiceFederal` postal-code-prefix → province
     * lookup table used when the geocoding cache only knows the country.
     */
    private function virtualProvinceByPostalPrefix(string $postalCode): ?string
    {
        $first = strtoupper(substr($postalCode, 0, 1));
        return match ($first) {
            'A' => 'Newfoundland and Labrador',
            'B' => 'Nova Scotia',
            'C' => 'Prince Edward Island',
            'E' => 'New Brunswick',
            'G' => 'Eastern Quebec',
            'H' => 'Metropolitan Montréal',
            'J' => 'Western Quebec',
            'K' => 'Eastern Ontario',
            'L' => 'Central Ontario',
            'M' => 'Metropolitan Toronto',
            'N' => 'Southwestern Ontario',
            'P' => 'Northern Ontario',
            'R' => 'Manitoba',
            'S' => 'Saskatchewan',
            'T' => 'Alberta',
            'V' => 'British Columbia',
            'X' => 'Northwest Territories and Nunavut',
            'Y' => 'Yukon',
            default => null,
        };
    }

    private function resolveNoc2016(string $raw): ?int
    {
        if ($raw === '') {
            return null;
        }
        $candidate = (int) $raw;
        if ($candidate <= 0) {
            return null;
        }
        if (!$this->isValidNoc($candidate)) {
            return null;
        }
        return $candidate;
    }

    private function resolveNoc2021(string $raw): ?int
    {
        if ($raw === '') {
            return null;
        }
        $candidate = (int) $raw;
        if ($candidate <= 0) {
            return null;
        }
        if (in_array($candidate, self::SPECIAL_NOC_2021_CODES, true)) {
            $candidate = self::SPECIAL_NOC_2021_REPLACEMENT;
        }
        if (!$this->isValidNoc2021($candidate)) {
            return null;
        }
        return $candidate;
    }

    private function isValidNoc(int $id): bool
    {
        if ($this->validNoc === null) {
            $rows = $this->db->query('SELECT "Id" FROM "NocCodes"')?->fetchAll(PDO::FETCH_COLUMN) ?: [];
            $this->validNoc = array_flip(array_map('intval', $rows));
            $this->log->info(count($this->validNoc) . ' NOC 2016 codes loaded');
        }
        return isset($this->validNoc[$id]);
    }

    private function isValidNoc2021(int $id): bool
    {
        if ($this->validNoc2021 === null) {
            $rows = $this->db->query('SELECT "Id" FROM "NocCodes2021"')?->fetchAll(PDO::FETCH_COLUMN) ?: [];
            $this->validNoc2021 = array_flip(array_map('intval', $rows));
            $this->log->info(count($this->validNoc2021) . ' NOC 2021 codes loaded');
        }
        return isset($this->validNoc2021[$id]);
    }

    /**
     * @return array{0:int} 0:workplaceTypeId
     */
    private function resolveWorkplaceType(DOMElement $job, DOMXPath $xpath): array
    {
        $typeId = self::WORKPLACE_TYPE_ON_SITE;

        $workplaceCategory = $xpath->query(
            'skill_categories/skill_category[@id="' . self::WORKPLACE_INFO_SKILL_CATEGORY_ID . '"]/options/option_name',
            $job
        );
        if ($workplaceCategory && $workplaceCategory->length > 0) {
            $first = $workplaceCategory->item(0);
            if ($first instanceof DOMElement) {
                $candidate = (int) $first->getAttribute('id');
                if (in_array($candidate, [
                    self::WORKPLACE_TYPE_HYBRID,
                    self::WORKPLACE_TYPE_TRAVELLING,
                    self::WORKPLACE_TYPE_VIRTUAL,
                ], true)) {
                    $typeId = $candidate;
                }
            }
        }

        return [$typeId];
    }

    /**
     * @return array{0:string, 1:int} csv, count
     *
     * Mirrors C# `FederalXmlLocations` + `XmlParsingServiceFederal` city loop:
     *   1. Pair city_name[i] with city_id[i] and province_cd[i].
     *   2. Keep only entries where province == "BC", deduped on (CleanUpCityName-applied) name.
     *   3. Sort by name (alphabetical, case-insensitive).
     *   4. Apply CleanUpCityName + GetJobCity(name, id) to each entry to add
     *      duplicate-city disambiguation suffix
     *      (e.g. "Mill Bay" → "Mill Bay - Vancouver Island / Coast").
     *   5. Join with ", " and dedupe identical labels (same as C# `cities.Distinct()`).
     */
    private function resolveCities(DOMElement $job, DOMXPath $xpath): array
    {
        $cityNodes = $xpath->query('city_name/string', $job);
        $idNodes = $xpath->query('city_id/string', $job);
        $provinceNodes = $xpath->query('province_cd/string', $job);
        $count = $cityNodes ? $cityNodes->length : 0;
        if ($count === 0) {
            return ['', 0];
        }

        $defaultProvince = ($provinceNodes && $provinceNodes->length > 0)
            ? trim((string) $provinceNodes->item(0)->textContent)
            : '';

        // Pair each city with its id; preserve XML order, dedupe by raw name.
        $bcEntries = [];
        $seen = [];
        for ($i = 0; $i < $count; $i++) {
            $cityNode = $cityNodes->item($i);
            if (!$cityNode instanceof DOMNode) {
                continue;
            }
            $city = trim((string) $cityNode->textContent);
            if ($city === '') {
                continue;
            }
            $province = $defaultProvince;
            if ($provinceNodes && $provinceNodes->length > $i) {
                $province = trim((string) $provinceNodes->item($i)->textContent);
            }
            if (strcasecmp($province, 'BC') !== 0) {
                continue;
            }
            if (isset($seen[$city])) {
                continue;
            }
            $seen[$city] = true;
            $cityId = 0;
            if ($idNodes && $idNodes->length > $i) {
                $cityId = (int) trim((string) $idNodes->item($i)->textContent);
            }
            $bcEntries[] = ['name' => $city, 'id' => $cityId];
        }

        if (empty($bcEntries)) {
            return ['', 0];
        }

        usort($bcEntries, static fn(array $a, array $b): int => strcasecmp($a['name'], $b['name']));

        $labels = [];
        foreach ($bcEntries as $entry) {
            $cleaned = $this->cleanUpCityName($entry['name']);
            $labels[$this->getJobCity($cleaned, $entry['id'])] = true;
        }

        $finalLabels = array_keys($labels);
        return [implode(', ', $finalLabels), count($finalLabels)];
    }

    /** Mirrors C# `XmlParsingServiceBase.CleanUpCityName`. */
    private function cleanUpCityName(string $cityName): string
    {
        if (str_contains($cityName, '&apos;')) {
            $cityName = str_replace('&apos;', "'", $cityName);
        }
        $key = strtolower($cityName);
        if (isset(self::ALTERNATE_CITY_NAMES[$key])) {
            return self::ALTERNATE_CITY_NAMES[$key];
        }
        return $cityName;
    }

    /** Mirrors C# `XmlParsingServiceBase.GetJobCity`. */
    private function getJobCity(string $cityName, int $cityId): string
    {
        $this->loadDuplicateCities();
        $lower = strtolower($cityName);
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
                if (strcasecmp((string) $row['City'], $cityName) === 0) {
                    return $row['Label'];
                }
            }
        }

        // Fallback: matching name, FederalCityId NULL.
        foreach ($this->duplicateCities as $row) {
            if (strcasecmp((string) $row['City'], $cityName) === 0 && $row['FederalCityId'] === null) {
                return $row['Label'];
            }
        }

        return $cityName;
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
                'LocationId'    => (int) $row['LocationId'],
                'Label'         => trim((string) ($row['Label'] ?? '')),
                'City'          => $cityName,
                'FederalCityId' => $row['FederalCityId'] === null ? null : (int) $row['FederalCityId'],
            ];
            if ($cityName !== '') {
                $this->duplicateCityNames[strtolower($cityName)] = true;
            }
        }
        $this->log->info(count($this->duplicateCities) . ' duplicate cities loaded for federal disambiguation');
    }

    private function resolveLocationId(int $cityCount, string $singleCity): int
    {
        if ($cityCount === 0) {
            return 0;
        }
        if ($cityCount > 1) {
            return self::MULTIPLE_LOCATIONS_ID;
        }
        return $this->getBestAvailableLocationId($singleCity);
    }

    /** Returns the first city from a "Cityname, Cityname" CSV. */
    private function firstCity(string $cityCsv): string
    {
        if ($cityCsv === '') {
            return '';
        }
        $parts = explode(',', $cityCsv, 2);
        return trim($parts[0]);
    }

    /**
     * @return array{0: ?float, 1: string} salary, summary
     */
    private function resolveSalary(DOMElement $job, DOMXPath $xpath): array
    {
        $hourly = $this->parseDecimal($this->text($job, 'salary_hourly'));
        $weekly = $this->parseDecimal($this->text($job, 'salary_weekly'));
        $yearly = $this->parseDecimal($this->text($job, 'salary_yearly'));
        $salaryString = trim((string) ($xpath->evaluate('string(salary/string)', $job) ?: ''));
        $hours = trim((string) ($xpath->evaluate('string(hours)', $job) ?: ''));

        $minWage = $this->getMinimumWage();
        $salary = $this->calculateAnnualSalary($hourly, $weekly, $yearly, $salaryString, $minWage);

        $summary = $this->formatSalarySummary($salary, $salaryString, $hourly, $weekly, $yearly);

        // Legacy quirk: when a salary string is present but the value is invalid,
        // fall back to the raw string instead of "N/A" (see C# legacy comment about
        // provincial legislation prohibiting jobs without a salary).
        if ($salary === null && $salaryString !== '') {
            $summary = $salaryString;
        }

        // hours is unused in the SQL Jobs table (the SalaryDescription column lives
        // on the ES document only), but we still parse it to match the legacy logic.
        unset($hours);

        return [$salary, $summary];
    }

    private function calculateAnnualSalary(
        ?float $hourly,
        ?float $weekly,
        ?float $yearly,
        string $salaryString,
        float $minWage
    ): ?float {
        $stringIsHourly = stripos($salaryString, 'hour') !== false;
        if ($stringIsHourly && $hourly !== null && $hourly >= 0.9 * $minWage && $hourly < self::MAX_HOURLY_RATE) {
            return $hourly * self::WEEKLY_WORK_HOURS * 52;
        }
        if ($yearly !== null && $yearly >= $minWage * self::WEEKLY_WORK_HOURS * 52 && $yearly < self::MAX_YEARLY_SALARY) {
            return $yearly;
        }
        if ($hourly !== null && $hourly >= $minWage && $hourly < self::MAX_HOURLY_RATE) {
            return $hourly * self::WEEKLY_WORK_HOURS * 52;
        }
        if ($weekly !== null && $weekly >= $minWage * self::WEEKLY_WORK_HOURS && $weekly < self::MAX_WEEKLY_SALARY) {
            return $weekly * 52;
        }
        return null;
    }

    private function formatSalarySummary(
        ?float $salary,
        string $salaryString,
        ?float $hourly,
        ?float $weekly,
        ?float $yearly
    ): string {
        if ($salaryString !== '') {
            $summary = $salaryString;
            if (stripos($summary, 'annually') !== false) {
                $summary = str_replace('.00', '', $summary);
            }
            return $summary;
        }
        if ($salary === null || $salary <= 0) {
            return 'N/A';
        }
        if ($yearly !== null && $yearly > 0) {
            return '$' . number_format($yearly) . ' annually';
        }
        if ($hourly !== null && $hourly > 0) {
            return '$' . number_format($hourly, 2) . ' hourly';
        }
        if ($weekly !== null && $weekly > 0) {
            return '$' . number_format($weekly, 2) . ' weekly';
        }
        return 'N/A';
    }

    private function getMinimumWage(): float
    {
        if ($this->minimumWage === null) {
            try {
                $stmt = $this->db->prepare('SELECT "Value" FROM "SystemSettings" WHERE "Name" = ? LIMIT 1');
                $stmt->execute(['shared.settings.minimumWage']);
                $value = $stmt->fetchColumn();
                if ($value !== false && $value !== null && (string) $value !== '') {
                    $this->minimumWage = (float) $value;
                }
            } catch (\Throwable) {
                // Table may be absent in older schemas — fall through to default.
            }
            if ($this->minimumWage === null) {
                $this->minimumWage = self::MINIMUM_WAGE_FALLBACK;
            }
        }
        return $this->minimumWage;
    }

    /**
     * @return array{0:bool,1:bool,2:bool} fullTime, partTime, leadingToFullTime
     */
    private function resolveWorkPeriod(string $code): array
    {
        $code = strtoupper(trim($code));
        return match ($code) {
            'F'     => [true, false, false],
            'P'     => [false, true, false],
            'L'     => [false, false, true],
            default => [false, false, false],
        };
    }

    /**
     * @return array{0:bool,1:bool,2:bool,3:bool} permanent, temporary, casual, seasonal
     */
    private function resolveWorkTerm(string $code): array
    {
        $code = strtoupper(trim($code));
        return match ($code) {
            'P'     => [true, false, false, false],
            'T'     => [false, true, false, false],
            'C'     => [false, false, true, false],
            'S'     => [false, false, false, true],
            default => [false, false, false, false],
        };
    }

    private function getBestAvailableLocationId(string $city): int
    {
        $this->loadLocationLookup();
        $key = strtolower(trim($city));
        if ($key === '' || $this->locationLookup === null) {
            return 0;
        }

        if (isset($this->locationLookup[$key])) {
            return $this->locationLookup[$key];
        }

        if (mb_strlen($key) < 5) {
            return 0;
        }

        foreach ($this->locationLookup as $label => $locId) {
            if (str_starts_with($key, $label)) {
                return $locId;
            }
        }
        foreach ($this->locationLookup as $label => $locId) {
            if (str_starts_with($label, $key)) {
                return $locId;
            }
        }
        foreach ($this->locationLookup as $label => $locId) {
            if (str_contains($key, $label)) {
                return $locId;
            }
        }
        foreach ($this->locationLookup as $label => $locId) {
            if (str_contains($label, $key)) {
                return $locId;
            }
        }
        return 0;
    }

    private function loadLocationLookup(): void
    {
        if ($this->locationLookup !== null) {
            return;
        }
        $rows = $this->db->query('
            SELECT "Label", "LocationId"
            FROM "Locations"
            WHERE "IsHidden" = FALSE
              AND ("IsDuplicate" = FALSE OR "FederalCityId" IS NOT NULL)
        ')?->fetchAll() ?: [];

        $this->locationLookup = [];
        foreach ($rows as $row) {
            if (!isset($row['Label'])) {
                continue;
            }
            $label = strtolower(trim((string) $row['Label']));
            if ($label === '') {
                continue;
            }
            $this->locationLookup[$label] = (int) $row['LocationId'];
        }
        $this->log->info(count($this->locationLookup) . ' locations loaded for lookup');
    }

    private function loadXml(string $body): ?DOMDocument
    {
        if ($body === '') {
            return null;
        }
        $previous = libxml_use_internal_errors(true);
        $doc = new DOMDocument();
        $doc->preserveWhiteSpace = false;
        $loaded = $doc->loadXML($body, LIBXML_NONET | LIBXML_NOERROR | LIBXML_NOWARNING);
        libxml_clear_errors();
        libxml_use_internal_errors($previous);
        return $loaded ? $doc : null;
    }

    private function text(DOMElement $parent, string $name): string
    {
        $children = $parent->getElementsByTagName($name);
        if ($children->length === 0) {
            return '';
        }
        $first = $children->item(0);
        if ($first === null || $first->parentNode !== $parent) {
            // First match might be nested; rescan with a direct-child filter
            foreach ($children as $candidate) {
                if ($candidate->parentNode === $parent) {
                    return $this->unicodeTrim((string) $candidate->textContent);
                }
            }
            return $this->unicodeTrim((string) $first?->textContent);
        }
        return $this->unicodeTrim((string) $first->textContent);
    }

    /**
     * Mirrors C# `String.Trim()` which strips ALL Unicode whitespace including
     * U+00A0 (non-breaking space). PHP's built-in `trim()` only knows about
     * ASCII whitespace, so federal titles that arrive with a trailing NBSP
     * (e.g. "hotel maintenance helper ") would otherwise survive.
     */
    private function unicodeTrim(string $value): string
    {
        if ($value === '') {
            return '';
        }
        return preg_replace('/^[\pZ\pC]+|[\pZ\pC]+$/u', '', $value) ?? $value;
    }

    private function parseDate(string $raw): ?string
    {
        if ($raw === '') {
            return null;
        }
        $ts = strtotime($raw);
        if ($ts === false) {
            return null;
        }
        return date('Y-m-d H:i:s', $ts);
    }

    private function parseDecimal(string $raw): ?float
    {
        if ($raw === '') {
            return null;
        }
        $clean = str_replace([',', ' '], ['', ''], $raw);
        if (!is_numeric($clean)) {
            return null;
        }
        return (float) $clean;
    }

    private function mbTruncate(string $value, int $max): string
    {
        if ($value === '' || mb_strlen($value) <= $max) {
            return $value;
        }
        return mb_substr($value, 0, $max);
    }
}
