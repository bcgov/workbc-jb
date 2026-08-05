@php
    use App\Support\JobSlug;
@endphp

<x-account.layout page-title="Recommended jobs" page-description="Jobs recommended from your saved jobs activity and profile signals.">
<section aria-labelledby="recommended-jobs-heading" class="space-y-6">
    <h2 id="recommended-jobs-heading" class="text-2xl font-semibold tracking-tight text-slate-900">
        Recommended jobs <span class="font-normal text-slate-600 tabular-nums">({{ $total }})</span>
    </h2>

    @if (! $hasSavedJobs)
        <x-alert type="info" title="No saved jobs yet">
            Save jobs from search results or a job detail page to get personalized recommendations.
        </x-alert>
    @elseif ($jobs === [])
        <x-alert type="info" title="No recommended jobs found">
            We could not find active jobs matching your saved-job signals right now.
        </x-alert>
    @else
        <ul class="space-y-4" aria-label="Recommended jobs list">
            @foreach ($jobs as $job)
                @php
                    $detailTitle = is_string($job['Title'] ?? null) && $job['Title'] !== '' ? $job['Title'] : null;
                @endphp
                <li class="rounded-lg border border-slate-200 bg-white p-4" wire:key="recommended-job-{{ $job['JobId'] ?? $loop->index }}">
                    <article class="space-y-3">
                        <header class="space-y-1">
                            <h3 class="text-lg font-semibold text-slate-900">
                                <a href="{{ route('jobs.show', ['job' => JobSlug::path((string) ($job['JobId'] ?? ''), $detailTitle)]) }}" wire:navigate
                                   class="text-blue-800 hover:underline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900">
                                    {{ $job['Title'] ?? ('Job '.($job['JobId'] ?? '')) }}
                                </a>
                            </h3>
                            <p class="text-sm text-slate-700">
                                @if (! empty($job['EmployerName']))
                                    <span class="font-medium">{{ $job['EmployerName'] }}</span>
                                @endif
                                @if (! empty($job['EmployerName']) && ! empty($job['City']))
                                    <span aria-hidden="true"> · </span>
                                @endif
                                @if (! empty($job['City']))
                                    {{ $job['City'] }}
                                @endif
                            </p>
                        </header>

                        <p class="rounded bg-slate-50 p-3 text-sm text-slate-800" data-testid="recommended-reason">
                            {{ $job['Reason'] ?? 'Recommended based on your saved jobs and profile.' }}
                        </p>

                        <p class="text-xs text-slate-500" data-testid="recommended-score">
                            Relevance score: {{ number_format((float) ($job['Score'] ?? 0), 2) }}
                        </p>
                    </article>
                </li>
            @endforeach
        </ul>
    @endif
</section>
</x-account.layout>
