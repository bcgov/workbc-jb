<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use App\Models\JobSeeker;
use App\Services\JobSeeker\SavedProfileService;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\Auth;

/**
 * ACCT-6 — save/status for career (NOC) profiles, called from the browser by
 * Drupal's career-profile pages.
 *
 * **Not a server-to-server contract** (ADR-009). `contracts.md §2.4` used to
 * describe Drupal forwarding an `Authorization` header; verified against the live
 * Drupal module, that never happened — `WorkbcJobboardSaveProfile.php` pushes these
 * URLs into `drupalSettings` and `js/workbc_jobboard.js` calls them from the page.
 * So these are ordinary **session-authenticated** routes returning 401 when
 * anonymous, reached cross-origin with `credentials: 'include'` (same-site under
 * `workbc.ca`, so a plain `SameSite=Lax` cookie is sent).
 *
 * `{profileId}` is the **`NocCodeId2021`** value directly — see
 * {@see SavedProfileService}.
 */
final class CareerProfileApiController extends Controller
{
    public function __construct(private readonly SavedProfileService $profiles) {}

    /**
     * GET — `{ saved: bool, csrf: string }`.
     *
     * The CSRF token rides in the body because the double-submit cookie pattern
     * cannot work here: Drupal's JS runs on `www.workbc.ca` and cannot read an
     * `XSRF-TOKEN` cookie belonging to our origin. Status is already fetched on
     * page load, so the save POST gets its token without an extra round-trip.
     */
    public function status(int $profileId): JsonResponse
    {
        $seeker = $this->seeker();

        return response()->json([
            'saved' => $this->profiles->hasCareerProfile($seeker, $profileId),
            'csrf' => csrf_token(),
        ]);
    }

    /**
     * POST — insert-if-absent. Saving an already-saved profile is a no-op that
     * still reports success, matching the legacy controller.
     */
    public function save(int $profileId): JsonResponse
    {
        $this->profiles->saveCareerProfile($this->seeker(), $profileId);

        return response()->json(['saved' => true]);
    }

    /**
     * DELETE — soft-delete only (legacy never hard-deletes).
     */
    public function remove(int $profileId): JsonResponse
    {
        $removed = $this->profiles->removeCareerProfile($this->seeker(), $profileId);

        return response()->json(['saved' => false, 'removed' => $removed]);
    }

    private function seeker(): JobSeeker
    {
        /** @var JobSeeker $seeker */
        $seeker = Auth::guard('web')->user();

        return $seeker;
    }
}
