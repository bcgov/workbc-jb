@props([
    'pageTitle',
    'pageDescription' => null,
])

@php
    $jobSeeker = \Illuminate\Support\Facades\Auth::guard('web')->user();
    $firstName = trim((string) ($jobSeeker?->FirstName ?? ''));
    $greetingName = $firstName !== '' ? $firstName : 'there';
    $jobsOpen = request()->routeIs('account.saved-jobs') || request()->routeIs('account.alerts*');
    $careersOpen = request()->routeIs('account.profiles');
@endphp

<section class="space-y-6">
    <header class="space-y-2">
        <p class="text-sm font-semibold text-slate-700">Hello, {{ $greetingName }}</p>
        <h1 class="text-3xl font-bold tracking-tight text-slate-900">{{ $pageTitle }}</h1>
        @if (is_string($pageDescription) && $pageDescription !== '')
            <p class="text-slate-700">{{ $pageDescription }}</p>
        @endif
    </header>

    <nav aria-label="Account navigation" class="rounded-lg border border-slate-200 bg-white px-4 py-3">
        <ul class="flex flex-wrap items-start gap-4 text-sm font-semibold text-slate-800">
            <li>
                <a href="{{ route('account.dashboard') }}" wire:navigate class="nav-link px-2 py-1" @if (request()->routeIs('account.dashboard')) aria-current="page" @endif>
                    Account profile
                </a>
            </li>

            <li>
                <details @if ($jobsOpen) open @endif>
                    <summary class="cursor-pointer rounded px-2 py-1 hover:bg-slate-100 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-workbc-navy">
                        Jobs
                    </summary>
                    <ul class="mt-2 space-y-1 border-l border-slate-200 ps-3 text-sm font-normal">
                        <li>
                            <a href="{{ route('account.saved-jobs') }}" wire:navigate class="nav-link block px-2 py-1" @if (request()->routeIs('account.saved-jobs')) aria-current="page" @endif>
                                Saved jobs
                            </a>
                        </li>
                        <li>
                            <a href="{{ route('account.alerts') }}" wire:navigate class="nav-link block px-2 py-1" @if (request()->routeIs('account.alerts')) aria-current="page" @endif>
                                Job alerts
                            </a>
                        </li>
                    </ul>
                </details>
            </li>

            <li>
                <details @if ($careersOpen) open @endif>
                    <summary class="cursor-pointer rounded px-2 py-1 hover:bg-slate-100 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-workbc-navy">
                        Careers &amp; industries
                    </summary>
                    <ul class="mt-2 space-y-1 border-l border-slate-200 ps-3 text-sm font-normal">
                        <li>
                            <a href="{{ route('account.profiles') }}" wire:navigate class="nav-link block px-2 py-1" @if (request()->routeIs('account.profiles')) aria-current="page" @endif>
                                Saved profiles
                            </a>
                        </li>
                    </ul>
                </details>
            </li>

            <li>
                <details>
                    <summary class="cursor-pointer rounded px-2 py-1 hover:bg-slate-100 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-workbc-navy">
                        Manage account
                    </summary>
                    <ul class="mt-2 space-y-1 border-l border-slate-200 ps-3 text-sm font-normal">
                        <li>
                            <span class="block rounded px-2 py-1 text-slate-500" aria-disabled="true">Personal settings (coming soon)</span>
                        </li>
                    </ul>
                </details>
            </li>
        </ul>
    </nav>

    {{ $slot }}
</section>