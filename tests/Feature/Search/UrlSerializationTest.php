<?php

namespace Tests\Feature\Search;

use Mockery;
use OpenSearch\Client;
use Tests\TestCase;

/**
 * SRCH-6 — the URL contract end to end: legacy alert deep-links redirect to the
 * canonical `/jobs?…` URL, and loading a shared/deep-linked URL reconstructs the
 * facet state on the search page (surfaced through the server-rendered
 * "copy search link", which mirrors the reconstructed filters).
 */
class UrlSerializationTest extends TestCase
{
    protected function setUp(): void
    {
        parent::setUp();

        // Deterministic empty result so the search page renders without a live index.
        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')->andReturn([
            'hits' => ['total' => ['value' => 0], 'hits' => []],
        ]);
        $this->app->instance(Client::class, $client);
    }

    protected function tearDown(): void
    {
        Mockery::close();
        parent::tearDown();
    }

    public function test_legacy_matrix_blob_redirects_to_canonical_url(): void
    {
        $blob = ';search=nurse;city=Victoria;noc=31301;jobsource=3';

        $response = $this->get('/job-search?p=' . urlencode($blob));

        $response->assertStatus(302);
        $location = $response->headers->get('Location');
        $this->assertStringContainsString('/jobs?', $location);
        $this->assertStringContainsString('q=nurse', $location);
        $this->assertStringContainsString('source=3', $location);
        $this->assertStringContainsString('noc=31301', $location);
    }

    public function test_legacy_query_params_redirect_to_canonical_url(): void
    {
        $response = $this->get('/job-search?search=welder&jobsource=1');

        $response->assertStatus(302);
        $location = $response->headers->get('Location');
        $this->assertStringContainsString('q=welder', $location);
        $this->assertStringContainsString('source=1', $location);
    }

    public function test_hashless_request_serves_the_forwarding_shim(): void
    {
        $response = $this->get('/job-search');

        $response->assertStatus(200);
        // The client-side shim reads the URL hash and continues to the search page.
        $response->assertSee(route('jobs.index'), false);
    }

    public function test_loading_a_deep_link_reconstructs_the_filters(): void
    {
        // loc/source/noc/salary are reconstructed in mount(); the server-rendered
        // "copy search link" re-encodes the live filters, so it must echo them back.
        $response = $this->get('/jobs?loc=c:Victoria&source=3&noc=31301&salary=2');

        $response->assertStatus(200);
        $response->assertSee('Victoria');       // committed location chip
        $response->assertSee('source=3', false); // canonical share URL reflects the state
        $response->assertSee('noc=31301', false);
        $response->assertSee('salary=2', false);
    }
}
