<?php

namespace App\Livewire;

use App\Models\JobSeeker;
use App\Services\JobSeeker\JobSeekerDashboardService;
use Illuminate\Support\Facades\Auth;
use Livewire\Attributes\Layout;
use Livewire\Attributes\Title;
use Livewire\Component;

#[Layout('components.layouts.app')]
#[Title('My account — WorkBC Job Board')]
final class JobSeekerDashboard extends Component
{
    public int $savedJobs = 0;

    public int $activeAlerts = 0;

    public int $savedCareerProfiles = 0;

    public int $savedIndustryProfiles = 0;

    public function mount(JobSeekerDashboardService $dashboardService): void
    {
        $this->refreshCounts($dashboardService);
    }

    public function refreshCounts(JobSeekerDashboardService $dashboardService): void
    {
        /** @var JobSeeker $jobSeeker */
        $jobSeeker = Auth::guard('web')->user();
        $summary = $dashboardService->summaryFor($jobSeeker);

        $this->savedJobs = $summary['savedJobs'];
        $this->activeAlerts = $summary['activeAlerts'];
        $this->savedCareerProfiles = $summary['savedCareerProfiles'];
        $this->savedIndustryProfiles = $summary['savedIndustryProfiles'];
    }

    public function render()
    {
        return view('livewire.job-seeker-dashboard');
    }
}
