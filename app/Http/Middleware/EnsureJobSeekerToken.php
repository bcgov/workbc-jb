<?php

namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;
use Symfony\Component\HttpFoundation\Response;

/**
 * SRCH-10 career-profile endpoints (docs/contracts.md §2.4) are authenticated:
 * Drupal forwards the job seeker's `Authorization` header. The job-seeker
 * account epic (EPIC-ACCOUNT — session auth, ADR-003) isn't built yet, so this
 * middleware only enforces the ROUTING/AUTH CONTRACT — a bearer token must be
 * present — and rejects anonymous calls with 401. It deliberately does NOT
 * verify the token's signature/claims or resolve a job seeker yet.
 *
 * TODO(EPIC-ACCOUNT): once job-seeker auth exists, replace this with real
 * verification (or bridge the forwarded token to the authenticated session)
 * and resolve the acting AspNetUserId for the controller.
 */
final class EnsureJobSeekerToken
{
    public function handle(Request $request, Closure $next): Response
    {
        $header = $request->header('Authorization', '');

        if (! str_starts_with($header, 'Bearer ') || trim(substr($header, 7)) === '') {
            return response()->json(['message' => 'Unauthenticated.'], 401);
        }

        return $next($request);
    }
}
