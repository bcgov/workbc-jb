<?php

use App\Http\Controllers\Api\CareerProfileApiController;
use App\Http\Controllers\Api\LocationApiController;
use App\Http\Controllers\Api\SearchApiController;
use App\Http\Middleware\EnsureJobSeekerToken;
use Illuminate\Support\Facades\Route;

/*
 * SRCH-10 — the Drupal-facing search API (docs/contracts.md §2). Consumed
 * server-to-server by the Drupal `workbc_jobboard` module, which reads these
 * paths/keys literally — segment casing below is part of the contract and
 * MUST NOT change without a version bump + ADR (contracts.md §3).
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

// §2.4 Career-profile save/status — authenticated: Drupal forwards the job
// seeker's Authorization header. EnsureJobSeekerToken only enforces the
// routing/auth CONTRACT for now (a bearer token must be present); see its
// docblock and the controller for the EPIC-ACCOUNT follow-up.
Route::middleware(EnsureJobSeekerToken::class)->group(function (): void {
    Route::post('career-profiles/save/{profileId}', [CareerProfileApiController::class, 'save']);
    Route::get('career-profiles/status/{profileId}', [CareerProfileApiController::class, 'status']);
});
