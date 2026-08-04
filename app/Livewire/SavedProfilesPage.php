<?php

namespace App\Livewire;

use App\Models\JobSeeker;
use App\Services\JobSeeker\SavedProfileService;
use Illuminate\Support\Facades\Auth;
use Livewire\Attributes\Layout;
use Livewire\Attributes\Title;
use Livewire\Component;

/**
 * ACCT-6 — the seeker's saved career (NOC) and industry profiles.
 *
 * Also closes the `/account/profiles` 404 that the ACCT-1 dashboard has been
 * linking to (see that story's "No dead navigation" criterion).
 *
 * The profiles themselves are authored on the Drupal side; this lists what the
 * seeker saved and lets them remove entries. Removal is soft-delete via the
 * owning service (constraint #3).
 */
#[Layout('components.layouts.app')]
#[Title('Saved profiles — WorkBC Job Board')]
final class SavedProfilesPage extends Component
{
    /** @var array<int, array<string, mixed>> */
    public array $careerProfiles = [];

    /** @var array<int, array<string, mixed>> */
    public array $industryProfiles = [];

    public string $statusMessage = '';

    public function mount(SavedProfileService $profiles): void
    {
        $this->reload($profiles);
    }

    public function removeCareerProfile(int $profileId, SavedProfileService $profiles): void
    {
        $removed = $profiles->removeCareerProfile($this->seeker(), $profileId);
        $this->statusMessage = $removed ? 'Career profile removed.' : 'That career profile was not saved.';
        $this->reload($profiles);
    }

    public function removeIndustryProfile(int $profileId, SavedProfileService $profiles): void
    {
        $removed = $profiles->removeIndustryProfile($this->seeker(), $profileId);
        $this->statusMessage = $removed ? 'Industry profile removed.' : 'That industry profile was not saved.';
        $this->reload($profiles);
    }

    public function render()
    {
        return view('livewire.saved-profiles-page');
    }

    private function reload(SavedProfileService $profiles): void
    {
        $seeker = $this->seeker();
        $this->careerProfiles = $profiles->careerProfilesFor($seeker)->all();
        $this->industryProfiles = $profiles->industryProfilesFor($seeker)->all();
    }

    private function seeker(): JobSeeker
    {
        /** @var JobSeeker $seeker */
        $seeker = Auth::guard('web')->user();

        return $seeker;
    }
}
