<?php

namespace App\Livewire;

use App\Models\Enums\AlertFrequency;
use App\Models\JobSeeker;
use App\Services\JobSeeker\JobAlertsService;
use Illuminate\Support\Facades\Auth;
use Livewire\Attributes\Layout;
use Livewire\Attributes\Title;
use Livewire\Component;

#[Layout('components.layouts.app')]
#[Title('Job alerts — WorkBC Job Board')]
final class JobAlertsList extends Component
{
    /** @var array<int, array<string, mixed>> */
    public array $alerts = [];

    public string $statusMessage = '';

    public function mount(JobAlertsService $alertsService): void
    {
        $this->reload($alertsService);
    }

    public function delete(JobAlertsService $alertsService, int $alertId): void
    {
        /** @var JobSeeker $jobSeeker */
        $jobSeeker = Auth::guard('web')->user();

        $deleted = $alertsService->delete($jobSeeker, $alertId);

        $this->statusMessage = $deleted ? 'Alert deleted.' : 'Unable to delete that alert.';

        $this->reload($alertsService);
    }

    public function render()
    {
        return view('livewire.job-alerts-list');
    }

    private function reload(JobAlertsService $alertsService): void
    {
        /** @var JobSeeker $jobSeeker */
        $jobSeeker = Auth::guard('web')->user();

        $this->alerts = $alertsService->listFor($jobSeeker)
            ->map(static fn ($alert): array => [
                'Id' => (int) $alert->Id,
                'Title' => (string) $alert->Title,
                'Frequency' => self::frequencyLabel($alert->AlertFrequency),
                'DateCreated' => $alert->DateCreated,
            ])
            ->values()
            ->all();
    }

    private static function frequencyLabel(?AlertFrequency $frequency): string
    {
        return match ($frequency) {
            AlertFrequency::Daily => 'Daily',
            AlertFrequency::Weekly => 'Weekly',
            AlertFrequency::BiWeekly => 'Bi-weekly',
            AlertFrequency::Monthly => 'Monthly',
            AlertFrequency::Never => 'Off (no emails)',
            default => 'Unknown',
        };
    }
}
