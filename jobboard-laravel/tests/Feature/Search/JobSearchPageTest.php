<?php

namespace Tests\Feature\Search;

use Livewire\Livewire;
use Mockery;
use OpenSearch\Client;
use Tests\TestCase;

/**
 * SRCH-1 page test: the search results page is reachable anonymously (no
 * authenticated session) and renders results into the INITIAL HTML
 * (server-rendered / crawlable), not injected client-side.
 */
class JobSearchPageTest extends TestCase
{
    protected function setUp(): void
    {
        parent::setUp();

        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')->andReturn([
            'hits' => [
                'total' => ['value' => 1],
                'hits' => [
                    ['_source' => ['JobId' => '777', 'Title' => 'Crawlable Job', 'EmployerName' => 'SEO Inc', 'City' => ['Kelowna'], 'DatePosted' => '2026-06-10T00:00:00']],
                ],
            ],
        ]);

        $this->app->instance(Client::class, $client);
    }

    protected function tearDown(): void
    {
        Mockery::close();
        parent::tearDown();
    }

    public function test_page_is_reachable_anonymously(): void
    {
        // No actingAs — anonymous visitor.
        $this->get('/jobs')->assertOk();
    }

    public function test_results_are_present_in_the_initial_html(): void
    {
        $response = $this->get('/jobs');

        $response->assertOk();
        // Rendered server-side into the document, with a crawlable path-based link.
        $response->assertSee('Crawlable Job');
        $response->assertSee('SEO Inc');
        $response->assertSee('href="'.route('jobs.show', ['job' => \App\Support\JobSlug::path('777', 'Crawlable Job')]).'"', false);
        // a11y: results live region and search landmark present in markup.
        $response->assertSee('aria-label="Search results"', false);
        $response->assertSee('role="search"', false);
    }

    public function test_bare_id_detail_url_redirects_to_the_canonical_slug(): void
    {
        // SRCH-1 links (and any missing/incorrect slug) 301 to one canonical URL.
        $this->get('/jobs/777')
            ->assertRedirect(route('jobs.show', ['job' => \App\Support\JobSlug::path('777', 'Crawlable Job')]));
    }
}
