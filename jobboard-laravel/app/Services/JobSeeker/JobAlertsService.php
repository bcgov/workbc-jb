<?php

namespace App\Services\JobSeeker;

use App\Models\Enums\AlertFrequency;
use App\Models\JobAlert;
use App\Models\JobSeeker;
use App\Search\Filters\JobSearchFilters;
use App\Search\Url\FilterUrlSerializer;
use App\Services\Search\JobSearchService;
use Illuminate\Database\DatabaseManager;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Str;
use Throwable;

final class JobAlertsService
{
    public function __construct(
        private readonly JobSearchService $jobSearchService,
        private readonly FilterUrlSerializer $filterUrlSerializer,
    ) {}

    /**
     * @return \Illuminate\Support\Collection<int, JobAlert>
     */
    public function listFor(JobSeeker $jobSeeker)
    {
        return JobAlert::query()
            ->where('AspNetUserId', (string) $jobSeeker->Id)
            ->orderByDesc('Id')
            ->get();
    }

    public function findFor(JobSeeker $jobSeeker, int $alertId): ?JobAlert
    {
        return JobAlert::query()
            ->where('AspNetUserId', (string) $jobSeeker->Id)
            ->where('Id', $alertId)
            ->first();
    }

    public function previewTotal(JobSearchFilters $filters): int
    {
        $preview = clone $filters;
        $preview->PageSize = 0;

        return $this->jobSearchService->search($preview)->count;
    }

    public function save(JobSeeker $jobSeeker, ?int $alertId, string $title, AlertFrequency $frequency, JobSearchFilters $filters): JobAlert
    {
        $title = trim($title);
        $canonicalFilters = $this->canonicalizeFilters($filters);
        $urlParameters = $this->filterUrlSerializer->toUrl($canonicalFilters);

        return DB::transaction(function () use ($jobSeeker, $alertId, $title, $frequency, $canonicalFilters, $urlParameters): JobAlert {
            $alert = $alertId !== null
                ? JobAlert::query()->where('AspNetUserId', (string) $jobSeeker->Id)->where('Id', $alertId)->first()
                : null;

            if ($alertId !== null && $alert === null) {
                throw new \RuntimeException('The job alert does not exist or has been deleted.');
            }

            if ($alert === null) {
                $alert = new JobAlert;
                $alert->AspNetUserId = (string) $jobSeeker->Id;
                $alert->DateCreated = now();
            }

            $alert->Title = $title;
            $alert->AlertFrequency = $frequency;
            $alert->JobSearchFiltersVersion = 1;
            $alert->JobSearchFilters = $canonicalFilters;
            $alert->UrlParameters = $urlParameters;
            $alert->DateModified = now();
            $alert->IsDeleted = false;
            $alert->DateDeleted = null;
            $alert->save();

            $this->writeAuditRow($alert->AspNetUserId, null, $title, $alertId === null ? 'created' : 'updated');

            return $alert;
        });
    }

    public function delete(JobSeeker $jobSeeker, int $alertId): bool
    {
        return DB::transaction(function () use ($jobSeeker, $alertId): bool {
            $alert = JobAlert::query()
                ->where('AspNetUserId', (string) $jobSeeker->Id)
                ->where('Id', $alertId)
                ->first();

            if ($alert === null) {
                return false;
            }

            $title = (string) $alert->Title;
            $alert->delete();
            $this->writeAuditRow((string) $jobSeeker->Id, null, $title, 'deleted');

            return true;
        });
    }

    private function canonicalizeFilters(JobSearchFilters $filters): JobSearchFilters
    {
        $canonical = clone $filters;
        $canonical->Page = 1;
        $canonical->PageSize = 20;
        $canonical->SortOrder = 1;

        if ($canonical->Keyword !== null) {
            $canonical->Keyword = trim(Str::of($canonical->Keyword)
                ->replace([';', '%'], ' ')
                ->replaceMatches('/\s+/', ' ')
                ->toString());
            if ($canonical->Keyword === '') {
                $canonical->Keyword = null;
            }
        }

        return $canonical;
    }

    private function writeAuditRow(string $userId, ?int $adminUserId, string $title, string $action): void
    {
        DB::table('JobSeekerChangeLog')->insert([
            'AspNetUserId' => $userId,
            'ModifiedByAdminUserId' => $adminUserId,
            'DateUpdated' => now(),
            'Field' => "Job alert '{$title}' {$action}",
            'OldValue' => '-',
            'NewValue' => '-',
        ]);
    }
}
