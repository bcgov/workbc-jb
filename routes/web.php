<?php

use App\Livewire\JobSearch;
use Illuminate\Pagination\LengthAwarePaginator;
use Illuminate\Support\Facades\Route;

Route::get('/', function () {
    return view('welcome');
});

// Public, server-rendered job search (SRCH-1). Reachable anonymously — no auth
// middleware. Full-page Livewire component: results render into the initial HTML.
Route::get('/jobs', JobSearch::class)->name('jobs.index');

// Placeholder path-based job-detail route so search results can link to a real,
// crawlable URL. The SEO detail page itself is built in SRCH-7.
Route::get('/jobs/{jobId}', function (string $jobId) {
    return view('jobs.show-placeholder', ['jobId' => $jobId]);
})->whereAlphaNumeric('jobId')->name('jobs.show');

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
