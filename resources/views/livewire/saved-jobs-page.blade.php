@php
    use App\Support\JobSlug;
    use Illuminate\Support\Carbon;
@endphp

<section aria-labelledby="saved-jobs-heading" class="space-y-6">
    <header class="space-y-1">
        <h1 id="saved-jobs-heading" class="text-3xl font-bold tracking-tight text-slate-900">Saved jobs</h1>
        <p class="text-slate-700">Review jobs you saved, update notes, or remove jobs from your list.</p>
    </header>

    <p class="sr-only" role="status" aria-live="polite" aria-atomic="true">{{ $statusMessage }}</p>

    @if ($savedJobs === [])
        <x-alert type="info" title="No saved jobs yet">
            Save jobs from search results or a job detail page to see them here.
        </x-alert>
    @else
        <ul class="space-y-4" aria-label="Saved jobs list">
            @foreach ($savedJobs as $saved)
                @php
                    $expired = $saved['ExpireDate'] ? Carbon::parse($saved['ExpireDate'])->isPast() : false;
                    $detailTitle = is_string($saved['Title']) && $saved['Title'] !== '' ? $saved['Title'] : null;
                @endphp
                <li class="rounded-lg border border-slate-200 bg-white p-4" wire:key="saved-job-{{ $saved['JobId'] }}">
                    <article>
                        <header class="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
                            <div>
                                <h2 class="text-lg font-semibold text-slate-900">
                                    <a href="{{ route('jobs.show', ['job' => JobSlug::path($saved['JobId'], $detailTitle)]) }}" wire:navigate
                                       class="text-blue-800 hover:underline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900">
                                        {{ $saved['Title'] ?: 'Job '.$saved['JobId'] }}
                                    </a>
                                </h2>
                                <p class="mt-1 text-sm text-slate-700">
                                    @if ($saved['EmployerName'])
                                        <span class="font-medium">{{ $saved['EmployerName'] }}</span>
                                    @endif
                                    @if ($saved['EmployerName'] && $saved['City'])
                                        <span aria-hidden="true"> · </span>
                                    @endif
                                    @if ($saved['City'])
                                        {{ $saved['City'] }}
                                    @endif
                                </p>
                                @if ($saved['DateSaved'])
                                    <p class="mt-1 text-xs text-slate-500">Saved {{ Carbon::parse($saved['DateSaved'])->timezone('America/Vancouver')->format('M j, Y g:i a') }}</p>
                                @endif
                                @if ($expired)
                                    <p class="mt-1 text-xs font-medium text-amber-800">This posting may be expired.</p>
                                @endif
                            </div>
                            <button type="button"
                                    wire:click="unsave('{{ $saved['JobId'] }}')"
                                    aria-label="Unsave {{ $saved['Title'] ?: 'job '.$saved['JobId'] }}"
                                    class="inline-flex items-center rounded border border-slate-300 bg-white px-3 py-2 text-sm font-medium text-slate-800 hover:bg-slate-50 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900">
                                Unsave
                            </button>
                        </header>

                        <section class="mt-4 border-t border-slate-200 pt-4" aria-labelledby="note-heading-{{ $saved['Id'] }}">
                            <h3 id="note-heading-{{ $saved['Id'] }}" class="text-sm font-semibold text-slate-900">Note</h3>

                            @if ($editingJobId === $saved['JobId'])
                                <div class="mt-2 space-y-2">
                                    <label for="note-{{ $saved['Id'] }}" class="sr-only">Note for {{ $saved['Title'] ?: 'job '.$saved['JobId'] }}</label>
                                    <textarea id="note-{{ $saved['Id'] }}"
                                              wire:model="noteDraft"
                                              maxlength="800"
                                              rows="3"
                                              class="block w-full rounded-md border border-slate-400 px-3 py-2 text-slate-900 shadow-sm focus-visible:border-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900"></textarea>
                                    @error('noteDraft')
                                        <p class="text-sm font-medium text-red-800" role="alert">{{ $message }}</p>
                                    @enderror
                                    <p class="text-xs text-slate-600">Maximum 800 characters.</p>
                                    <div class="flex gap-2">
                                        <x-button type="button" wire:click="saveNote('{{ $saved['JobId'] }}')">Save note</x-button>
                                        <x-button type="button" variant="secondary" wire:click="cancelEditing">Cancel</x-button>
                                    </div>
                                </div>
                            @else
                                <div class="mt-2 space-y-2">
                                    <p class="text-sm text-slate-800">{{ $saved['Note'] ?: 'No note added.' }}</p>
                                    @if ($saved['NoteUpdatedDate'])
                                        <p class="text-xs text-slate-500">Last updated {{ Carbon::parse($saved['NoteUpdatedDate'])->timezone('America/Vancouver')->format('M j, Y g:i a') }}</p>
                                    @endif
                                    <x-button type="button" variant="secondary" wire:click="startEditing('{{ $saved['JobId'] }}')">
                                        {{ $saved['Note'] ? 'Edit note' : 'Add note' }}
                                    </x-button>
                                </div>
                            @endif
                        </section>
                    </article>
                </li>
            @endforeach
        </ul>
    @endif
</section>
