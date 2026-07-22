<?php

namespace App\Search\Results;

/**
 * A single job in a search response — the API response projection of an
 * OpenSearch document (docs/contracts.md §2.1), NOT the raw index doc.
 *
 * Response-layer rules applied here (which differ from the raw index):
 *  - NullValueHandling.Ignore: empty/null fields are omitted, so federal and
 *    external items carry different key sets.
 *  - City is a CSV *string* (the index City is an array).
 *  - Region and Location stay arrays; Location[] = { Lat, Lon } (strings).
 *  - Noc2021 is a zero-padded 5-char string (the index stores a float); there is
 *    no Noc (2016) field in the response.
 *
 * Read-only mapping (Rule B): copies index values; never recomputes Salary,
 * ExpireDate or resolved NOC. Decoration fields that need the DB or a scoring
 * pass (Views, IsNew, Score/Reason) are added by the API resource layer, not here.
 */
final class JobResult
{
    /**
     * @param  array<string, mixed>  $data
     */
    private function __construct(private array $data) {}

    /**
     * @param  array<string, mixed>  $source  an OpenSearch hit _source
     */
    public static function fromSource(array $source): self
    {
        $out = [];
        $put = static function (string $key, mixed $value) use (&$out): void {
            if ($value === null || $value === '' || $value === []) {
                return;
            }
            $out[$key] = $value;
        };

        $put('JobId', $source['JobId'] ?? null);
        $put('Title', $source['Title'] ?? null);
        $put('EmployerName', $source['EmployerName'] ?? null);
        $put('DatePosted', $source['DatePosted'] ?? null);
        $put('ExpireDate', $source['ExpireDate'] ?? null);

        // City: join the index's City array into a CSV string (ListToCsvConverter)
        $put('City', self::csv($source['City'] ?? null));
        $put('Province', $source['Province'] ?? null);
        $put('Region', self::stringList($source['Region'] ?? null));
        $put('Location', self::locations($source['Location'] ?? null));

        $put('Noc2021', self::noc2021($source['Noc2021'] ?? null));
        $put('NocGroup', $source['NocGroup'] ?? null);

        if (array_key_exists('Salary', $source) && $source['Salary'] !== null) {
            $out['Salary'] = (float) $source['Salary'];
        }
        $put('SalarySummary', $source['SalarySummary'] ?? null);

        if (array_key_exists('IsFederalJob', $source)) {
            $out['IsFederalJob'] = (bool) $source['IsFederalJob'];
        }

        $put('HoursOfWork', self::description($source['HoursOfWork'] ?? null));
        $put('PeriodOfEmployment', self::description($source['PeriodOfEmployment'] ?? null));
        $put('EmploymentTerms', self::description($source['EmploymentTerms'] ?? null));
        $put('WorkplaceType', self::workplaceType($source['WorkplaceType'] ?? null));
        $put('WorkLangCd', self::description($source['WorkLangCd'] ?? null));
        $put('WageClass', $source['WageClass'] ?? null);
        $put('SalaryConditions', self::description($source['SalaryConditions'] ?? null));
        $put('SkillCategories', $source['SkillCategories'] ?? null);

        $put('JobDescription', $source['JobDescription'] ?? null);
        $put('ExternalSource', $source['ExternalSource'] ?? null);
        $put('ApplyWebsite', $source['ApplyWebsite'] ?? null);

        return new self($out);
    }

    /**
     * @return array<string, mixed>
     */
    public function toArray(): array
    {
        return $this->data;
    }

    /**
     * Join an index string array into a CSV string; null when empty.
     */
    private static function csv(mixed $value): ?string
    {
        if (! is_array($value)) {
            return is_string($value) && $value !== '' ? $value : null;
        }

        $parts = array_values(array_filter(
            array_map(static fn ($v): string => (string) $v, $value),
            static fn (string $v): bool => $v !== '',
        ));

        return $parts === [] ? null : implode(', ', $parts);
    }

    /**
     * @return string[]|null
     */
    private static function stringList(mixed $value): ?array
    {
        if (! is_array($value) || $value === []) {
            return null;
        }

        return array_values(array_map(static fn ($v): string => (string) $v, $value));
    }

    /**
     * @return array<int, array{Lat: string, Lon: string}>|null
     */
    private static function locations(mixed $value): ?array
    {
        if (! is_array($value) || $value === []) {
            return null;
        }

        $out = [];
        foreach ($value as $point) {
            if (is_array($point) && isset($point['Lat'], $point['Lon'])) {
                $out[] = ['Lat' => (string) $point['Lat'], 'Lon' => (string) $point['Lon']];
            }
        }

        return $out === [] ? null : $out;
    }

    /**
     * Zero-padded 5-char string from the index's float Noc2021.
     */
    private static function noc2021(mixed $value): ?string
    {
        if ($value === null || $value === '') {
            return null;
        }

        return str_pad((string) (int) round((float) $value), 5, '0', STR_PAD_LEFT);
    }

    /**
     * Normalize a { Description: string[] } wrapper; null when empty.
     *
     * @return array{Description: string[]}|null
     */
    private static function description(mixed $value): ?array
    {
        if (! is_array($value) || ! isset($value['Description']) || ! is_array($value['Description'])) {
            return null;
        }

        $descriptions = array_values(array_filter(
            array_map(static fn ($v): string => (string) $v, $value['Description']),
            static fn (string $v): bool => $v !== '',
        ));

        return $descriptions === [] ? null : ['Description' => $descriptions];
    }

    /**
     * @return array{Id: int, Description: string}|null
     */
    private static function workplaceType(mixed $value): ?array
    {
        if (! is_array($value) || ! isset($value['Id'])) {
            return null;
        }

        return [
            'Id' => (int) $value['Id'],
            'Description' => (string) ($value['Description'] ?? ''),
        ];
    }
}
