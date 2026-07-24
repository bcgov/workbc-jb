<?php

namespace App\Http\Controllers\Auth;

use App\Http\Controllers\Controller;
use App\Http\Requests\Auth\JobSeekerRegisterRequest;
use App\Models\JobSeeker;
use App\Services\JobSeeker\JobSeekerAuthService;
use Illuminate\Http\JsonResponse;

final class JobSeekerRegistrationController extends Controller
{
    public function __construct(private readonly JobSeekerAuthService $authService) {}

    public function store(JobSeekerRegisterRequest $request): JsonResponse
    {
        $existing = JobSeeker::query()
            ->where('NormalizedEmail', mb_strtoupper((string) $request->string('email'), 'UTF-8'))
            ->first();

        if ($existing !== null) {
            return response()->json(['message' => 'Email already exists.'], 422);
        }

        $seeker = $this->authService->register(
            (string) $request->string('email'),
            $request->filled('username') ? (string) $request->string('username') : null,
            (string) $request->string('password')
        );

        return response()->json([
            'id' => $seeker->Id,
            'verificationGuid' => $seeker->VerificationGuid,
        ], 201);
    }

    public function verify(string $userId, string $guid): JsonResponse
    {
        $ok = $this->authService->verifyEmail($userId, $guid);

        return $ok
            ? response()->json(['status' => 'verified'])
            : response()->json(['status' => 'invalid_verification'], 422);
    }
}
