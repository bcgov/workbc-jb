<?php

namespace Tests\Feature\Livewire;

use App\Livewire\JobSearch;
use Livewire\Livewire;
use Mockery;
use OpenSearch\Client;
use Tests\TestCase;

/**
 * SRCH-3 — standard filter facets. Setting the bound facet state produces the
 * expected FND-7 JobSearchQuery groups: OR within a facet (one bool/should with
 * several clauses), AND across facets (separate must groups). A fake OpenSearch
 * client captures each request body so the assertions are deterministic.
 */
class JobSearchFacetsTest extends TestCase
{
    /** @var array<int, array<string, mixed>> */
    private array $bodies = [];

    protected function setUp(): void
    {
        parent::setUp();

        $this->bodies = [];

        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')->andReturnUsing(function (array $params): array {
            $this->bodies[] = $params['body'];

            return ['hits' => ['total' => ['value' => 0], 'hits' => []]];
        });

        $this->app->instance(Client::class, $client);
    }

    protected function tearDown(): void
    {
        Mockery::close();
        parent::tearDown();
    }

    /**
     * @return array<string, mixed>
     */
    private function lastBody(): array
    {
        return $this->bodies[array_key_last($this->bodies)];
    }

    /**
     * The should[] clauses of the must-group that contains a term on $field, or null.
     *
     * @return array<int, array<string, mixed>>|null
     */
    private function shouldGroupFor(string $field): ?array
    {
        foreach ($this->lastBody()['query']['bool']['must'] as $group) {
            $should = $group['bool']['should'] ?? [];
            foreach ($should as $clause) {
                if (isset($clause['term'][$field])) {
                    return $should;
                }
            }
        }

        return null;
    }

    /**
     * @param  array<int, array<string, mixed>>  $should
     * @return array<int, mixed> the term values on $field within the group
     */
    private function termValues(array $should, string $field): array
    {
        $out = [];
        foreach ($should as $clause) {
            if (isset($clause['term'][$field])) {
                $out[] = $clause['term'][$field];
            }
        }

        return $out;
    }

    /**
     * The DatePosted range clause anywhere in the must groups, or null.
     *
     * @return array<string, mixed>|null
     */
    private function datePostedRange(): ?array
    {
        foreach ($this->lastBody()['query']['bool']['must'] as $group) {
            foreach ($group['bool']['should'] ?? [] as $clause) {
                if (isset($clause['range']['DatePosted'])) {
                    return $clause['range']['DatePosted'];
                }
            }
        }

        return null;
    }

    public function test_job_type_hours_map_to_hours_of_work_terms_or_within(): void
    {
        Livewire::test(JobSearch::class)
            ->set('hours', ['FullTime', 'PartTime']);

        $should = $this->shouldGroupFor('HoursOfWork.Description.keyword');
        $this->assertNotNull($should);
        $this->assertEqualsCanonicalizing(
            ['Full-time', 'Part-time'],
            $this->termValues($should, 'HoursOfWork.Description.keyword'),
        );
    }

    public function test_job_type_period_maps_to_period_of_employment_terms(): void
    {
        Livewire::test(JobSearch::class)
            ->set('period', ['Temporary', 'Seasonal']);

        $should = $this->shouldGroupFor('PeriodOfEmployment.Description.keyword');
        $this->assertNotNull($should);
        $this->assertEqualsCanonicalizing(
            ['Temporary', 'Seasonal'],
            $this->termValues($should, 'PeriodOfEmployment.Description.keyword'),
        );
    }

    public function test_job_type_terms_map_to_employment_terms(): void
    {
        Livewire::test(JobSearch::class)
            ->set('terms', ['Weekend']);

        $should = $this->shouldGroupFor('EmploymentTerms.Description.keyword');
        $this->assertNotNull($should);
        $this->assertSame(['Weekend'], $this->termValues($should, 'EmploymentTerms.Description.keyword'));
    }

    public function test_workplace_types_map_to_workplace_type_ids(): void
    {
        Livewire::test(JobSearch::class)
            ->set('workplace', ['OnSite', 'Virtual']);

        $should = $this->shouldGroupFor('WorkplaceType.Id');
        $this->assertNotNull($should);
        $this->assertEqualsCanonicalizing([0, 15141], $this->termValues($should, 'WorkplaceType.Id'));
    }

    public function test_industry_maps_to_naics_id_terms(): void
    {
        // Values arrive from checkboxes as strings; the component casts + whitelists.
        // Real Industries.Id values: 35 = Health care, 31 = Professional services.
        Livewire::test(JobSearch::class)
            ->set('industries', ['35', '31']);

        $should = $this->shouldGroupFor('NaicsId');
        $this->assertNotNull($should);
        $this->assertEqualsCanonicalizing([35, 31], $this->termValues($should, 'NaicsId'));
    }

    public function test_unknown_industry_id_is_discarded(): void
    {
        Livewire::test(JobSearch::class)
            ->set('industries', ['35', '999']);

        $should = $this->shouldGroupFor('NaicsId');
        $this->assertNotNull($should);
        $this->assertSame([35], $this->termValues($should, 'NaicsId'));
    }

    public function test_education_maps_to_edu_level_terms(): void
    {
        Livewire::test(JobSearch::class)
            ->set('educationLevels', ['University', 'No education']);

        $should = $this->shouldGroupFor('EduLevel.keyword');
        $this->assertNotNull($should);
        $this->assertEqualsCanonicalizing(
            ['University', 'No education'],
            $this->termValues($should, 'EduLevel.keyword'),
        );
    }

    public function test_facets_and_across_groups(): void
    {
        Livewire::test(JobSearch::class)
            ->set('hours', ['FullTime'])
            ->set('educationLevels', ['University']);

        // Two independent must groups → AND across facets.
        $this->assertNotNull($this->shouldGroupFor('HoursOfWork.Description.keyword'));
        $this->assertNotNull($this->shouldGroupFor('EduLevel.keyword'));
    }

    public function test_date_today_uses_start_of_day_in_vancouver(): void
    {
        Livewire::test(JobSearch::class)
            ->set('dateSelection', '1');

        $range = $this->datePostedRange();
        $this->assertSame('now/d', $range['gte']);
        $this->assertSame('now+1d/d', $range['lt']);
        $this->assertSame('America/Vancouver', $range['time_zone']);
    }

    public function test_date_past_three_days(): void
    {
        Livewire::test(JobSearch::class)
            ->set('dateSelection', '2');

        $range = $this->datePostedRange();
        $this->assertSame('now-3d/d', $range['gte']);
        $this->assertSame('now', $range['lte']);
        $this->assertSame('America/Vancouver', $range['time_zone']);
    }

    public function test_date_custom_range_is_end_of_day_inclusive_in_vancouver(): void
    {
        Livewire::test(JobSearch::class)
            ->set('dateSelection', '3')
            ->set('startDate', '2026-07-01')
            ->set('endDate', '2026-07-20');

        $range = $this->datePostedRange();
        $this->assertSame('2026-07-01T00:00:00.000', $range['gte']);
        $this->assertSame('2026-07-20T23:59:59.999', $range['lte']);
        $this->assertSame('America/Vancouver', $range['time_zone']);
    }

    public function test_clear_filters_resets_every_facet(): void
    {
        Livewire::test(JobSearch::class)
            ->set('hours', ['FullTime'])
            ->set('industries', ['35'])
            ->set('educationLevels', ['University'])
            ->set('dateSelection', '2')
            ->call('clearFilters')
            ->assertSet('hours', [])
            ->assertSet('period', [])
            ->assertSet('terms', [])
            ->assertSet('workplace', [])
            ->assertSet('industries', [])
            ->assertSet('educationLevels', [])
            ->assertSet('dateSelection', '0')
            ->assertSet('startDate', '')
            ->assertSet('endDate', '');

        // With no facets the only remaining must clause is the keyword-less base;
        // there must be no DatePosted range or facet term groups.
        $this->assertNull($this->datePostedRange());
    }
}
