<?php

namespace App\Services\JobSeeker;

use App\Models\JobSeeker;
use App\Models\SavedCareerProfile;
use App\Models\SavedIndustryProfile;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Support\Collection;
use Illuminate\Support\Facades\DB;

/**
 * ACCT-6 — the owning service for `SavedCareerProfiles` / `SavedIndustryProfiles`
 * (constraint #3: only the owning JobSeeker service writes these aggregates).
 *
 * Legacy semantics ported from `WorkBC.Web/Controllers/CareerProfilesController.cs`
 * and `IndustryProfilesController.cs`:
 *
 *  - The route's `profileId` **is** `NocCodeId2021` / `IndustryId` directly. It is
 *    NOT the row's own `Id`, and NOT `EDM_CareerProfile_CareerProfileId` (which the
 *    .NET code always wrote as null and never read).
 *  - Save is **insert-if-absent**, never a toggle: saving an already-saved profile
 *    is a no-op that still reports success.
 *  - Remove is **soft-delete only** — the .NET controller's hard-delete line is
 *    commented out, deliberately.
 *
 * One divergence, matching {@see SavedJobService} rather than the legacy: when a
 * soft-deleted row exists we **restore** it instead of inserting a second row.
 * Observably identical (one active row, `DateSaved` refreshed), but it stops the
 * table accumulating a row per save/unsave cycle.
 */
final class SavedProfileService
{
    // --- Career (NOC) profiles ---------------------------------------------

    public function hasCareerProfile(JobSeeker $jobSeeker, int $nocCodeId): bool
    {
        return SavedCareerProfile::query()
            ->where('AspNetUserId', (string) $jobSeeker->Id)
            ->where('NocCodeId2021', $nocCodeId)
            ->exists();
    }

    public function saveCareerProfile(JobSeeker $jobSeeker, int $nocCodeId): SavedCareerProfile
    {
        return DB::transaction(function () use ($jobSeeker, $nocCodeId): SavedCareerProfile {
            $row = SavedCareerProfile::query()
                ->where('AspNetUserId', (string) $jobSeeker->Id)
                ->where('NocCodeId2021', $nocCodeId)
                ->first()
                ?? SavedCareerProfile::withTrashed()
                    ->where('AspNetUserId', (string) $jobSeeker->Id)
                    ->where('NocCodeId2021', $nocCodeId)
                    ->first();

            if ($row === null) {
                $row = new SavedCareerProfile;
                $row->AspNetUserId = (string) $jobSeeker->Id;
                $row->NocCodeId2021 = $nocCodeId;
                // Vestigial in the legacy schema; always null. See the model docblock.
                $row->EDM_CareerProfile_CareerProfileId = null;
            }

            return $this->activate($row);
        });
    }

    public function removeCareerProfile(JobSeeker $jobSeeker, int $nocCodeId): bool
    {
        return $this->softDeleteAll(
            SavedCareerProfile::query()
                ->where('AspNetUserId', (string) $jobSeeker->Id)
                ->where('NocCodeId2021', $nocCodeId)
                ->get()
        );
    }

    /**
     * Saved career profiles with their NOC title/code, newest first.
     *
     * @return Collection<int, array{id:int, profileId:int, code:?string, title:string, savedAt:?string}>
     */
    public function careerProfilesFor(JobSeeker $jobSeeker): Collection
    {
        return SavedCareerProfile::query()
            ->leftJoin('NocCodes2021', 'NocCodes2021.Id', '=', 'SavedCareerProfiles.NocCodeId2021')
            ->where('SavedCareerProfiles.AspNetUserId', (string) $jobSeeker->Id)
            ->orderByDesc('SavedCareerProfiles.DateSaved')
            ->get([
                'SavedCareerProfiles.Id as id',
                'SavedCareerProfiles.NocCodeId2021 as profileId',
                'SavedCareerProfiles.DateSaved as savedAt',
                'NocCodes2021.Code as code',
                'NocCodes2021.Title as title',
            ])
            ->unique('profileId')
            ->values()
            ->map(static fn (Model $r): array => [
                'id' => (int) $r->id,
                'profileId' => (int) $r->profileId,
                'code' => $r->code !== null ? (string) $r->code : null,
                'title' => (string) ($r->title ?? 'Unknown career profile'),
                'savedAt' => $r->savedAt !== null ? (string) $r->savedAt : null,
            ]);
    }

    // --- Industry profiles --------------------------------------------------

    public function hasIndustryProfile(JobSeeker $jobSeeker, int $industryId): bool
    {
        return SavedIndustryProfile::query()
            ->where('AspNetUserId', (string) $jobSeeker->Id)
            ->where('IndustryId', $industryId)
            ->exists();
    }

    public function saveIndustryProfile(JobSeeker $jobSeeker, int $industryId): SavedIndustryProfile
    {
        return DB::transaction(function () use ($jobSeeker, $industryId): SavedIndustryProfile {
            $row = SavedIndustryProfile::query()
                ->where('AspNetUserId', (string) $jobSeeker->Id)
                ->where('IndustryId', $industryId)
                ->first()
                ?? SavedIndustryProfile::withTrashed()
                    ->where('AspNetUserId', (string) $jobSeeker->Id)
                    ->where('IndustryId', $industryId)
                    ->first();

            if ($row === null) {
                $row = new SavedIndustryProfile;
                $row->AspNetUserId = (string) $jobSeeker->Id;
                $row->IndustryId = $industryId;
            }

            return $this->activate($row);
        });
    }

    public function removeIndustryProfile(JobSeeker $jobSeeker, int $industryId): bool
    {
        return $this->softDeleteAll(
            SavedIndustryProfile::query()
                ->where('AspNetUserId', (string) $jobSeeker->Id)
                ->where('IndustryId', $industryId)
                ->get()
        );
    }

    /**
     * Saved industry profiles with their title, newest first.
     *
     * Joins `Industries.TitleBC` — the B.C.-specific title the legacy
     * IndustryProfilesController orders and displays by — falling back to
     * `Title` when it is blank.
     *
     * @return Collection<int, array{id:int, profileId:int, title:string, savedAt:?string}>
     */
    public function industryProfilesFor(JobSeeker $jobSeeker): Collection
    {
        return SavedIndustryProfile::query()
            ->leftJoin('Industries', 'Industries.Id', '=', 'SavedIndustryProfiles.IndustryId')
            ->where('SavedIndustryProfiles.AspNetUserId', (string) $jobSeeker->Id)
            ->orderByDesc('SavedIndustryProfiles.DateSaved')
            ->get([
                'SavedIndustryProfiles.Id as id',
                'SavedIndustryProfiles.IndustryId as profileId',
                'SavedIndustryProfiles.DateSaved as savedAt',
                'Industries.TitleBC as titleBc',
                'Industries.Title as title',
            ])
            ->unique('profileId')
            ->values()
            ->map(static fn (Model $r): array => [
                'id' => (int) $r->id,
                'profileId' => (int) $r->profileId,
                'title' => (string) ($r->titleBc ?: $r->title ?: 'Unknown industry profile'),
                'savedAt' => $r->savedAt !== null ? (string) $r->savedAt : null,
            ]);
    }

    // --- Shared -------------------------------------------------------------

    /**
     * @template T of SavedCareerProfile|SavedIndustryProfile
     *
     * @param  T  $row
     * @return T
     */
    private function activate(Model $row): Model
    {
        $row->IsDeleted = false;
        $row->DateDeleted = null;
        $row->DateSaved = now();
        $row->save();

        return $row;
    }

    /**
     * Soft-delete every active row in the set. Legacy data can contain duplicate
     * pairs, so removing clears them all rather than just the first.
     *
     * @param  Collection<int, SavedCareerProfile|SavedIndustryProfile>  $rows
     */
    private function softDeleteAll(Collection $rows): bool
    {
        if ($rows->isEmpty()) {
            return false;
        }

        foreach ($rows as $row) {
            $row->delete();
        }

        return true;
    }
}
