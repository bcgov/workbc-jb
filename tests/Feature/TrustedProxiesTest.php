<?php

namespace Tests\Feature;

use Tests\TestCase;

/**
 * Guards the CloudFront/ALB proxy contract (docs/integration/drupal-embed.md,
 * ADR-009). CloudFront sends the ORIGIN hostname in `Host` and the real public
 * hostname in `X-Forwarded-Host`, so absolute URLs must be built from the
 * forwarded headers — otherwise the SRCH-7 canonical links and every SRCH-8
 * sitemap URL would leak the internal origin hostname to crawlers.
 *
 * The sitemap index is used as the probe because it emits absolute URLs from
 * route() without needing an OpenSearch round-trip.
 */
class TrustedProxiesTest extends TestCase
{
    private const FORWARDED = [
        'X-Forwarded-Host' => 'api-jobboard.workbc.ca',
        'X-Forwarded-Proto' => 'https',
    ];

    public function test_absolute_urls_use_the_forwarded_host_not_the_origin_host(): void
    {
        $response = $this->get('/sitemap.xml', self::FORWARDED);

        $response->assertOk();
        $response->assertSee('https://api-jobboard.workbc.ca/sitemap-en.xml', escape: false);
        $response->assertSee('https://api-jobboard.workbc.ca/sitemap-fr.xml', escape: false);
    }

    public function test_the_forwarded_proto_makes_generated_urls_https(): void
    {
        $this->get('/sitemap.xml', self::FORWARDED)
            ->assertOk()
            // A trusted X-Forwarded-Proto must not leave http:// URLs behind a
            // TLS-terminating CDN (mixed content + wrong canonical).
            ->assertDontSee('http://api-jobboard.workbc.ca', escape: false);
    }

    public function test_without_forwarded_headers_the_request_host_is_still_used(): void
    {
        // Local/dev and container health checks reach the app directly.
        $this->get('/sitemap.xml')
            ->assertOk()
            ->assertSee('/sitemap-en.xml', escape: false);
    }
}
