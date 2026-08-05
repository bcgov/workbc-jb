<?php

namespace App\Http\Controllers\Auth;

use App\Http\Controllers\Controller;
use App\Http\Requests\Auth\JobSeekerForgotPasswordRequest;
use App\Http\Requests\Auth\JobSeekerResetPasswordRequest;
use App\Services\JobSeeker\JobSeekerAuthService;
use Illuminate\Http\JsonResponse;

final class JobSeekerPasswordResetController extends Controller
{
    public function __construct(private readonly JobSeekerAuthService $authService) {}

    public function request(JobSeekerForgotPasswordRequest $request): JsonResponse
    {
        $this->authService->createPasswordResetToken((string) $request->string('email'));

        return response()->json(['status' => 'reset_link_queued']);
    }

    public function reset(JobSeekerResetPasswordRequest $request): JsonResponse
    {
        $ok = $this->authService->resetPassword(
            (string) $request->string('email'),
            (string) $request->string('token'),
            (string) $request->string('password')
        );

        return $ok
            ? response()->json(['status' => 'password_reset'])
            : response()->json(['status' => 'invalid_token'], 422);
    }
}
