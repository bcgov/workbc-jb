<x-account.layout page-title="Job alerts" page-description="Alerts run your saved search on a schedule and email you new matching jobs.">
<section aria-labelledby="job-alerts-heading" class="space-y-6">
    <div class="flex flex-wrap items-center justify-between gap-3">
        <h2 id="job-alerts-heading" class="text-2xl font-semibold tracking-tight text-slate-900">Your alerts</h2>
        <x-button :href="route('account.alerts.create')">Create alert</x-button>
    </div>

    {{-- Delete outcome announced to assistive tech. --}}
    <p class="sr-only" role="status" aria-live="polite">{{ $statusMessage }}</p>

    @if ($alerts === [])
        <div class="rounded-lg border border-slate-200 bg-white p-6 text-center">
            <p class="text-slate-700">You have no job alerts yet.</p>
            <p class="mt-2">
                <a href="{{ route('account.alerts.create') }}" class="font-medium text-blue-800 underline hover:text-workbc-navy">
                    Create your first alert
                </a>
            </p>
        </div>
    @else
        <ul role="list" class="space-y-3">
            @foreach ($alerts as $alert)
                <li wire:key="alert-{{ $alert['Id'] }}"
                    class="flex flex-wrap items-center justify-between gap-4 rounded-lg border border-slate-200 bg-white p-4 shadow-workbc">
                    <div class="min-w-0">
                        <h2 class="truncate text-lg font-semibold text-slate-900">
                            {{ $alert['Title'] !== '' ? $alert['Title'] : 'Untitled alert' }}
                        </h2>
                        <p class="mt-1 text-sm text-slate-600">
                            Email frequency: <span class="font-medium text-slate-900">{{ $alert['Frequency'] }}</span>
                        </p>
                    </div>

                    <div class="flex items-center gap-2">
                        <x-button variant="secondary" :href="route('account.alerts.edit', ['alertId' => $alert['Id']])">
                            Edit<span class="sr-only"> alert {{ $alert['Title'] }}</span>
                        </x-button>
                        <x-button
                            variant="danger"
                            type="button"
                            wire:click="delete({{ $alert['Id'] }})"
                            wire:confirm="Delete this job alert? This can't be undone."
                        >
                            Delete<span class="sr-only"> alert {{ $alert['Title'] }}</span>
                        </x-button>
                    </div>
                </li>
            @endforeach
        </ul>
    @endif

</section>
</x-account.layout>
