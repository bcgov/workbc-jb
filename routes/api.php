<?php

use App\Http\Controllers\Api\LocationApiController;
use App\Http\Controllers\Api\SearchApiController;
use Illuminate\Support\Facades\Route;

/*
 * SRCH-10 — the Drupal-facing search API (docs/contracts.md §2). Consumed
 * server-to-server by the Drupal `workbc_jobboard` module, which reads these
 * paths/keys literally — segment casing below is part of the contract and
 * MUST NOT change without a version bump + ADR (contracts.md §3).
 *
 * These three are the ONLY genuine server-to-server endpoints. The career /
 * industry profile routes are NOT here: they are called from the browser and
 * authenticate with the seeker's session, so they live in web.php to pick up
 * the session/cookie/CSRF middleware (ACCT-6, ADR-009).
 */

// §2.1 Job search. The optional {language} segment selects jobs_fr (e.g. "fr");
// anything else (including omitted) uses the English index.
Route::post('Search/JobSearch/{language?}', [SearchApiController::class, 'jobSearch'])
    ->where('language', '[A-Za-z]{2}');

// §2.2 Total active job count.
Route::get('Search/gettotaljobs/{language?}', [SearchApiController::class, 'totalJobs'])
    ->where('language', '[A-Za-z]{2}');

// §2.3 City autocomplete — {includeRegion} is a fixed boolean-like segment in
// the existing Drupal contract; the current suggestion list (SRCH-2) doesn't
// vary by it, so it's accepted but unused.
Route::get('location/cities/{cityName}/{includeRegion}', [LocationApiController::class, 'cities'])
    ->where('includeRegion', '[A-Za-z]+');
