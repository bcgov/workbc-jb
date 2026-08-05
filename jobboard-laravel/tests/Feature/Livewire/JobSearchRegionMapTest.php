<?php

namespace Tests\Feature\Livewire;

use App\Livewire\JobSearch;
use Livewire\Livewire;
use Mockery;
use OpenSearch\Client;
use Tests\TestCase;

/**
 * SRCH-12 — the BC region-map selector. Toggling a region commits/removes it as a
 * Region location; it flows through the existing location → Region.keyword pipeline
 * (no new backend). Unknown region names are rejected.
 */
class JobSearchRegionMapTest extends TestCase
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

    public function test_toggling_a_region_commits_then_removes_it(): void
    {
        Livewire::test(JobSearch::class)
            ->call('toggleRegion', 'Cariboo')
            ->assertSet('locations', [['Region' => 'Cariboo']])
            ->call('toggleRegion', 'Cariboo')
            ->assertSet('locations', []);
    }

    public function test_unknown_region_names_are_ignored(): void
    {
        Livewire::test(JobSearch::class)
            ->call('toggleRegion', 'Atlantis')
            ->assertSet('locations', []);
    }

    public function test_all_seven_regions_render_as_labelled_buttons(): void
    {
        $component = Livewire::test(JobSearch::class);

        foreach (JobSearch::regionOptions() as $name) {
            // Blade escapes the attribute value (e.g. & → &amp;), so compare escaped.
            $component->assertSeeHtml('aria-label="'.e($name).'"');
        }

        // Selecting a region surfaces it as a removable chip (SRCH-11 strip).
        $component->call('toggleRegion', 'Vancouver Island / Coast')
            ->assertSee('Remove filter Vancouver Island / Coast');
    }

    public function test_a_selected_region_becomes_a_region_keyword_term(): void
    {
        Livewire::test(JobSearch::class)->call('toggleRegion', 'Kootenay');

        $json = json_encode($this->bodies[array_key_last($this->bodies)]);
        $this->assertStringContainsString('"Region.keyword":"Kootenay"', $json);
    }
}
