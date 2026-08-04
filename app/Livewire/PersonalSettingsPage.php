<?php

namespace App\Livewire;

use App\Models\JobSeeker;
use App\Services\JobSeeker\PersonalSettingsService;
use Illuminate\Validation\Rule;
use Livewire\Attributes\Layout;
use Livewire\Attributes\Title;
use Livewire\Component;

#[Layout('components.layouts.app')]
#[Title('Personal settings — WorkBC Job Board')]
final class PersonalSettingsPage extends Component
{
    public ?string $firstName = null;

    public ?string $lastName = null;

    public ?int $countryId = null;

    public ?int $provinceId = null;

    public ?int $locationId = null;

    public ?string $city = null;

    public string $newEmail = '';

    public string $currentPassword = '';

    public string $newPassword = '';

    public string $newPasswordConfirmation = '';

    /** @var array<int, array{id:int,name:string}> */
    public array $countries = [];

    /** @var array<int, array{id:int,name:string}> */
    public array $provinces = [];

    /** @var array<int, array{id:int,label:string}> */
    public array $locations = [];

    public string $profileStatus = '';

    public string $emailStatus = '';

    public string $passwordStatus = '';

    public function mount(PersonalSettingsService $service): void
    {
        $seeker = $this->seeker();

        $this->firstName = $seeker->FirstName;
        $this->lastName = $seeker->LastName;
        $this->countryId = $seeker->CountryId !== null ? (int) $seeker->CountryId : null;
        $this->provinceId = $seeker->ProvinceId !== null ? (int) $seeker->ProvinceId : null;
        $this->locationId = $seeker->LocationId !== null ? (int) $seeker->LocationId : null;
        $this->city = $seeker->City !== null ? (string) $seeker->City : null;
        $this->newEmail = (string) $seeker->Email;

        $this->countries = $service->countries();
        $this->provinces = $service->provinces();
        $this->locations = $service->locations();
    }

    public function updatedProvinceId(): void
    {
        if ((int) ($this->provinceId ?? 0) !== 2) {
            $this->locationId = null;
            $this->city = null;
        }
    }

    public function updatedLocationId(): void
    {
        $this->city = $this->derivedCity($this->locationId);
    }

    public function saveProfile(PersonalSettingsService $service): void
    {
        $validated = $this->validate([
            'firstName' => ['nullable', 'string', 'max:50'],
            'lastName' => ['nullable', 'string', 'max:50'],
            'countryId' => ['nullable', 'integer', Rule::exists('Countries', 'Id')],
            // Provinces keys on `ProvinceId`, not `Id` (unlike Countries above).
            'provinceId' => ['nullable', 'integer', Rule::exists('Provinces', 'ProvinceId')],
            'locationId' => [
                Rule::requiredIf(fn (): bool => (int) ($this->provinceId ?? 0) === 2),
                'nullable',
                'integer',
                Rule::exists('Locations', 'LocationId'),
            ],
        ]);

        $changed = $service->updateProfile($this->seeker(), [
            'firstName' => $validated['firstName'] ?? null,
            'lastName' => $validated['lastName'] ?? null,
            'countryId' => isset($validated['countryId']) ? (int) $validated['countryId'] : null,
            'provinceId' => isset($validated['provinceId']) ? (int) $validated['provinceId'] : null,
            'locationId' => isset($validated['locationId']) ? (int) $validated['locationId'] : null,
        ]);

        $seeker = $this->seeker()->fresh();
        $this->city = $seeker->City !== null ? (string) $seeker->City : null;
        $this->locationId = $seeker->LocationId !== null ? (int) $seeker->LocationId : null;

        $this->profileStatus = $changed ? 'Profile updated.' : 'No profile changes to save.';
    }

    public function saveEmail(PersonalSettingsService $service): void
    {
        $this->validate([
            'newEmail' => ['required', 'email:rfc', 'max:256'],
        ]);

        $result = $service->updateEmail($this->seeker(), $this->newEmail);

        if ($result === 'duplicate') {
            $this->addError('newEmail', 'That email address is already in use.');
            $this->emailStatus = '';

            return;
        }

        if ($result === 'unchanged') {
            $this->emailStatus = 'Email is unchanged.';

            return;
        }

        $this->newEmail = (string) $this->seeker()->fresh()->Email;
        $this->emailStatus = 'Email updated. Please check your new inbox to verify this address.';
    }

    public function savePassword(PersonalSettingsService $service): void
    {
        $validated = $this->validate([
            'currentPassword' => ['required', 'string'],
            'newPassword' => ['required', 'string', 'min:8', 'same:newPasswordConfirmation'],
            'newPasswordConfirmation' => ['required', 'string', 'min:8'],
        ], [], [
            'newPassword' => 'new password',
        ]);

        $changed = $service->changePassword(
            $this->seeker(),
            (string) $validated['currentPassword'],
            (string) $validated['newPassword'],
        );

        if (! $changed) {
            $this->addError('currentPassword', 'The current password is incorrect.');
            $this->passwordStatus = '';

            return;
        }

        $this->currentPassword = '';
        $this->newPassword = '';
        $this->newPasswordConfirmation = '';
        $this->passwordStatus = 'Password changed.';
    }

    public function render()
    {
        return view('livewire.personal-settings-page');
    }

    private function seeker(): JobSeeker
    {
        /** @var JobSeeker $seeker */
        $seeker = auth('web')->user();

        return $seeker;
    }

    private function derivedCity(?int $locationId): ?string
    {
        if ((int) ($this->provinceId ?? 0) !== 2 || $locationId === null) {
            return null;
        }

        foreach ($this->locations as $location) {
            if ((int) $location['id'] === (int) $locationId) {
                return (string) $location['label'];
            }
        }

        return null;
    }
}