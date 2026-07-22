<div class="space-y-6">
    @php
        $lastPage = (int) max(1, (int) ceil($result->count / max(1, $result->pageSize)));
        $current = min(max(1, $result->pageNumber), $lastPage);
        $firstItem = $result->count === 0 ? 0 : (($current - 1) * $result->pageSize) + 1;
        $lastItem = min($result->count, $current * $result->pageSize);
        $windowStart = max(1, $current - 2);
        $windowEnd = min($lastPage, $current + 2);
    @endphp

    <div>
        <h1 class="text-3xl font-bold">Find jobs</h1>
        <p class="mt-1 text-slate-700">Search current job postings across British Columbia.</p>
    </div>

    {{-- Keyword + scope: submitted together (data → Livewire). --}}
    <form wire:submit="applySearch" role="search" aria-label="Job search"
          class="grid gap-4 rounded-lg border border-slate-200 bg-white p-4 sm:grid-cols-[1fr_auto_auto] sm:items-end">
        <x-form-field
            name="keyword"
            type="search"
            label="Keywords"
            wire:model="keyword"
            hint="Spaces mean AND, commas or pipes mean OR, and &quot;quotes&quot; match an exact phrase." />

        <x-form-field name="searchIn" label="Search by">
            <x-slot:control>
                <select id="searchIn" name="searchIn" wire:model="searchIn"
                        class="block w-full rounded-md border border-slate-400 px-3 py-2 text-slate-900 shadow-sm focus-visible:border-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900 sm:w-44">
                    <option value="all">All fields</option>
                    <option value="title">Job title</option>
                    <option value="employer">Employer</option>
                    <option value="jobId">Job ID</option>
                </select>
            </x-slot:control>
        </x-form-field>

        <div>
            <x-button type="submit">
                <span wire:loading.remove wire:target="applySearch">Search</span>
                <span wire:loading wire:target="applySearch">Searching…</span>
            </x-button>
        </div>
    </form>

    @if ($unavailable)
        <x-alert type="error" title="Search is temporarily unavailable">
            We could not reach the job index. Please try again in a few moments.
        </x-alert>
    @endif

    {{-- Result count + sort control. The count is announced politely on update. --}}
    <div class="flex flex-col gap-3 border-b border-slate-200 pb-3 sm:flex-row sm:items-center sm:justify-between">
        <p role="status" aria-live="polite" class="text-sm text-slate-700">
            @if ($result->count === 0)
                No jobs found.
            @else
                Showing <span class="font-medium">{{ number_format($firstItem) }}</span>&ndash;<span class="font-medium">{{ number_format($lastItem) }}</span>
                of <span class="font-medium">{{ number_format($result->count) }}</span> {{ \Illuminate\Support\Str::plural('job', $result->count) }}
            @endif
        </p>

        <div class="flex items-center gap-2">
            <label for="sort" class="text-sm font-medium text-slate-900">Sort by</label>
            <select id="sort" wire:model.live="sort"
                    class="rounded-md border border-slate-400 px-3 py-2 text-sm text-slate-900 shadow-sm focus-visible:border-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                @foreach ($sortOptions as $value => $label)
                    <option value="{{ $value }}">{{ $label }}</option>
                @endforeach
            </select>
        </div>
    </div>

    {{-- Results: an ARIA live region; aria-busy flips while Livewire is loading. --}}
    <div
        role="region"
        aria-label="Search results"
        aria-live="polite"
        aria-busy="false"
        wire:loading.attr="aria-busy"
        class="space-y-3"
    >
        @forelse ($result->results as $job)
            @php $j = $job->toArray(); @endphp
            <article class="rounded-lg border border-slate-200 bg-white p-4 focus-within:ring-2 focus-within:ring-blue-700">
                <h2 class="text-lg font-semibold">
                    {{-- Path-based, crawlable detail URL (placeholder route until SRCH-7). --}}
                    <a href="{{ route('jobs.show', ['jobId' => $j['JobId']]) }}" wire:navigate
                       class="text-blue-800 hover:underline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900">
                        {{ $j['Title'] ?? 'Untitled position' }}
                    </a>
                </h2>
                <p class="mt-1 text-sm text-slate-700">
                    @if (! empty($j['EmployerName']))
                        <span class="font-medium">{{ $j['EmployerName'] }}</span>
                    @endif
                    @if (! empty($j['City']))
                        <span aria-hidden="true"> · </span>{{ $j['City'] }}
                    @endif
                </p>
                @if (! empty($j['DatePosted']))
                    <p class="mt-1 text-xs text-slate-500">
                        Posted <time datetime="{{ $j['DatePosted'] }}">{{ \Illuminate\Support\Carbon::parse($j['DatePosted'])->timezone('America/Vancouver')->format('M j, Y') }}</time>
                    </p>
                @endif
            </article>
        @empty
            @unless ($unavailable)
                <x-alert type="info" title="No matches">
                    No jobs match your search. Try broadening your keywords or clearing filters.
                </x-alert>
            @endunless
        @endforelse
    </div>

    {{-- Pagination: Livewire-updated (data) with accessible controls. --}}
    @if ($lastPage > 1)
        <nav role="navigation" aria-label="Search results pages" class="flex items-center justify-center">
            <ul class="flex flex-wrap items-center gap-1">
                <li>
                    <button type="button" wire:click="gotoPage({{ $current - 1 }})" @disabled($current <= 1)
                            aria-label="Go to previous page"
                            class="inline-flex min-w-9 items-center justify-center rounded-md px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-100 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900 disabled:cursor-not-allowed disabled:text-slate-400 disabled:hover:bg-transparent">
                        <span aria-hidden="true">&lsaquo;</span>
                    </button>
                </li>

                @if ($windowStart > 1)
                    <li>
                        <button type="button" wire:click="gotoPage(1)" aria-label="Go to page 1"
                                class="inline-flex min-w-9 items-center justify-center rounded-md px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-100 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900">1</button>
                    </li>
                    @if ($windowStart > 2)
                        <li aria-hidden="true" class="px-2 text-slate-400">&hellip;</li>
                    @endif
                @endif

                @for ($p = $windowStart; $p <= $windowEnd; $p++)
                    <li>
                        @if ($p === $current)
                            <span aria-current="page"
                                  class="inline-flex min-w-9 items-center justify-center rounded-md bg-blue-700 px-3 py-2 text-sm font-medium text-white">
                                <span class="sr-only">Page </span>{{ $p }}<span class="sr-only"> (current)</span>
                            </span>
                        @else
                            <button type="button" wire:click="gotoPage({{ $p }})" aria-label="Go to page {{ $p }}"
                                    class="inline-flex min-w-9 items-center justify-center rounded-md px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-100 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900">{{ $p }}</button>
                        @endif
                    </li>
                @endfor

                @if ($windowEnd < $lastPage)
                    @if ($windowEnd < $lastPage - 1)
                        <li aria-hidden="true" class="px-2 text-slate-400">&hellip;</li>
                    @endif
                    <li>
                        <button type="button" wire:click="gotoPage({{ $lastPage }})" aria-label="Go to page {{ $lastPage }}"
                                class="inline-flex min-w-9 items-center justify-center rounded-md px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-100 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900">{{ $lastPage }}</button>
                    </li>
                @endif

                <li>
                    <button type="button" wire:click="gotoPage({{ $current + 1 }})" @disabled($current >= $lastPage)
                            aria-label="Go to next page"
                            class="inline-flex min-w-9 items-center justify-center rounded-md px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-100 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900 disabled:cursor-not-allowed disabled:text-slate-400 disabled:hover:bg-transparent">
                        <span aria-hidden="true">&rsaquo;</span>
                    </button>
                </li>
            </ul>
        </nav>
    @endif
</div>
