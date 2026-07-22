<x-layouts.app title="Job detail — WorkBC Job Board">
    <div class="space-y-4">
        <p>
            <a href="{{ route('jobs.index') }}" wire:navigate
               class="text-blue-800 hover:underline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900">
                &lsaquo; Back to search
            </a>
        </p>
        <h1 class="text-3xl font-bold">Job&nbsp;{{ $jobId }}</h1>
        <x-alert type="info" title="Coming soon">
            The full job-detail page (with <code>schema.org/JobPosting</code> structured data) is delivered in SRCH-7.
            This placeholder confirms the path-based, crawlable URL.
        </x-alert>
    </div>
</x-layouts.app>
