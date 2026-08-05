<?php

namespace App\Services\Admin;

use App\Models\AdminUser;
use App\Models\JobSeeker;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Str;

/**
 * FND-6 / ADM-4 scaffold: an admin acting as a job seeker. Writes the audit
 * trail (`ImpersonationLog` — Token/AspNetUserId/AdminUserId/DateTokenCreated;
 * there is no "ended" column, confirmed against the real schema) and switches
 * the session to the seeker.
 *
 * The `admin` and `web` guards are independent Laravel session guards, so
 * logging the seeker into `web` never disturbs the admin's own `admin`-guard
 * session — ending impersonation is just logging `web` back out again.
 */
final class ImpersonationService
{
    private const SESSION_ADMIN_ID_KEY = 'impersonation.admin_id';

    private const SESSION_TOKEN_KEY = 'impersonation.token';

    public function start(AdminUser $admin, JobSeeker $seeker): string
    {
        $token = Str::random(128);

        DB::table('ImpersonationLog')->insert([
            'Token' => $token,
            'AspNetUserId' => (string) $seeker->Id,
            'AdminUserId' => (int) $admin->Id,
            'DateTokenCreated' => now(),
        ]);

        Auth::guard('web')->login($seeker);

        session()->put(self::SESSION_ADMIN_ID_KEY, (int) $admin->Id);
        session()->put(self::SESSION_TOKEN_KEY, $token);

        return $token;
    }

    public function end(): void
    {
        // Guard-scoped logout only — never session()->invalidate(), which would
        // also drop the admin's own (unrelated) `admin`-guard session.
        Auth::guard('web')->logout();

        session()->forget([self::SESSION_ADMIN_ID_KEY, self::SESSION_TOKEN_KEY]);
    }

    public function isActive(): bool
    {
        return session()->has(self::SESSION_TOKEN_KEY) && Auth::guard('web')->check();
    }

    public function impersonatingAdminId(): ?int
    {
        $id = session(self::SESSION_ADMIN_ID_KEY);

        return $id !== null ? (int) $id : null;
    }
}
