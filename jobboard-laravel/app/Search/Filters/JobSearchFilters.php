<?php

namespace App\Search\Filters;

use Illuminate\Contracts\Database\Eloquent\Castable;

/**
 * Shared search-criteria value object — docs/contracts.md §1.
 *
 * ONE object used by three consumers: the search UI (Livewire), saved alerts
 * (stored as JSON in JobAlerts.JobSearchFilters), and the alert-email sender
 * (re-runs stored filters). Field names/shape MUST match the existing C#
 * serialization exactly (PascalCase) so old JobAlerts rows keep deserializing.
 *
 * Versioning (contracts §1 "Versioning"): JobAlerts.JobSearchFiltersVersion is
 * 0 (DB default) or 1 (old C# model) — the SAME shape today. The deserializer
 * accepts both. Bump only if the shape changes, and keep a deserializer per
 * prior version forever.
 *
 * Strict input (contracts §1 "Serialization rules"): unknown fields are
 * rejected (InvalidFilterException → HTTP 400), mirroring the current
 * MissingMemberHandling.Error. Booleans default false except
 * SearchIsPostingsInEnglish which defaults true.
 */
final class JobSearchFilters implements Castable
{
    /** Filter shape versions this class can deserialize (all the current shape). */
    public const SUPPORTED_VERSIONS = [0, 1];

    /** The version new filters serialize as. */
    public const CURRENT_VERSION = 0;

    // Paging / sort
    public int $Page = 1;
    public int $PageSize = 20;
    public int $SortOrder = 1;

    // Keyword
    public ?string $Keyword = null;
    public string $SearchInField = 'all';

    // Date
    public string $SearchDateSelection = '0';
    public ?DateField $StartDate = null;
    public ?DateField $EndDate = null;

    // Job type — hours
    public bool $SearchJobTypeFullTime = false;
    public bool $SearchJobTypePartTime = false;
    public bool $SearchJobTypeLeadingToFullTime = false;

    // Job type — period
    public bool $SearchJobTypePermanent = false;
    public bool $SearchJobTypeTemporary = false;
    public bool $SearchJobTypeCasual = false;
    public bool $SearchJobTypeSeasonal = false;

    // Job type — terms
    public bool $SearchJobTypeDay = false;
    public bool $SearchJobTypeEarly = false;
    public bool $SearchJobTypeEvening = false;
    public bool $SearchJobTypeFlexible = false;
    public bool $SearchJobTypeMorning = false;
    public bool $SearchJobTypeNight = false;
    public bool $SearchJobTypeOnCall = false;
    public bool $SearchJobTypeOvertime = false;
    public bool $SearchJobTypeShift = false;
    public bool $SearchJobTypeTbd = false;
    public bool $SearchJobTypeWeekend = false;

    // Workplace type
    public bool $SearchJobTypeOnSite = false;
    public bool $SearchJobTypeHybrid = false;
    public bool $SearchJobTypeTravelling = false;
    public bool $SearchJobTypeVirtual = false;

    // Education
    /** @var string[] */
    public array $SearchJobEducationLevel = [];

    // Salary
    public int $SalaryType = 0;
    public bool $SalaryBracket1 = false;
    public bool $SalaryBracket2 = false;
    public bool $SalaryBracket3 = false;
    public bool $SalaryBracket4 = false;
    public bool $SalaryBracket5 = false;
    public bool $SalaryBracket6 = false;
    public bool $SearchSalaryUnknown = false;
    public ?string $SalaryMin = null;
    public ?string $SalaryMax = null;
    /** @var string[] */
    public array $SearchSalaryConditions = [];

    // Location
    public int $SearchLocationDistance = 0;
    /** @var LocationField[] */
    public array $SearchLocations = [];

    // Industry (NAICS ids)
    /** @var int[] */
    public array $SearchIndustry = [];

    // Equity groups ("More" filters)
    public bool $SearchIsApprentice = false;
    public bool $SearchIsVeterans = false;
    public bool $SearchIsIndigenous = false;
    public bool $SearchIsMatureWorkers = false;
    public bool $SearchIsNewcomers = false;
    public bool $SearchIsPeopleWithDisabilities = false;
    public bool $SearchIsStudents = false;
    public bool $SearchIsVisibleMinority = false;
    public bool $SearchIsYouth = false;

    // Posting language
    public bool $SearchIsPostingsInEnglish = true;
    public bool $SearchIsPostingsInEnglishAndFrench = false;

    // NOC / source
    public ?string $SearchNocField = null;
    public ?string $NocCode = null;
    public string $SearchJobSource = '0';
    public bool $SearchExcludePlacementAgencyJobs = false;
    public bool $SearchNjbJobsFirst = false;

    /** Integer-typed scalar fields. */
    private const INT_FIELDS = [
        'Page', 'PageSize', 'SortOrder', 'SalaryType', 'SearchLocationDistance',
    ];

    /** String-typed scalar fields. */
    private const STRING_FIELDS = [
        'Keyword', 'SearchInField', 'SearchDateSelection', 'SalaryMin', 'SalaryMax',
        'SearchNocField', 'NocCode', 'SearchJobSource',
    ];

    /** Boolean scalar fields. */
    private const BOOL_FIELDS = [
        'SearchJobTypeFullTime', 'SearchJobTypePartTime', 'SearchJobTypeLeadingToFullTime',
        'SearchJobTypePermanent', 'SearchJobTypeTemporary', 'SearchJobTypeCasual', 'SearchJobTypeSeasonal',
        'SearchJobTypeDay', 'SearchJobTypeEarly', 'SearchJobTypeEvening', 'SearchJobTypeFlexible',
        'SearchJobTypeMorning', 'SearchJobTypeNight', 'SearchJobTypeOnCall', 'SearchJobTypeOvertime',
        'SearchJobTypeShift', 'SearchJobTypeTbd', 'SearchJobTypeWeekend',
        'SearchJobTypeOnSite', 'SearchJobTypeHybrid', 'SearchJobTypeTravelling', 'SearchJobTypeVirtual',
        'SalaryBracket1', 'SalaryBracket2', 'SalaryBracket3', 'SalaryBracket4', 'SalaryBracket5', 'SalaryBracket6',
        'SearchSalaryUnknown',
        'SearchIsApprentice', 'SearchIsVeterans', 'SearchIsIndigenous', 'SearchIsMatureWorkers',
        'SearchIsNewcomers', 'SearchIsPeopleWithDisabilities', 'SearchIsStudents', 'SearchIsVisibleMinority',
        'SearchIsYouth', 'SearchIsPostingsInEnglish', 'SearchIsPostingsInEnglishAndFrench',
        'SearchExcludePlacementAgencyJobs', 'SearchNjbJobsFirst',
    ];

    /** @var string[] string-array fields */
    private const STRING_ARRAY_FIELDS = ['SearchJobEducationLevel', 'SearchSalaryConditions'];

    /** DateField sub-object fields. */
    private const DATE_FIELDS = ['StartDate', 'EndDate'];

    /**
     * Deserialize a raw payload into the value object.
     *
     * @param  array<string, mixed>  $data
     *
     * @throws InvalidFilterException on an unsupported version or an unknown field
     */
    public static function fromArray(array $data, int $version = self::CURRENT_VERSION): self
    {
        if (! in_array($version, self::SUPPORTED_VERSIONS, true)) {
            throw new InvalidFilterException("Unsupported JobSearchFilters version: {$version}");
        }

        $known = self::knownFields();
        foreach (array_keys($data) as $key) {
            if (! in_array($key, $known, true)) {
                throw new InvalidFilterException("Unknown JobSearchFilters field: {$key}");
            }
        }

        $filters = new self;

        foreach (self::INT_FIELDS as $field) {
            if (array_key_exists($field, $data)) {
                $filters->{$field} = (int) $data[$field];
            }
        }

        foreach (self::STRING_FIELDS as $field) {
            if (array_key_exists($field, $data)) {
                $filters->{$field} = $data[$field] === null ? null : (string) $data[$field];
            }
        }

        foreach (self::BOOL_FIELDS as $field) {
            if (array_key_exists($field, $data)) {
                $filters->{$field} = (bool) $data[$field];
            }
        }

        foreach (self::STRING_ARRAY_FIELDS as $field) {
            if (array_key_exists($field, $data) && is_array($data[$field])) {
                $filters->{$field} = array_values(array_map('strval', $data[$field]));
            }
        }

        if (array_key_exists('SearchIndustry', $data) && is_array($data['SearchIndustry'])) {
            $filters->SearchIndustry = array_values(array_map('intval', $data['SearchIndustry']));
        }

        foreach (self::DATE_FIELDS as $field) {
            if (isset($data[$field]) && is_array($data[$field])) {
                $filters->{$field} = DateField::fromArray($data[$field]);
            }
        }

        if (array_key_exists('SearchLocations', $data) && is_array($data['SearchLocations'])) {
            $filters->SearchLocations = array_values(array_map(
                static fn (array $loc): LocationField => LocationField::fromArray($loc),
                $data['SearchLocations'],
            ));
        }

        return $filters;
    }

    /**
     * Serialize back to the exact contract shape (PascalCase keys).
     *
     * @return array<string, mixed>
     */
    public function toArray(): array
    {
        $out = [
            'Page' => $this->Page,
            'PageSize' => $this->PageSize,
            'SortOrder' => $this->SortOrder,
            'Keyword' => $this->Keyword,
            'SearchInField' => $this->SearchInField,
            'SearchDateSelection' => $this->SearchDateSelection,
            'StartDate' => $this->StartDate?->toArray(),
            'EndDate' => $this->EndDate?->toArray(),
        ];

        foreach (self::BOOL_FIELDS as $field) {
            $out[$field] = $this->{$field};
        }

        $out['SearchJobEducationLevel'] = $this->SearchJobEducationLevel;
        $out['SalaryType'] = $this->SalaryType;
        $out['SalaryMin'] = $this->SalaryMin;
        $out['SalaryMax'] = $this->SalaryMax;
        $out['SearchSalaryConditions'] = $this->SearchSalaryConditions;
        $out['SearchLocationDistance'] = $this->SearchLocationDistance;
        $out['SearchLocations'] = array_map(
            static fn (LocationField $loc): array => $loc->toArray(),
            $this->SearchLocations,
        );
        $out['SearchIndustry'] = $this->SearchIndustry;
        $out['SearchNocField'] = $this->SearchNocField;
        $out['NocCode'] = $this->NocCode;
        $out['SearchJobSource'] = $this->SearchJobSource;

        return $out;
    }

    /**
     * Every recognised field name (used for strict unknown-field rejection).
     *
     * @return string[]
     */
    public static function knownFields(): array
    {
        return array_merge(
            ['Page', 'PageSize', 'SortOrder', 'Keyword', 'SearchInField', 'SearchDateSelection'],
            self::DATE_FIELDS,
            self::BOOL_FIELDS,
            self::STRING_ARRAY_FIELDS,
            ['SalaryType', 'SalaryMin', 'SalaryMax', 'SearchLocationDistance', 'SearchLocations', 'SearchIndustry'],
            ['SearchNocField', 'NocCode', 'SearchJobSource'],
        );
    }

    /**
     * Cast contract — lets Eloquent store/read this on JobAlerts.JobSearchFilters.
     *
     * @param  array<int, string>  $arguments
     */
    public static function castUsing(array $arguments): JobSearchFiltersCast
    {
        // Optional first argument = the sibling column holding the shape version.
        return new JobSearchFiltersCast($arguments[0] ?? 'JobSearchFiltersVersion');
    }
}
