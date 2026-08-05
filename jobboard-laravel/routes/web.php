<?php

use App\Http\Controllers\JobDetailController;
use App\Http\Controllers\LegacyAlertRedirectController;
use App\Http\Controllers\Admin\ImpersonationController;
use App\Http\Controllers\Api\CareerProfileApiController;
use App\Http\Controllers\Api\IndustryProfileApiController;
use App\Http\Controllers\Auth\JobSeekerPasswordResetController;
use App\Http\Controllers\Auth\JobSeekerRegistrationController;
use App\Http\Controllers\Auth\JobSeekerSessionController;
use App\Http\Controllers\Web\SitemapController;
use App\Http\Middleware\EnsureJobSeekerSession;
use App\Livewire\JobAlertsList;
use App\Livewire\JobSeekerDashboard;
use App\Livewire\PersonalSettingsPage;
use App\Livewire\JobSearch;
use App\Livewire\RecommendedJobsPage;
use App\Livewire\SavedJobsPage;
use App\Livewire\SavedProfilesPage;
use Illuminate\Pagination\LengthAwarePaginator;
use Illuminate\Support\Facades\Route;

Route::get('/', function () {
    return view('welcome');
});

Route::post('/auth/job-seeker/login', [JobSeekerSessionController::class, 'store'])
    ->middleware('throttle:job-seeker-login')
    ->name('job-seeker.login');
Route::post('/auth/job-seeker/logout', [JobSeekerSessionController::class, 'destroy'])->name('job-seeker.logout');
Route::post('/auth/job-seeker/register', [JobSeekerRegistrationController::class, 'store'])
    ->middleware('throttle:job-seeker-register')
    ->name('job-seeker.register');
Route::get('/auth/job-seeker/verify/{userId}/{guid}', [JobSeekerRegistrationController::class, 'verify'])->name('job-seeker.verify');
Route::post('/auth/job-seeker/forgot-password', [JobSeekerPasswordResetController::class, 'request'])
    ->middleware('throttle:job-seeker-forgot-password')
    ->name('job-seeker.forgot-password');
Route::post('/auth/job-seeker/reset-password', [JobSeekerPasswordResetController::class, 'reset'])
    ->middleware('throttle:job-seeker-reset-password')
    ->name('job-seeker.reset-password');

// Placeholder login page route name for auth middleware guest redirects.
Route::view('/login', 'welcome')->name('login');

Route::middleware('auth:web')->group(function (): void {
    Route::get('/account', JobSeekerDashboard::class)->name('account.dashboard');
    Route::get('/account/saved-jobs', SavedJobsPage::class)->name('account.saved-jobs');
    Route::get('/account/recommended', RecommendedJobsPage::class)->name('account.recommended');

    // ACCT-3: alert management. Create/edit reuse the JobSearch component in
    // "alert mode" — detected from the route name in mount() (see JobSearch).
    Route::get('/account/alerts', JobAlertsList::class)->name('account.alerts');
    Route::get('/account/alerts/create', JobSearch::class)->name('account.alerts.create');
    Route::get('/account/alerts/{alertId}/edit', JobSearch::class)
        ->whereNumber('alertId')
        ->name('account.alerts.edit');

    // ACCT-6: saved career & industry profiles.
    Route::get('/account/profiles', SavedProfilesPage::class)->name('account.profiles');

    // ACCT-7: personal settings.
    Route::get('/account/settings', PersonalSettingsPage::class)->name('account.settings');

    // FND-6 / ADM-4 scaffold: ends the impersonated seeker session (web guard
    // only) and returns to the admin panel. The admin's own `admin`-guard
    // session was never touched by impersonation, so this is a clean return.
    Route::post('/account/impersonation/end', [ImpersonationController::class, 'end'])
        ->name('account.impersonation.end');
});

/*
 * ACCT-6 / ADR-009 — career & industry profile save/status.
 *
 * These live in web.php, not api.php, despite the `api/` path: they are called
 * from the BROWSER by Drupal's profile pages and authenticate with the job
 * seeker's session cookie, so they need the `web` group's session + cookie +
 * CSRF middleware. Laravel 12's `api` group is throttle + SubstituteBindings
 * only — a session cookie arriving there is ignored entirely.
 *
 * The `api/` path prefix is retained because Drupal's `WorkbcJobboardSaveProfile`
 * block builds these URLs; changing it would be a needless Drupal edit.
 * `{profileId}` is the NocCodeId2021 / IndustryId value itself.
 */
Route::middleware(EnsureJobSeekerSession::class)
    ->prefix('api')
    ->whereNumber('profileId')
    ->group(function (): void {
        Route::get('career-profiles/status/{profileId}', [CareerProfileApiController::class, 'status']);
        Route::post('career-profiles/save/{profileId}', [CareerProfileApiController::class, 'save']);
        Route::delete('career-profiles/{profileId}', [CareerProfileApiController::class, 'remove']);

        Route::get('industry-profiles/status/{profileId}', [IndustryProfileApiController::class, 'status']);
        Route::post('industry-profiles/save/{profileId}', [IndustryProfileApiController::class, 'save']);
        Route::delete('industry-profiles/{profileId}', [IndustryProfileApiController::class, 'remove']);
    });

// Local-only preview helpers live in a gitignored file so they can never be
// committed (routes/dev-preview.php). Harmless if this loader is ever committed:
// it only runs in local and no-ops if the file is absent.
if (app()->environment('local') && file_exists(base_path('routes/dev-preview.php'))) {
    require base_path('routes/dev-preview.php');
}

// SRCH-8: job sitemap (SEO). ~35 k active jobs × 2 languages > 50 k URL limit,
// so a sitemap index points to two language shards. Shards are cached in Redis
// (default 4 h TTL via SITEMAP_CACHE_TTL env). Both routes must sit above the
// catch-all /jobs/{job} route to avoid the segment constraint swallowing them.
Route::get('/sitemap.xml', [SitemapController::class, 'index'])->name('sitemap.index');
Route::get('/sitemap-{language}.xml', [SitemapController::class, 'shard'])
    ->where('language', 'en|fr')
    ->name('sitemap.shard');

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
