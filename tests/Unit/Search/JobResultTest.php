<?php

namespace Tests\Unit\Search;

use App\Search\Results\JobResult;
use App\Search\Results\SearchResult;
use PHPUnit\Framework\TestCase;

/**
 * Result-projection tests (docs/contracts.md §2.1): index docs are mapped to the
 * API response shape — City CSV string, Region/Location arrays, zero-padded
 * Noc2021 string, empty/null fields omitted, and federal vs external key sets.
 */
class JobResultTest extends TestCase
{
    /**
     * @return array<string, mixed>
     */
    private function federalSource(): array
    {
        return [
            'JobId' => '40000001',
            'IsFederalJob' => true,
            'Title' => 'Cooks',
            'EmployerName' => 'Acme Restaurants',
            'DatePosted' => '2026-04-14T21:40:00+00:00',
            'ExpireDate' => '2026-06-01T00:00:00+00:00',
            'City' => ['Vancouver'],
            'Province' => 'BC',
            'Region' => ['Mainland / Southwest'],
            'Location' => [['Lat' => '49.2827', 'Lon' => '-123.1207']],
            'Noc2021' => 63200.0,
            'NocGroup' => 'Cooks (63200)',
            'Salary' => 50960.0,
            'SalarySummary' => '$50,960 annually',
            'HoursOfWork' => ['Description' => ['Full-time']],
            'WorkplaceType' => ['Id' => 0, 'Description' => 'On-site only'],
            'WorkLangCd' => ['Description' => ['English']],
            'WageClass' => 'A',
            'SalaryConditions' => ['Description' => []],
        ];
    }

    /**
     * @return array<string, mixed>
     */
    private function externalSource(): array
    {
        return [
            'JobId' => 'cmnze80pj264pr8t2pr0mfujk',
            'IsFederalJob' => false,
            'Title' => 'Senior Analyst',
            'EmployerName' => 'Hiive',
            'DatePosted' => '2026-06-09T13:58:07',
            'ExpireDate' => '2026-09-07T13:58:07',
            'City' => ['Vancouver'],
            'Province' => 'British Columbia',
            'Region' => ['Mainland / Southwest'],
            'Noc2021' => 62100.0,
            'Salary' => 110000.0,
            'JobDescription' => "We are hiring.\nApply now.",
            'ExternalSource' => ['Source' => [['Url' => 'https://jobs.ashbyhq.com/x', 'Source' => 'jobs.ashbyhq.com']]],
            'ApplyWebsite' => 'https://jobs.ashbyhq.com/x',
        ];
    }

    public function test_federal_projection(): void
    {
        $result = JobResult::fromSource($this->federalSource())->toArray();

        // City is a CSV string, not an array
        $this->assertSame('Vancouver', $result['City']);
        // Region and Location stay arrays
        $this->assertSame(['Mainland / Southwest'], $result['Region']);
        // Noc2021 is a zero-padded 5-char string; no Noc (2016) field
        $this->assertSame('63200', $result['Noc2021']);
        $this->assertArrayNotHasKey('Noc', $result);
        $this->assertTrue($result['IsFederalJob']);
        $this->assertSame(['Description' => ['English']], $result['WorkLangCd']);
        $this->assertSame('A', $result['WageClass']);

        // federal jobs carry no external-only keys
        $this->assertArrayNotHasKey('ExternalSource', $result);
        $this->assertArrayNotHasKey('ApplyWebsite', $result);
        $this->assertArrayNotHasKey('JobDescription', $result);
        // empty Description wrapper is omitted
        $this->assertArrayNotHasKey('SalaryConditions', $result);
    }

    public function test_external_projection(): void
    {
        $result = JobResult::fromSource($this->externalSource())->toArray();

        $this->assertFalse($result['IsFederalJob']);
        $this->assertSame('British Columbia', $result['Province']);
        $this->assertArrayHasKey('ExternalSource', $result);
        $this->assertSame('https://jobs.ashbyhq.com/x', $result['ApplyWebsite']);
        $this->assertArrayHasKey('JobDescription', $result);

        // external jobs carry no federal-only keys
        $this->assertArrayNotHasKey('WorkLangCd', $result);
        $this->assertArrayNotHasKey('WageClass', $result);
    }

    public function test_noc_2021_is_zero_padded(): void
    {
        $result = JobResult::fromSource(['Noc2021' => 10.0])->toArray();
        $this->assertSame('00010', $result['Noc2021']);
    }

    public function test_city_array_joined_as_csv(): void
    {
        $result = JobResult::fromSource(['City' => ['Vancouver', 'Surrey']])->toArray();
        $this->assertSame('Vancouver, Surrey', $result['City']);
    }

    public function test_location_points_are_strings(): void
    {
        $result = JobResult::fromSource([
            'Location' => [['Lat' => 49.28, 'Lon' => -123.12]],
        ])->toArray();

        $this->assertSame([['Lat' => '49.28', 'Lon' => '-123.12']], $result['Location']);
    }

    public function test_search_result_wrapper_casing(): void
    {
        $response = [
            'hits' => [
                'total' => ['value' => 2, 'relation' => 'eq'],
                'hits' => [
                    ['_source' => $this->externalSource()],
                    ['_source' => $this->federalSource()],
                ],
            ],
        ];

        $wrapper = SearchResult::fromOpenSearchResponse($response, 1, 20)->toArray();

        // wrapper keys are camelCase; item keys stay PascalCase
        $this->assertSame(2, $wrapper['count']);
        $this->assertSame(1, $wrapper['pageNumber']);
        $this->assertSame(20, $wrapper['pageSize']);
        $this->assertCount(2, $wrapper['result']);
        $this->assertArrayHasKey('JobId', $wrapper['result'][0]);
    }
}
