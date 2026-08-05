<?php

namespace Tests\Feature\Ui;

use Tests\TestCase;

class UiKitPageTest extends TestCase
{
    public function test_ui_kit_page_renders_the_accessible_base_layout(): void
    {
        $response = $this->get('/ui');

        $response->assertOk();

        // Base layout landmarks and skip link.
        $response->assertSee('Skip to main content');
        $response->assertSee('id="main"', false);
        $response->assertSee('aria-label="Primary"', false);
        $response->assertSee('<header', false);
        $response->assertSee('<footer', false);
        $response->assertSee('<html lang="', false);
        $response->assertSee('name="viewport"', false);
    }

    public function test_ui_kit_page_shows_each_component(): void
    {
        $response = $this->get('/ui');

        // Form field label association.
        $response->assertSee('<label for="full_name"', false);
        // Alert roles.
        $response->assertSee('role="alert"', false);
        $response->assertSee('role="status"', false);
        // Alpine view-state toggle.
        $response->assertSee('aria-controls="disclosure-panel"', false);
        // Pagination landmark.
        $response->assertSee('aria-label="Pagination"', false);
        // Livewire component present.
        $response->assertSee('Count:');
    }
}
