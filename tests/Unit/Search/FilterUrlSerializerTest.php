<?php

namespace Tests\Unit\Search;

use App\Search\Filters\JobSearchFilters;
use App\Search\Filters\LocationField;
use App\Search\Url\FilterUrlSerializer;
use PHPUnit\Framework\TestCase;

/**
 * SRCH-6 — the shareable-URL contract. These pure-unit tests prove the
 * round-trip (filters → URL → filters) is lossless for every facet, that the
 * decoder is version 0/1 aware and tamper-resistant, and that legacy alert
 * deep-links map onto the same value object.
 */
class FilterUrlSerializerTest extends TestCase
{
    private FilterUrlSerializer $serializer;

    protected function setUp(): void
    {
        parent::setUp();
        $this->serializer = new FilterUrlSerializer;
    }

    /** A filter set that exercises every facet at once. */
    private function fullyPopulated(): JobSearchFilters
    {
        return JobSearchFilters::fromArray([
            'Keyword' => 'registered nurse',
            'SearchInField' => 'title',
            'SortOrder' => 9,
            'Page' => 3,
            'PageSize' => 50,
            'SearchLocations' => [
                ['City' => 'Victoria'],
                ['City' => 'Nanaimo'],
                ['Region' => 'Vancouver Island'],
                ['Postal' => 'V6B1A1'],
            ],
            'SearchLocationDistance' => 25,
            'SearchJobTypeFullTime' => true,
            'SearchJobTypePermanent' => true,
            'SearchJobTypeWeekend' => true,
            'SearchJobTypeVirtual' => true,
            'SearchIndustry' => [4, 9, 19],
            'SearchJobEducationLevel' => ['University', 'No education'],
            'SearchDateSelection' => '3',
            'StartDate' => ['Year' => 2024, 'Month' => 1, 'Day' => 15],
            'EndDate' => ['Year' => 2024, 'Month' => 6, 'Day' => 30],
            'SalaryType' => 4,
            'SalaryBracket2' => true,
            'SalaryBracket5' => true,
            'SalaryBracket6' => true,
            'SearchSalaryUnknown' => true,
            'SalaryMin' => '50000',
            'SalaryMax' => '90000',
            'SearchSalaryConditions' => ['Bonus', 'Dental plan'],
            'SearchIsIndigenous' => true,
            'SearchIsYouth' => true,
            'SearchIsPostingsInEnglish' => false,
            'SearchIsPostingsInEnglishAndFrench' => true,
            'NocCode' => '31301',
            'SearchNocField' => '31301',
            'SearchJobSource' => '3',
            'SearchExcludePlacementAgencyJobs' => true,
            'SearchNjbJobsFirst' => true,
        ]);
    }

    public function test_round_trip_is_lossless_for_every_facet(): void
    {
        $filters = $this->fullyPopulated();

        $restored = $this->serializer->fromQuery($this->serializer->toQuery($filters));

        $this->assertEquals($filters, $restored);
    }

    public function test_default_filters_produce_an_empty_query(): void
    {
        $this->assertSame([], $this->serializer->toQuery(new JobSearchFilters));
    }

    public function test_round_trip_of_defaults_returns_defaults(): void
    {
        $defaults = new JobSearchFilters;

        $restored = $this->serializer->fromQuery($this->serializer->toQuery($defaults));

        $this->assertEquals($defaults, $restored);
    }

    public function test_only_non_default_facets_are_emitted(): void
    {
        $filters = new JobSearchFilters;
        $filters->Keyword = 'welder';
        $filters->SearchJobSource = '2';

        $query = $this->serializer->toQuery($filters);

        $this->assertSame(['q' => 'welder', 'source' => '2'], $query);
    }

    public function test_version_zero_and_one_both_decode_the_current_shape(): void
    {
        $filters = $this->fullyPopulated();
        $query = $this->serializer->toQuery($filters);

        $this->assertEquals($filters, $this->serializer->fromQuery($query + ['v' => '0']));
        $this->assertEquals($filters, $this->serializer->fromQuery($query + ['v' => '1']));
    }

    public function test_unsupported_version_falls_back_to_current(): void
    {
        $query = $this->serializer->toQuery($this->fullyPopulated()) + ['v' => '99'];

        // Does not throw; decodes as the current shape.
        $restored = $this->serializer->fromQuery($query);

        $this->assertSame('registered nurse', $restored->Keyword);
    }

    public function test_unknown_and_tampered_params_are_ignored(): void
    {
        $restored = $this->serializer->fromQuery([
            'q' => 'nurse',
            'utm_source' => 'newsletter',   // unrelated tracking param
            'jt' => 'FullTime,Bogus',        // one valid, one tampered suffix
            'in' => 'sql-injection',         // invalid scope
            'ind' => '4,999',                // one valid, one unknown industry
            'source' => '42',                // invalid source enum
        ]);

        $this->assertSame('nurse', $restored->Keyword);
        $this->assertTrue($restored->SearchJobTypeFullTime);
        $this->assertSame('all', $restored->SearchInField);      // fell back to default
        $this->assertSame([4], $restored->SearchIndustry);       // unknown id dropped
        $this->assertSame('0', $restored->SearchJobSource);      // invalid enum ignored
    }

    public function test_partial_filter_round_trips(): void
    {
        $filters = new JobSearchFilters;
        $filters->SearchLocations = [LocationField::fromArray(['City' => 'Kelowna'])];
        $filters->SearchLocationDistance = 50;
        $filters->SearchDateSelection = '2';

        $restored = $this->serializer->fromQuery($this->serializer->toQuery($filters));

        $this->assertEquals($filters, $restored);
    }

    public function test_to_url_builds_a_canonical_jobs_path(): void
    {
        $filters = new JobSearchFilters;
        $filters->Keyword = 'nurse';
        $filters->SearchJobSource = '3';

        $url = $this->serializer->toUrl($filters);

        $this->assertStringStartsWith('/jobs?', $url);
        $this->assertStringContainsString('q=nurse', $url);
        $this->assertStringContainsString('source=3', $url);
    }

    public function test_to_url_with_no_filters_is_the_bare_path(): void
    {
        $this->assertSame('/jobs', $this->serializer->toUrl(new JobSearchFilters));
    }

    public function test_legacy_matrix_string_maps_onto_filters(): void
    {
        $legacy = ';search=registered nurse;city=Victoria;radius=25;sortby=9;'
            . 'hoursofwork=1;education=1,4;benefits=2,4;employmentgroups=2,9;'
            . 'salaryinterval=4;salaryrange=2,5,7;salaryrangemin=50000;'
            . 'noc=31301;jobsource=3;placementagency=1;language=1;'
            . 'datetype=3;startdate=20240115;enddate=20240630';

        $f = $this->serializer->fromLegacy($legacy);

        $this->assertSame('registered nurse', $f->Keyword);
        $this->assertSame('all', $f->SearchInField);
        $this->assertSame('Victoria', $f->SearchLocations[0]->City);
        $this->assertSame(25, $f->SearchLocationDistance);
        $this->assertSame(9, $f->SortOrder);
        $this->assertTrue($f->SearchJobTypeFullTime);
        $this->assertSame(['University', 'No education'], $f->SearchJobEducationLevel);
        $this->assertSame(['Bonus', 'Dental plan'], $f->SearchSalaryConditions);
        $this->assertTrue($f->SearchIsIndigenous);
        $this->assertTrue($f->SearchIsYouth);
        $this->assertSame(4, $f->SalaryType);
        $this->assertTrue($f->SalaryBracket2);
        $this->assertTrue($f->SalaryBracket5);
        $this->assertTrue($f->SearchSalaryUnknown);
        $this->assertSame('50000', $f->SalaryMin);
        $this->assertSame('31301', $f->NocCode);
        $this->assertSame('31301', $f->SearchNocField);
        $this->assertSame('3', $f->SearchJobSource);
        $this->assertTrue($f->SearchExcludePlacementAgencyJobs);
        $this->assertFalse($f->SearchIsPostingsInEnglish);
        $this->assertTrue($f->SearchIsPostingsInEnglishAndFrench);
        $this->assertSame('3', $f->SearchDateSelection);
        $this->assertSame(2024, $f->StartDate->Year);
        $this->assertSame(1, $f->StartDate->Month);
        $this->assertSame(15, $f->StartDate->Day);
        $this->assertSame(30, $f->EndDate->Day);
    }

    public function test_legacy_scope_variants_set_search_in_field(): void
    {
        $this->assertSame('title', $this->serializer->fromLegacy('title=engineer')->SearchInField);
        $this->assertSame('employer', $this->serializer->fromLegacy('employer=acme')->SearchInField);
        $this->assertSame('jobId', $this->serializer->fromLegacy('job=12345')->SearchInField);
    }

    public function test_legacy_matrix_string_tolerates_the_hash_route_prefix(): void
    {
        $f = $this->serializer->fromLegacy('#/job-search;search=nurse;noc=00042');

        $this->assertSame('nurse', $f->Keyword);
        $this->assertSame('00042', $f->NocCode);
        $this->assertSame('00042', $f->SearchNocField);
    }

    public function test_legacy_array_form_is_accepted(): void
    {
        $f = $this->serializer->fromLegacy(['Search' => 'nurse', 'JobSource' => '1']);

        $this->assertSame('nurse', $f->Keyword);
        $this->assertSame('1', $f->SearchJobSource);
    }
}
