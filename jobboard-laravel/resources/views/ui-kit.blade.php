<x-layouts.app title="UI Kit — WorkBC Job Board">
    <div class="space-y-12">
        <div class="space-y-2">
            <h1 class="text-3xl font-bold">Component library</h1>
            <p class="max-w-2xl text-slate-700">
                The internal, server-rendered Blade component set. Every component targets WCAG&nbsp;2.1&nbsp;AA:
                keyboard operable, visible focus, sufficient contrast, and correct ARIA. This page is the
                automated accessibility (pa11y) target in CI.
            </p>
        </div>

        {{-- Buttons ------------------------------------------------------------ --}}
        <section aria-labelledby="buttons-heading" class="space-y-4">
            <h2 id="buttons-heading" class="text-xl font-semibold">Buttons</h2>
            <div class="flex flex-wrap items-center gap-3">
                <x-button>Primary</x-button>
                <x-button variant="secondary">Secondary</x-button>
                <x-button variant="danger">Danger</x-button>
                <x-button variant="ghost">Ghost</x-button>
                <x-button href="{{ route('ui-kit') }}">Link button</x-button>
                <x-button disabled>Disabled</x-button>
            </div>
        </section>

        {{-- Form fields -------------------------------------------------------- --}}
        <section aria-labelledby="fields-heading" class="space-y-4">
            <h2 id="fields-heading" class="text-xl font-semibold">Form fields</h2>
            <form class="grid max-w-xl gap-5" novalidate>
                <x-form-field
                    name="full_name"
                    label="Full name"
                    :required="true"
                    hint="As it appears on your government ID." />

                <x-form-field
                    name="email"
                    type="email"
                    label="Email address"
                    placeholder="you@example.com" />

                <x-form-field
                    name="postal_code"
                    label="Postal code"
                    value="V6"
                    error="Enter a valid Canadian postal code (e.g. V6B 1A1)." />

                <div>
                    <x-button type="submit">Submit</x-button>
                </div>
            </form>
        </section>

        {{-- Alerts ------------------------------------------------------------- --}}
        <section aria-labelledby="alerts-heading" class="space-y-4">
            <h2 id="alerts-heading" class="text-xl font-semibold">Alerts</h2>
            <div class="grid gap-3">
                <x-alert type="info" title="Heads up">Your saved search will email you daily.</x-alert>
                <x-alert type="success" title="Saved">Your job alert was created.</x-alert>
                <x-alert type="warning" title="Session expiring">You will be signed out in 5 minutes.</x-alert>
                <x-alert type="error" title="Something went wrong" dismissible>
                    We could not save your changes. Try again.
                </x-alert>
            </div>
        </section>

        {{-- Livewire (data) ---------------------------------------------------- --}}
        <section aria-labelledby="livewire-heading" class="space-y-4">
            <h2 id="livewire-heading" class="text-xl font-semibold">Livewire &mdash; reactive data</h2>
            <p class="max-w-2xl text-slate-700">
                State lives on the server; each click is a round-trip that re-renders. Use Livewire for
                data-backed reactivity (search filters, forms).
            </p>
            <livewire:counter />
        </section>

        {{-- Alpine (view state) ------------------------------------------------ --}}
        <section aria-labelledby="alpine-heading" class="space-y-4">
            <h2 id="alpine-heading" class="text-xl font-semibold">Alpine &mdash; view state</h2>
            <p class="max-w-2xl text-slate-700">
                A disclosure toggle handled entirely in the browser &mdash; no server round-trip. Use Alpine
                for pure view state (toggles, dropdowns, modals).
            </p>
            <div x-data="{ open: false }" class="max-w-xl">
                <x-button
                    variant="secondary"
                    x-on:click="open = ! open"
                    x-bind:aria-expanded="open.toString()"
                    aria-controls="disclosure-panel">
                    <span x-text="open ? 'Hide details' : 'Show details'">Show details</span>
                </x-button>
                <div id="disclosure-panel" x-show="open" x-cloak class="mt-3 rounded-md border border-slate-200 bg-white p-4 text-sm text-slate-700">
                    This panel's visibility is view state, so Alpine owns it. <code>aria-expanded</code> and
                    <code>aria-controls</code> keep the toggle accessible.
                </div>
            </div>
        </section>

        {{-- Pagination --------------------------------------------------------- --}}
        <section aria-labelledby="pagination-heading" class="space-y-4">
            <h2 id="pagination-heading" class="text-xl font-semibold">Pagination</h2>
            <x-pagination :paginator="$paginator" />
        </section>
    </div>
</x-layouts.app>
