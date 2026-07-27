<?php

declare(strict_types=1);

namespace WorkBC\JobAlertNotifier\Search;

/**
 * Port of WorkBC.ElasticSearch.Models.Filters.JobSearchFilters (plus the
 * nested DateField / LocationField types).
 *
 * The JobAlerts.JobSearchFilters column stores the JSON the Angular SPA
 * produced when the alert was saved (PascalCase keys). The C# task
 * deserializes it with Newtonsoft, which matches property names
 * case-insensitively and leaves missing properties at their CLR defaults —
 * fromJson() mirrors both behaviours.
 */
final class JobSearchFilters
{
    public ?DateField $startDate = null;
    public ?DateField $endDate = null;
    public int $page = 0;
    public int $pageSize = 0;
    public ?string $keyword = null;
    public ?string $searchInField = null;

    public bool $searchJobTypeFullTime = false;
    public bool $searchJobTypePartTime = false;
    public bool $searchJobTypeLeadingToFullTime = false;
    public bool $searchJobTypePermanent = false;
    public bool $searchJobTypeTemporary = false;
    public bool $searchJobTypeCasual = false;
    public bool $searchJobTypeSeasonal = false;
    public bool $searchJobTypeDay = false;
    public bool $searchJobTypeEarly = false;
    public bool $searchJobTypeEvening = false;
    public bool $searchJobTypeFlexible = false;
    public bool $searchJobTypeMorning = false;
    public bool $searchJobTypeNight = false;
    public bool $searchJobTypeOnCall = false;
    public bool $searchJobTypeOvertime = false;
    public bool $searchJobTypeShift = false;
    public bool $searchJobTypeTbd = false;
    public bool $searchJobTypeWeekend = false;
    public bool $searchJobTypeOnSite = false;
    public bool $searchJobTypeHybrid = false;
    public bool $searchJobTypeTravelling = false;
    public bool $searchJobTypeVirtual = false;

    /** ANY_DATE = "0", TODAY = "1", PAST_THREE_DAYS = "2", DATE_RANGE = "3" */
    public ?string $searchDateSelection = null;

    /** @var list<string>|null */
    public ?array $searchJobEducationLevel = null;

    public int $salaryType = 0;
    public int $searchLocationDistance = 0;

    /** @var list<LocationField>|null */
    public ?array $searchLocations = null;

    /** @var list<string>|null */
    public ?array $searchSalaryConditions = null;

    public int $sortOrder = 0;

    public bool $salaryBracket1 = false;
    public bool $salaryBracket2 = false;
    public bool $salaryBracket3 = false;
    public bool $salaryBracket4 = false;
    public bool $salaryBracket5 = false;
    public bool $salaryBracket6 = false;
    public bool $searchSalaryUnknown = false;
    public ?string $salaryMin = null;
    public ?string $salaryMax = null;

    /** @var list<int>|null */
    public ?array $searchIndustry = null;

    public bool $searchIsApprentice = false;
    public bool $searchIsVeterans = false;
    public bool $searchIsIndigenous = false;
    public bool $searchIsMatureWorkers = false;
    public bool $searchIsNewcomers = false;
    public bool $searchIsPeopleWithDisabilities = false;
    public bool $searchIsStudents = false;
    public bool $searchIsVisibleMinority = false;
    public bool $searchIsYouth = false;
    public bool $searchIsPostingsInEnglish = false;
    public bool $searchIsPostingsInEnglishAndFrench = false;

    public ?string $searchNocField = null;
    public ?string $searchJobSource = null;
    public bool $searchExcludePlacementAgencyJobs = false;

    public static function fromJson(string $json): self
    {
        $data = json_decode($json, true);
        if (!is_array($data)) {
            throw new \InvalidArgumentException('JobSearchFilters column does not contain valid JSON');
        }
        return self::fromArray($data);
    }

    /** @param array<string, mixed> $data */
    public static function fromArray(array $data): self
    {
        // Newtonsoft matches property names case-insensitively.
        $map = [];
        foreach ($data as $key => $value) {
            $map[strtolower((string) $key)] = $value;
        }

        $f = new self();

        $f->startDate = DateField::fromValue($map['startdate'] ?? null);
        $f->endDate = DateField::fromValue($map['enddate'] ?? null);
        $f->page = self::toInt($map['page'] ?? 0);
        $f->pageSize = self::toInt($map['pagesize'] ?? 0);
        $f->keyword = self::toStringOrNull($map['keyword'] ?? null);
        $f->searchInField = self::toStringOrNull($map['searchinfield'] ?? null);

        foreach ([
            'searchJobTypeFullTime', 'searchJobTypePartTime', 'searchJobTypeLeadingToFullTime',
            'searchJobTypePermanent', 'searchJobTypeTemporary', 'searchJobTypeCasual',
            'searchJobTypeSeasonal', 'searchJobTypeDay', 'searchJobTypeEarly',
            'searchJobTypeEvening', 'searchJobTypeFlexible', 'searchJobTypeMorning',
            'searchJobTypeNight', 'searchJobTypeOnCall', 'searchJobTypeOvertime',
            'searchJobTypeShift', 'searchJobTypeTbd', 'searchJobTypeWeekend',
            'searchJobTypeOnSite', 'searchJobTypeHybrid', 'searchJobTypeTravelling',
            'searchJobTypeVirtual',
            'salaryBracket1', 'salaryBracket2', 'salaryBracket3', 'salaryBracket4',
            'salaryBracket5', 'salaryBracket6', 'searchSalaryUnknown',
            'searchIsApprentice', 'searchIsVeterans', 'searchIsIndigenous',
            'searchIsMatureWorkers', 'searchIsNewcomers', 'searchIsPeopleWithDisabilities',
            'searchIsStudents', 'searchIsVisibleMinority', 'searchIsYouth',
            'searchIsPostingsInEnglish', 'searchIsPostingsInEnglishAndFrench',
            'searchExcludePlacementAgencyJobs',
        ] as $prop) {
            $f->{$prop} = self::toBool($map[strtolower($prop)] ?? false);
        }

        $f->searchDateSelection = self::toStringOrNull($map['searchdateselection'] ?? null);
        $f->searchJobEducationLevel = self::toStringList($map['searchjobeducationlevel'] ?? null);
        $f->salaryType = self::toInt($map['salarytype'] ?? 0);
        $f->searchLocationDistance = self::toInt($map['searchlocationdistance'] ?? 0);
        $f->searchSalaryConditions = self::toStringList($map['searchsalaryconditions'] ?? null);
        $f->sortOrder = self::toInt($map['sortorder'] ?? 0);
        $f->salaryMin = self::toStringOrNull($map['salarymin'] ?? null);
        $f->salaryMax = self::toStringOrNull($map['salarymax'] ?? null);
        $f->searchNocField = self::toStringOrNull($map['searchnocfield'] ?? null);
        $f->searchJobSource = self::toStringOrNull($map['searchjobsource'] ?? null);

        $industries = $map['searchindustry'] ?? null;
        if (is_array($industries)) {
            $f->searchIndustry = array_values(array_map(fn($v) => self::toInt($v), $industries));
        }

        $locations = $map['searchlocations'] ?? null;
        if (is_array($locations)) {
            $f->searchLocations = array_values(array_map(
                fn($loc) => LocationField::fromValue($loc),
                $locations
            ));
        }

        return $f;
    }

    private static function toBool(mixed $value): bool
    {
        if (is_bool($value)) {
            return $value;
        }
        if (is_string($value)) {
            return strtolower($value) === 'true' || $value === '1';
        }
        return (bool) $value;
    }

    private static function toInt(mixed $value): int
    {
        if (is_int($value)) {
            return $value;
        }
        if (is_numeric($value)) {
            return (int) $value;
        }
        return 0;
    }

    private static function toStringOrNull(mixed $value): ?string
    {
        if ($value === null) {
            return null;
        }
        if (is_scalar($value)) {
            return (string) $value;
        }
        return null;
    }

    /** @return list<string>|null */
    private static function toStringList(mixed $value): ?array
    {
        if (!is_array($value)) {
            return null;
        }
        return array_values(array_map(fn($v) => (string) $v, $value));
    }
}

/**
 * Port of the C# DateField. ToString() renders the same
 * "{Year}-{Month:00}-{Day:00}T{Hour:00}:{Minute:00}:{Second:00}.{Millisecond:000}"
 * format used inside the DatePosted range filter.
 */
final class DateField
{
    public int $year = 0;
    public int $month = 0;
    public int $day = 0;
    public int $hour = 0;
    public int $minute = 0;
    public int $second = 0;
    public int $millisecond = 0;

    public static function fromDateTime(\DateTimeInterface $date): self
    {
        $d = new self();
        $d->year = (int) $date->format('Y');
        $d->month = (int) $date->format('n');
        $d->day = (int) $date->format('j');
        $d->hour = (int) $date->format('G');
        $d->minute = (int) $date->format('i');
        $d->second = (int) $date->format('s');
        $d->millisecond = (int) $date->format('v');
        return $d;
    }

    public static function fromValue(mixed $value): ?self
    {
        if (!is_array($value)) {
            return null;
        }

        $map = [];
        foreach ($value as $key => $v) {
            $map[strtolower((string) $key)] = $v;
        }

        $d = new self();
        $d->year = (int) ($map['year'] ?? 0);
        $d->month = (int) ($map['month'] ?? 0);
        $d->day = (int) ($map['day'] ?? 0);
        $d->hour = (int) ($map['hour'] ?? 0);
        $d->minute = (int) ($map['minute'] ?? 0);
        $d->second = (int) ($map['second'] ?? 0);
        $d->millisecond = (int) ($map['millisecond'] ?? 0);
        return $d;
    }

    public function __toString(): string
    {
        return sprintf(
            '%d-%02d-%02dT%02d:%02d:%02d.%03d',
            $this->year, $this->month, $this->day,
            $this->hour, $this->minute, $this->second, $this->millisecond
        );
    }
}

/**
 * Port of the C# LocationField. The Postal getter uppercases and strips
 * spaces, exactly like the C# property.
 */
final class LocationField
{
    public ?string $city = null;
    public ?string $region = null;
    private ?string $postal = null;

    public static function fromValue(mixed $value): self
    {
        $l = new self();
        if (!is_array($value)) {
            return $l;
        }

        $map = [];
        foreach ($value as $key => $v) {
            $map[strtolower((string) $key)] = $v;
        }

        $l->city = isset($map['city']) && is_scalar($map['city']) ? (string) $map['city'] : null;
        $l->region = isset($map['region']) && is_scalar($map['region']) ? (string) $map['region'] : null;
        $l->postal = isset($map['postal']) && is_scalar($map['postal']) ? (string) $map['postal'] : null;
        return $l;
    }

    public function postal(): ?string
    {
        if ($this->postal === null) {
            return null;
        }
        return str_replace(' ', '', strtoupper($this->postal));
    }
}
