<?php

namespace Tests\Unit\Search;

use App\Search\Filters\JobSearchFilters;
use App\Search\Queries\JobSearchQuery;
use PHPUnit\Framework\TestCase;

/**
 * SRCH-9 map query-body tests: the map path reuses the SAME faceted filters as
 * the paged search (SRCH-1..6) but returns only the pin fields, caps at 5000,
 * and additionally requires an indexed LocationGeo — a faithful port of
 * Resources/jobsearch_googlemap.json.
 */
class JobSearchMapQueryTest extends TestCase
{
    /**
     * @param  array<string, mixed>  $overrides
     * @return array<string, mixed>
     */
    private function map(array $overrides = []): array
    {
        return (new JobSearchQuery(JobSearchFilters::fromArray($overrides)))->buildMapQuery();
    }

    public function test_map_query_caps_size_and_projects_only_pin_fields(): void
    {
        $body = $this->map();

        $this->assertSame(5000, $body['size']);
        $this->assertSame(['Location', 'JobId', 'City', 'Region', 'Title'], $body['_source']);
        // The map is not paged — no "from".
        $this->assertArrayNotHasKey('from', $body);
    }

    public function test_map_query_requires_active_jobs_and_a_location(): void
    {
        $body = $this->map();

        $filterMust = $body['query']['bool']['filter']['bool']['must'];

        $this->assertContains(
            ['range' => ['ExpireDate' => ['gte' => 'now/d', 'time_zone' => 'America/Vancouver']]],
            $filterMust,
        );
        $this->assertContains(['exists' => ['field' => 'LocationGeo']], $filterMust);
    }

    public function test_map_query_reuses_the_same_facet_filters(): void
    {
        // A keyword + an education facet must appear in the map query's must
        // clauses exactly as they would in the paged search.
        $body = $this->map([
            'Keyword' => 'welder',
            'SearchJobEducationLevel' => ['High school'],
            'SearchExcludePlacementAgencyJobs' => true,
        ]);

        $must = $body['query']['bool']['must'];
        $encoded = json_encode($must);

        $this->assertStringContainsString('welder', $encoded);
        $this->assertStringContainsString('EduLevel.keyword', $encoded);

        // The placement-agency exclusion is carried through to must_not.
        $this->assertContains(
            ['term' => ['EmployerTypeId' => 1]],
            $body['query']['bool']['must_not'],
        );
    }
}
