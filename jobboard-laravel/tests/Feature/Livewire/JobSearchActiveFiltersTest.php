<?php

namespace Tests\Feature\Livewire;

use App\Livewire\JobSearch;
use Livewire\Livewire;
use Mockery;
use OpenSearch\Client;
use Tests\TestCase;

/**
 * SRCH-11 — the active-filter chips strip. Every applied facet value renders as a
 * removable chip (parity with the old lib-search-criteries); removing one clears
 * just that value and returns to the first page, and keyword/scope are excluded.
 */
class JobSearchActiveFiltersTest extends TestCase
{
    protected function setUp(): void
    {
        parent::setUp();

        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')->andReturn(['hits' => ['total' => ['value' => 0], 'hits' => []]]);
        $this->app->instance(Client::class, $client);
    }

    protected function tearDown(): void
    {
        Mockery::close();
        parent::tearDown();
    }

    public function test_applied_filters_render_as_removable_chips(): void
    {
        Livewire::test(JobSearch::class)
            ->set('locations', [['City' => 'Victoria']])
            ->set('hours', ['FullTime'])
            ->set('industries', [35]) // Industries.Id 35 = Health care and social assistance
            ->set('salaryUnknown', true)
            ->assertSee('Filters:')
            // The aria-label text is unique to the chip (not the facet dropdowns).
            ->assertSee('Remove filter Victoria')
            ->assertSee('Remove filter Full-time')
            ->assertSee('Remove filter Health care and social assistance')
            ->assertSee('Includes no salary listed');
    }

    public function test_removing_a_multi_value_chip_clears_only_that_value_and_resets_page(): void
    {
        Livewire::test(JobSearch::class)
            ->set('page', 3)
            ->set('hours', ['FullTime', 'PartTime'])
            ->call('removeFilter', 'hours', 'FullTime')
            ->assertSet('hours', ['PartTime'])
            ->assertSet('page', 1);
    }

    public function test_removing_a_location_chip_uses_the_index(): void
    {
        Livewire::test(JobSearch::class)
            ->set('locations', [['City' => 'Victoria'], ['City' => 'Nanaimo']])
            ->call('removeFilter', 'location', '1')
            ->assertSet('locations', [['City' => 'Victoria']]);
    }

    public function test_single_value_facets_reset_to_their_default(): void
    {
        Livewire::test(JobSearch::class)
            ->set('jobSource', '3')
            ->set('excludePlacementAgency', true)
            ->set('nocCode', '12345')
            ->set('dateSelection', '1')
            ->call('removeFilter', 'jobSource', '')
            ->assertSet('jobSource', '0')
            ->call('removeFilter', 'excludeAgency', '')
            ->assertSet('excludePlacementAgency', false)
            ->call('removeFilter', 'noc', '')
            ->assertSet('nocCode', '')
            ->call('removeFilter', 'date', '')
            ->assertSet('dateSelection', '0');
    }

    public function test_keyword_and_scope_are_not_chips(): void
    {
        Livewire::test(JobSearch::class)
            ->set('keyword', 'developer')
            ->set('searchIn', 'title')
            ->assertDontSee('Remove filter developer')
            ->assertDontSee('Filters:');
    }
}
