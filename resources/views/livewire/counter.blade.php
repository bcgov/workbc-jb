<div class="flex items-center gap-3">
    <x-button wire:click="decrement" aria-label="Decrease count" variant="secondary">&minus;</x-button>

    {{-- aria-live announces the new value after each Livewire re-render (WCAG 4.1.3). --}}
    <p class="min-w-32 text-center text-sm" aria-live="polite">
        Count: <span class="text-lg font-bold tabular-nums">{{ $count }}</span>
    </p>

    <x-button wire:click="increment" aria-label="Increase count" variant="secondary">+</x-button>
</div>
