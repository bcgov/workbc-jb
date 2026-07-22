@props([
    'label',
    'name',
    'type' => 'text',
    'id' => null,
    'value' => '',
    'required' => false,
    'hint' => null,
    'error' => null,
    'placeholder' => null,
])

@php
    $id = $id ?? $name;
    $hintId = $hint ? "{$id}-hint" : null;
    $errorId = $error ? "{$id}-error" : null;
    // Associate hint + error text with the control for screen readers (WCAG 3.3.2 / 1.3.1).
    $describedBy = collect([$hintId, $errorId])->filter()->implode(' ');
@endphp

<div class="space-y-1">
    {{-- Every input is bound to its own visible <label> via for/id (WCAG 4.1.2). --}}
    <label for="{{ $id }}" class="block text-sm font-medium text-slate-900">
        {{ $label }}
        @if ($required)
            <span class="text-red-700" aria-hidden="true">*</span>
            <span class="sr-only">(required)</span>
        @endif
    </label>

    @if ($hint)
        <p id="{{ $hintId }}" class="text-sm text-slate-600">{{ $hint }}</p>
    @endif

    @isset($control)
        {{ $control }}
    @else
        <input
            type="{{ $type }}"
            id="{{ $id }}"
            name="{{ $name }}"
            value="{{ $value }}"
            @if ($placeholder) placeholder="{{ $placeholder }}" @endif
            @if ($required) required aria-required="true" @endif
            @if ($error) aria-invalid="true" @endif
            @if ($describedBy) aria-describedby="{{ $describedBy }}" @endif
            {{ $attributes->merge(['class' => 'block w-full rounded-md border border-slate-400 px-3 py-2 text-slate-900 shadow-sm placeholder:text-slate-500 focus-visible:border-blue-700 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-blue-900 aria-invalid:border-red-700']) }}
        />
    @endisset

    @if ($error)
        {{-- Icon + text so the error is not signalled by colour alone (WCAG 1.4.1). --}}
        <p id="{{ $errorId }}" role="alert" class="flex items-center gap-1 text-sm font-medium text-red-800">
            <svg class="size-4 shrink-0" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM9 9a1 1 0 012 0v4a1 1 0 11-2 0V9zm1-4a1 1 0 100 2 1 1 0 000-2z" clip-rule="evenodd" />
            </svg>
            <span>{{ $error }}</span>
        </p>
    @endif
</div>
