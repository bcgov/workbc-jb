<?php

namespace App\Services\JobSeeker;

use App\Models\JobSeeker;
use App\Models\SavedJob;
use Illuminate\Database\Eloquent\Collection;
use Illuminate\Support\Facades\DB;

final class SavedJobService
{
    /**
     * @param  list<string>  $visibleJobIds
     * @return array<string, true>
     */
    public function savedJobIdMapForVisible(JobSeeker $jobSeeker, array $visibleJobIds): array
    {
        $jobIds = array_values(array_unique(array_filter($visibleJobIds, static fn (string $jobId): bool => $jobId !== '')));

        if ($jobIds === []) {
            return [];
        }

        $savedJobIds = SavedJob::query()
            ->where('AspNetUserId', (string) $jobSeeker->Id)
            ->whereIn('JobId', $jobIds)
            ->pluck('JobId')
            ->all();

        return array_fill_keys(array_map(static fn ($id): string => (string) $id, $savedJobIds), true);
    }

    public function isSaved(JobSeeker $jobSeeker, string $jobId): bool
    {
        if ($jobId === '') {
            return false;
        }

        return SavedJob::query()
            ->where('AspNetUserId', (string) $jobSeeker->Id)
            ->where('JobId', $jobId)
            ->exists();
    }

    /**
     * Idempotent save. The legacy SavedJobs table has NO unique (AspNetUserId,
     * JobId) constraint and the restored data already contains duplicate pairs, so
     * single-membership is enforced here: reuse an active row if one exists, else
     * restore a soft-deleted one, else create — never add a second active row.
     */
    public function save(JobSeeker $jobSeeker, string $jobId): SavedJob
    {
        return DB::transaction(function () use ($jobSeeker, $jobId): SavedJob {
            // Prefer an existing ACTIVE row (global scope excludes soft-deleted),
            // else a soft-deleted row to restore, else create — so a pair never ends
            // up with two active rows.
            $savedJob = SavedJob::query()
                ->where('AspNetUserId', (string) $jobSeeker->Id)
                ->where('JobId', $jobId)
                ->first()
                ?? SavedJob::withTrashed()
                    ->where('AspNetUserId', (string) $jobSeeker->Id)
                    ->where('JobId', $jobId)
                    ->first();

            if ($savedJob === null) {
                $savedJob = new SavedJob;
                $savedJob->AspNetUserId = (string) $jobSeeker->Id;
                $savedJob->JobId = $jobId;
            }

            $savedJob->IsDeleted = false;
            $savedJob->DateDeleted = null;
            $savedJob->DateSaved = now();
            $savedJob->save();

            return $savedJob;
        });
    }

    /**
     * Soft-delete the current user's saved-job row(s) for the given JobId. Removes
     * ALL active rows (legacy data can have duplicate pairs), so unsaving fully
     * clears the job in one action.
     */
    public function unsave(JobSeeker $jobSeeker, string $jobId): bool
    {
        $rows = SavedJob::query()
            ->where('AspNetUserId', (string) $jobSeeker->Id)
            ->where('JobId', $jobId)
            ->get();

        if ($rows->isEmpty()) {
            return false;
        }

        foreach ($rows as $row) {
            $row->delete();
        }

        return true;
    }

    public function setNote(JobSeeker $jobSeeker, string $jobId, ?string $note): bool
    {
        $savedJob = SavedJob::query()
            ->where('AspNetUserId', (string) $jobSeeker->Id)
            ->where('JobId', $jobId)
            ->first();

        if ($savedJob === null) {
            return false;
        }

        $savedJob->Note = $note !== null && trim($note) !== '' ? $note : null;
        $savedJob->NoteUpdatedDate = now();

        return $savedJob->save();
    }

    /**
     * @return Collection<int, SavedJob>
     */
    public function listFor(JobSeeker $jobSeeker): Collection
    {
        return SavedJob::query()
            ->with('job')
            ->where('AspNetUserId', (string) $jobSeeker->Id)
            ->orderByDesc('DateSaved')
            ->get()
            // Collapse any legacy duplicate pairs to one (most-recent) row per job.
            ->unique('JobId')
            ->values();
    }
}
