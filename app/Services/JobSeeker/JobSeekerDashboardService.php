<?php

namespace App\Services\JobSeeker;

use App\Models\Enums\AlertFrequency;
use App\Models\JobSeeker;
use Illuminate\Support\Facades\DB;

final class JobSeekerDashboardService
{
    public function __construct(private RecommendedJobsService $recommendedJobs) {}

    /**
     * @return array{savedJobs:int,recommendedJobs:int,activeAlerts:int,savedCareerProfiles:int,savedIndustryProfiles:int}
     */
    public function summaryFor(JobSeeker $jobSeeker): array
    {
        $userId = (string) $jobSeeker->Id;

        return [
            // SavedJob/JobAlert soft-deletes are excluded by the model global scope.
            'savedJobs' => $jobSeeker->savedJobs()->count(),
            // ACCT-5 reads OpenSearch recommendations derived from saved-job signals.
            // Dashboard should stay available if OpenSearch is temporarily unavailable.
            'recommendedJobs' => $this->recommendedCount($jobSeeker),
            // ACCT-1 active alerts: not deleted and frequency is not Never (5).
            'activeAlerts' => $jobSeeker->jobAlerts()
                ->where('AlertFrequency', '!=', AlertFrequency::Never->value)
                ->count(),
            // Models for Saved*Profiles arrive in ACCT-6; query-builder reads only.
            'savedCareerProfiles' => DB::table('SavedCareerProfiles')
                ->where('AspNetUserId', $userId)
                ->where('IsDeleted', false)
                ->count(),
            'savedIndustryProfiles' => DB::table('SavedIndustryProfiles')
                ->where('AspNetUserId', $userId)
                ->where('IsDeleted', false)
                ->count(),
        ];
    }

    private function recommendedCount(JobSeeker $jobSeeker): int
    {
        try {
            return $this->recommendedJobs->recommendedCountFor($jobSeeker);
        } catch (\Throwable $e) {
            report($e);

            return 0;
        }
    }
}
