<?php

use App\Http\Controllers\JobDetailController;
use App\Http\Controllers\LegacyAlertRedirectController;
use App\Livewire\JobSearch;
use Illuminate\Pagination\LengthAwarePaginator;
use Illuminate\Support\Facades\Route;

Route::get('/', function () {
    return view('welcome');
});

// Public, server-rendered job search (SRCH-1). Reachable anonymously — no auth
// middleware. Full-page Livewire component: results render into the initial HTML.
Route::get('/jobs', JobSearch::class)->name('jobs.index');

// SRCH-6 redirect shim: forwards legacy alert-email deep-links
// (/job-search#/…;key=value matrix params) to the canonical /jobs?… URL.
Route::get('/job-search', LegacyAlertRedirectController::class)->name('jobs.alert-redirect');

// SRCH-7 job-detail page: path-based, crawlable `/jobs/{slug}-{JobId}` URL.
// Server-rendered Blade (SEO-critical, not Livewire); emits schema.org/JobPosting
// JSON-LD. The `{job}` segment carries a hyphenated slug plus the alphanumeric
// JobId, so allow letters, digits and hyphens.
Route::get('/jobs/{job}', JobDetailController::class)
    ->where('job', '[A-Za-z0-9-]+')
    ->name('jobs.show');

// UI Kit: renders the base layout + accessible component library. Serves as the
// automated accessibility (pa11y) target in CI. No database access required.
Route::get('/ui', function () {
    $items = collect(range(1, 47));
    $perPage = 10;
    $page = LengthAwarePaginator::resolveCurrentPage();

    $paginator = new LengthAwarePaginator(
        $items->forPage($page, $perPage)->values(),
        $items->count(),
        $perPage,
        $page,
        ['path' => LengthAwarePaginator::resolveCurrentPath()]
    );

    return view('ui-kit', ['paginator' => $paginator]);
})->name('ui-kit');
