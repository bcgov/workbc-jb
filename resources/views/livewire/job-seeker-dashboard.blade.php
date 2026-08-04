<x-account.layout page-title="My account" page-description="View your saved activity summary and continue to account areas.">
    @if ($welcomeMessage !== null)
        <section x-data="{ visible: true }" x-show="visible" x-cloak class="rounded-lg border border-blue-200 bg-blue-50 p-4" aria-labelledby="account-welcome-title">
            <div class="flex items-start justify-between gap-3">
                <div class="space-y-2">
                    @if ($welcomeMessage['title'] !== '')
                        <h2 id="account-welcome-title" class="text-lg font-semibold text-blue-900">{{ $welcomeMessage['title'] }}</h2>
                    @endif
                    @if ($welcomeMessage['bodyHtml'] !== '')
                        <div class="prose max-w-none prose-p:my-0 prose-ul:my-1">{!! $welcomeMessage['bodyHtml'] !!}</div>
                    @endif
                </div>
                <button type="button" class="rounded border border-blue-300 px-2 py-1 text-sm font-semibold text-blue-900 hover:bg-blue-100" @click="visible = false">
                    Dismiss
                </button>
            </div>
        </section>
    @endif

    @foreach ($notifications as $index => $notification)
        <section class="rounded-lg border border-amber-300 bg-amber-50 p-4" aria-labelledby="account-notification-title-{{ $index }}" data-testid="account-notification">
            @if ($notification['title'] !== '')
                <h2 id="account-notification-title-{{ $index }}" class="text-lg font-semibold text-amber-900">{{ $notification['title'] }}</h2>
            @endif
            @if ($notification['bodyHtml'] !== '')
                <div class="prose mt-2 max-w-none prose-p:my-0 prose-ul:my-1">{!! $notification['bodyHtml'] !!}</div>
            @endif
        </section>
    @endforeach

    @if ($introTextHtml !== '')
        <section class="prose max-w-none rounded-lg border border-slate-200 bg-white p-4" aria-label="Dashboard introduction">
            {!! $introTextHtml !!}
        </section>
    @endif

    <section aria-labelledby="account-summary-heading" class="space-y-4">
        <div class="flex items-center justify-between gap-4">
            <h2 id="account-summary-heading" class="text-xl font-semibold text-slate-900">Account summary</h2>
            <x-button type="button" variant="secondary" wire:click="refreshCounts" aria-label="Refresh account counts">
                Refresh counts
            </x-button>
        </div>

        <p class="sr-only" aria-live="polite" aria-atomic="true">
            Saved jobs {{ $savedJobs }}, active alerts {{ $activeAlerts }}, saved career profiles {{ $savedCareerProfiles }}, saved industry profiles {{ $savedIndustryProfiles }}.
        </p>

        <div class="grid gap-4 lg:grid-cols-3">
            <section class="rounded-lg border border-slate-200 bg-white p-4" aria-labelledby="jobs-card-heading">
                <h3 id="jobs-card-heading" class="text-lg font-semibold text-slate-900">Jobs</h3>
                @if ($jobsDescriptionHtml !== '')
                    <div class="prose mt-2 max-w-none text-sm prose-p:my-0">{!! $jobsDescriptionHtml !!}</div>
                @endif
                <ul class="mt-4 space-y-2" aria-label="Jobs account links">
                    <li>
                        <a href="{{ route('account.saved-jobs') }}" wire:navigate class="flex items-center justify-between rounded border border-slate-200 px-3 py-2 font-medium text-slate-900 hover:bg-slate-50">
                            <span>Saved jobs</span>
                            <span class="tabular-nums" data-testid="saved-jobs-count">{{ $savedJobs }}</span>
                        </a>
                    </li>
                    <li>
                        <a href="{{ route('account.alerts') }}" wire:navigate class="flex items-center justify-between rounded border border-slate-200 px-3 py-2 font-medium text-slate-900 hover:bg-slate-50">
                            <span>Job alerts</span>
                            <span class="tabular-nums" data-testid="active-alerts-count">{{ $activeAlerts }}</span>
                        </a>
                    </li>
                </ul>
            </section>

            <section class="rounded-lg border border-slate-200 bg-white p-4" aria-labelledby="careers-card-heading">
                <h3 id="careers-card-heading" class="text-lg font-semibold text-slate-900">Careers &amp; industries</h3>
                @if ($careersDescriptionHtml !== '')
                    <div class="prose mt-2 max-w-none text-sm prose-p:my-0">{!! $careersDescriptionHtml !!}</div>
                @endif
                <ul class="mt-4 space-y-2" aria-label="Careers and industries account links">
                    <li>
                        <a href="{{ route('account.profiles') }}" wire:navigate class="flex items-center justify-between rounded border border-slate-200 px-3 py-2 font-medium text-slate-900 hover:bg-slate-50">
                            <span>Saved career profiles</span>
                            <span class="tabular-nums" data-testid="saved-career-profiles-count">{{ $savedCareerProfiles }}</span>
                        </a>
                    </li>
                    <li>
                        <a href="{{ route('account.profiles') }}" wire:navigate class="flex items-center justify-between rounded border border-slate-200 px-3 py-2 font-medium text-slate-900 hover:bg-slate-50">
                            <span>Saved industry profiles</span>
                            <span class="tabular-nums" data-testid="saved-industry-profiles-count">{{ $savedIndustryProfiles }}</span>
                        </a>
                    </li>
                </ul>
            </section>

            <section class="rounded-lg border border-slate-200 bg-white p-4" aria-labelledby="manage-card-heading">
                <h3 id="manage-card-heading" class="text-lg font-semibold text-slate-900">Manage account</h3>
                @if ($accountDescriptionHtml !== '')
                    <div class="prose mt-2 max-w-none text-sm prose-p:my-0">{!! $accountDescriptionHtml !!}</div>
                @endif
                <ul class="mt-4 space-y-2" aria-label="Manage account links">
                    <li>
                        <span class="flex items-center justify-between rounded border border-dashed border-slate-300 px-3 py-2 font-medium text-slate-500" aria-disabled="true">
                            <span>Personal settings (coming soon)</span>
                            <span class="tabular-nums">-</span>
                        </span>
                    </li>
                </ul>
            </section>
        </div>
    </section>

    @if ($resources !== [])
        <section aria-labelledby="recommended-resources-heading" class="space-y-4">
            <h2 id="recommended-resources-heading" class="text-xl font-semibold text-slate-900">Recommended resources</h2>
            <ul class="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
                @foreach ($resources as $resource)
                    <li class="rounded-lg border border-slate-200 bg-white p-4">
                        @if ($resource['url'] !== '')
                            <a href="{{ $resource['url'] }}" target="_blank" rel="noopener noreferrer" class="block rounded focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-workbc-navy">
                                @if ($resource['title'] !== '')
                                    <h3 class="text-base font-semibold text-blue-800 hover:underline">{{ $resource['title'] }}</h3>
                                @endif
                                @if ($resource['bodyHtml'] !== '')
                                    <div class="prose mt-2 max-w-none text-sm prose-p:my-0">{!! $resource['bodyHtml'] !!}</div>
                                @endif
                            </a>
                        @else
                            @if ($resource['title'] !== '')
                                <h3 class="text-base font-semibold text-slate-900">{{ $resource['title'] }}</h3>
                            @endif
                            @if ($resource['bodyHtml'] !== '')
                                <div class="prose mt-2 max-w-none text-sm prose-p:my-0">{!! $resource['bodyHtml'] !!}</div>
                            @endif
                        @endif
                    </li>
                @endforeach
            </ul>
        </section>
    @endif
</x-account.layout>
