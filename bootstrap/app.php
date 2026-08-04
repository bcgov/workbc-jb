<?php

use Illuminate\Foundation\Application;
use Illuminate\Foundation\Configuration\Exceptions;
use Illuminate\Foundation\Configuration\Middleware;
use Illuminate\Http\Request;

return Application::configure(basePath: dirname(__DIR__))
    ->withRouting(
        web: __DIR__.'/../routes/web.php',
        api: __DIR__.'/../routes/api.php',
        commands: __DIR__.'/../routes/console.php',
        health: '/up',
    )
    ->withMiddleware(function (Middleware $middleware): void {
        /*
         * The app is served behind CloudFront → AWS ALB (see
         * docs/integration/drupal-embed.md). CloudFront sends the ORIGIN's
         * hostname in `Host` and the real public hostname in a
         * `X-Forwarded-Host` custom origin header; the ALB adds
         * X-Forwarded-For/Proto/Port. Without trusting those, url()/route()
         * would build absolute URLs from the internal origin hostname —
         * corrupting the SRCH-7 canonical/hreflang links, every URL in the
         * SRCH-8 sitemap, and post-login redirects.
         *
         * TRUSTED_PROXIES defaults to '*' because the ingress IP set is
         * CloudFront's (large and changing). NOTE: the origin is also
         * reachable directly, so '*' means `X-Forwarded-Host` is spoofable by
         * anyone who can reach it — mitigate with TrustHosts in production
         * (see the ADR-009 "Host spoofing" note) rather than by narrowing
         * this list.
         */
        $middleware->trustProxies(
            at: env('TRUSTED_PROXIES', '*'),
            headers: Request::HEADER_X_FORWARDED_FOR
                | Request::HEADER_X_FORWARDED_HOST
                | Request::HEADER_X_FORWARDED_PORT
                | Request::HEADER_X_FORWARDED_PROTO,
        );

        /*
         * Host spoofing mitigation (ADR-009): when enabled, only accept
         * forwarded/request hosts matching the configured regex patterns.
         *
         * Intentionally inert by default — an empty value means "no
         * restriction" so local dev, test runs, and health checks are
         * unaffected until explicitly enabled per environment.
         */
        $trustedHostPatterns = array_values(array_filter(
            array_map('trim', explode(',', (string) env('TRUSTED_HOST_PATTERNS', ''))),
            static fn (string $pattern): bool => $pattern !== '',
        ));

        if ($trustedHostPatterns === []) {
            $trustedHostPatterns = ['^.+$'];
        }

        $middleware->trustHosts(at: $trustedHostPatterns);
    })
    ->withExceptions(function (Exceptions $exceptions): void {
        //
    })->create();
