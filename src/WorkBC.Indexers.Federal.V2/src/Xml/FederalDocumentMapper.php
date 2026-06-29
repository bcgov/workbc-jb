<?php

declare(strict_types=1);

namespace WorkBC\FederalJobIndexer\Xml;

use DOMDocument;
use DOMElement;
use DOMNode;
use DOMXPath;
use Monolog\Logger;
use PDO;
use WorkBC\FederalJobIndexer\Geocoding\GeocodingService;

/**
 * Converts a Federal Job-Bank XML body into the FULL Elasticsearch document
 * (the PascalCase shape defined by ElasticSearchJob.cs + Resources/jobs_index.json).
 *
 * This is a faithful port of the legacy C#:
 *   - WorkBC.ElasticSearch.Indexing.Services.XmlParsingServiceFederal.ConvertToElasticJob
 *   - WorkBC.ElasticSearch.Indexing.Services.XmlParsingServiceBase (helpers)
 *   - WorkBC.ElasticSearch.Indexing.XmlParsingHelpers.FederalXmlLocations
 *   - WorkBC.ElasticSearch.Indexing.XmlParsingHelpers.XmlManualOverRides
 *
 * Returns an associative array keyed by ES field name, or null when the XML is
 * empty / malformed / "not available" (numFound != 1). Null-valued fields are
 * left in the array and stripped just before serialization (mirrors the C#
 * NullValueHandling.Ignore on the JsonSerializer).
 */
final class FederalDocumentMapper
{
    // skill_category ids that carry special meaning in the federal feed
    private const WORKPLACE_INFO_SKILL_CATEGORY_ID = 100000;
    private const HEALTH_BENEFITS_CATEGORY_ID = 102001;
    private const FINANCIAL_BENEFITS_CATEGORY_ID = 102002;
    private const LONG_TERM_BENEFITS_CATEGORY_ID = 102003;
    private const OTHER_BENEFITS_CATEGORY_ID = 102004;

    // WorkplaceTypeId (WorkBC.Shared.Constants.WorkplaceType)
    private const WORKPLACE_ON_SITE = 0;
    private const WORKPLACE_HYBRID = 100000;
    private const WORKPLACE_TRAVELLING = 100001;
    private const WORKPLACE_VIRTUAL = 15141;

    // NOC-387: senior-manager unit groups 00011..00015 consolidate to 00018.
    private const SPECIAL_NOC_2021_CODES = [11, 12, 13, 14, 15];
    private const SPECIAL_NOC_2021_REPLACEMENT = 18;

    private const VIRTUAL_JOB_BASED_IN_EN = 'Virtual job based in';
    private const VIRTUAL_JOB_BASED_IN_FR = 'Emploi virtuel basé à';

    // Salary calculation constants (XmlParsingServiceFederal.CalculateSalary)
    private const WEEKLY_WORK_HOURS = 40.0;
    private const MAX_HOURLY_RATE = 2500.0;
    private const MAX_WEEKLY_SALARY = 100000.0;
    private const MAX_YEARLY_SALARY = 5000000.0;

    private const WORKPLACE_TYPE_EN = [
        self::WORKPLACE_ON_SITE => 'On-site only',
        self::WORKPLACE_HYBRID => 'On-site or remote work',
        self::WORKPLACE_TRAVELLING => 'Work location varies',
        self::WORKPLACE_VIRTUAL => 'Virtual job',
    ];
    private const WORKPLACE_TYPE_FR = [
        self::WORKPLACE_ON_SITE => 'Présentiel seulement',
        self::WORKPLACE_HYBRID => 'Présentiel ou télétravail',
        self::WORKPLACE_TRAVELLING => 'Lieux de travail variés',
        self::WORKPLACE_VIRTUAL => 'Emploi virtuel',
    ];

    /**
     * Mirrors XmlManualOverRides.AlternateCityNames. Keys MUST be lowercase.
     */
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

    /** @var array<int,true>|null */
    private ?array $validNoc = null;
    /** @var array<int,true>|null */
    private ?array $validNoc2021 = null;
    /** @var array<int,array{title:?string,frenchTitle:?string}>|null */
    private ?array $nocGroups2021 = null;
    /** @var array<string,string>|null  lower-case city name → region label */
    private ?array $uniqueCities = null;
    /** @var list<array{LocationId:int,Label:string,City:string,FederalCityId:?int}>|null */
    private ?array $duplicateCities = null;
    /** @var array<string,true>|null */
    private ?array $duplicateCityNames = null;
    private ?float $minimumWage = null;

    public function __construct(
        private readonly PDO $db,
        private readonly GeocodingService $geocoding,
        private readonly Logger $log,
    ) {}

    /**
     * @return array<string,mixed>|null
     */
    public function convertToElasticJob(?string $federalXml, bool $isFrench = false): ?array
    {
        if ($federalXml === null || trim($federalXml) === '') {
            return null;
        }

        $doc = $this->loadXml($federalXml);
        if ($doc === null) {
            $this->log->warning('Malformed federal XML skipped');
            return null;
        }

        try {
            $xpath = new DOMXPath($doc);
            $numFound = (int) ($xpath->evaluate('string(/SolrResponse/Header/numFound)') ?: '0');
            if ($numFound !== 1) {
                return null;
            }

            $jobNode = $xpath->query('/SolrResponse/Documents/Document')->item(0);
            if (!$jobNode instanceof DOMElement) {
                return null;
            }

            return $this->buildDocument($jobNode, $xpath, $isFrench);
        } catch (\Throwable $e) {
            $this->log->error('Exception in convertToElasticJob(): ' . $e->getMessage());
            return null;
        }
    }

    /**
     * @return array<string,mixed>
     */
    private function buildDocument(DOMElement $job, DOMXPath $xpath, bool $isFrench): array
    {
        $noc = $this->resolveNoc2016($this->childText($job, 'noc2016'));
        $noc2021 = $this->resolveNoc2021($this->childText($job, 'noc2021'));

        $doc = [
            'JobId' => $this->childText($job, 'jobs_id'),
            // Title and EmployerName are the only two fields the C# parser trims.
            'Title' => $this->unicodeTrim($this->childText($job, 'title')),
            'DatePosted' => $this->isoDate($this->childText($job, 'date_posted')),
            'EmployerName' => $this->unicodeTrim($this->childText($job, 'employer_name_string')),
            'EmployerTypeId' => (int) ($this->childText($job, 'employer_type_id') ?: '0'),
            'Lang' => $this->childText($job, 'lang'),
            'WorkLangCd' => null, // set below
            'PostalCode' => $this->childText($job, 'postal_code'),
            'Salary' => null, // set below
            'WorkHours' => $this->parseDecimal($this->childText($job, 'work_hours')) ?? 0.0,
            'WageClass' => $this->childText($job, 'wage_class'),
            'HoursOfWork' => null,
            'PeriodOfEmployment' => null,
            'EmploymentTerms' => null,
            'IsAboriginal' => $this->boolText($job, 'job_aboriginal_flag'),
            'IsApprentice' => $this->boolText($job, 'job_apprentice_flag'),
            'IsStudent' => $this->boolText($job, 'job_student_flag'),
            'IsNewcomer' => $this->boolText($job, 'job_newcomer_flag'),
            'IsVeteran' => $this->boolText($job, 'job_veteran_flag'),
            'IsVismin' => $this->boolText($job, 'job_vismin_flag'),
            'IsYouth' => $this->boolText($job, 'job_youth_flag'),
            'IsMatureWorker' => $this->boolText($job, 'job_senior_flag'),
            'IsDisability' => $this->boolText($job, 'job_disability_flag'),
            'Location' => [],
            'SkillCategories' => [],
            'SalaryConditions' => ['Description' => []],
            'IsFederalJob' => true,
            'ExpireDate' => $this->isoDate($this->childText($job, 'display_until')),
            'LastUpdated' => $this->isoDate($this->childText($job, 'file_update_date')),
            'PositionsAvailable' => (int) ($this->childText($job, 'num_positions') ?: '0'),
            'Noc' => $noc,
            'Noc2021' => $noc2021,
            'IsVariousLocation' => $this->boolText($job, 'various_location_flag'),
            'ProgramName' => $this->childText($job, 'program_name'),
            'ProgramDescription' => $this->childText($job, 'program_desc'),
            'NaicsId' => (int) ($this->childText($job, 'naics_id') ?: '0'),
        ];

        // ── Hours of work ──────────────────────────────────────────
        $doc['HoursOfWork'] = ['Description' => [$this->mapWorkPeriod($this->childText($job, 'work_period_cd'))]];

        // ── Job language ───────────────────────────────────────────
        $doc['WorkLangCd'] = ['Description' => [$this->mapWorkLanguage($this->childText($job, 'work_lang_cd'))]];

        // ── Period of employment ───────────────────────────────────
        $doc['PeriodOfEmployment'] = ['Description' => [$this->mapWorkTerm($this->childText($job, 'work_term_cd'))]];

        // ── Employment terms ───────────────────────────────────────
        $terms = [];
        foreach ($xpath->query('employment_terms/string', $job) as $s) {
            $term = (string) $s->textContent;
            if (trim($term) !== '') {
                $terms[] = $this->capitalize($term);
            }
        }
        $doc['EmploymentTerms'] = ['Description' => $terms];

        // ── Skills / workplace type / benefits ─────────────────────
        $workplaceTypeId = self::WORKPLACE_ON_SITE;
        foreach ($xpath->query('skill_categories/skill_category', $job) as $skillCategory) {
            if (!$skillCategory instanceof DOMElement) {
                continue;
            }
            $id = (int) $skillCategory->getAttribute('id');
            $nameNode = $this->firstChildElement($skillCategory);
            $name = $nameNode !== null ? (string) $nameNode->textContent : '';

            if ($id === self::WORKPLACE_INFO_SKILL_CATEGORY_ID) {
                $optionName = $xpath->query('options/option_name', $skillCategory)->item(0);
                if ($optionName instanceof DOMElement) {
                    $candidate = (int) $optionName->getAttribute('id');
                    if (in_array($candidate, [self::WORKPLACE_HYBRID, self::WORKPLACE_TRAVELLING, self::WORKPLACE_VIRTUAL], true)) {
                        $workplaceTypeId = $candidate;
                    }
                }
                continue;
            }

            if ($id === self::FINANCIAL_BENEFITS_CATEGORY_ID || $id === self::LONG_TERM_BENEFITS_CATEGORY_ID) {
                foreach ($xpath->query('options/option_name', $skillCategory) as $option) {
                    $benefit = $this->capitalize((string) $option->textContent);
                    if (str_contains($benefit, 'rrsp')) {
                        $benefit = 'RRSP benefits';
                    }
                    if (str_contains($benefit, 'resp')) {
                        $benefit = 'RESP benefits';
                    }
                    if (str_contains($benefit, 'Life insurance')) {
                        $benefit = 'Life insurance benefits';
                    }
                    if (str_contains($benefit, 'Pension plan')) {
                        $benefit = 'Pension plan benefits';
                    }
                    $doc['SalaryConditions']['Description'][] = $benefit;
                }
                continue;
            }

            if ($id === self::HEALTH_BENEFITS_CATEGORY_ID) {
                foreach ($xpath->query('options/option_name', $skillCategory) as $option) {
                    $doc['SalaryConditions']['Description'][] = $this->capitalize((string) $option->textContent);
                }
                continue;
            }

            if ($id === self::OTHER_BENEFITS_CATEGORY_ID) {
                $doc['SalaryConditions']['Description'][] = 'Other benefits';
                continue;
            }

            // Education categories are forced to id 195 (both EN and FR labels).
            $lowerName = mb_strtolower($name);
            if ($lowerName === 'education' || $lowerName === 'études') {
                $id = 195;
            }

            $skills = [];
            foreach ($xpath->query('options/option_name', $skillCategory) as $option) {
                $skills[] = (string) $option->textContent;
            }

            $doc['SkillCategories'][] = [
                'Category' => ['Id' => $id, 'Name' => $name],
                'SkillCount' => count($skills),
                'Skills' => $skills,
            ];
        }

        $doc['WorkplaceType'] = [
            'Id' => $workplaceTypeId,
            'Description' => $isFrench
                ? self::WORKPLACE_TYPE_FR[$workplaceTypeId]
                : self::WORKPLACE_TYPE_EN[$workplaceTypeId],
        ];

        // ── Job location ───────────────────────────────────────────
        if ($workplaceTypeId === self::WORKPLACE_VIRTUAL) {
            $doc['City'] = $this->resolveVirtualCity($job, $isFrench);
            $doc['Region'] = [];
            $doc['Location'] = [];
            // Province / LocationGeo are left unset for virtual jobs (C# parity).
        } else {
            [$locations, $locationGeos, $province, $cityNames, $cityIds] = $this->resolveLocations($job, $xpath);
            $doc['Location'] = $locations;
            if (!empty($locationGeos)) {
                $doc['LocationGeo'] = array_values(array_unique($locationGeos));
            }
            $doc['Province'] = $province;

            $cities = [];
            $regions = [];
            $count = count($cityNames);
            for ($i = 0; $i < $count; $i++) {
                $cityName = $this->cleanUpCityName($cityNames[$i]);
                $cities[] = $this->getJobCity($cityName, $cityIds[$i]);
                $regions[] = $this->getJobRegion($cityName, $cityIds[$i]);
            }
            $doc['Region'] = $this->distinct($regions);
            $doc['City'] = $this->distinct($cities);
        }

        // ── Salary ─────────────────────────────────────────────────
        $hourly = $this->parseDecimal($this->childText($job, 'salary_hourly'));
        $weekly = $this->parseDecimal($this->childText($job, 'salary_weekly'));
        $yearly = $this->parseDecimal($this->childText($job, 'salary_yearly'));
        // null when the <salary><string> node is absent (mirrors SelectSingleNode),
        // so the summary branches below distinguish "missing" from "empty".
        $salaryString = $this->nodeTextOrNull($xpath, 'salary/string', $job);
        $doc['Salary'] = $this->calculateSalary($hourly, $weekly, $yearly, $salaryString);

        // ── Salary conditions (string list) ────────────────────────
        foreach ($xpath->query('salary_conditions/string', $job) as $condition) {
            $doc['SalaryConditions']['Description'][] = $this->capitalize((string) $condition->textContent);
        }

        // ── Apply: email ───────────────────────────────────────────
        $doc['ApplyEmailAddress'] = $this->nodeText($xpath, 'app_methods/app_email', $job);

        // ── Apply: phone ───────────────────────────────────────────
        $doc['ApplyPhoneNumber'] = $this->nodeText($xpath, 'app_methods/app_phone/app_phone_number', $job);
        $ext = $this->nodeTextOrNull($xpath, 'app_methods/app_phone/app_phone_ext', $job);
        if ($ext !== null) {
            $doc['ApplyPhoneNumberExt'] = $ext;
        }
        $startBus = $this->nodeTextOrNull($xpath, 'app_methods/app_phone/app_phone_start_bus_hours', $job);
        $endBus = $this->nodeTextOrNull($xpath, 'app_methods/app_phone/app_phone_end_bus_hours', $job);
        if ($startBus !== null && $endBus !== null && $startBus !== '') {
            $doc['ApplyPhoneNumberTimeFrom'] = $this->convertToTime((float) $startBus / 60);
            $doc['ApplyPhoneNumberTimeTo'] = $this->convertToTime((float) $endBus / 60);
        }
        $fax = $this->nodeTextOrNull($xpath, 'app_methods/app_fax', $job);
        if ($fax !== null) {
            $doc['ApplyFaxNumber'] = $fax;
        }

        // ── Apply: website ─────────────────────────────────────────
        $doc['ApplyWebsite'] = $this->nodeText($xpath, 'app_methods/app_online', $job);

        // ── Education level ────────────────────────────────────────
        $doc['EduLevel'] = $this->mapEducationLevel((int) ($this->childText($job, 'los_id') ?: '0'));

        // ── NOC group (2021) ───────────────────────────────────────
        $doc['NocGroup'] = $this->getNocGroup2021($noc2021, $isFrench);
        $doc['NocJobTitle'] = $doc['Title'];

        // ── Salary description / summary ───────────────────────────
        $hoursStr = $this->nodeTextOrNull($xpath, 'hours', $job);
        if ($salaryString !== null && $hoursStr !== null) {
            $doc['SalaryDescription'] = $salaryString . ($isFrench ? ' pour ' : ' for ') . $hoursStr;
        }

        if ($salaryString !== null) {
            $summary = $salaryString;
            if (str_contains($summary, 'annually')) {
                $summary = str_replace('.00', '', $summary);
            }
            $doc['SalarySummary'] = $summary;
        } elseif (($doc['Salary'] ?? 0.0) > 0.0) {
            if (($yearly ?? 0) > 0) {
                $doc['SalarySummary'] = '$' . number_format($yearly) . ' annually';
            } elseif (($hourly ?? 0) > 0) {
                $doc['SalarySummary'] = '$' . number_format($hourly, 2) . ' hourly';
            } elseif (($weekly ?? 0) > 0) {
                $doc['SalarySummary'] = '$' . number_format($weekly, 2) . ' weekly';
            } else {
                $doc['SalarySummary'] = 'N/A';
            }
        }

        // Invalid-salary fallback: a job with a salary string but no computed
        // Salary still shows the raw string (provincial legislation prohibits
        // listing jobs without a salary). Mirrors the C# null-salary branch.
        if ($doc['Salary'] === null
            && ($this->childText($job, 'salary_yearly') !== '' || $this->childText($job, 'salary_hourly') !== '')
            && $salaryString !== null
        ) {
            $doc['SalarySummary'] = $salaryString;
            $doc['SalaryDescription'] = $salaryString;
        }

        // ── Apply: person ──────────────────────────────────────────
        $doc['ApplyPersonRoom'] = $this->nodeText($xpath, 'app_methods/app_person/app_person_room', $job);
        $doc['ApplyPersonStreet'] = $this->nodeText($xpath, 'app_methods/app_person/app_person_street', $job);
        $doc['ApplyPersonCity'] = $this->nodeText($xpath, 'app_methods/app_person/app_person_city', $job);
        $doc['ApplyPersonPostalCode'] = $this->nodeText($xpath, 'app_methods/app_person/app_person_pstlcd', $job);
        $doc['ApplyPersonProvince'] = $this->nodeText($xpath, 'app_methods/app_person/app_person_province', $job);
        $startPerson = $this->nodeTextOrNull($xpath, 'app_methods/app_person/app_person_start_bus_hours', $job);
        $endPerson = $this->nodeTextOrNull($xpath, 'app_methods/app_person/app_person_end_bus_hours', $job);
        if ($startPerson !== null && $startPerson !== '') {
            $doc['ApplyPersonTimeFrom'] = $this->convertToTime((float) $startPerson / 60);
            $doc['ApplyPersonTimeTo'] = $this->convertToTime((float) ($endPerson ?? '0') / 60);
        }

        // ── Apply: mail ────────────────────────────────────────────
        $doc['ApplyMailRoom'] = $this->nodeText($xpath, 'app_methods/app_mail/app_mail_room', $job);
        $doc['ApplyMailStreet'] = $this->nodeText($xpath, 'app_methods/app_mail/app_mail_street', $job);
        $doc['ApplyMailCity'] = $this->nodeText($xpath, 'app_methods/app_mail/app_mail_city', $job);
        $doc['ApplyMailPostalCode'] = $this->nodeText($xpath, 'app_methods/app_mail/app_mail_pstlcd', $job);
        $doc['ApplyMailProvince'] = $this->nodeText($xpath, 'app_methods/app_mail/app_mail_province', $job);

        // ── Start date (+8h, BC offset; date-only field) ───────────
        // C#: Convert.ToDateTime(start_date).AddHours(8). The bare date parses
        // as UTC midnight, so a 2026-03-16 start becomes 2026-03-16T08:00:00Z.
        $startDate = $this->childText($job, 'start_date');
        if ($startDate !== '') {
            $parsed = $this->parseDateUtc($startDate);
            if ($parsed !== null) {
                $doc['StartDate'] = $this->formatUtc($parsed->add(new \DateInterval('PT8H')));
            }
        }

        // ── Salary sort ────────────────────────────────────────────
        $doc['SalarySort'] = $this->salarySort($doc['Salary'], $doc['SalarySummary'] ?? null);

        return $doc;
    }

    // ── Locations ──────────────────────────────────────────────────

    /**
     * Ports FederalXmlLocations: keep BC-only cities, dedupe by raw name,
     * sort alphabetically, build Location[]/LocationGeo[] (BC-bbox filtered).
     *
     * @return array{0:list<array{Lat:string,Lon:string}>,1:list<string>,2:string,3:list<string>,4:list<int>}
     */
    private function resolveLocations(DOMElement $job, DOMXPath $xpath): array
    {
        $cityIdNodes = $xpath->query('city_id/string', $job);
        $cityNameNodes = $xpath->query('city_name/string', $job);
        $provinceNodes = $xpath->query('province_cd/string', $job);
        $geoNodes = $xpath->query('latlng/string', $job);

        if ($cityNameNodes === false || $cityNameNodes->length === 0) {
            return [[], [], '', [''], [0]];
        }

        $count = $cityIdNodes->length;
        if ($cityNameNodes->length < $count) {
            $count = $cityNameNodes->length;
        }
        if ($geoNodes->length < $count) {
            $count = $geoNodes->length;
        }

        // City record: name, id, optional Location. Dedupe by raw name, BC only.
        $cities = [];
        $seen = [];
        for ($i = 0; $i < $count; $i++) {
            $province = $provinceNodes->length <= $i
                ? (string) $provinceNodes->item(0)->textContent
                : (string) $provinceNodes->item($i)->textContent;

            $cityName = (string) $cityNameNodes->item($i)->textContent;
            $cityIdRaw = (string) $cityIdNodes->item($i)->textContent;
            $geo = (string) $geoNodes->item($i)->textContent;

            if ($province === 'BC' && !isset($seen[$cityName])) {
                $seen[$cityName] = true;
                $coords = explode(',', $geo);
                $cities[] = [
                    'name' => $cityName,
                    'id' => (int) $cityIdRaw,
                    'location' => isset($coords[0], $coords[1])
                        ? $this->locationOrNull($coords[0], $coords[1])
                        : null,
                ];
            }
        }

        if (empty($cities)) {
            return [[], [], '', [''], [0]];
        }

        usort($cities, static fn(array $a, array $b): int => strcmp($a['name'], $b['name']));

        $cityNames = array_map(static fn(array $c): string => $c['name'], $cities);
        $cityIds = array_map(static fn(array $c): int => $c['id'], $cities);

        $locations = [];
        $geos = [];
        foreach ($cities as $c) {
            if ($c['location'] !== null) {
                $locations[] = $c['location'];
                $geos[] = $c['location']['Lat'] . ',' . $c['location']['Lon'];
            }
        }

        return [$locations, $geos, 'BC', $cityNames, $cityIds];
    }

    /**
     * Mirrors Location.LocationOrNull: returns the point only when it parses
     * and falls inside a rough BC bounding box.
     *
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

    /**
     * @return list<string>
     */
    private function resolveVirtualCity(DOMElement $job, bool $isFrench): array
    {
        $postalCode = $this->childText($job, 'employer_postal_code');
        $cache = $this->geocoding->getLocation($postalCode . ', CANADA');

        if ($cache === null && strlen($postalCode) === 6) {
            $cache = $this->geocoding->getLocation(substr($postalCode, 0, 3) . ', CANADA');
        }

        $prefixEn = self::VIRTUAL_JOB_BASED_IN_EN;
        $prefixFr = self::VIRTUAL_JOB_BASED_IN_FR;

        if ($cache === null) {
            return $isFrench
                ? ["{$prefixFr} {$postalCode}"]
                : ["{$prefixEn} {$postalCode}"];
        }

        $city = $cache['City'] ?? null;
        $frenchCity = $cache['FrenchCity'] ?? null;
        $province = $cache['Province'] ?? null;

        if ($isFrench) {
            return $frenchCity !== null
                ? ["{$prefixFr} {$frenchCity}, {$province}"]
                : ["{$prefixFr} {$province}"];
        }

        // English branch. The C# code has a special path when City and Province
        // are both null but a postal code is present (postal-prefix → region).
        if ($city === null && $province === null && $postalCode !== '') {
            $byPrefix = $this->virtualEnglishByPostalPrefix($postalCode, $isFrench);
            if ($byPrefix !== null) {
                return [$byPrefix];
            }
        }

        return $city !== null
            ? ["{$prefixEn} {$city}, {$province}"]
            : ["{$prefixEn} {$province}"];
    }

    /**
     * Ports the postal-prefix → province/region switch from the C# English
     * virtual branch (used only when the cache row has null City AND Province).
     */
    private function virtualEnglishByPostalPrefix(string $postalCode, bool $isFrench): ?string
    {
        $en = self::VIRTUAL_JOB_BASED_IN_EN;
        $fr = self::VIRTUAL_JOB_BASED_IN_FR;
        $first = strtoupper(substr($postalCode, 0, 1));

        $map = [
            'A' => ['Newfoundland and Labrador', 'Terre-Neuve-et-Labrador'],
            'B' => ['Nova Scotia', 'Nouvelle-Écosse'],
            'C' => ['Prince Edward Island', 'Île-du-Prince-Édouard'],
            'E' => ['New Brunswick', 'Nouveau-Brunswick'],
            'G' => ['Eastern Quebec', 'Est du Québec'],
            'H' => ['Metropolitan Montréal', 'Grand Montréal'],
            'J' => ['Western Quebec', 'Ouest du Québec'],
            'K' => ['Eastern Ontario', "Est de l'Ontario"],
            'L' => ['Central Ontario', "Centre de l'Ontario"],
            'M' => ['Metropolitan Toronto', 'Grand Toronto'],
            'N' => ['Southwestern Ontario', "Sud-ouest de l'Ontario"],
            'P' => ['Northern Ontario', "Nord de l'Ontario"],
            'R' => ['Manitoba', 'Manitoba'],
            'S' => ['Saskatchewan', 'Saskatchewan'],
            'T' => ['Alberta', 'Alberta'],
            'V' => ['British Columbia', 'Colombie-Britannique'],
            'X' => ['Northwest Territories and Nunavut', 'Territoires du Nord-Ouest et Nunavut'],
            'Y' => ['Yukon', 'Yukon'],
        ];

        if (!isset($map[$first])) {
            return $isFrench ? 'Emploi virtuel, base inconnue' : 'Virtual job, base unknown';
        }

        [$enName, $frName] = $map[$first];
        return $isFrench ? "{$fr} {$frName}" : "{$en} {$enName}";
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

    private function getJobRegion(string $cityName, int $cityId): string
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

    private function resolveNoc2016(string $raw): ?int
    {
        if ($raw === '') {
            return null;
        }
        $candidate = (int) $raw;
        if ($candidate === 0 || !$this->isValidNoc($candidate)) {
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
        if ($candidate === 0) {
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

    private function getNocGroup2021(?int $nocId, bool $isFrench): string
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

    // ── Salary ─────────────────────────────────────────────────────

    private function calculateSalary(?float $hourly, ?float $weekly, ?float $yearly, ?string $salaryString): ?float
    {
        $hourly ??= 0.0;
        $weekly ??= 0.0;
        $yearly ??= 0.0;
        $salaryString ??= '';
        $minWage = $this->getMinimumWage();

        // The C# does this arithmetic in `decimal`, which is exact; PHP floats
        // pick up binary-representation noise (e.g. 57595.200000000004). Hourly
        // and weekly rates in the feed carry at most 2 decimals and the
        // multipliers are integers, so the true result has at most 2 decimals —
        // rounding there reproduces the decimal output without changing values.
        if (str_contains(strtolower($salaryString), 'hour')) {
            if ($hourly >= 0.9 * $minWage && $hourly < self::MAX_HOURLY_RATE) {
                return round($hourly * self::WEEKLY_WORK_HOURS * 52, 2);
            }
        }
        if ($yearly >= $minWage * self::WEEKLY_WORK_HOURS * 52 && $yearly < self::MAX_YEARLY_SALARY) {
            return round($yearly, 2);
        }
        if ($hourly >= $minWage && $hourly < self::MAX_HOURLY_RATE) {
            return round($hourly * self::WEEKLY_WORK_HOURS * 52, 2);
        }
        if ($weekly >= $minWage * self::WEEKLY_WORK_HOURS && $weekly < self::MAX_WEEKLY_SALARY) {
            return round($weekly * 52, 2);
        }
        return null;
    }

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

    // ── Code → label maps ──────────────────────────────────────────

    private function mapWorkPeriod(string $code): string
    {
        return match (strtolower($code)) {
            'l' => 'Part-time leading to full-time',
            'f' => 'Full-time',
            'p' => 'Part-time',
            default => $code,
        };
    }

    private function mapWorkLanguage(string $code): string
    {
        return match (strtolower($code)) {
            'e' => 'English',
            'f' => 'French',
            'u' => 'English or French',
            'b' => 'Bilingual',
            'o' => 'Other',
            default => $code,
        };
    }

    private function mapWorkTerm(string $code): string
    {
        return match (strtolower($code)) {
            't' => 'Temporary',
            'p' => 'Permanent',
            'c' => 'Casual',
            's' => 'Seasonal',
            default => '',
        };
    }

    private function mapEducationLevel(int $levelOfStudy): string
    {
        return match (true) {
            $levelOfStudy === 2 => 'Secondary school or job-specific training',
            $levelOfStudy >= 3 && $levelOfStudy <= 7 => 'College or apprenticeship',
            $levelOfStudy >= 8 && $levelOfStudy <= 11 => 'University',
            default => 'No education',
        };
    }

    /**
     * Faithful port of XmlParsingServiceBase.ConvertToTime:
     *   fullHour = Abs(time);  minutes = time - Truncate(time);
     *   new TimeSpan(Convert.ToInt32(fullHour), Convert.ToInt32(minutes), 0)
     *   formatted "h:mm tt".
     * Convert.ToInt32 uses banker's rounding (half-to-even), so we replicate
     * that on both the hour and the fractional-minute component.
     */
    private function convertToTime(float $time): string
    {
        $abs = abs($time);
        $hour = $this->bankersRound($abs);
        $minutes = $this->bankersRound($time - (float) (int) $time);

        // TimeSpan(hour, minutes, 0) → carry minutes into hours, wrap at 24h.
        $totalMinutes = $hour * 60 + $minutes;
        $h = intdiv($totalMinutes, 60) % 24;
        $m = $totalMinutes % 60;

        $dt = \DateTime::createFromFormat('!H:i', sprintf('%02d:%02d', $h, $m));
        return $dt !== false ? $dt->format('g:i A') : sprintf('%d:%02d', $h, $m);
    }

    /** Round half to even, matching .NET Convert.ToInt32(decimal/double). */
    private function bankersRound(float $value): int
    {
        return (int) round($value, 0, PHP_ROUND_HALF_EVEN);
    }

    // ── DB lookups (lazy, cached) ──────────────────────────────────

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
        $this->loadNocGroups2021();
        return isset($this->validNoc2021[$id]);
    }

    private function loadNocGroups2021(): void
    {
        if ($this->nocGroups2021 !== null) {
            return;
        }
        $rows = $this->db->query('SELECT "Id","Title","FrenchTitle" FROM "NocCodes2021"')?->fetchAll() ?: [];
        $this->nocGroups2021 = [];
        $this->validNoc2021 = [];
        foreach ($rows as $row) {
            $id = (int) $row['Id'];
            $this->nocGroups2021[$id] = [
                'title' => $row['Title'] !== null ? (string) $row['Title'] : null,
                'frenchTitle' => $row['FrenchTitle'] !== null ? (string) $row['FrenchTitle'] : null,
            ];
            $this->validNoc2021[$id] = true;
        }
        $this->log->info(count($this->validNoc2021) . ' NOC 2021 codes loaded');
    }

    private function loadUniqueCities(): void
    {
        if ($this->uniqueCities !== null) {
            return;
        }
        // Lower-case city → region name. Mirrors CityIndexingService
        // .GetUniqueCitiesForIndexing(): non-duplicate, visible locations with a
        // RegionId, joined to Regions where (Id = 0 OR NOT IsHidden).
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
        $this->log->info(count($this->duplicateCities) . ' duplicate cities loaded for federal disambiguation');
    }

    private function getMinimumWage(): float
    {
        if ($this->minimumWage !== null) {
            return $this->minimumWage;
        }

        // IMPORTANT — preserve a bug in XmlParsingServiceFederal.CalculateSalary:
        //
        //   if (SystemSettings.All(c => c.Name == "shared.settings.minimumWage"))
        //       minimumWage = Convert.ToDecimal(... .Value);
        //
        // `.All(...)` is true only when EVERY SystemSettings row is the minimum-
        // wage setting. In any real database there are many other settings, so
        // the condition is false and `minimumWage` stays 0 — i.e. the .NET
        // indexer never actually filters out below-minimum salaries. We replicate
        // that exactly (count of the minimum-wage name == total row count) so the
        // computed Salary / SalarySort match the .NET output byte-for-byte. The
        // intended behaviour (Any instead of All) is a separate decision; do not
        // "fix" it here without changing the .NET side too.
        $this->minimumWage = 0.0;
        try {
            $total = (int) $this->db->query('SELECT COUNT(*) FROM "SystemSettings"')->fetchColumn();
            $stmt = $this->db->prepare('SELECT "Value" FROM "SystemSettings" WHERE "Name" = ? LIMIT 1');
            $stmt->execute(['shared.settings.minimumWage']);
            $value = $stmt->fetchColumn();

            $allRowsAreMinimumWage = $total > 0
                && (int) $this->db->query(
                    'SELECT COUNT(*) FROM "SystemSettings" WHERE "Name" = \'shared.settings.minimumWage\''
                )->fetchColumn() === $total;

            if ($allRowsAreMinimumWage && $value !== false && $value !== null && (string) $value !== '') {
                $this->minimumWage = (float) $value;
            }
        } catch (\Throwable) {
            // older schemas may lack the table — leave minimumWage at 0
        }

        return $this->minimumWage;
    }

    // ── XML helpers ────────────────────────────────────────────────

    private function loadXml(string $body): ?DOMDocument
    {
        $previous = libxml_use_internal_errors(true);
        $doc = new DOMDocument();
        $doc->preserveWhiteSpace = false;
        $loaded = $doc->loadXML($body, LIBXML_NONET | LIBXML_NOERROR | LIBXML_NOWARNING);
        libxml_clear_errors();
        libxml_use_internal_errors($previous);
        return $loaded ? $doc : null;
    }

    /**
     * First direct-child element's raw text (mirrors C# xmlNode["x"].InnerText).
     * NOT trimmed — the C# parser only trims Title and EmployerName explicitly;
     * every other field keeps the feed's surrounding whitespace verbatim.
     */
    private function childText(DOMElement $parent, string $name): string
    {
        foreach ($parent->childNodes as $child) {
            if ($child instanceof DOMElement && $child->localName === $name) {
                return (string) $child->textContent;
            }
        }
        return '';
    }

    private function boolText(DOMElement $parent, string $name): bool
    {
        $value = $this->childText($parent, $name);
        return strtolower($value) === 'true' || $value === '1';
    }

    /**
     * XPath single-node raw text relative to $context, or null if absent
     * (mirrors SelectSingleNode(...)?.InnerText). NOT trimmed — see childText().
     */
    private function nodeTextOrNull(DOMXPath $xpath, string $expr, DOMNode $context): ?string
    {
        $node = $xpath->query($expr, $context)->item(0);
        return $node === null ? null : (string) $node->textContent;
    }

    /** Like nodeTextOrNull but returns '' for an absent node (for fields the C# defaults to string.Empty). */
    private function nodeText(DOMXPath $xpath, string $expr, DOMNode $context): string
    {
        return $this->nodeTextOrNull($xpath, $expr, $context) ?? '';
    }

    private function firstChildElement(DOMElement $parent): ?DOMElement
    {
        foreach ($parent->childNodes as $child) {
            if ($child instanceof DOMElement) {
                return $child;
            }
        }
        return null;
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

    private function parseDecimal(string $raw): ?float
    {
        if ($raw === '') {
            return null;
        }
        $clean = str_replace([',', ' '], '', $raw);
        return is_numeric($clean) ? (float) $clean : null;
    }

    private function isoDate(string $raw): ?string
    {
        $dt = $this->parseDateUtc($raw);
        return $dt === null ? null : $this->formatUtc($dt);
    }

    /**
     * Parses an XML datetime, treating a naive (offset-less) value as UTC.
     *
     * The .NET indexer serializes its dates as UTC with a "+00:00" offset and
     * its Convert.ToDateTime treats the federal feed's bare datetimes as UTC
     * (e.g. a bare "2026-03-16" start date becomes 00:00:00Z, not midnight in
     * the container's local zone). We must do the same so the serialized
     * strings match — using the process default TZ here would shift every date.
     */
    private function parseDateUtc(string $raw): ?\DateTimeImmutable
    {
        if ($raw === '') {
            return null;
        }
        try {
            return new \DateTimeImmutable($raw, new \DateTimeZone('UTC'));
        } catch (\Throwable) {
            return null;
        }
    }

    private function formatUtc(\DateTimeImmutable $dt): string
    {
        // "2025-11-27T11:15:00+00:00" — matches Newtonsoft's UTC ISO 8601 output.
        return $dt->setTimezone(new \DateTimeZone('UTC'))->format('Y-m-d\TH:i:sP');
    }

    /**
     * @param list<string> $values
     * @return list<string>
     */
    private function distinct(array $values): array
    {
        $out = [];
        $seen = [];
        foreach ($values as $v) {
            if (!isset($seen[$v])) {
                $seen[$v] = true;
                $out[] = $v;
            }
        }
        return $out;
    }

    private function capitalize(string $s): string
    {
        $len = mb_strlen($s);
        if ($len >= 2) {
            return mb_strtoupper(mb_substr($s, 0, 1)) . mb_strtolower(mb_substr($s, 1));
        }
        if ($len === 1) {
            return mb_strtoupper($s);
        }
        return '';
    }
}
