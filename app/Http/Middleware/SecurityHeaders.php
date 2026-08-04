<?php

namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;
use Symfony\Component\HttpFoundation\Response;

/**
 * FND-4 — security response headers.
 *
 * Before this, the app set **no** framing headers and neither does CloudFront
 * (verified 2026-07-29, ADR-009), so any site could frame our authenticated
 * pages — clickjacking against saved-jobs/alerts/settings actions.
 *
 * `frame-ancestors` is used rather than `X-Frame-Options`: the latter is
 * all-or-nothing (`DENY`/`SAMEORIGIN`) and would **break the ADR-006 embed**,
 * since Drupal frames us from a different origin. CSP lets us name exactly which
 * parents are allowed. We deliberately never emit `X-Frame-Options` at all —
 * where both are present browsers may honour the stricter one.
 *
 * Origins come from `config/embed.php` (per-environment). With none configured
 * the policy falls back to `frame-ancestors 'none'`, so a misconfigured
 * environment fails closed — unframeable — rather than open to everyone.
 */
final class SecurityHeaders
{
    public function handle(Request $request, Closure $next): Response
    {
        $response = $next($request);

        /** @var list<string> $origins */
        $origins = config('embed.parent_origins', []);

        $frameAncestors = $origins === []
            ? "'none'"
            : implode(' ', $origins);

        $response->headers->set('Content-Security-Policy', "frame-ancestors {$frameAncestors}");

        // Stop browsers second-guessing declared content types (MIME sniffing
        // can turn a user-supplied upload into executable script).
        $response->headers->set('X-Content-Type-Options', 'nosniff');

        // Send the origin but not the path when leaving for another site, so
        // job-search URLs (which carry the seeker's filters) don't leak to
        // employer sites via the Referer header on "apply" links.
        $response->headers->set('Referrer-Policy', 'strict-origin-when-cross-origin');

        // NOTE: X-Frame-Options is intentionally NOT set — see the class docblock.

        return $response;
    }
}
