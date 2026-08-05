<?php

namespace Tests\Unit\Search;

use App\Search\Contracts\Geocoder;
use App\Search\Filters\JobSearchFilters;
use App\Search\Queries\JobSearchQuery;
use App\Search\Support\GeoPoint;
use PHPUnit\Framework\Attributes\DataProvider;
use PHPUnit\Framework\TestCase;

/**
 * JobSearchQuery structured-body tests (FND-7 acceptance): the query is built as
 * a structured array (no string concatenation), always carries the
 * ExpireDate >= now/d America/Vancouver base filter, honours the 11 sort orders,
 * and pages correctly.
 */
class JobSearchQueryTest extends TestCase
{
    /**
     * @param  array<string, mixed>  $overrides
     */
    private function build(array $overrides = [], ?Geocoder $geocoder = null): array
    {
        $filters = JobSearchFilters::fromArray($overrides);

        return (new JobSearchQuery($filters, $geocoder))->build();
    }

    /**
     * Flatten every bool/should clause across the must groups.
     *
     * @param  array<string, mixed>  $body
     * @return array<int, array<string, mixed>>
     */
    private function shouldEntries(array $body): array
    {
        $entries = [];
        foreach ($body['query']['bool']['must'] as $group) {
            foreach ($group['bool']['should'] as $entry) {
                $entries[] = $entry;
            }
        }

        return $entries;
    }

    private function assertHasEntry(array $body, array $entry): void
    {
        $this->assertContains($entry, $this->shouldEntries($body), 'Expected clause not found: '.json_encode($entry));
    }

    public function test_base_expire_date_filter_is_vancouver(): void
    {
        $body = $this->build();

        $this->assertSame([
            'range' => [
                'ExpireDate' => ['gte' => 'now/d', 'time_zone' => 'America/Vancouver'],
            ],
        ], $body['query']['bool']['filter']);
    }

    public function test_track_total_hits_enabled(): void
    {
        $this->assertTrue($this->build()['track_total_hits']);
    }

    public function test_default_paging(): void
    {
        $body = $this->build();
        $this->assertSame(20, $body['size']);
        $this->assertSame(0, $body['from']);
    }

    public function test_paging_offset(): void
    {
        $body = $this->build(['Page' => 3, 'PageSize' => 10]);
        $this->assertSame(10, $body['size']);
        $this->assertSame(20, $body['from']);
    }

    /**
     * @return array<string, array{0: int, 1: array<string, string>}>
     */
    public static function sortOrders(): array
    {
        return [
            'date desc' => [1, ['DatePosted' => 'desc']],
            'date asc' => [2, ['DatePosted' => 'asc']],
            'title asc' => [3, ['Title.normalize' => 'asc']],
            'title desc' => [4, ['Title.normalize' => 'desc']],
            'city asc' => [5, ['City.normalize' => 'asc']],
            'city desc' => [6, ['City.normalize' => 'desc']],
            'employer asc' => [7, ['EmployerName.normalize' => 'asc']],
            'employer desc' => [8, ['EmployerName.normalize' => 'desc']],
            'salary asc' => [9, ['SalarySort.Ascending' => 'asc']],
            'salary desc' => [10, ['SalarySort.Descending' => 'desc']],
            'relevance' => [11, ['_score' => 'desc']],
        ];
    }

    /**
     * @param  array<string, string>  $expectedPrimary
     */
    #[DataProvider('sortOrders')]
    public function test_all_eleven_sort_orders(int $sortOrder, array $expectedPrimary): void
    {
        $body = $this->build(['SortOrder' => $sortOrder]);

        $this->assertSame($expectedPrimary, $body['sort'][0]);
        // secondary tie-breakers are always appended
        $this->assertContains(['DatePosted' => 'desc'], $body['sort']);
        $this->assertContains(['JobId.keyword' => 'asc'], $body['sort']);
    }

    public function test_keyword_all_fields_with_boosts(): void
    {
        $body = $this->build(['Keyword' => 'program manager']);

        $this->assertHasEntry($body, [
            'simple_query_string' => [
                'query' => 'program manager',
                'fields' => ['EmployerName^4', 'JobId^10', 'Title^5', 'AllSkills^1', 'JobDescription^1', 'City^2'],
                'default_operator' => 'AND',
                'quote_field_suffix' => '.exact',
            ],
        ]);
    }

    public function test_keyword_single_field(): void
    {
        $body = $this->build(['Keyword' => 'acme', 'SearchInField' => 'employer']);

        $this->assertHasEntry($body, [
            'simple_query_string' => [
                'query' => 'acme',
                'fields' => ['EmployerName'],
                'default_operator' => 'AND',
                'quote_field_suffix' => '.exact',
            ],
        ]);
    }

    public function test_salary_bracket_annual_range(): void
    {
        // bracket 2 annually → $40,000 – $59,999.99
        $body = $this->build(['SalaryBracket2' => true, 'SalaryType' => 4]);

        $this->assertHasEntry($body, ['range' => ['Salary' => ['gte' => 40000.0, 'lte' => 59999.99]]]);
    }

    public function test_salary_unknown_uses_sentinel(): void
    {
        $body = $this->build(['SearchSalaryUnknown' => true]);

        $this->assertHasEntry($body, ['range' => ['SalarySort.Descending' => ['lte' => -99999999]]]);
    }

    public function test_hours_of_work_term(): void
    {
        $body = $this->build(['SearchJobTypeFullTime' => true]);
        $this->assertHasEntry($body, ['term' => ['HoursOfWork.Description.keyword' => 'Full-time']]);
    }

    public function test_workplace_virtual_term(): void
    {
        $body = $this->build(['SearchJobTypeVirtual' => true]);
        $this->assertHasEntry($body, ['term' => ['WorkplaceType.Id' => 15141]]);
    }

    public function test_education_term(): void
    {
        $body = $this->build(['SearchJobEducationLevel' => ['University']]);
        $this->assertHasEntry($body, ['term' => ['EduLevel.keyword' => 'University']]);
    }

    public function test_industry_term(): void
    {
        $body = $this->build(['SearchIndustry' => [52]]);
        $this->assertHasEntry($body, ['term' => ['NaicsId' => 52]]);
    }

    public function test_equity_indigenous_maps_to_is_aboriginal(): void
    {
        $body = $this->build(['SearchIsIndigenous' => true]);
        $this->assertHasEntry($body, ['term' => ['IsAboriginal' => true]]);
    }

    public function test_noc_2021_term(): void
    {
        $body = $this->build(['SearchNocField' => '62100']);
        $this->assertHasEntry($body, ['term' => ['Noc2021' => '62100']]);
    }

    public function test_job_source_federal(): void
    {
        $body = $this->build(['SearchJobSource' => '1']);
        $this->assertHasEntry($body, ['term' => ['IsFederalJob' => true]]);
    }

    public function test_job_source_municipal_uses_nested_external_source(): void
    {
        $body = $this->build(['SearchJobSource' => '4']);

        $this->assertHasEntry($body, ['term' => ['EmployerTypeId' => ['value' => '4']]]);
        $this->assertHasEntry($body, ['nested' => [
            'path' => 'ExternalSource',
            'query' => ['bool' => ['should' => [
                ['match_phrase' => ['ExternalSource.Source.Source' => 'CivicInfoBC']],
                ['match_phrase' => ['ExternalSource.Source.Source' => 'CivicJobs.ca']],
            ]]],
        ]]);
    }

    public function test_exclude_placement_agency_goes_to_must_not(): void
    {
        $body = $this->build(['SearchExcludePlacementAgencyJobs' => true]);
        $this->assertContains(['term' => ['EmployerTypeId' => 1]], $body['query']['bool']['must_not']);
    }

    public function test_exact_city_location_includes_virtual_jobs(): void
    {
        $body = $this->build([
            'SearchLocationDistance' => -1,
            'SearchLocations' => [['City' => 'Vancouver']],
        ]);

        $entries = $this->shouldEntries($body);
        $this->assertContains(['term' => ['City.normalize' => 'vancouver']], $entries);
        $this->assertContains(['term' => ['WorkplaceType.Id' => ['value' => 15141, 'boost' => 0]]], $entries);
    }

    public function test_region_location_uses_region_keyword(): void
    {
        $body = $this->build([
            'SearchLocations' => [['Region' => 'Mainland / Southwest']],
        ]);

        $this->assertContains(
            ['term' => ['Region.keyword' => 'Mainland / Southwest']],
            $this->shouldEntries($body),
        );
    }

    public function test_radius_search_uses_geocoder_and_adds_geo_sort(): void
    {
        $geocoder = new class implements Geocoder {
            public function resolve(string $locationKey): ?GeoPoint
            {
                return new GeoPoint(49.2827, -123.1207);
            }
        };

        $body = $this->build([
            'SearchLocationDistance' => 25,
            'SearchLocations' => [['City' => 'Vancouver']],
        ], $geocoder);

        $this->assertContains(
            ['geo_distance' => ['distance' => '25km', 'LocationGeo' => ['lat' => 49.2827, 'lon' => -123.1207]]],
            $this->shouldEntries($body),
        );

        // a geo-distance sort is prepended to the tie-breakers
        $this->assertSame('_geo_distance', array_key_first($body['sort'][1]));
    }

    public function test_no_filters_produces_empty_must(): void
    {
        $body = $this->build();
        $this->assertSame([], $body['query']['bool']['must']);
        $this->assertSame([], $body['query']['bool']['must_not']);
    }
}
