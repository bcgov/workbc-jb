<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use App\Http\Middleware\EnsureJobSeekerToken;
use Illuminate\Http\JsonResponse;

/**
 * SRCH-10 — `POST /api/career-profiles/save/{profileId}` and
 * `GET /api/career-profiles/status/{profileId}` (contracts.md §2.4).
 *
 * The job-seeker account epic (EPIC-ACCOUNT) — which owns `SavedCareerProfiles`
 * — isn't built yet (constraint #3: only the owning service writes an
 * aggregate). These are intentionally thin stubs behind
 * {@see EnsureJobSeekerToken} so the routing/auth contract
 * is correct now; persistence is a documented follow-up.
 *
 * TODO(EPIC-ACCOUNT): once job-seeker auth + `SavedCareerProfiles` land, wire
 * these to the owning JobSeeker service (mirrors the legacy
 * WorkBC.Web CareerProfilesController.SaveCareerProfile / GetCareerProfileStatus
 * — resolve the acting AspNetUserId, look up `NocCodes2021` by `profileId`,
 * upsert/soft-delete the saved-profile row).
 */
final class CareerProfileApiController extends Controller
{
    /**
     * Stub: always reports success without persisting anything yet.
     */
    public function save(string $profileId): JsonResponse
    {
        return response()->json(true);
    }

    /**
     * Stub: always reports "not saved" — avoids a false-positive "saved" state
     * in the Drupal UI until persistence exists.
     */
    public function status(string $profileId): JsonResponse
    {
        return response()->json(false);
    }
}
