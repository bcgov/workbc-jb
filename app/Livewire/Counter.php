<?php

namespace App\Livewire;

use Livewire\Component;

/**
 * Trivial data-backed reactive component.
 *
 * Demonstrates the "Livewire for data" side of ADR-002: state lives on the
 * server and each button click is a server round-trip that re-renders the view.
 * Contrast with the Alpine toggle on the UI Kit page, which is pure view state.
 */
final class Counter extends Component
{
    public int $count = 0;

    public function increment(): void
    {
        $this->count++;
    }

    public function decrement(): void
    {
        $this->count--;
    }

    public function render()
    {
        return view('livewire.counter');
    }
}
