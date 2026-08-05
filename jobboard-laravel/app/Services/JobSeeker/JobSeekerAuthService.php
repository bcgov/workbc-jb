<?php

namespace App\Services\JobSeeker;

use App\Auth\JobSeekerUserProvider;
use App\Models\Enums\AccountStatus;
use App\Models\JobSeeker;
use Illuminate\Support\Carbon;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\Hash;
use Illuminate\Support\Facades\Password;
use Illuminate\Support\Str;

final class JobSeekerAuthService
{
    public function attemptLogin(string $email, string $password, bool $remember = false): string
    {
        if (request()->hasSession()) {
            request()->session()->forget(JobSeekerUserProvider::FORCE_RESET_SESSION_KEY);
        }

        $jobSeeker = $this->findByEmail($email);

        if ($jobSeeker !== null && $this->isLockoutEnabled($jobSeeker)) {
            if ($this->isLockedOut($jobSeeker)) {
                return 'invalid';
            }

            $this->clearExpiredLockout($jobSeeker);
        }

        $ok = Auth::guard('web')->attempt([
            'Email' => $email,
            'password' => $password,
        ], $remember);

        if ($ok) {
            if ($jobSeeker !== null && $this->isLockoutEnabled($jobSeeker)) {
                $this->clearFailures($jobSeeker);
            }

            return 'ok';
        }

        if ($jobSeeker !== null && request()->hasSession()) {
            $forcedId = (string) request()->session()->get(JobSeekerUserProvider::FORCE_RESET_SESSION_KEY, '');

            if ($forcedId === (string) $jobSeeker->Id) {
                return 'force_reset';
            }
        }

        if ($jobSeeker !== null && $this->isLockoutEnabled($jobSeeker)) {
            $this->recordFailedAttempt($jobSeeker);
        }

        return 'invalid';
    }

    public function register(string $email, ?string $username, string $password): JobSeeker
    {
        $normalizedEmail = $this->normalize($email);

        $seeker = new JobSeeker;
        $seeker->forceFill([
            'Id' => (string) Str::uuid(),
            'UserName' => $username ?? $email,
            'NormalizedUserName' => $this->normalize($username ?? $email),
            'Email' => $email,
            'NormalizedEmail' => $normalizedEmail,
            'PasswordHash' => Hash::make($password),
            'SecurityStamp' => (string) Str::uuid(),
            'AccountStatus' => AccountStatus::Pending->value,
            'EmailConfirmed' => false,
            'VerificationGuid' => (string) Str::uuid(),
            'DateRegistered' => now(),
            'LastModified' => now(),
        ]);
        $seeker->save();

        return $seeker->fresh();
    }

    public function verifyEmail(string $userId, string $verificationGuid): bool
    {
        $jobSeeker = JobSeeker::query()->find($userId);
        if ($jobSeeker === null) {
            return false;
        }

        if ((string) $jobSeeker->VerificationGuid !== $verificationGuid) {
            return false;
        }

        $jobSeeker->forceFill([
            'EmailConfirmed' => true,
            'AccountStatus' => AccountStatus::Active->value,
            'VerificationGuid' => null,
            'LastModified' => now(),
        ]);
        $jobSeeker->save();

        return true;
    }

    public function createPasswordResetToken(string $email): void
    {
        $user = JobSeeker::query()
            ->where('NormalizedEmail', $this->normalize($email))
            ->orWhere('Email', $email)
            ->first();

        if ($user === null) {
            return;
        }

        Password::broker('job_seekers')->createToken($user);
    }

    public function resetPassword(string $email, string $token, string $password): bool
    {
        $status = Password::broker('job_seekers')->reset([
            'email' => $email,
            'token' => $token,
            'password' => $password,
            'password_confirmation' => $password,
        ], function (JobSeeker $user, string $password): void {
            $user->forceFill([
                'PasswordHash' => Hash::make($password),
                'SecurityStamp' => (string) Str::uuid(),
                'NormalizedEmail' => $this->normalize((string) $user->Email),
                'NormalizedUserName' => $this->normalize((string) $user->UserName),
                'LastModified' => now(),
            ]);

            if (! $user->EmailConfirmed) {
                $user->EmailConfirmed = true;
                $user->AccountStatus = AccountStatus::Active->value;
            }

            $user->save();
        });

        return $status === Password::PASSWORD_RESET;
    }

    private function normalize(string $value): string
    {
        return mb_strtoupper(trim($value), 'UTF-8');
    }

    private function findByEmail(string $email): ?JobSeeker
    {
        return JobSeeker::query()
            ->where('NormalizedEmail', $this->normalize($email))
            ->orWhere('Email', $email)
            ->first();
    }

    private function isLockoutEnabled(JobSeeker $jobSeeker): bool
    {
        return (bool) $jobSeeker->LockoutEnabled;
    }

    private function isLockedOut(JobSeeker $jobSeeker): bool
    {
        if (! ($jobSeeker->LockoutEnd instanceof Carbon)) {
            return false;
        }

        return $jobSeeker->LockoutEnd->isFuture();
    }

    private function clearExpiredLockout(JobSeeker $jobSeeker): void
    {
        if (($jobSeeker->LockoutEnd instanceof Carbon) && $jobSeeker->LockoutEnd->isPast()) {
            $this->clearFailures($jobSeeker);
        }
    }

    private function clearFailures(JobSeeker $jobSeeker): void
    {
        if ((int) ($jobSeeker->AccessFailedCount ?? 0) === 0 && $jobSeeker->LockoutEnd === null && $jobSeeker->DateLocked === null) {
            return;
        }

        $jobSeeker->forceFill([
            'AccessFailedCount' => 0,
            'LockoutEnd' => null,
            'DateLocked' => null,
            'LastModified' => now(),
        ]);

        $jobSeeker->save();
    }

    private function recordFailedAttempt(JobSeeker $jobSeeker): void
    {
        $maxFailedAttempts = max(1, (int) config('auth.job_seeker_lockout.max_failed_attempts', 5));
        $lockoutMinutes = max(1, (int) config('auth.job_seeker_lockout.minutes', 30));
        $failedAttempts = (int) ($jobSeeker->AccessFailedCount ?? 0) + 1;

        $update = [
            'AccessFailedCount' => $failedAttempts,
            'LastModified' => now(),
        ];

        if ($failedAttempts >= $maxFailedAttempts) {
            $update['LockoutEnd'] = now()->addMinutes($lockoutMinutes);
            $update['DateLocked'] = now();
        }

        $jobSeeker->forceFill($update);
        $jobSeeker->save();
    }
}
