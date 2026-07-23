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

    {{-- Location facet (SRCH-2). Accessible combobox: Alpine owns the open/active
         view state; Livewire owns the suggestion data, validation and committed
         locations. --}}
    <div
        class="grid gap-4 rounded-lg border border-slate-200 bg-white p-4 sm:grid-cols-[1fr_auto] sm:items-start"
        x-data="{
            open: false,
            activeIndex: -1,
            get options() { return Array.from(this.$refs.listbox ? this.$refs.listbox.querySelectorAll('[role=option]') : []); },
            hasOptions() { return this.options.length > 0; },
            activeId() { return this.activeIndex >= 0 ? 'location-option-' + this.activeIndex : null; },
            openIfOptions() { this.open = this.hasOptions(); },
            next() { this.open = true; const n = this.options.length; if (!n) return; this.activeIndex = (this.activeIndex + 1) % n; },
            prev() { this.open = true; const n = this.options.length; if (!n) return; this.activeIndex = (this.activeIndex - 1 + n) % n; },
            selectIndex(i) { const el = this.options[i]; if (!el) return; $wire.selectSuggestion(el.dataset.value); this.reset(); },
            onEnter() {
                if (this.activeIndex >= 0 && this.options[this.activeIndex]) { this.selectIndex(this.activeIndex); return; }
                const val = this.$refs.input.value.trim();
                if (val === '') return;
                $wire.set('locationInput', val).then(() => $wire.addLocation());
                this.reset();
            },
            onBlur() {
                this.close();
                const val = this.$refs.input.value.trim();
                if (val === '') return;
                // Validate typed-but-unselected text when leaving the field.
                $wire.set('locationInput', val).then(() => $wire.addLocation());
            },
            close() { this.open = false; this.activeIndex = -1; },
            reset() { this.open = false; this.activeIndex = -1; },
        }"
        x-on:keydown.escape="close()"
        x-on:click.outside="close()"
    >
        <div>
            <label for="location-input" class="block text-sm font-medium text-slate-900">City or postal code</label>
            <p id="location-hint" class="mt-1 text-sm text-slate-600">Type a city and choose a suggestion, or enter a postal code, then add it.</p>

            <div class="relative mt-1">
                <input
                    id="location-input"
                    x-ref="input"
                    type="text"
                    autocomplete="off"
                    role="combobox"
                    aria-controls="location-listbox"
                    aria-autocomplete="list"
                    aria-expanded="false"
                    x-bind:aria-expanded="(open && hasOptions()).toString()"
                    x-bind:aria-activedescendant="activeId()"
                    aria-describedby="location-hint @if ($locationError) location-error @endif"
                    wire:model.live.debounce.300ms="locationInput"
                    x-on:focus="openIfOptions()"
                    x-on:input="open = true"
                    x-on:blur="onBlur()"
                    x-on:keydown.arrow-down.prevent="next()"
                    x-on:keydown.arrow-up.prevent="prev()"
                    x-on:keydown.enter.prevent="onEnter()"
                    class="block w-full rounded-md border border-slate-400 px-3 py-2 text-slate-900 shadow-sm placeholder:text-slate-500 focus-visible:border-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900"
                    placeholder="e.g. Victoria or V8W 1P6"
                />

                <ul
                    id="location-listbox"
                    x-ref="listbox"
                    role="listbox"
                    aria-label="City suggestions"
                    x-show="open && hasOptions()"
                    x-cloak
                    class="absolute z-10 mt-1 max-h-60 w-full overflow-auto rounded-md border border-slate-300 bg-white py-1 shadow-lg"
                >
                    @foreach ($suggestions as $i => $city)
                        <li
                            id="location-option-{{ $i }}"
                            role="option"
                            data-value="{{ $city }}"
                            aria-selected="false"
                            x-bind:aria-selected="(activeIndex === {{ $i }}).toString()"
                            x-bind:class="activeIndex === {{ $i }} ? 'bg-blue-50 text-blue-900' : 'text-slate-900'"
                            x-on:mousedown.prevent="selectIndex({{ $i }})"
                            x-on:mouseenter="activeIndex = {{ $i }}"
                            class="cursor-pointer px-3 py-2 text-sm"
                        >
                            {{ $city }}
                        </li>
                    @endforeach
                </ul>
            </div>

            {{-- Validation errors are announced assertively. --}}
            <p id="location-error" role="alert" aria-live="assertive" class="mt-1 min-h-5 text-sm font-medium text-red-800">
                @if ($locationError)
                    <span class="inline-flex items-center gap-1">
                        <svg class="size-4 shrink-0" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM9 9a1 1 0 012 0v4a1 1 0 11-2 0V9zm1-4a1 1 0 100 2 1 1 0 000-2z" clip-rule="evenodd" />
                        </svg>
                        <span>{{ $locationError }}</span>
                    </span>
                @endif
            </p>

            <div class="mt-2">
                <x-button type="button" variant="secondary" wire:click="addLocation">Add location</x-button>
            </div>

            {{-- Committed locations. --}}
            @if (! empty($locations))
                <ul aria-label="Selected locations" class="mt-3 flex flex-wrap gap-2">
                    @foreach ($locations as $i => $loc)
                        @php
                            $label = $loc['City'] ?? $loc['Region'] ?? (isset($loc['Postal']) ? \App\Search\Filters\LocationField::fromArray($loc)->getPostal() : 'Location');
                        @endphp
                        <li class="inline-flex items-center gap-1 rounded-full bg-blue-50 py-1 pl-3 pr-1 text-sm text-blue-900">
                            <span>{{ $label }}</span>
                            <button
                                type="button"
                                wire:click="removeLocation({{ $i }})"
                                aria-label="Remove location {{ $label }}"
                                class="inline-flex size-5 items-center justify-center rounded-full hover:bg-blue-100 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900"
                            >
                                <svg class="size-3.5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                                    <path d="M6.28 5.22a.75.75 0 00-1.06 1.06L8.94 10l-3.72 3.72a.75.75 0 101.06 1.06L10 11.06l3.72 3.72a.75.75 0 101.06-1.06L11.06 10l3.72-3.72a.75.75 0 00-1.06-1.06L10 8.94 6.28 5.22z" />
                                </svg>
                            </button>
                        </li>
                    @endforeach
                </ul>
            @endif
        </div>

        <div>
            <label for="distance" class="block text-sm font-medium text-slate-900">Distance</label>
            <select id="distance" wire:model.live="distance"
                    class="mt-1 block w-full rounded-md border border-slate-400 px-3 py-2 text-slate-900 shadow-sm focus-visible:border-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900 sm:w-44">
                @foreach ($distanceOptions as $value => $label)
                    <option value="{{ $value }}">{{ $label }}</option>
                @endforeach
            </select>
        </div>
    </div>

    {{-- Standard filter facets (SRCH-3). Each dropdown is an accessible disclosure:
         Alpine owns the open/close view state; Livewire owns the checkbox/radio
         selections (the data) and applies them. AND across facets, OR within. --}}
    @php
        $jobTypeCount = count($hours) + count($period) + count($terms) + count($workplace);
        $salaryCount = count($salaryBrackets) + ($salaryCustom ? 1 : 0)
            + ($salaryUnknown ? 1 : 0) + count($salaryConditions);
        $moreCount = count($equityGroups) + ($postingLanguage !== '1' ? 1 : 0)
            + (trim($nocCode) !== '' ? 1 : 0) + ($jobSource !== '0' ? 1 : 0)
            + ($excludePlacementAgency ? 1 : 0);
        $activeFilterCount = $jobTypeCount + count($industries) + count($educationLevels)
            + ($dateSelection !== '0' ? 1 : 0) + $salaryCount + $moreCount + count($locations);
    @endphp
    <div class="flex flex-wrap items-center gap-3" role="group" aria-label="Filter results">
        {{-- Job type --}}
        <div x-data="{ open: false }" x-on:keydown.escape="open = false" x-on:click.outside="open = false" class="relative">
            <button
                type="button"
                x-on:click="open = ! open"
                aria-controls="facet-jobtype"
                aria-expanded="false"
                x-bind:aria-expanded="open.toString()"
                class="inline-flex items-center gap-1.5 rounded-md border border-slate-400 bg-white px-3 py-2 text-sm font-medium text-slate-900 shadow-sm hover:bg-slate-50 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900"
            >
                <span>Job type</span>
                @if ($jobTypeCount > 0)
                    <span class="inline-flex min-w-5 items-center justify-center rounded-full bg-blue-700 px-1.5 text-xs font-semibold text-white">{{ $jobTypeCount }}<span class="sr-only"> selected</span></span>
                @endif
                <svg class="size-4 text-slate-500" x-bind:class="open ? 'rotate-180' : ''" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                    <path fill-rule="evenodd" d="M5.22 8.22a.75.75 0 011.06 0L10 11.94l3.72-3.72a.75.75 0 111.06 1.06l-4.25 4.25a.75.75 0 01-1.06 0L5.22 9.28a.75.75 0 010-1.06z" clip-rule="evenodd" />
                </svg>
            </button>
            <div id="facet-jobtype" x-show="open" x-cloak role="group" aria-label="Job type"
                 class="absolute z-20 mt-1 max-h-96 w-72 space-y-4 overflow-auto rounded-md border border-slate-300 bg-white p-4 shadow-lg">
                <fieldset>
                    <legend class="text-xs font-semibold uppercase tracking-wide text-slate-500">Hours</legend>
                    @foreach ($jobTypeHoursOptions as $key => $label)
                        <label class="flex items-center gap-2 py-1 text-sm text-slate-900">
                            <input type="checkbox" value="{{ $key }}" wire:model.live="hours"
                                   class="size-4 rounded border-slate-400 text-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                            <span>{{ $label }}</span>
                        </label>
                    @endforeach
                </fieldset>
                <fieldset>
                    <legend class="text-xs font-semibold uppercase tracking-wide text-slate-500">Employment period</legend>
                    @foreach ($jobTypePeriodOptions as $key => $label)
                        <label class="flex items-center gap-2 py-1 text-sm text-slate-900">
                            <input type="checkbox" value="{{ $key }}" wire:model.live="period"
                                   class="size-4 rounded border-slate-400 text-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                            <span>{{ $label }}</span>
                        </label>
                    @endforeach
                </fieldset>
                <fieldset>
                    <legend class="text-xs font-semibold uppercase tracking-wide text-slate-500">Shift</legend>
                    @foreach ($jobTypeTermsOptions as $key => $label)
                        <label class="flex items-center gap-2 py-1 text-sm text-slate-900">
                            <input type="checkbox" value="{{ $key }}" wire:model.live="terms"
                                   class="size-4 rounded border-slate-400 text-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                            <span>{{ $label }}</span>
                        </label>
                    @endforeach
                </fieldset>
                <fieldset>
                    <legend class="text-xs font-semibold uppercase tracking-wide text-slate-500">Workplace</legend>
                    @foreach ($workplaceOptions as $key => $label)
                        <label class="flex items-center gap-2 py-1 text-sm text-slate-900">
                            <input type="checkbox" value="{{ $key }}" wire:model.live="workplace"
                                   class="size-4 rounded border-slate-400 text-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                            <span>{{ $label }}</span>
                        </label>
                    @endforeach
                </fieldset>
            </div>
        </div>

        {{-- Industry --}}
        <div x-data="{ open: false }" x-on:keydown.escape="open = false" x-on:click.outside="open = false" class="relative">
            <button
                type="button"
                x-on:click="open = ! open"
                aria-controls="facet-industry"
                aria-expanded="false"
                x-bind:aria-expanded="open.toString()"
                class="inline-flex items-center gap-1.5 rounded-md border border-slate-400 bg-white px-3 py-2 text-sm font-medium text-slate-900 shadow-sm hover:bg-slate-50 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900"
            >
                <span>Industry</span>
                @if (count($industries) > 0)
                    <span class="inline-flex min-w-5 items-center justify-center rounded-full bg-blue-700 px-1.5 text-xs font-semibold text-white">{{ count($industries) }}<span class="sr-only"> selected</span></span>
                @endif
                <svg class="size-4 text-slate-500" x-bind:class="open ? 'rotate-180' : ''" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                    <path fill-rule="evenodd" d="M5.22 8.22a.75.75 0 011.06 0L10 11.94l3.72-3.72a.75.75 0 111.06 1.06l-4.25 4.25a.75.75 0 01-1.06 0L5.22 9.28a.75.75 0 010-1.06z" clip-rule="evenodd" />
                </svg>
            </button>
            <div id="facet-industry" x-show="open" x-cloak role="group" aria-label="Industry"
                 class="absolute z-20 mt-1 max-h-96 w-80 overflow-auto rounded-md border border-slate-300 bg-white p-4 shadow-lg">
                @foreach ($industryOptions as $id => $label)
                    <label class="flex items-center gap-2 py-1 text-sm text-slate-900">
                        <input type="checkbox" value="{{ $id }}" wire:model.live="industries"
                               class="size-4 rounded border-slate-400 text-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                        <span>{{ $label }}</span>
                    </label>
                @endforeach
            </div>
        </div>

        {{-- Education --}}
        <div x-data="{ open: false }" x-on:keydown.escape="open = false" x-on:click.outside="open = false" class="relative">
            <button
                type="button"
                x-on:click="open = ! open"
                aria-controls="facet-education"
                aria-expanded="false"
                x-bind:aria-expanded="open.toString()"
                class="inline-flex items-center gap-1.5 rounded-md border border-slate-400 bg-white px-3 py-2 text-sm font-medium text-slate-900 shadow-sm hover:bg-slate-50 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900"
            >
                <span>Education</span>
                @if (count($educationLevels) > 0)
                    <span class="inline-flex min-w-5 items-center justify-center rounded-full bg-blue-700 px-1.5 text-xs font-semibold text-white">{{ count($educationLevels) }}<span class="sr-only"> selected</span></span>
                @endif
                <svg class="size-4 text-slate-500" x-bind:class="open ? 'rotate-180' : ''" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                    <path fill-rule="evenodd" d="M5.22 8.22a.75.75 0 011.06 0L10 11.94l3.72-3.72a.75.75 0 111.06 1.06l-4.25 4.25a.75.75 0 01-1.06 0L5.22 9.28a.75.75 0 010-1.06z" clip-rule="evenodd" />
                </svg>
            </button>
            <div id="facet-education" x-show="open" x-cloak role="group" aria-label="Education"
                 class="absolute z-20 mt-1 w-72 overflow-auto rounded-md border border-slate-300 bg-white p-4 shadow-lg">
                @foreach ($educationOptions as $level)
                    <label class="flex items-center gap-2 py-1 text-sm text-slate-900">
                        <input type="checkbox" value="{{ $level }}" wire:model.live="educationLevels"
                               class="size-4 rounded border-slate-400 text-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                        <span>{{ $level }}</span>
                    </label>
                @endforeach
            </div>
        </div>

        {{-- Date posted --}}
        <div x-data="{ open: false }" x-on:keydown.escape="open = false" x-on:click.outside="open = false" class="relative">
            <button
                type="button"
                x-on:click="open = ! open"
                aria-controls="facet-date"
                aria-expanded="false"
                x-bind:aria-expanded="open.toString()"
                class="inline-flex items-center gap-1.5 rounded-md border border-slate-400 bg-white px-3 py-2 text-sm font-medium text-slate-900 shadow-sm hover:bg-slate-50 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900"
            >
                <span>Date posted</span>
                @if ($dateSelection !== '0')
                    <span class="inline-flex items-center justify-center rounded-full bg-blue-700 px-2 text-xs font-semibold text-white">{{ $dateOptions[$dateSelection] ?? '' }}</span>
                @endif
                <svg class="size-4 text-slate-500" x-bind:class="open ? 'rotate-180' : ''" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                    <path fill-rule="evenodd" d="M5.22 8.22a.75.75 0 011.06 0L10 11.94l3.72-3.72a.75.75 0 111.06 1.06l-4.25 4.25a.75.75 0 01-1.06 0L5.22 9.28a.75.75 0 010-1.06z" clip-rule="evenodd" />
                </svg>
            </button>
            <div id="facet-date" x-show="open" x-cloak class="absolute z-20 mt-1 w-72 rounded-md border border-slate-300 bg-white p-4 shadow-lg">
                <fieldset>
                    <legend class="text-xs font-semibold uppercase tracking-wide text-slate-500">Date posted</legend>
                    @foreach ($dateOptions as $value => $label)
                        <label class="flex items-center gap-2 py-1 text-sm text-slate-900">
                            <input type="radio" name="dateSelection" value="{{ $value }}" wire:model.live="dateSelection"
                                   class="size-4 border-slate-400 text-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                            <span>{{ $label }}</span>
                        </label>
                    @endforeach
                </fieldset>
                @if ($dateSelection === '3')
                    <div class="mt-3 space-y-2 border-t border-slate-200 pt-3">
                        <div>
                            <label for="date-start" class="block text-sm font-medium text-slate-900">From</label>
                            <input id="date-start" type="date" wire:model.live="startDate"
                                   class="mt-1 block w-full rounded-md border border-slate-400 px-3 py-2 text-slate-900 shadow-sm focus-visible:border-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                        </div>
                        <div>
                            <label for="date-end" class="block text-sm font-medium text-slate-900">To</label>
                            <input id="date-end" type="date" wire:model.live="endDate"
                                   class="mt-1 block w-full rounded-md border border-slate-400 px-3 py-2 text-slate-900 shadow-sm focus-visible:border-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                        </div>
                    </div>
                @endif
            </div>
        </div>

        {{-- Salary --}}
        <div x-data="{ open: false }" x-on:keydown.escape="open = false" x-on:click.outside="open = false" class="relative">
            <button
                type="button"
                x-on:click="open = ! open"
                aria-controls="facet-salary"
                aria-expanded="false"
                x-bind:aria-expanded="open.toString()"
                class="inline-flex items-center gap-1.5 rounded-md border border-slate-400 bg-white px-3 py-2 text-sm font-medium text-slate-900 shadow-sm hover:bg-slate-50 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900"
            >
                <span>Salary</span>
                @if ($salaryCount > 0)
                    <span class="inline-flex min-w-5 items-center justify-center rounded-full bg-blue-700 px-1.5 text-xs font-semibold text-white">{{ $salaryCount }}<span class="sr-only"> selected</span></span>
                @endif
                <svg class="size-4 text-slate-500" x-bind:class="open ? 'rotate-180' : ''" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                    <path fill-rule="evenodd" d="M5.22 8.22a.75.75 0 011.06 0L10 11.94l3.72-3.72a.75.75 0 111.06 1.06l-4.25 4.25a.75.75 0 01-1.06 0L5.22 9.28a.75.75 0 010-1.06z" clip-rule="evenodd" />
                </svg>
            </button>
            <div id="facet-salary" x-show="open" x-cloak role="group" aria-label="Salary"
                 class="absolute z-20 mt-1 max-h-96 w-80 space-y-4 overflow-auto rounded-md border border-slate-300 bg-white p-4 shadow-lg">
                <div>
                    <label for="salary-type" class="block text-xs font-semibold uppercase tracking-wide text-slate-500">Pay type</label>
                    <select id="salary-type" wire:model.live="salaryType"
                            class="mt-1 block w-full rounded-md border border-slate-400 px-3 py-2 text-sm text-slate-900 shadow-sm focus-visible:border-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                        @foreach ($salaryTypeOptions as $value => $label)
                            <option value="{{ $value }}">{{ $label }}</option>
                        @endforeach
                    </select>
                </div>
                <fieldset>
                    <legend class="text-xs font-semibold uppercase tracking-wide text-slate-500">Amount</legend>
                    @foreach ($salaryBracketLabels as $bracket => $label)
                        <label class="flex items-center gap-2 py-1 text-sm text-slate-900">
                            <input type="checkbox" value="{{ $bracket }}" wire:model.live="salaryBrackets"
                                   class="size-4 rounded border-slate-400 text-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                            <span>{{ $label }}</span>
                        </label>
                    @endforeach
                    <label class="flex items-center gap-2 py-1 text-sm text-slate-900">
                        <input type="checkbox" wire:model.live="salaryCustom"
                               class="size-4 rounded border-slate-400 text-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                        <span>Custom range</span>
                    </label>
                    @if ($salaryCustom)
                        <div class="mt-2 grid grid-cols-2 gap-2 border-t border-slate-200 pt-2">
                            <div>
                                <label for="salary-min" class="block text-sm font-medium text-slate-900">Min</label>
                                <input id="salary-min" type="number" min="0" inputmode="decimal" wire:model.live="salaryMin"
                                       class="mt-1 block w-full rounded-md border border-slate-400 px-3 py-2 text-slate-900 shadow-sm focus-visible:border-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                            </div>
                            <div>
                                <label for="salary-max" class="block text-sm font-medium text-slate-900">Max</label>
                                <input id="salary-max" type="number" min="0" inputmode="decimal" wire:model.live="salaryMax"
                                       class="mt-1 block w-full rounded-md border border-slate-400 px-3 py-2 text-slate-900 shadow-sm focus-visible:border-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                            </div>
                        </div>
                    @endif
                    <label class="mt-2 flex items-center gap-2 border-t border-slate-200 py-1 pt-2 text-sm text-slate-900">
                        <input type="checkbox" wire:model.live="salaryUnknown"
                               class="size-4 rounded border-slate-400 text-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                        <span>Include jobs with no salary listed</span>
                    </label>
                </fieldset>
                <fieldset>
                    <legend class="text-xs font-semibold uppercase tracking-wide text-slate-500">Benefits</legend>
                    @foreach ($salaryConditionOptions as $condition)
                        <label class="flex items-center gap-2 py-1 text-sm text-slate-900">
                            <input type="checkbox" value="{{ $condition }}" wire:model.live="salaryConditions"
                                   class="size-4 rounded border-slate-400 text-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                            <span>{{ $condition }}</span>
                        </label>
                    @endforeach
                </fieldset>
            </div>
        </div>

        {{-- More filters (SRCH-5) --}}
        <div x-data="{ open: false }" x-on:keydown.escape="open = false" x-on:click.outside="open = false" class="relative">
            <button
                type="button"
                x-on:click="open = ! open"
                aria-controls="facet-more"
                aria-expanded="false"
                x-bind:aria-expanded="open.toString()"
                class="inline-flex items-center gap-1.5 rounded-md border border-slate-400 bg-white px-3 py-2 text-sm font-medium text-slate-900 shadow-sm hover:bg-slate-50 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900"
            >
                <span>More filters</span>
                @if ($moreCount > 0)
                    <span class="inline-flex min-w-5 items-center justify-center rounded-full bg-blue-700 px-1.5 text-xs font-semibold text-white">{{ $moreCount }}<span class="sr-only"> selected</span></span>
                @endif
                <svg class="size-4 text-slate-500" x-bind:class="open ? 'rotate-180' : ''" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                    <path fill-rule="evenodd" d="M5.22 8.22a.75.75 0 011.06 0L10 11.94l3.72-3.72a.75.75 0 111.06 1.06l-4.25 4.25a.75.75 0 01-1.06 0L5.22 9.28a.75.75 0 010-1.06z" clip-rule="evenodd" />
                </svg>
            </button>
            <div id="facet-more" x-show="open" x-cloak role="group" aria-label="More filters"
                 class="absolute z-20 mt-1 max-h-96 w-80 space-y-4 overflow-auto rounded-md border border-slate-300 bg-white p-4 shadow-lg">
                <fieldset>
                    <legend class="text-xs font-semibold uppercase tracking-wide text-slate-500">Employment groups</legend>
                    @foreach ($equityOptions as $key => $label)
                        <label class="flex items-center gap-2 py-1 text-sm text-slate-900">
                            <input type="checkbox" value="{{ $key }}" wire:model.live="equityGroups"
                                   class="size-4 rounded border-slate-400 text-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                            <span>{{ $label }}</span>
                        </label>
                    @endforeach
                </fieldset>
                <div>
                    <label for="more-noc" class="block text-xs font-semibold uppercase tracking-wide text-slate-500">NOC 2021 code</label>
                    <input id="more-noc" type="text" inputmode="numeric" autocomplete="off" placeholder="5-digit NOC code"
                           wire:model.live.debounce.400ms="nocCode"
                           class="mt-1 block w-full rounded-md border border-slate-400 px-3 py-2 text-sm text-slate-900 shadow-sm focus-visible:border-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                </div>
                <div>
                    <label for="more-source" class="block text-xs font-semibold uppercase tracking-wide text-slate-500">Job source</label>
                    <select id="more-source" wire:model.live="jobSource"
                            class="mt-1 block w-full rounded-md border border-slate-400 px-3 py-2 text-sm text-slate-900 shadow-sm focus-visible:border-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                        @foreach ($jobSourceOptions as $value => $label)
                            <option value="{{ $value }}">{{ $label }}</option>
                        @endforeach
                    </select>
                </div>
                <label class="flex items-center gap-2 py-1 text-sm text-slate-900">
                    <input type="checkbox" wire:model.live="excludePlacementAgency"
                           class="size-4 rounded border-slate-400 text-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                    <span>Exclude placement agency jobs</span>
                </label>
                <fieldset>
                    <legend class="text-xs font-semibold uppercase tracking-wide text-slate-500">Job posting language</legend>
                    @foreach ($postingLanguageOptions as $value => $label)
                        <label class="flex items-center gap-2 py-1 text-sm text-slate-900">
                            <input type="radio" name="postingLanguage" value="{{ $value }}" wire:model.live="postingLanguage"
                                   class="size-4 border-slate-400 text-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                            <span>{{ $label }}</span>
                        </label>
                    @endforeach
                </fieldset>
            </div>
        </div>

        {{-- Clear all filters (referenced by the empty-state hint). --}}
        @if ($activeFilterCount > 0)
            <button type="button" wire:click="clearFilters"
                    class="inline-flex items-center gap-1.5 rounded-md px-3 py-2 text-sm font-medium text-blue-800 underline hover:text-blue-900 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                Clear filters<span class="sr-only"> ({{ $activeFilterCount }} active)</span>
            </button>
        @endif

        {{-- SRCH-6: copy a shareable link that reconstructs the current filters. --}}
        <div x-data="{ copied: false }" class="inline-flex items-center gap-2">
            <button type="button"
                    x-on:click="
                        navigator.clipboard?.writeText(@js($shareUrl)).then(() => {
                            copied = true;
                            setTimeout(() => copied = false, 2000);
                        })
                    "
                    class="inline-flex items-center gap-1.5 rounded-md px-3 py-2 text-sm font-medium text-blue-800 underline hover:text-blue-900 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900">
                Copy search link
            </button>
            <span role="status" aria-live="polite" class="text-sm text-green-700" x-cloak x-show="copied">
                Link copied
            </span>
        </div>
    </div>

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
