<?php

namespace App\Auth;

use Illuminate\Auth\EloquentUserProvider;
use Illuminate\Contracts\Auth\Authenticatable as UserContract;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Support\Facades\Hash;
use Illuminate\Support\Str;

final class JobSeekerUserProvider extends EloquentUserProvider
{
    public const FORCE_RESET_SESSION_KEY = 'auth.job_seeker.force_reset_user_id';

    public function __construct($hasher, string $model, private readonly LegacyPasswordHasher $legacyPasswordHasher)
    {
        parent::__construct($hasher, $model);
    }

    public function retrieveByCredentials(array $credentials): ?UserContract
    {
        if ($credentials === []) {
            return null;
        }

        $query = $this->createModel()->newQuery();

        if (array_key_exists('email', $credentials) || array_key_exists('Email', $credentials)) {
            $email = (string) ($credentials['email'] ?? $credentials['Email']);

            return $query
                ->where('NormalizedEmail', mb_strtoupper($email, 'UTF-8'))
                ->orWhere('Email', $email)
                ->first();
        }

        return parent::retrieveByCredentials($credentials);
    }

    public function validateCredentials(UserContract $user, array $credentials): bool
    {
        $plain = (string) ($credentials['password'] ?? '');
        if ($plain === '') {
            return false;
        }

        $result = $this->legacyPasswordHasher->verify((string) $user->getAuthPassword(), $plain);

        if ($result === LegacyHashVerificationResult::ForceReset) {
            $this->markForceReset($user);

            return false;
        }

        if ($result === LegacyHashVerificationResult::Failed) {
            $this->clearForceReset();

            return false;
        }

        $this->clearForceReset();

        if ($result === LegacyHashVerificationResult::VerifiedNeedsRehash) {
            $this->rehashPassword($user, $plain);
        }

        return true;
    }

    private function rehashPassword(UserContract $user, string $plain): void
    {
        if (! $user instanceof Model) {
            return;
        }

        $user->forceFill([
            'PasswordHash' => Hash::make($plain),
            'SecurityStamp' => (string) Str::uuid(),
        ]);

        $user->save();
    }

    private function markForceReset(UserContract $user): void
    {
        if (app()->bound('request') && request()->hasSession()) {
            request()->session()->put(self::FORCE_RESET_SESSION_KEY, (string) $user->getAuthIdentifier());
        }
    }

    private function clearForceReset(): void
    {
        if (app()->bound('request') && request()->hasSession()) {
            request()->session()->forget(self::FORCE_RESET_SESSION_KEY);
        }
    }
}
