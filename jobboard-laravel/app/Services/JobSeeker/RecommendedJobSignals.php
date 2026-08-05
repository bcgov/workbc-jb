<?php

namespace App\Services\JobSeeker;

/**
 * Aggregated recommendation signals from the seeker's saved jobs/profile.
 */
final readonly class RecommendedJobSignals
{
    /**
     * @param  list<string>  $savedJobIds
     * @param  array<int, int>  $nocCounts
     * @param  array<string, int>  $employerCounts
     * @param  array<string, int>  $titleCounts
     * @param  list<string>  $equityFields
     */
    public function __construct(
        public array $savedJobIds,
        public array $nocCounts,
        public array $employerCounts,
        public array $titleCounts,
        public ?string $city,
        public array $equityFields,
    ) {}

    public function hasSavedJobs(): bool
    {
        return $this->savedJobIds !== [];
    }
}
