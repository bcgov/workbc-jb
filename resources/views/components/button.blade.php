@props([
    'variant' => 'primary',
    'type' => 'button',
    'href' => null,
    'disabled' => false,
])

@php
    // WorkBC button: 2px border, bold, 6px radius (brand-alignment.md).
    $base = 'inline-flex items-center justify-center gap-2 rounded-md border-2 px-6 py-2 text-sm font-bold '
        .'transition-colors focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-workbc-navy';

    // All variants keep text/background contrast at >= 4.5:1 (WCAG 1.4.3);
    // #2e6ab0 on white and white on #2e6ab0 both measure ≈5.5:1.
    $variants = [
        // Signature WorkBC treatment: solid blue that inverts to blue-on-mist on hover/focus.
        'primary' => 'border-workbc-blue bg-workbc-blue text-white hover:bg-workbc-mist hover:text-workbc-blue focus-visible:bg-workbc-mist focus-visible:text-workbc-blue',
        // Outlined blue on white, fills to mist on hover.
        'secondary' => 'border-workbc-blue bg-white text-workbc-blue hover:bg-workbc-mist',
        'danger' => 'border-red-700 bg-red-700 text-white hover:bg-red-800',
        'ghost' => 'border-transparent bg-transparent text-workbc-ink hover:bg-workbc-mist',
    ];

    $classes = $base . ' ' . ($variants[$variant] ?? $variants['primary']);
    $disabledClasses = ' cursor-not-allowed opacity-60';
@endphp

@if ($href && ! $disabled)
    <a href="{{ $href }}" {{ $attributes->merge(['class' => $classes]) }}>
        {{ $slot }}
    </a>
@elseif ($href)
    {{-- A disabled link is not focusable/actionable, so expose the disabled state to AT. --}}
    <a role="link" aria-disabled="true" {{ $attributes->merge(['class' => $classes . $disabledClasses]) }}>
        {{ $slot }}
    </a>
@else
    <button
        type="{{ $type }}"
        @disabled($disabled)
        {{ $attributes->merge(['class' => $classes . ($disabled ? $disabledClasses : '')]) }}
    >
        {{ $slot }}
    </button>
@endif
