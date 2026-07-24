<?php

namespace App\Livewire;

use App\Models\JobSeeker;
use App\Services\JobSeeker\SavedJobService;
use Illuminate\Support\Facades\Auth;
use Livewire\Attributes\Layout;
use Livewire\Attributes\Title;
use Livewire\Component;

#[Layout('components.layouts.app')]
#[Title('Saved jobs — WorkBC Job Board')]
final class SavedJobsPage extends Component
{
    /** @var array<int, array<string, mixed>> */
    public array $savedJobs = [];

    public ?string $editingJobId = null;

    public string $noteDraft = '';

    public string $statusMessage = '';

    protected function rules(): array
    {
        return [
            'noteDraft' => ['nullable', 'string', 'max:800'],
        ];
    }

    public function mount(SavedJobService $savedJobService): void
    {
        $this->reload($savedJobService);
    }

    public function startEditing(string $jobId): void
    {
        $job = collect($this->savedJobs)->first(static fn (array $row): bool => $row['JobId'] === $jobId);

        if (! is_array($job)) {
            return;
        }

        $this->editingJobId = $jobId;
        $this->noteDraft = (string) ($job['Note'] ?? '');
        $this->resetValidation();
    }

    public function cancelEditing(): void
    {
        $this->editingJobId = null;
        $this->noteDraft = '';
        $this->resetValidation();
    }

    public function saveNote(SavedJobService $savedJobService, string $jobId): void
    {
        $this->editingJobId = $jobId;
        $this->validateOnly('noteDraft');

        /** @var JobSeeker $jobSeeker */
        $jobSeeker = Auth::guard('web')->user();

        $saved = $savedJobService->setNote($jobSeeker, $jobId, $this->noteDraft);

        if (! $saved) {
            $this->statusMessage = 'Unable to save note for that job.';

            return;
        }

        $this->statusMessage = 'Note saved.';
        $this->cancelEditing();
        $this->reload($savedJobService);
    }

    public function unsave(SavedJobService $savedJobService, string $jobId): void
    {
        /** @var JobSeeker $jobSeeker */
        $jobSeeker = Auth::guard('web')->user();

        $removed = $savedJobService->unsave($jobSeeker, $jobId);

        $this->statusMessage = $removed ? 'Job removed from saved jobs.' : 'Unable to remove that saved job.';

        $this->cancelEditing();
        $this->reload($savedJobService);
    }

    public function render()
    {
        return view('livewire.saved-jobs-page');
    }

    private function reload(SavedJobService $savedJobService): void
    {
        /** @var JobSeeker $jobSeeker */
        $jobSeeker = Auth::guard('web')->user();

        $this->savedJobs = $savedJobService
            ->listFor($jobSeeker)
            ->map(static function ($savedJob): array {
                $job = $savedJob->job;

                return [
                    'Id' => (int) $savedJob->Id,
                    'JobId' => (string) $savedJob->JobId,
                    'DateSaved' => $savedJob->DateSaved,
                    'Note' => $savedJob->Note,
                    'NoteUpdatedDate' => $savedJob->NoteUpdatedDate,
                    'Title' => $job?->Title,
                    'EmployerName' => $job?->EmployerName,
                    'City' => $job?->City,
                    'ExpireDate' => $job?->ExpireDate,
                ];
            })
            ->values()
            ->all();
    }
}
