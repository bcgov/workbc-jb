<?php

namespace App\Livewire;

use App\Models\JobSeeker;
use App\Services\JobSeeker\RecommendedJobsService;
use Illuminate\Support\Facades\Auth;
use Livewire\Attributes\Layout;
use Livewire\Attributes\Title;
use Livewire\Component;

#[Layout('components.layouts.app')]
#[Title('Recommended jobs — WorkBC Job Board')]
final class RecommendedJobsPage extends Component
{
    /** @var list<array<string, mixed>> */
    public array $jobs = [];

    public int $total = 0;

    public bool $hasSavedJobs = false;

    public function mount(RecommendedJobsService $recommendedJobs): void
    {
        $this->reload($recommendedJobs);
    }

    public function render()
    {
        return view('livewire.recommended-jobs-page');
    }

    private function reload(RecommendedJobsService $recommendedJobs): void
    {
        $data = $recommendedJobs->recommendationsFor($this->seeker(), page: 1, pageSize: 20);

        $this->jobs = $data['jobs'];
        $this->total = $data['total'];
        $this->hasSavedJobs = $data['signals']->hasSavedJobs();
    }

    private function seeker(): JobSeeker
    {
        /** @var JobSeeker $seeker */
        $seeker = Auth::guard('web')->user();

        return $seeker;
    }
}
