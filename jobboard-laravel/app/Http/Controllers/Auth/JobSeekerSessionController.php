<?php

namespace App\Http\Controllers\Auth;

use App\Http\Controllers\Controller;
use App\Http\Requests\Auth\JobSeekerLoginRequest;
use App\Services\JobSeeker\JobSeekerAuthService;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\Auth;

final class JobSeekerSessionController extends Controller
{
    public function __construct(private readonly JobSeekerAuthService $authService) {}

    public function store(JobSeekerLoginRequest $request): JsonResponse
    {
        $result = $this->authService->attemptLogin(
            (string) $request->string('email'),
            (string) $request->string('password'),
            (bool) $request->boolean('remember')
        );

        return match ($result) {
            'ok' => response()->json(['status' => 'ok']),
            'force_reset' => response()->json(['status' => 'force_reset_required'], 409),
            default => response()->json(['status' => 'invalid_credentials'], 422),
        };
    }

    public function destroy(): JsonResponse
    {
        Auth::guard('web')->logout();

        request()->session()->invalidate();
        request()->session()->regenerateToken();

        return response()->json(['status' => 'signed_out']);
    }
}
