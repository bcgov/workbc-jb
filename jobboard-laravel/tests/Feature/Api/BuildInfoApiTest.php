<?php

namespace Tests\Feature\Api;

use Tests\TestCase;

/**
 * GET /api/SystemSettings/BuildInfo (contracts.md §2.5).
 *
 * This endpoint gates the entire Drupal integration: workbc_jobboard.module's
 * jbTestConnection() calls it before rendering the Find Jobs and Account pages
 * and, on any failure, replaces the job-board region with "The Job Board is
 * currently unavailable." These tests exist so nobody deletes it as an unused
 * debug route.
 */
class BuildInfoApiTest extends TestCase
{
    public function test_it_returns_build_provenance_in_dotnet_key_casing(): void
    {
        config([
            'build.sha' => 'abc123',
            'build.run_number' => '42',
            'build.build_date' => '2026-08-05T00:00:00Z',
        ]);

        $response = $this->getJson('/api/SystemSettings/BuildInfo');

        $response->assertOk();
        $response->assertExactJson([
            'SHA' => 'abc123',
            'RunNumber' => '42',
            'BuildDate' => '2026-08-05T00:00:00Z',
        ]);
    }

    /**
     * Drupal's probe is only `!empty($response)` on a Guzzle GET, so what
     * actually matters is that the route exists and returns 2xx even when the
     * build args were never supplied (local builds, `docker build` with no
     * --build-arg). An exception here would take down the Drupal pages.
     */
    public function test_it_still_answers_when_build_metadata_is_absent(): void
    {
        config([
            'build.sha' => null,
            'build.run_number' => null,
            'build.build_date' => null,
        ]);

        $response = $this->getJson('/api/SystemSettings/BuildInfo');

        $response->assertOk();
        $this->assertSame(['SHA', 'RunNumber', 'BuildDate'], array_keys($response->json()));
    }

    /**
     * The path Drupal hardcodes. If a future refactor moves or renames this,
     * this assertion is the thing that should fail.
     */
    public function test_the_exact_path_drupal_probes_is_registered(): void
    {
        $this->get('/api/SystemSettings/BuildInfo')->assertOk();
    }
}
