<?php

use App\Http\Controllers\Api\LocationApiController;
use App\Http\Controllers\Api\SearchApiController;
use Illuminate\Support\Facades\Route;

/*
 * SRCH-10 — the Drupal-facing search API (docs/contracts.md §2). Consumed
 * server-to-server by the Drupal `workbc_jobboard` module, which reads these
 * paths/keys literally.
 *
 * CASING IS LOAD-BEARING, AND MORE THAN ONE CASING IS LIVE.
 * ASP.NET Core matches routes case-INSENSITIVELY, so WorkBC.Web answered
 * `GetTotalJobs`, `gettotaljobs` and anything between with the same action, and
 * callers settled on whichever they felt like — src/scripts/test/cases.txt uses
 * `/api/Search/GetTotalJobs` and `/api/Location/cities/...`, while our own
 * contracts.md had recorded the all-lowercase forms.
 *
 * Laravel matches case-SENSITIVELY. Registering only one casing 404s the other,
 * which is precisely what put "Search NaN jobs in B.C." on the WorkBC home page:
 * Drupal's cron hit `GetTotalJobs`, got a 404 body, and coerced it to a number.
 *
 * So: register every casing known to be in live use. Until the Drupal module is
 * read end-to-end (its repo is separate — see docs/integration/drupal-embed.md),
 * assume any documented casing may have a caller.
 *
 * These three are the ONLY genuine server-to-server endpoints. The career /
 * industry profile routes are NOT here: they are called from the browser and
 * authenticate with the seeker's session, so they live in web.php to pick up
 * the session/cookie/CSRF middleware (ACCT-6, ADR-009).
 */

// §2.1 Job search. The optional {language} segment selects jobs_fr (e.g. "fr");
// anything else (including omitted) uses the English index.
// `Search/JobSearch` is the only casing seen in the wild, and it already matches.
Route::post('Search/JobSearch/{language?}', [SearchApiController::class, 'jobSearch'])
    ->where('language', '[A-Za-z]{2}');

// §2.2 Total active job count. `GetTotalJobs` is the casing real callers use;
// `gettotaljobs` is kept because our own docs published it.
foreach (['GetTotalJobs', 'gettotaljobs'] as $totalJobsPath) {
    Route::get("Search/{$totalJobsPath}/{language?}", [SearchApiController::class, 'totalJobs'])
        ->where('language', '[A-Za-z]{2}');
}

// §2.3 City autocomplete — {includeRegion} is a fixed boolean-like segment in
// the existing Drupal contract; the current suggestion list (SRCH-2) doesn't
// vary by it, so it's accepted but unused. Capital-L `Location` is what
// cases.txt exercises; lowercase is what contracts.md published.
foreach (['Location', 'location'] as $locationPrefix) {
    Route::get("{$locationPrefix}/cities/{cityName}/{includeRegion}", [LocationApiController::class, 'cities'])
        ->where('includeRegion', '[A-Za-z]+');
}
