<?php

namespace Tests\Unit\Search;

use App\Search\Filters\DateField;
use App\Search\Filters\InvalidFilterException;
use App\Search\Filters\JobSearchFilters;
use App\Search\Filters\JobSearchFiltersCast;
use App\Search\Filters\LocationField;
use Illuminate\Database\Eloquent\Model;
use PHPUnit\Framework\TestCase;

/**
 * JobSearchFilters value-object contract tests (docs/contracts.md §1):
 * versioning (accept 0 and 1), strict unknown-field rejection, exact PascalCase
 * defaults, sub-object parsing, and the Eloquent cast round-trip.
 */
class JobSearchFiltersTest extends TestCase
{
    public function test_accepts_version_0(): void
    {
        $filters = JobSearchFilters::fromArray(['Page' => 2], 0);
        $this->assertSame(2, $filters->Page);
    }

    public function test_accepts_version_1(): void
    {
        $filters = JobSearchFilters::fromArray(['Page' => 2], 1);
        $this->assertSame(2, $filters->Page);
    }

    public function test_rejects_unsupported_version(): void
    {
        $this->expectException(InvalidFilterException::class);
        JobSearchFilters::fromArray(['Page' => 2], 2);
    }

    public function test_rejects_unknown_field(): void
    {
        $this->expectException(InvalidFilterException::class);
        JobSearchFilters::fromArray(['NotARealField' => true]);
    }

    public function test_defaults_match_contract(): void
    {
        $filters = JobSearchFilters::fromArray([]);

        $this->assertSame(20, $filters->PageSize);
        $this->assertSame('all', $filters->SearchInField);
        $this->assertSame('0', $filters->SearchDateSelection);
        $this->assertSame('0', $filters->SearchJobSource);
        $this->assertTrue($filters->SearchIsPostingsInEnglish);
        $this->assertFalse($filters->SearchIsPostingsInEnglishAndFrench);
        $this->assertSame([], $filters->SearchLocations);
    }

    public function test_parses_sub_objects(): void
    {
        $filters = JobSearchFilters::fromArray([
            'StartDate' => ['Year' => 2026, 'Month' => 6, 'Day' => 9],
            'SearchLocations' => [['City' => 'Surrey', 'Postal' => 'v6b 1a1']],
        ]);

        $this->assertInstanceOf(DateField::class, $filters->StartDate);
        $this->assertSame('2026-06-09T00:00:00.000', (string) $filters->StartDate);

        $this->assertCount(1, $filters->SearchLocations);
        $this->assertInstanceOf(LocationField::class, $filters->SearchLocations[0]);
        // Postal is normalized: uppercased, spaces removed
        $this->assertSame('V6B1A1', $filters->SearchLocations[0]->getPostal());
    }

    public function test_round_trips_through_array(): void
    {
        $filters = JobSearchFilters::fromArray([
            'Page' => 3,
            'PageSize' => 10,
            'SortOrder' => 11,
            'Keyword' => 'baker',
            'SearchInField' => 'title',
            'SearchIndustry' => [52, 62],
            'SearchJobEducationLevel' => ['University'],
            'SearchJobTypeFullTime' => true,
        ]);

        $rebuilt = JobSearchFilters::fromArray($filters->toArray());

        $this->assertSame(3, $rebuilt->Page);
        $this->assertSame(11, $rebuilt->SortOrder);
        $this->assertSame('baker', $rebuilt->Keyword);
        $this->assertSame([52, 62], $rebuilt->SearchIndustry);
        $this->assertSame(['University'], $rebuilt->SearchJobEducationLevel);
        $this->assertTrue($rebuilt->SearchJobTypeFullTime);
    }

    public function test_cast_reads_stored_json_using_version_column(): void
    {
        $cast = new JobSearchFiltersCast;
        $model = new class extends Model {};

        $json = json_encode(['Keyword' => 'cook', 'Page' => 4]);

        $filters = $cast->get($model, 'JobSearchFilters', $json, ['JobSearchFiltersVersion' => 1]);

        $this->assertInstanceOf(JobSearchFilters::class, $filters);
        $this->assertSame('cook', $filters->Keyword);
        $this->assertSame(4, $filters->Page);
    }

    public function test_cast_rejects_unknown_field_from_stored_json(): void
    {
        $cast = new JobSearchFiltersCast;
        $model = new class extends Model {};

        $this->expectException(InvalidFilterException::class);
        $cast->get($model, 'JobSearchFilters', json_encode(['Bogus' => 1]), ['JobSearchFiltersVersion' => 0]);
    }

    public function test_cast_serializes_value_object_to_json(): void
    {
        $cast = new JobSearchFiltersCast;
        $model = new class extends Model {};

        $filters = JobSearchFilters::fromArray(['Keyword' => 'nurse', 'Page' => 2]);
        $stored = $cast->set($model, 'JobSearchFilters', $filters, []);

        $decoded = json_decode($stored['JobSearchFilters'], true);
        $this->assertSame('nurse', $decoded['Keyword']);
        $this->assertSame(2, $decoded['Page']);
    }
}
