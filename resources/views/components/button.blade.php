@props([
    'variant' => 'primary',
    'type' => 'button',
    'href' => null,
    'disabled' => false,
])

@php
    $base = 'inline-flex items-center justify-center gap-2 rounded-md px-4 py-2 text-sm font-semibold '
        .'transition focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-900';

    // All variants keep text/background contrast at >= 4.5:1 (WCAG 1.4.3).
    $variants = [
        'primary' => 'bg-blue-700 text-white hover:bg-blue-800',
        'secondary' => 'bg-white text-blue-800 ring-1 ring-inset ring-blue-700 hover:bg-blue-50',
        'danger' => 'bg-red-700 text-white hover:bg-red-800',
        'ghost' => 'bg-transparent text-slate-800 hover:bg-slate-100',
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
