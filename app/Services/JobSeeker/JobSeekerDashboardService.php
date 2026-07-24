<?php

namespace App\Services\JobSeeker;

use App\Models\Enums\AlertFrequency;
use App\Models\JobSeeker;
use Illuminate\Support\Facades\DB;

final class JobSeekerDashboardService
{
    /**
     * @return array{savedJobs:int,activeAlerts:int,savedCareerProfiles:int,savedIndustryProfiles:int}
     */
    public function summaryFor(JobSeeker $jobSeeker): array
    {
        $userId = (string) $jobSeeker->Id;

        return [
            // SavedJob/JobAlert soft-deletes are excluded by the model global scope.
            'savedJobs' => $jobSeeker->savedJobs()->count(),
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
}
