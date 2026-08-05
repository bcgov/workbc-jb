@props([
    'type' => 'info',
    'title' => null,
    'dismissible' => false,
])

@php
    // Errors/warnings interrupt (role=alert, assertive); info/success are polite (role=status).
    $config = [
        'info' => ['role' => 'status', 'box' => 'bg-blue-50 border-blue-500 text-blue-900', 'label' => 'Information:'],
        'success' => ['role' => 'status', 'box' => 'bg-green-50 border-green-600 text-green-900', 'label' => 'Success:'],
        'warning' => ['role' => 'alert', 'box' => 'bg-amber-50 border-amber-500 text-amber-900', 'label' => 'Warning:'],
        'error' => ['role' => 'alert', 'box' => 'bg-red-50 border-red-600 text-red-900', 'label' => 'Error:'],
    ];
    $c = $config[$type] ?? $config['info'];

    // Distinct icon per type so meaning is not carried by colour alone (WCAG 1.4.1).
    $icons = [
        'info' => 'M10 18a8 8 0 100-16 8 8 0 000 16zm1-11a1 1 0 11-2 0 1 1 0 012 0zm-1 3a1 1 0 00-1 1v3a1 1 0 102 0v-3a1 1 0 00-1-1z',
        'success' => 'M16.7 6.3a1 1 0 010 1.4l-6.5 6.5a1 1 0 01-1.4 0L5.3 10.7a1 1 0 011.4-1.4l2.8 2.79 5.8-5.8a1 1 0 011.4 0z',
        'warning' => 'M8.3 3.2a2 2 0 013.4 0l6 10.2A2 2 0 0116 16.5H4a2 2 0 01-1.7-3.1l6-10.2zM10 8a1 1 0 00-1 1v2a1 1 0 102 0V9a1 1 0 00-1-1zm0 6a1 1 0 100 2 1 1 0 000-2z',
        'error' => 'M10 18a8 8 0 100-16 8 8 0 000 16zM9 9a1 1 0 012 0v4a1 1 0 11-2 0V9zm1-4a1 1 0 100 2 1 1 0 000-2z',
    ];
    $iconPath = $icons[$type] ?? $icons['info'];
@endphp

<div
    @if ($dismissible) x-data="{ show: true }" x-show="show" @endif
    role="{{ $c['role'] }}"
    {{ $attributes->merge(['class' => 'flex items-start gap-3 rounded-md border-l-4 p-4 ' . $c['box']]) }}
>
    <svg class="mt-0.5 size-5 shrink-0" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
        <path fill-rule="evenodd" d="{{ $iconPath }}" clip-rule="evenodd" />
    </svg>

    <div class="flex-1">
        {{-- Hidden text label gives non-visual users the alert type. --}}
        <span class="sr-only">{{ $c['label'] }}</span>
        @if ($title)
            <p class="font-semibold">{{ $title }}</p>
        @endif
        <div class="text-sm">{{ $slot }}</div>
    </div>

    @if ($dismissible)
        {{-- Alpine handles pure view state (show/hide); no server round-trip needed. --}}
        <button
            type="button"
            @click="show = false"
            aria-label="Dismiss this message"
            class="-m-1 rounded p-1 hover:bg-black/5 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-current"
        >
            <svg class="size-4" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                <path d="M6.28 5.22a.75.75 0 00-1.06 1.06L8.94 10l-3.72 3.72a.75.75 0 101.06 1.06L10 11.06l3.72 3.72a.75.75 0 101.06-1.06L11.06 10l3.72-3.72a.75.75 0 00-1.06-1.06L10 8.94 6.28 5.22z" />
            </svg>
        </button>
    @endif
</div>
