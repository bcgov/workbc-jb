<?php

namespace Tests\Feature\Api;

use Tests\TestCase;

/**
 * SRCH-10 — POST /api/career-profiles/save/{profileId} and
 * GET /api/career-profiles/status/{profileId} (contracts.md §2.4): the
 * routing/auth contract (Drupal forwards the job seeker's Authorization
 * header) — persistence itself is a documented EPIC-ACCOUNT follow-up.
 */
class CareerProfileApiTest extends TestCase
{
    public function test_save_without_an_authorization_header_is_rejected(): void
    {
        $this->postJson('/api/career-profiles/save/11101')->assertStatus(401);
    }

    public function test_status_without_an_authorization_header_is_rejected(): void
    {
        $this->getJson('/api/career-profiles/status/11101')->assertStatus(401);
    }

    public function test_save_with_a_bearer_token_is_accepted(): void
    {
        $response = $this->withHeader('Authorization', 'Bearer test-token')
            ->postJson('/api/career-profiles/save/11101');

        $response->assertOk();
        $this->assertSame('true', $response->getContent());
    }

    public function test_status_with_a_bearer_token_is_accepted(): void
    {
        $response = $this->withHeader('Authorization', 'Bearer test-token')
            ->getJson('/api/career-profiles/status/11101');

        $response->assertOk();
        $this->assertSame('false', $response->getContent());
    }

    public function test_an_empty_bearer_token_is_rejected(): void
    {
        $this->withHeader('Authorization', 'Bearer ')
            ->postJson('/api/career-profiles/save/11101')
            ->assertStatus(401);
    }
}
