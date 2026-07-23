@php
    use Illuminate\Support\Carbon;

    $title = $job['Title'] ?? 'Job posting';
    $employer = $job['EmployerName'] ?? null;
    $city = $job['City'] ?? null;
    $province = $job['Province'] ?? null;
    $regions = $job['Region'] ?? [];
    $datePosted = $job['DatePosted'] ?? null;
    $expireDate = $job['ExpireDate'] ?? null;
    $salarySummary = $job['SalarySummary'] ?? null;
    $hours = $job['HoursOfWork']['Description'] ?? [];
    $employmentTerms = $job['EmploymentTerms']['Description'] ?? [];
    $noc = $job['Noc2021'] ?? null;
    $nocGroup = $job['NocGroup'] ?? null;
    $workplace = $job['WorkplaceType']['Description'] ?? null;
    $views = $job['Views'] ?? null;

    $expired = $expireDate ? Carbon::parse($expireDate)->isPast() : false;

    $fmt = static fn (?string $d): ?string => $d ? Carbon::parse($d)->timezone('America/Vancouver')->format('M j, Y') : null;
    $location = collect([$city, $province])->filter()->implode(', ');
@endphp

<x-layouts.app :title="$metaTitle" :description="$metaDescription">
    <x-slot:head>
        <link rel="canonical" href="{{ $canonicalUrl }}">
        <link rel="alternate" hreflang="en" href="{{ $alternateEnUrl }}">
        <link rel="alternate" hreflang="fr" href="{{ $alternateFrUrl }}">
        <script type="application/ld+json">
            {!! json_encode($jsonLd, JSON_UNESCAPED_UNICODE) !!}
        </script>
    </x-slot:head>

    <div class="mx-auto max-w-3xl px-4 py-6">
        <nav aria-label="Breadcrumb" class="mb-4 text-sm">
            <a href="{{ route('jobs.index') }}" wire:navigate
               class="text-blue-800 hover:underline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900">
                &larr; Back to search
            </a>
        </nav>

        <article aria-labelledby="job-title">
            <header class="border-b border-slate-200 pb-4">
                <h1 id="job-title" class="text-2xl font-bold text-slate-900">{{ $title }}</h1>

                @if ($employer || $location)
                    <p class="mt-1 text-slate-700">
                        @if ($employer)<span class="font-medium">{{ $employer }}</span>@endif
                        @if ($employer && $location)<span aria-hidden="true"> · </span>@endif
                        @if ($location){{ $location }}@endif
                    </p>
                @endif

                @if ($expired)
                    <div class="mt-3">
                        <x-alert type="warning" title="This posting has expired">
                            This job may no longer be accepting applications.
                        </x-alert>
                    </div>
                @endif

                <div class="mt-4 flex flex-wrap gap-2">
                    <button type="button" onclick="window.print()"
                            class="rounded border border-slate-300 bg-white px-3 py-1.5 text-sm font-medium text-slate-800 hover:bg-slate-50 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900">
                        Print
                    </button>
                    <button type="button" data-share-url="{{ $canonicalUrl }}" data-share-title="{{ $title }}"
                            class="rounded border border-slate-300 bg-white px-3 py-1.5 text-sm font-medium text-slate-800 hover:bg-slate-50 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900"
                            hidden>
                        Share
                    </button>
                </div>
            </header>

            <section aria-labelledby="job-overview-heading" class="py-4">
                <h2 id="job-overview-heading" class="sr-only">Job overview</h2>
                <dl class="grid grid-cols-1 gap-x-6 gap-y-3 sm:grid-cols-2">
                    @if ($fmt($datePosted))
                        <div>
                            <dt class="text-xs font-medium uppercase tracking-wide text-slate-500">Posted</dt>
                            <dd class="text-slate-900"><time datetime="{{ $datePosted }}">{{ $fmt($datePosted) }}</time></dd>
                        </div>
                    @endif
                    @if ($fmt($expireDate))
                        <div>
                            <dt class="text-xs font-medium uppercase tracking-wide text-slate-500">Closes</dt>
                            <dd class="text-slate-900"><time datetime="{{ $expireDate }}">{{ $fmt($expireDate) }}</time></dd>
                        </div>
                    @endif
                    @if ($salarySummary)
                        <div>
                            <dt class="text-xs font-medium uppercase tracking-wide text-slate-500">Salary</dt>
                            <dd class="text-slate-900">{{ $salarySummary }}</dd>
                        </div>
                    @endif
                    @if (! empty($hours))
                        <div>
                            <dt class="text-xs font-medium uppercase tracking-wide text-slate-500">Hours of work</dt>
                            <dd class="text-slate-900">{{ implode(', ', (array) $hours) }}</dd>
                        </div>
                    @endif
                    @if (! empty($employmentTerms))
                        <div>
                            <dt class="text-xs font-medium uppercase tracking-wide text-slate-500">Employment terms</dt>
                            <dd class="text-slate-900">{{ implode(', ', (array) $employmentTerms) }}</dd>
                        </div>
                    @endif
                    @if ($workplace)
                        <div>
                            <dt class="text-xs font-medium uppercase tracking-wide text-slate-500">Workplace</dt>
                            <dd class="text-slate-900">{{ $workplace }}</dd>
                        </div>
                    @endif
                    @if ($noc)
                        <div>
                            <dt class="text-xs font-medium uppercase tracking-wide text-slate-500">NOC 2021</dt>
                            <dd class="text-slate-900">{{ $noc }}@if ($nocGroup) — {{ $nocGroup }}@endif</dd>
                        </div>
                    @endif
                    @if (! empty($regions))
                        <div>
                            <dt class="text-xs font-medium uppercase tracking-wide text-slate-500">Region</dt>
                            <dd class="text-slate-900">{{ implode(', ', (array) $regions) }}</dd>
                        </div>
                    @endif
                </dl>
            </section>

            @if ($descriptionText !== '')
                <section aria-labelledby="job-description-heading" class="border-t border-slate-200 py-4">
                    <h2 id="job-description-heading" class="text-lg font-semibold text-slate-900">Job description</h2>
                    @if ($apply && $apply['isExternal'] && $apply['sourceName'])
                        <p class="mt-1 text-sm text-slate-500">via {{ $apply['sourceName'] }}</p>
                    @endif
                    <div class="prose prose-slate mt-2 max-w-none">
                        {!! nl2br(e($descriptionText)) !!}
                    </div>
                </section>
            @endif

            @if ($apply)
                <section aria-labelledby="job-apply-heading" class="border-t border-slate-200 py-4">
                    <h2 id="job-apply-heading" class="text-lg font-semibold text-slate-900">How to apply</h2>
                    @if ($apply['isExternal'] && $apply['sourceName'])
                        <p class="mt-1 text-sm text-slate-600">Posted via {{ $apply['sourceName'] }}.</p>
                    @endif
                    @if ($apply['isExternal'] && $expired)
                        <p class="mt-1 text-sm text-slate-500">This posting has expired — the original listing may no longer be available.</p>
                    @endif
                    <p class="mt-2">
                        <a href="{{ $apply['url'] }}" rel="nofollow noopener" target="_blank"
                           class="inline-flex items-center rounded bg-blue-800 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-900 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900">
                            @if ($apply['isExternal'])
                                {{ $apply['sourceName'] ? 'Apply on '.$apply['sourceName'] : 'View original posting' }}
                            @else
                                Apply now
                            @endif
                            <span class="sr-only"> (opens in a new tab)</span>
                        </a>
                    </p>
                </section>
            @endif

            @if ($views !== null)
                <footer class="border-t border-slate-200 pt-4 text-sm text-slate-500">
                    {{ number_format((int) $views) }} {{ \Illuminate\Support\Str::plural('view', (int) $views) }}
                </footer>
            @endif
        </article>
    </div>

    <script>
        (function () {
            var btn = document.querySelector('[data-share-url]');
            if (!btn) return;
            btn.hidden = false;
            btn.addEventListener('click', function () {
                var url = btn.getAttribute('data-share-url');
                var title = btn.getAttribute('data-share-title');
                if (navigator.share) {
                    navigator.share({ title: title, url: url }).catch(function () {});
                } else if (navigator.clipboard) {
                    navigator.clipboard.writeText(url);
                }
            });
        })();
    </script>
</x-layouts.app>
