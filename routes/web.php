<?php

use Illuminate\Pagination\LengthAwarePaginator;
use Illuminate\Support\Facades\Route;

Route::get('/', function () {
    return view('welcome');
});

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
