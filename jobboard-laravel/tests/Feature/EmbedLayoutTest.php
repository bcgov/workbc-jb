<?php

namespace Tests\Feature;

use Tests\TestCase;

/**
 * FND-4 / ADR-006 — the chrome-less embed layout, the content-height bridge,
 * and the security response headers.
 */
class EmbedLayoutTest extends TestCase
{
    private const PARENT = 'https://dev2.workbc.ca';

    // --- Embed layout --------------------------------------------------------

    public function test_the_normal_page_renders_the_full_site_chrome(): void
    {
        $this->get('/jobs')
            ->assertOk()
            ->assertSee('Skip to main content')
            ->assertSee('aria-label="Primary"', escape: false)
            ->assertSee('Province of British Columbia');
    }

    public function test_embed_mode_strips_the_header_nav_and_footer(): void
    {
        $response = $this->get('/jobs?embed=1')->assertOk();

        // Drupal's host page renders all of this; emitting ours too would stack
        // two sets of site chrome inside one page.
        $response->assertDontSee('aria-label="Primary"', escape: false);
        $response->assertDontSee('Province of British Columbia');
        $response->assertDontSee('Skip to main content');
    }

    public function test_embed_mode_still_renders_the_search_itself(): void
    {
        // Stripping chrome must not strip the thing we are embedding.
        $this->get('/jobs?embed=1')
            ->assertOk()
            ->assertSee('id="main"', escape: false)
            ->assertSee('wire:id', escape: false);
    }

    // --- Height bridge -------------------------------------------------------

    public function test_the_height_bridge_posts_only_to_configured_origins(): void
    {
        config(['embed.parent_origins' => [self::PARENT]]);

        $response = $this->get('/jobs?embed=1')->assertOk();

        $response->assertSee('jobboard:height', escape: false);

        // Asserted on the host, not the full URL: @js() escapes the scheme's
        // slashes (`https:\/\/dev2.workbc.ca`), so the literal origin string
        // never appears verbatim even though the JS parses to the right value.
        $response->assertSee('dev2.workbc.ca', escape: false);

        // Never a wildcard target — that would leak page dimensions to any site
        // that framed us. The origins are always passed as a variable.
        $response->assertDontSee(", '*')", escape: false);
        $response->assertDontSee(', "*")', escape: false);
    }

    public function test_no_height_bridge_is_emitted_when_no_parent_is_configured(): void
    {
        config(['embed.parent_origins' => []]);

        $this->get('/jobs?embed=1')
            ->assertOk()
            ->assertDontSee('jobboard:height', escape: false);
    }

    // --- Security headers ----------------------------------------------------

    public function test_frame_ancestors_names_the_configured_parents(): void
    {
        config(['embed.parent_origins' => [self::PARENT]]);

        $this->get('/jobs')
            ->assertOk()
            ->assertHeader('Content-Security-Policy', 'frame-ancestors '.self::PARENT);
    }

    public function test_frame_ancestors_fails_closed_when_nothing_is_configured(): void
    {
        config(['embed.parent_origins' => []]);

        // A misconfigured environment must be unframeable, not framable by anyone.
        $this->get('/jobs')
            ->assertOk()
            ->assertHeader('Content-Security-Policy', "frame-ancestors 'none'");
    }

    public function test_x_frame_options_is_never_sent(): void
    {
        config(['embed.parent_origins' => [self::PARENT]]);

        // X-Frame-Options is all-or-nothing and would break the ADR-006 embed;
        // where both headers are present browsers may honour the stricter one.
        $this->get('/jobs?embed=1')
            ->assertOk()
            ->assertHeaderMissing('X-Frame-Options');
    }

    public function test_the_other_security_headers_are_present(): void
    {
        $this->get('/jobs')
            ->assertOk()
            ->assertHeader('X-Content-Type-Options', 'nosniff')
            ->assertHeader('Referrer-Policy', 'strict-origin-when-cross-origin');
    }

    public function test_headers_apply_to_non_search_pages_too(): void
    {
        $this->get('/')
            ->assertOk()
            ->assertHeader('X-Content-Type-Options', 'nosniff');
    }
}
