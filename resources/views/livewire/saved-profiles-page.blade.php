@php
    use Illuminate\Support\Carbon;
@endphp

<section aria-labelledby="saved-profiles-heading" class="space-y-8">
    <header class="space-y-1">
        <h1 id="saved-profiles-heading" class="text-3xl font-bold tracking-tight text-slate-900">Saved profiles</h1>
        <p class="text-slate-700">Career and industry profiles you saved while exploring WorkBC.</p>
    </header>

    <p class="sr-only" role="status" aria-live="polite" aria-atomic="true">{{ $statusMessage }}</p>

    <section aria-labelledby="saved-career-profiles-heading" class="space-y-4">
        <h2 id="saved-career-profiles-heading" class="text-xl font-semibold text-slate-900">
            Career profiles <span class="font-normal text-slate-600 tabular-nums">({{ count($careerProfiles) }})</span>
        </h2>

        @if ($careerProfiles === [])
            <x-alert type="info" title="No saved career profiles yet">
                Save a career profile while browsing careers on WorkBC to see it here.
            </x-alert>
        @else
            <ul class="space-y-3" aria-label="Saved career profiles list">
                @foreach ($careerProfiles as $profile)
                    <li class="flex flex-col gap-3 rounded-lg border border-slate-200 bg-white p-4 sm:flex-row sm:items-center sm:justify-between"
                        wire:key="career-profile-{{ $profile['profileId'] }}">
                        <div>
                            <p class="font-semibold text-slate-900">{{ $profile['title'] }}</p>
                            <p class="text-sm text-slate-600">
                                @if ($profile['code'])
                                    <span class="tabular-nums">NOC {{ $profile['code'] }}</span>
                                @endif
                                @if ($profile['savedAt'])
                                    <span class="ms-1">&middot; Saved {{ Carbon::parse($profile['savedAt'])->format('M j, Y') }}</span>
                                @endif
                            </p>
                        </div>
                        <x-button type="button" variant="secondary"
                                  wire:click="removeCareerProfile({{ $profile['profileId'] }})"
                                  aria-label="Remove saved career profile {{ $profile['title'] }}">
                            Remove
                        </x-button>
                    </li>
                @endforeach
            </ul>
        @endif
    </section>

    <section aria-labelledby="saved-industry-profiles-heading" class="space-y-4">
        <h2 id="saved-industry-profiles-heading" class="text-xl font-semibold text-slate-900">
            Industry profiles <span class="font-normal text-slate-600 tabular-nums">({{ count($industryProfiles) }})</span>
        </h2>

        @if ($industryProfiles === [])
            <x-alert type="info" title="No saved industry profiles yet">
                Save an industry profile while exploring industries on WorkBC to see it here.
            </x-alert>
        @else
            <ul class="space-y-3" aria-label="Saved industry profiles list">
                @foreach ($industryProfiles as $profile)
                    <li class="flex flex-col gap-3 rounded-lg border border-slate-200 bg-white p-4 sm:flex-row sm:items-center sm:justify-between"
                        wire:key="industry-profile-{{ $profile['profileId'] }}">
                        <div>
                            <p class="font-semibold text-slate-900">{{ $profile['title'] }}</p>
                            @if ($profile['savedAt'])
                                <p class="text-sm text-slate-600">Saved {{ Carbon::parse($profile['savedAt'])->format('M j, Y') }}</p>
                            @endif
                        </div>
                        <x-button type="button" variant="secondary"
                                  wire:click="removeIndustryProfile({{ $profile['profileId'] }})"
                                  aria-label="Remove saved industry profile {{ $profile['title'] }}">
                            Remove
                        </x-button>
                    </li>
                @endforeach
            </ul>
        @endif
    </section>
</section>
