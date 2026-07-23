<?php

namespace App\Search\Seo;

use Illuminate\Support\Str;

/**
 * Builds a schema.org/JobPosting structured-data graph for a job-detail page
 * (architecture.md §6). The array is JSON-encoded into a server-rendered
 * `<script type="application/ld+json">` block so crawlers can index the job.
 *
 * Only fields that are present are emitted (federal and external postings carry
 * different keys — contracts.md §2.1 NullValueHandling.Ignore).
 */
final class JobPostingSchema
{
    /**
     * @param  array<string, mixed>  $job  Projected §2.1 job fields.
     * @return array<string, mixed>
     */
    public static function build(array $job, string $canonicalUrl): array
    {
        $schema = [
            '@context' => 'https://schema.org/',
            '@type' => 'JobPosting',
            'title' => (string) ($job['Title'] ?? 'Job posting'),
            'description' => self::description($job),
            'url' => $canonicalUrl,
        ];

        if (! empty($job['JobId'])) {
            $schema['identifier'] = [
                '@type' => 'PropertyValue',
                'name' => 'WorkBC Job Number',
                'value' => (string) $job['JobId'],
            ];
        }

        if (! empty($job['DatePosted'])) {
            $schema['datePosted'] = (string) $job['DatePosted'];
        }

        if (! empty($job['ExpireDate'])) {
            $schema['validThrough'] = (string) $job['ExpireDate'];
        }

        if ($employmentType = self::employmentType($job)) {
            $schema['employmentType'] = $employmentType;
        }

        if (! empty($job['EmployerName'])) {
            $schema['hiringOrganization'] = [
                '@type' => 'Organization',
                'name' => (string) $job['EmployerName'],
            ];
        }

        if ($location = self::jobLocation($job)) {
            $schema['jobLocation'] = $location;
        }

        if (isset($job['Salary']) && is_numeric($job['Salary']) && (float) $job['Salary'] > 0) {
            $schema['baseSalary'] = [
                '@type' => 'MonetaryAmount',
                'currency' => 'CAD',
                'value' => array_filter([
                    '@type' => 'QuantitativeValue',
                    'value' => (float) $job['Salary'],
                    'unitText' => self::salaryUnit($job),
                ], static fn ($v): bool => $v !== null),
            ];
        }

        return $schema;
    }

    /**
     * @param  array<string, mixed>  $job
     */
    private static function description(array $job): string
    {
        if (! empty($job['JobDescription'])) {
            return trim(strip_tags((string) $job['JobDescription']));
        }

        $parts = array_filter([
            $job['Title'] ?? null,
            $job['EmployerName'] ?? null,
            $job['City'] ?? null,
            $job['SalarySummary'] ?? null,
        ]);

        return $parts === [] ? 'Job posting' : implode(' — ', $parts);
    }

    /**
     * @param  array<string, mixed>  $job
     * @return array<string, mixed>|null
     */
    private static function jobLocation(array $job): ?array
    {
        $address = array_filter([
            '@type' => 'PostalAddress',
            'addressLocality' => $job['City'] ?? null,
            'addressRegion' => $job['Province'] ?? null,
            'addressCountry' => 'CA',
        ], static fn ($v): bool => $v !== null && $v !== '');

        // Only 'CA' present means we know nothing useful about the place.
        if (count($address) <= 2) {
            return null;
        }

        $place = ['@type' => 'Place', 'address' => $address];

        $geo = $job['Location'][0] ?? null;
        if (is_array($geo) && isset($geo['Lat'], $geo['Lon'])) {
            $place['geo'] = [
                '@type' => 'GeoCoordinates',
                'latitude' => (string) $geo['Lat'],
                'longitude' => (string) $geo['Lon'],
            ];
        }

        return $place;
    }

    /**
     * @param  array<string, mixed>  $job
     */
    private static function employmentType(array $job): ?string
    {
        $descriptions = $job['HoursOfWork']['Description'] ?? [];
        $text = strtolower(implode(' ', (array) $descriptions));

        return match (true) {
            str_contains($text, 'full') => 'FULL_TIME',
            str_contains($text, 'part') => 'PART_TIME',
            default => null,
        };
    }

    /**
     * @param  array<string, mixed>  $job
     */
    private static function salaryUnit(array $job): ?string
    {
        $summary = strtolower((string) ($job['SalarySummary'] ?? ''));

        return match (true) {
            str_contains($summary, 'hour') => 'HOUR',
            str_contains($summary, 'week') => 'WEEK',
            str_contains($summary, 'month') => 'MONTH',
            str_contains($summary, 'year') || str_contains($summary, 'annual') => 'YEAR',
            default => null,
        };
    }
}
