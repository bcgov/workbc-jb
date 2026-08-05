<x-account.layout page-title="Personal settings" page-description="Manage your profile details, email, and password.">
    <section class="space-y-8" aria-labelledby="personal-settings-heading">
        <h2 id="personal-settings-heading" class="text-2xl font-semibold tracking-tight text-slate-900">Account settings</h2>

        <section class="rounded-lg border border-slate-200 bg-white p-4" aria-labelledby="profile-settings-heading">
            <h3 id="profile-settings-heading" class="text-lg font-semibold text-slate-900">Profile details</h3>
            <p class="mt-1 text-sm text-slate-600">Update your name and location profile.</p>

            <p class="sr-only" role="status" aria-live="polite">{{ $profileStatus }}</p>

            <form class="mt-4 space-y-4" wire:submit="saveProfile">
                <div class="grid gap-4 sm:grid-cols-2">
                    <div>
                        <label for="first-name" class="mb-1 block text-sm font-medium text-slate-800">First name</label>
                        <input id="first-name" type="text" wire:model="firstName" maxlength="50"
                               class="w-full rounded border border-slate-300 px-3 py-2 text-slate-900 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-workbc-navy">
                        @error('firstName') <p class="mt-1 text-sm text-red-800" role="alert">{{ $message }}</p> @enderror
                    </div>

                    <div>
                        <label for="last-name" class="mb-1 block text-sm font-medium text-slate-800">Last name</label>
                        <input id="last-name" type="text" wire:model="lastName" maxlength="50"
                               class="w-full rounded border border-slate-300 px-3 py-2 text-slate-900 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-workbc-navy">
                        @error('lastName') <p class="mt-1 text-sm text-red-800" role="alert">{{ $message }}</p> @enderror
                    </div>
                </div>

                <div class="grid gap-4 sm:grid-cols-2">
                    <div>
                        <label for="country-id" class="mb-1 block text-sm font-medium text-slate-800">Country</label>
                        <select id="country-id" wire:model="countryId"
                                class="w-full rounded border border-slate-300 px-3 py-2 text-slate-900 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-workbc-navy">
                            <option value="">Select country</option>
                            @foreach ($countries as $country)
                                <option value="{{ $country['id'] }}">{{ $country['name'] }}</option>
                            @endforeach
                        </select>
                        @error('countryId') <p class="mt-1 text-sm text-red-800" role="alert">{{ $message }}</p> @enderror
                    </div>

                    <div>
                        <label for="province-id" class="mb-1 block text-sm font-medium text-slate-800">Province / territory</label>
                        <select id="province-id" wire:model="provinceId"
                                class="w-full rounded border border-slate-300 px-3 py-2 text-slate-900 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-workbc-navy">
                            <option value="">Select province</option>
                            @foreach ($provinces as $province)
                                <option value="{{ $province['id'] }}">{{ $province['name'] }}</option>
                            @endforeach
                        </select>
                        @error('provinceId') <p class="mt-1 text-sm text-red-800" role="alert">{{ $message }}</p> @enderror
                    </div>
                </div>

                <div class="grid gap-4 sm:grid-cols-2">
                    <div>
                        <label for="location-id" class="mb-1 block text-sm font-medium text-slate-800">Location (B.C. only)</label>
                        <select id="location-id" wire:model="locationId" @if ((int) ($provinceId ?? 0) !== 2) disabled @endif
                                class="w-full rounded border border-slate-300 px-3 py-2 text-slate-900 disabled:bg-slate-100 disabled:text-slate-500 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-workbc-navy">
                            <option value="">Select location</option>
                            @foreach ($locations as $location)
                                <option value="{{ $location['id'] }}">{{ $location['label'] }}</option>
                            @endforeach
                        </select>
                        @error('locationId') <p class="mt-1 text-sm text-red-800" role="alert">{{ $message }}</p> @enderror
                    </div>

                    <div>
                        <label for="city" class="mb-1 block text-sm font-medium text-slate-800">City</label>
                        <input id="city" type="text" value="{{ $city ?? '' }}" readonly
                               class="w-full rounded border border-slate-300 bg-slate-100 px-3 py-2 text-slate-700">
                        <p class="mt-1 text-xs text-slate-600">City is derived from your selected location when Province is B.C.</p>
                    </div>
                </div>

                <x-button type="submit">Save profile</x-button>
            </form>
        </section>

        <section class="rounded-lg border border-slate-200 bg-white p-4" aria-labelledby="email-settings-heading">
            <h3 id="email-settings-heading" class="text-lg font-semibold text-slate-900">Email address</h3>
            <p class="mt-1 text-sm text-slate-600">Changing your email requires confirming the new address.</p>

            <p class="sr-only" role="status" aria-live="polite">{{ $emailStatus }}</p>

            <form class="mt-4 space-y-4" wire:submit="saveEmail">
                <div>
                    <label for="new-email" class="mb-1 block text-sm font-medium text-slate-800">New email</label>
                    <input id="new-email" type="email" wire:model="newEmail"
                           class="w-full rounded border border-slate-300 px-3 py-2 text-slate-900 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-workbc-navy">
                    @error('newEmail') <p class="mt-1 text-sm text-red-800" role="alert">{{ $message }}</p> @enderror
                </div>

                <x-button type="submit">Update email</x-button>
            </form>
        </section>

        <section class="rounded-lg border border-slate-200 bg-white p-4" aria-labelledby="password-settings-heading">
            <h3 id="password-settings-heading" class="text-lg font-semibold text-slate-900">Password</h3>
            <p class="mt-1 text-sm text-slate-600">Enter your current password, then choose a new one.</p>

            <p class="sr-only" role="status" aria-live="polite">{{ $passwordStatus }}</p>

            <form class="mt-4 space-y-4" wire:submit="savePassword">
                <div>
                    <label for="current-password" class="mb-1 block text-sm font-medium text-slate-800">Current password</label>
                    <input id="current-password" type="password" wire:model="currentPassword"
                           class="w-full rounded border border-slate-300 px-3 py-2 text-slate-900 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-workbc-navy">
                    @error('currentPassword') <p class="mt-1 text-sm text-red-800" role="alert">{{ $message }}</p> @enderror
                </div>

                <div class="grid gap-4 sm:grid-cols-2">
                    <div>
                        <label for="new-password" class="mb-1 block text-sm font-medium text-slate-800">New password</label>
                        <input id="new-password" type="password" wire:model="newPassword"
                               class="w-full rounded border border-slate-300 px-3 py-2 text-slate-900 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-workbc-navy">
                        @error('newPassword') <p class="mt-1 text-sm text-red-800" role="alert">{{ $message }}</p> @enderror
                    </div>

                    <div>
                        <label for="new-password-confirmation" class="mb-1 block text-sm font-medium text-slate-800">Confirm new password</label>
                        <input id="new-password-confirmation" type="password" wire:model="newPasswordConfirmation"
                               class="w-full rounded border border-slate-300 px-3 py-2 text-slate-900 focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-workbc-navy">
                    </div>
                </div>

                <x-button type="submit">Change password</x-button>
            </form>
        </section>
    </section>
</x-account.layout>
