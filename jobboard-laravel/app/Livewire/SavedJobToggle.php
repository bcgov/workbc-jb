<?php

namespace App\Livewire;

use App\Models\JobSeeker;
use App\Services\JobSeeker\SavedJobService;
use Illuminate\Support\Facades\Auth;
use Livewire\Component;

final class SavedJobToggle extends Component
{
    public string $jobId = '';

    public bool $isAuthenticated = false;

    public bool $isSaved = false;

    public string $statusMessage = '';

    public function mount(SavedJobService $savedJobService, string $jobId): void
    {
        $this->jobId = $jobId;

        $this->isAuthenticated = Auth::guard('web')->check();
        if (! $this->isAuthenticated) {
            return;
        }

        /** @var JobSeeker $jobSeeker */
        $jobSeeker = Auth::guard('web')->user();
        $this->isSaved = $savedJobService->isSaved($jobSeeker, $this->jobId);
    }

    public function toggle(SavedJobService $savedJobService): void
    {
        if (! $this->isAuthenticated) {
            return;
        }

        /** @var JobSeeker $jobSeeker */
        $jobSeeker = Auth::guard('web')->user();

        if ($this->isSaved) {
            $removed = $savedJobService->unsave($jobSeeker, $this->jobId);
            if ($removed) {
                $this->isSaved = false;
                $this->statusMessage = 'Job removed from saved jobs.';
            }

            return;
        }

        $savedJobService->save($jobSeeker, $this->jobId);
        $this->isSaved = true;
        $this->statusMessage = 'Job saved.';
    }

    public function render()
    {
        return view('livewire.saved-job-toggle');
    }
}
