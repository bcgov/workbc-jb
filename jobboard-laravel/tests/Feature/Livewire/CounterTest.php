<?php

namespace Tests\Feature\Livewire;

use App\Livewire\Counter;
use Livewire\Livewire;
use Tests\TestCase;

class CounterTest extends TestCase
{
    public function test_it_starts_at_zero(): void
    {
        Livewire::test(Counter::class)
            ->assertSet('count', 0)
            ->assertSee('Count:');
    }

    public function test_it_increments(): void
    {
        Livewire::test(Counter::class)
            ->call('increment')
            ->assertSet('count', 1)
            ->call('increment')
            ->assertSet('count', 2);
    }

    public function test_it_decrements(): void
    {
        Livewire::test(Counter::class)
            ->call('increment')
            ->call('decrement')
            ->assertSet('count', 0);
    }

    public function test_count_is_announced_via_live_region(): void
    {
        Livewire::test(Counter::class)
            ->assertSeeHtml('aria-live="polite"');
    }
}
