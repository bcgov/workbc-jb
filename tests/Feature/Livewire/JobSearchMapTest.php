<?php

namespace Tests\Feature\Livewire;

use App\Livewire\JobSearch;
use Livewire\Livewire;
use Mockery;
use OpenSearch\Client;
use Tests\TestCase;

/**
 * SRCH-9 Livewire test: the map view is reachable from the list, renders pins
 * built from the same filters, wires the Maps key from config, and — for a11y —
 * keeps the list available as the full equivalent.
 */
class JobSearchMapTest extends TestCase
{
    /** @var array<int, array<string, mixed>> */
    private array $bodies = [];

    protected function setUp(): void
    {
        parent::setUp();

        $this->bodies = [];
        config()->set('services.google_maps.js_key', 'test-browser-key');

        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')->andReturnUsing(function (array $params): array {
            $this->bodies[] = $params['body'];

            // A map query (size 5000) returns pin docs; the paged search returns
            // the usual list projection.
            if (($params['body']['size'] ?? null) === 5000) {
                return [
                    'hits' => [
                        'total' => ['value' => 1],
                        'hits' => [
                            ['_source' => [
                                'JobId' => '900', 'Title' => 'Mapped Job',
                                'City' => ['Victoria'], 'Region' => ['Vancouver Island/Coast'],
                                'Location' => [['Lat' => '48.4', 'Lon' => '-123.3']],
                            ]],
                        ],
                    ],
                ];
            }

            return [
                'hits' => [
                    'total' => ['value' => 1],
                    'hits' => [
                        ['_source' => ['JobId' => '900', 'Title' => 'Mapped Job', 'EmployerName' => 'Acme', 'City' => ['Victoria']]],
                    ],
                ],
            ];
        });

        $this->app->instance(Client::class, $client);
    }

    protected function tearDown(): void
    {
        Mockery::close();
        parent::tearDown();
    }

    public function test_list_view_is_the_default_and_does_not_run_the_map_query(): void
    {
        Livewire::test(JobSearch::class)
            ->assertSet('view', 'list')
            ->assertSee('Mapped Job');

        // No map query (size 5000) was issued while in the list view.
        $this->assertEmpty(array_filter($this->bodies, static fn (array $b): bool => ($b['size'] ?? null) === 5000));
    }

    public function test_switching_to_map_runs_the_map_query_and_embeds_pins(): void
    {
        $component = Livewire::test(JobSearch::class)
            ->call('showMapView')
            ->assertSet('view', 'map');

        // The map query path ran.
        $this->assertNotEmpty(array_filter($this->bodies, static fn (array $b): bool => ($b['size'] ?? null) === 5000));

        // The pin (with its detail URL) is embedded for the browser component,
        // and the Maps key comes from config (never hardcoded).
        $html = $component->html();
        $this->assertStringContainsString('test-browser-key', $html);
        $this->assertStringContainsString('900', $html);
        $this->assertStringContainsString(route('jobs.show', ['job' => \App\Support\JobSlug::path('900', 'Mapped Job')]), $html);
    }

    public function test_map_view_keeps_the_list_available_for_accessibility(): void
    {
        Livewire::test(JobSearch::class)
            ->call('showMapView')
            // The List toggle remains present so the map is never the only way in.
            ->assertSeeHtml('wire:click="showListView"')
            ->assertSee('List');
    }

    public function test_can_return_to_the_list_view(): void
    {
        Livewire::test(JobSearch::class)
            ->call('showMapView')
            ->assertSet('view', 'map')
            ->call('showListView')
            ->assertSet('view', 'list')
            ->assertSee('Mapped Job');
    }

    public function test_a_tampered_view_value_falls_back_to_list(): void
    {
        Livewire::test(JobSearch::class)
            ->set('view', 'evil')
            ->assertSet('view', 'list');
    }
}
