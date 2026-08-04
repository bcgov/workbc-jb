<?php

namespace App\Services\JobSeeker;

use App\Models\JobSeeker;
use App\Notifications\JobSeekerEmailVerificationNotification;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Hash;
use Illuminate\Support\Str;

final class PersonalSettingsService
{
    /**
     * @param  array{firstName:?string,lastName:?string,countryId:?int,provinceId:?int,locationId:?int}  $input
     */
    public function updateProfile(JobSeeker $jobSeeker, array $input): bool
    {
        $profile = $this->normalizedProfileInput($input);

        $effectiveLocationId = $profile['provinceId'] === 2 ? $profile['locationId'] : null;
        $effectiveCity = null;

        if ($effectiveLocationId !== null) {
            $location = DB::table('Locations')
                ->where('LocationId', $effectiveLocationId)
                ->first(['City', 'Label']);

            $effectiveCity = $location !== null
                ? trim((string) (($location->City ?? '') !== '' ? $location->City : ($location->Label ?? '')))
                : null;
        }

        $changes = [];
        $oldValues = [];
        $newValues = [];

        $this->trackTextChange(
            'First name edited',
            'First name',
            (string) ($jobSeeker->FirstName ?? ''),
            (string) ($profile['firstName'] ?? ''),
            $changes,
            $oldValues,
            $newValues,
        );

        $this->trackTextChange(
            'Last name edited',
            'Last name',
            (string) ($jobSeeker->LastName ?? ''),
            (string) ($profile['lastName'] ?? ''),
            $changes,
            $oldValues,
            $newValues,
        );

        $oldCountryLabel = $this->countryLabel($jobSeeker->CountryId !== null ? (int) $jobSeeker->CountryId : null);
        $newCountryLabel = $this->countryLabel($profile['countryId']);
        $this->trackTextChange(
            'Country edited',
            'Country',
            $oldCountryLabel,
            $newCountryLabel,
            $changes,
            $oldValues,
            $newValues,
        );

        $oldProvinceLabel = $this->provinceLabel($jobSeeker->ProvinceId !== null ? (int) $jobSeeker->ProvinceId : null);
        $newProvinceLabel = $this->provinceLabel($profile['provinceId']);
        $this->trackTextChange(
            'Province edited',
            'Province',
            $oldProvinceLabel,
            $newProvinceLabel,
            $changes,
            $oldValues,
            $newValues,
        );

        $oldCityLabel = $this->cityLabel(
            $jobSeeker->ProvinceId !== null ? (int) $jobSeeker->ProvinceId : null,
            $jobSeeker->LocationId !== null ? (int) $jobSeeker->LocationId : null,
            $jobSeeker->City,
        );
        $newCityLabel = $this->cityLabel($profile['provinceId'], $effectiveLocationId, $effectiveCity);
        $this->trackTextChange(
            'City edited',
            'City',
            $oldCityLabel,
            $newCityLabel,
            $changes,
            $oldValues,
            $newValues,
        );

        if ($changes === []) {
            return false;
        }

        DB::transaction(function () use ($jobSeeker, $profile, $effectiveLocationId, $effectiveCity, $changes, $oldValues, $newValues): void {
            $jobSeeker->forceFill([
                'FirstName' => $profile['firstName'],
                'LastName' => $profile['lastName'],
                'CountryId' => $profile['countryId'],
                'ProvinceId' => $profile['provinceId'],
                'LocationId' => $effectiveLocationId,
                'City' => $effectiveCity,
                'LastModified' => now(),
            ]);
            $jobSeeker->save();

            $this->writeAuditRow((string) $jobSeeker->Id, implode(', ', $changes), implode(', ', $oldValues), implode(', ', $newValues));
        });

        return true;
    }

    public function updateEmail(JobSeeker $jobSeeker, string $newEmail): string
    {
        $email = trim($newEmail);
        $normalized = $this->normalize($email);

        if ($normalized === (string) $jobSeeker->NormalizedEmail) {
            return 'unchanged';
        }

        $duplicate = JobSeeker::query()
            ->where('NormalizedEmail', $normalized)
            ->where('Id', '!=', (string) $jobSeeker->Id)
            ->exists();

        if ($duplicate) {
            return 'duplicate';
        }

        DB::transaction(function () use ($jobSeeker, $email, $normalized): void {
            $oldEmail = (string) $jobSeeker->Email;

            $jobSeeker->forceFill([
                'Email' => $email,
                'UserName' => $email,
                'NormalizedEmail' => $normalized,
                'NormalizedUserName' => $normalized,
                'EmailConfirmed' => false,
                'VerificationGuid' => (string) Str::uuid(),
                'LastModified' => now(),
            ]);
            $jobSeeker->save();

            $this->writeAuditRow(
                (string) $jobSeeker->Id,
                'Email edited',
                'Email: '.$this->displayValue($oldEmail),
                'Email: '.$this->displayValue($email),
            );
        });

        $jobSeeker->notify(new JobSeekerEmailVerificationNotification(
            route('job-seeker.verify', ['userId' => (string) $jobSeeker->Id, 'guid' => (string) $jobSeeker->VerificationGuid]),
        ));

        return 'updated';
    }

    public function changePassword(JobSeeker $jobSeeker, string $currentPassword, string $newPassword): bool
    {
        if (! Hash::check($currentPassword, (string) $jobSeeker->PasswordHash)) {
            return false;
        }

        DB::transaction(function () use ($jobSeeker, $newPassword): void {
            $jobSeeker->forceFill([
                'PasswordHash' => Hash::make($newPassword),
                'SecurityStamp' => (string) Str::uuid(),
                'LastModified' => now(),
            ]);
            $jobSeeker->save();

            $this->writeAuditRow((string) $jobSeeker->Id, 'Password changed', '-', '-');
        });

        return true;
    }

    /**
     * @return array<int, array{id:int,name:string}>
     */
    public function countries(): array
    {
        return DB::table('Countries')
            ->orderBy('Name')
            ->get(['Id', 'Name'])
            ->map(static fn ($row): array => ['id' => (int) $row->Id, 'name' => (string) $row->Name])
            ->all();
    }

    /**
     * @return array<int, array{id:int,name:string}>
     */
    public function provinces(): array
    {
        // NB: Provinces' primary key is `ProvinceId`, not `Id` — unlike Countries,
        // which does use `Id`. Verified against the live schema.
        return DB::table('Provinces')
            ->orderBy('Name')
            ->get(['ProvinceId', 'Name'])
            ->map(static fn ($row): array => ['id' => (int) $row->ProvinceId, 'name' => (string) $row->Name])
            ->all();
    }

    /**
     * @return array<int, array{id:int,label:string}>
     */
    public function locations(): array
    {
        return DB::table('Locations')
            ->where('LocationId', '>', 0)
            ->where('IsHidden', false)
            ->where('IsDuplicate', false)
            ->orderBy('Label')
            ->get(['LocationId', 'Label', 'City'])
            ->map(static fn ($row): array => [
                'id' => (int) $row->LocationId,
                'label' => trim((string) (($row->Label ?? '') !== '' ? $row->Label : ($row->City ?? ''))),
            ])
            ->all();
    }

    /**
     * @param  array{firstName:?string,lastName:?string,countryId:?int,provinceId:?int,locationId:?int}  $input
     * @return array{firstName:?string,lastName:?string,countryId:?int,provinceId:?int,locationId:?int}
     */
    private function normalizedProfileInput(array $input): array
    {
        return [
            'firstName' => $this->nullableTrim($input['firstName'] ?? null),
            'lastName' => $this->nullableTrim($input['lastName'] ?? null),
            'countryId' => $input['countryId'] !== null ? (int) $input['countryId'] : null,
            'provinceId' => $input['provinceId'] !== null ? (int) $input['provinceId'] : null,
            'locationId' => $input['locationId'] !== null ? (int) $input['locationId'] : null,
        ];
    }

    private function countryLabel(?int $countryId): string
    {
        if ($countryId === null) {
            return '-';
        }

        $name = DB::table('Countries')->where('Id', $countryId)->value('Name');

        return $this->displayValue($name);
    }

    private function provinceLabel(?int $provinceId): string
    {
        if ($provinceId === null) {
            return '-';
        }

        // Provinces keys on `ProvinceId`, not `Id` — see provinces() above.
        $name = DB::table('Provinces')->where('ProvinceId', $provinceId)->value('Name');

        return $this->displayValue($name);
    }

    private function cityLabel(?int $provinceId, ?int $locationId, mixed $cityFallback): string
    {
        if ($provinceId !== 2 || $locationId === null) {
            return '-';
        }

        $label = DB::table('Locations')->where('LocationId', $locationId)->value('Label');

        return $this->displayValue($label ?? $cityFallback);
    }

    /**
     * @param  list<string>  $changes
     * @param  list<string>  $oldValues
     * @param  list<string>  $newValues
     */
    private function trackTextChange(
        string $changeLabel,
        string $valueLabel,
        string $old,
        string $new,
        array &$changes,
        array &$oldValues,
        array &$newValues,
    ): void {
        if ($this->displayValue($old) === $this->displayValue($new)) {
            return;
        }

        $changes[] = $changeLabel;
        $oldValues[] = $valueLabel.': '.$this->displayValue($old);
        $newValues[] = $valueLabel.': '.$this->displayValue($new);
    }

    private function writeAuditRow(string $userId, string $field, string $oldValue, string $newValue): void
    {
        DB::table('JobSeekerChangeLog')->insert([
            'AspNetUserId' => $userId,
            'ModifiedByAdminUserId' => null,
            'DateUpdated' => now(),
            'Field' => $field,
            'OldValue' => $oldValue,
            'NewValue' => $newValue,
        ]);
    }

    private function nullableTrim(?string $value): ?string
    {
        if ($value === null) {
            return null;
        }

        $trimmed = trim($value);

        return $trimmed !== '' ? $trimmed : null;
    }

    private function displayValue(mixed $value): string
    {
        $text = trim((string) ($value ?? ''));

        return $text !== '' ? $text : '-';
    }

    private function normalize(string $value): string
    {
        return mb_strtoupper(trim($value), 'UTF-8');
    }
}