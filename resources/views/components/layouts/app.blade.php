<!DOCTYPE html>
<html lang="{{ str_replace('_', '-', app()->getLocale()) }}" class="h-full">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="color-scheme" content="light">
    <title>{{ $title ?? 'WorkBC Job Board' }}</title>
    @vite(['resources/css/app.css', 'resources/js/app.js'])
</head>
<body class="flex min-h-full flex-col bg-slate-50 text-slate-900 antialiased">
    {{-- Keyboard users can jump straight to content (WCAG 2.4.1 Bypass Blocks). --}}
    <a href="#main" class="skip-link">Skip to main content</a>

    <header class="border-b border-slate-200 bg-white">
        <div class="mx-auto flex max-w-7xl flex-wrap items-center justify-between gap-4 px-4 py-3">
            <a href="/"
               class="rounded text-lg font-bold text-blue-800 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900">
                WorkBC Job Board
            </a>
            <nav aria-label="Primary">
                <ul class="flex flex-wrap items-center gap-1">
                    <li>
                        <a href="/" class="nav-link">Home</a>
                    </li>
                    <li>
                        <a href="{{ route('jobs.index') }}"
                           class="nav-link"
                           @if (request()->routeIs('jobs.index')) aria-current="page" @endif>
                            Find jobs
                        </a>
                    </li>
                    <li>
                        <a href="{{ route('ui-kit') }}"
                           class="nav-link"
                           @if (request()->routeIs('ui-kit')) aria-current="page" @endif>
                            UI Kit
                        </a>
                    </li>
                </ul>
            </nav>
        </div>
    </header>

    {{-- tabindex="-1" lets the skip link move focus here without adding it to the tab order. --}}
    <main id="main" tabindex="-1" class="mx-auto w-full max-w-7xl flex-1 px-4 py-8 focus:outline-none">
        {{ $slot }}
    </main>

    <footer class="border-t border-slate-200 bg-white">
        <div class="mx-auto max-w-7xl px-4 py-6 text-sm text-slate-600">
            <p>&copy; {{ date('Y') }} Province of British Columbia. Server-rendered with Blade &amp; Livewire.</p>
        </div>
    </footer>
</body>
</html>
