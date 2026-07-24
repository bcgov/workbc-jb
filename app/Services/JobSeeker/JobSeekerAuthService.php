<?php

namespace App\Services\JobSeeker;

use App\Auth\JobSeekerUserProvider;
use App\Models\Enums\AccountStatus;
use App\Models\JobSeeker;
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

        $ok = Auth::guard('web')->attempt([
            'Email' => $email,
            'password' => $password,
        ], $remember);

        if ($ok) {
            return 'ok';
        }

        $jobSeeker = JobSeeker::query()
            ->where('NormalizedEmail', $this->normalize($email))
            ->orWhere('Email', $email)
            ->first();

        if ($jobSeeker !== null && request()->hasSession()) {
            $forcedId = (string) request()->session()->get(JobSeekerUserProvider::FORCE_RESET_SESSION_KEY, '');

            if ($forcedId === (string) $jobSeeker->Id) {
                return 'force_reset';
            }
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
}
