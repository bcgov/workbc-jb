<?php

namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;
use Symfony\Component\HttpFoundation\Response;

/**
 * ACCT-6 / ADR-009 — session gate for the browser-called profile endpoints.
 *
 * Replaces `EnsureJobSeekerToken`, which required a bearer token because
 * `contracts.md §2.4` described Drupal forwarding an `Authorization` header. That
 * mechanism never existed: the calls come from the page, not from Drupal's PHP.
 *
 * Deliberately not `auth:web`. Laravel's `Authenticate` middleware only returns
 * 401 when the request `expectsJson()`, and Drupal's JS sends a wildcard `Accept`
 * header — so `auth:web` would answer an anonymous call with a **302 to the login
 * page**, and the caller's `fetch` would follow it and read the login HTML as
 * success. The client contract is "branch on 401", so 401 is what we return.
 */
final class EnsureJobSeekerSession
{
    public function handle(Request $request, Closure $next): Response
    {
        if (! Auth::guard('web')->check()) {
            return response()->json(['message' => 'Unauthenticated.'], 401);
        }

        return $next($request);
    }
}
