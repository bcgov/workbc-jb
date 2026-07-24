<section aria-labelledby="account-dashboard-heading" class="space-y-8">
    <header class="space-y-2">
        <h1 id="account-dashboard-heading" class="text-3xl font-bold tracking-tight text-slate-900">My account</h1>
        <p class="text-slate-700">View your saved activity summary and continue to account areas.</p>
    </header>

    <section aria-labelledby="account-summary-heading" class="space-y-4">
        <div class="flex items-center justify-between gap-4">
            <h2 id="account-summary-heading" class="text-xl font-semibold text-slate-900">Summary</h2>
            <x-button type="button" variant="secondary" wire:click="refreshCounts" aria-label="Refresh account counts">
                Refresh counts
            </x-button>
        </div>

        <p class="sr-only" aria-live="polite" aria-atomic="true">
            Saved jobs {{ $savedJobs }}, active alerts {{ $activeAlerts }}, saved career profiles {{ $savedCareerProfiles }}, saved industry profiles {{ $savedIndustryProfiles }}.
        </p>

        <dl class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4" aria-describedby="account-summary-help">
            <div class="rounded-lg border border-slate-200 bg-white p-4">
                <dt class="text-sm font-medium text-slate-600">Saved jobs</dt>
                <dd class="mt-2 text-3xl font-bold text-slate-900 tabular-nums" data-testid="saved-jobs-count">{{ $savedJobs }}</dd>
            </div>

            <div class="rounded-lg border border-slate-200 bg-white p-4">
                <dt class="text-sm font-medium text-slate-600">Active alerts</dt>
                <dd class="mt-2 text-3xl font-bold text-slate-900 tabular-nums" data-testid="active-alerts-count">{{ $activeAlerts }}</dd>
            </div>

            <div class="rounded-lg border border-slate-200 bg-white p-4">
                <dt class="text-sm font-medium text-slate-600">Saved career profiles</dt>
                <dd class="mt-2 text-3xl font-bold text-slate-900 tabular-nums" data-testid="saved-career-profiles-count">{{ $savedCareerProfiles }}</dd>
            </div>

            <div class="rounded-lg border border-slate-200 bg-white p-4">
                <dt class="text-sm font-medium text-slate-600">Saved industry profiles</dt>
                <dd class="mt-2 text-3xl font-bold text-slate-900 tabular-nums" data-testid="saved-industry-profiles-count">{{ $savedIndustryProfiles }}</dd>
            </div>
        </dl>

        <p id="account-summary-help" class="text-sm text-slate-600">Counts only include your account data.</p>
    </section>

    <nav aria-label="Account areas" class="space-y-3">
        <h2 class="text-xl font-semibold text-slate-900">Go to</h2>
        <ul class="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
            <li>
                <a href="/account/saved-jobs" class="nav-link block rounded-md border border-slate-200 bg-white px-4 py-3 font-medium">
                    Saved jobs
                </a>
            </li>
            <li>
                <a href="/account/alerts" class="nav-link block rounded-md border border-slate-200 bg-white px-4 py-3 font-medium">
                    Alerts
                </a>
            </li>
            <li>
                <a href="/account/profiles" class="nav-link block rounded-md border border-slate-200 bg-white px-4 py-3 font-medium">
                    Profiles
                </a>
            </li>
            <li>
                <a href="/account/settings" class="nav-link block rounded-md border border-slate-200 bg-white px-4 py-3 font-medium">
                    Settings
                </a>
            </li>
        </ul>
    </nav>
</section>
