{{--
    SRCH-12 — BC economic-region selector.

    An enhancement over the typed city/postal input: click (or keyboard-activate)
    a region of B.C. to add/remove it as a Region location filter. The backend is
    unchanged — a region resolves to the index's Region.keyword term through the
    same location pipeline as cities/postals.

    Alpine owns the hover/focus tooltip (pure view state); Livewire owns the
    committed regions (data) and the active highlight. Each region is a focusable
    button with an accessible name, so the map is never the only way to choose a
    region — the city/postal combobox above remains the primary, accessible path.

    Geometry: config/bc_regions.php, extracted verbatim from the old app's
    #region-map SVG (viewBox 0 0 270 290); keys are the exact Region.keyword values.
--}}
@php $regions = config('bc_regions', []); @endphp

<div x-data="{ hovered: '' }" class="mt-2">
    {{-- The disclosure button above is the visible label; keep an sr-only name so
         the SVG group is still announced. --}}
    <p id="region-map-label" class="sr-only">Regions of British Columbia</p>
    <p id="region-map-hint" class="text-sm text-slate-600">
        Select one or more regions on the map. Selected regions are highlighted; activate a region again to remove it.
    </p>

    <div class="relative mt-2 inline-block">
        <svg viewBox="0 0 270 290" role="group"
             aria-labelledby="region-map-label" aria-describedby="region-map-hint"
             class="region-map h-72 w-auto max-w-full" xmlns="http://www.w3.org/2000/svg">
            @foreach ($regions as $name => $d)
                @php $isActive = in_array($name, $selectedRegions, true); @endphp
                <path
                    d="{{ $d }}"
                    role="button"
                    tabindex="0"
                    aria-label="{{ $name }}"
                    aria-pressed="{{ $isActive ? 'true' : 'false' }}"
                    @class(['is-active' => $isActive])
                    x-on:click="$wire.toggleRegion(@js($name))"
                    x-on:keydown.enter.prevent="$wire.toggleRegion(@js($name))"
                    x-on:keydown.space.prevent="$wire.toggleRegion(@js($name))"
                    x-on:mouseenter="hovered = @js($name)"
                    x-on:mouseleave="hovered = ''"
                    x-on:focus="hovered = @js($name)"
                    x-on:blur="hovered = ''"
                />
            @endforeach
        </svg>

        {{-- Hover/focus tooltip — decorative; each path already carries its name. --}}
        <div x-show="hovered !== ''" x-cloak aria-hidden="true"
             class="pointer-events-none absolute left-1/2 top-1 -translate-x-1/2 rounded bg-workbc-navy px-2 py-1 text-xs font-medium text-white shadow-workbc"
             x-text="hovered"></div>
    </div>
</div>
