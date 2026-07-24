<div>
    @if ($isAuthenticated)
        <button type="button"
                wire:click="toggle"
                aria-pressed="{{ $isSaved ? 'true' : 'false' }}"
                data-testid="save-job-toggle-{{ $jobId }}"
                aria-label="{{ $isSaved ? 'Unsave this job' : 'Save this job' }}"
                class="inline-flex items-center rounded border border-slate-300 bg-white px-3 py-1.5 text-sm font-medium text-slate-800 hover:bg-slate-50 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900">
            {{ $isSaved ? 'Saved' : 'Save job' }}
        </button>
        <p class="sr-only" role="status" aria-live="polite" aria-atomic="true">{{ $statusMessage }}</p>
    @else
        <a href="{{ route('login') }}"
           class="text-sm font-medium text-blue-800 hover:underline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900">
            Sign in to save
        </a>
    @endif
</div>
