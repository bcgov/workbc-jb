<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use App\Models\JobSeeker;
use App\Services\JobSeeker\SavedProfileService;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\Auth;

/**
 * ACCT-6 — save/status for industry profiles, the parallel of
 * {@see CareerProfileApiController} (see its docblock for the auth model and why
 * the CSRF token is returned in the status body).
 *
 * These endpoints did not exist before: `routes/api.php` only ever had the
 * `career-profiles/*` group, so Drupal's industry pages had nothing to call.
 *
 * `{profileId}` is the **`IndustryId`** value directly.
 */
final class IndustryProfileApiController extends Controller
{
    public function __construct(private readonly SavedProfileService $profiles) {}

    public function status(int $profileId): JsonResponse
    {
        return response()->json([
            'saved' => $this->profiles->hasIndustryProfile($this->seeker(), $profileId),
            'csrf' => csrf_token(),
        ]);
    }

    public function save(int $profileId): JsonResponse
    {
        $this->profiles->saveIndustryProfile($this->seeker(), $profileId);

        return response()->json(['saved' => true]);
    }

    public function remove(int $profileId): JsonResponse
    {
        $removed = $this->profiles->removeIndustryProfile($this->seeker(), $profileId);

        return response()->json(['saved' => false, 'removed' => $removed]);
    }

    private function seeker(): JobSeeker
    {
        /** @var JobSeeker $seeker */
        $seeker = Auth::guard('web')->user();

        return $seeker;
    }
}
