<?php

namespace App\Services\JobSeeker;

use App\Models\JobSeeker;
use App\Models\SavedJob;
use App\Search\Queries\RecommendedJobsQuery;
use App\Search\Results\JobResult;
use OpenSearch\Client;

final class RecommendedJobsService
{
    /** @var array<string, string> */
    private const SEEKER_FLAG_TO_INDEX_FIELD = [
        'IsApprentice' => 'IsApprentice',
        'IsVeteran' => 'IsVeteran',
        'IsIndigenousPerson' => 'IsAboriginal',
        'IsMatureWorker' => 'IsMatureWorker',
        'IsNewImmigrant' => 'IsNewcomer',
        'IsPersonWithDisability' => 'IsDisability',
        'IsStudent' => 'IsStudent',
        'IsVisibleMinority' => 'IsVismin',
        'IsYouth' => 'IsYouth',
    ];

    /** @var array<string, string> */
    private const EQUITY_REASON_LABELS = [
        'IsApprentice' => 'an apprentice',
        'IsVeteran' => 'a veteran',
        'IsAboriginal' => 'an Indigenous person',
        'IsMatureWorker' => 'a mature worker',
        'IsNewcomer' => 'a newcomer',
        'IsDisability' => 'a person with a disability',
        'IsStudent' => 'a student',
        'IsVismin' => 'a visible minority',
        'IsYouth' => 'youth',
    ];

    public function __construct(private Client $client) {}

    public function aggregateSignals(JobSeeker $jobSeeker): RecommendedJobSignals
    {
        $rows = SavedJob::query()
            ->leftJoin('Jobs', 'Jobs.JobId', '=', 'SavedJobs.JobId')
            ->where('SavedJobs.AspNetUserId', (string) $jobSeeker->Id)
            ->orderByDesc('SavedJobs.DateSaved')
            ->limit(RecommendedJobsQuery::MAX_SAVED_JOBS)
            ->get([
                'SavedJobs.JobId',
                'Jobs.NocCodeId2021',
                'Jobs.EmployerName',
                'Jobs.Title',
            ]);

        $nocCounts = [];
        $employerCounts = [];
        $titleCounts = [];
        $savedJobIds = [];

        foreach ($rows as $row) {
            $jobId = trim((string) ($row->JobId ?? ''));
            if ($jobId !== '') {
                $savedJobIds[$jobId] = true;
            }

            $noc = (int) ($row->NocCodeId2021 ?? 0);
            $nocCounts[$noc] = ($nocCounts[$noc] ?? 0) + 1;

            $employer = $this->normalizeText($row->EmployerName ?? null);
            if ($employer !== '') {
                $employerCounts[$employer] = ($employerCounts[$employer] ?? 0) + 1;
            }

            $title = $this->normalizeText($row->Title ?? null);
            if ($title !== '') {
                $titleCounts[$title] = ($titleCounts[$title] ?? 0) + 1;
            }
        }

        $equityFields = [];
        $flags = $jobSeeker->flags()->first();

        if ($flags !== null) {
            foreach (self::SEEKER_FLAG_TO_INDEX_FIELD as $seekerField => $indexField) {
                if ((bool) ($flags->{$seekerField} ?? false)) {
                    $equityFields[] = $indexField;
                }
            }
        }

        return new RecommendedJobSignals(
            savedJobIds: array_values(array_keys($savedJobIds)),
            nocCounts: $nocCounts,
            employerCounts: $employerCounts,
            titleCounts: $titleCounts,
            city: $this->normalizeText($jobSeeker->City ?? null),
            equityFields: $equityFields,
        );
    }

    /**
     * @return array{signals: RecommendedJobSignals, total: int, jobs: list<array<string, mixed>>}
     */
    public function recommendationsFor(JobSeeker $jobSeeker, int $page = 1, int $pageSize = 20): array
    {
        $signals = $this->aggregateSignals($jobSeeker);

        if (! $signals->hasSavedJobs()) {
            return ['signals' => $signals, 'total' => 0, 'jobs' => []];
        }

        $response = $this->client->search([
            'index' => $this->index(),
            'body' => (new RecommendedJobsQuery(
                nocCounts: $signals->nocCounts,
                employerCounts: $signals->employerCounts,
                titleCounts: $signals->titleCounts,
                city: $signals->city,
                equityFields: $signals->equityFields,
                ignoreJobIds: $signals->savedJobIds,
                page: $page,
                pageSize: $pageSize,
            ))->build(),
        ]);

        $hits = $response['hits']['hits'] ?? [];
        $total = (int) ($response['hits']['total']['value'] ?? 0);

        $jobs = [];
        foreach ($hits as $hit) {
            $source = is_array($hit['_source'] ?? null) ? $hit['_source'] : [];
            $job = JobResult::fromSource($source)->toArray();
            $job['Score'] = (float) ($hit['_score'] ?? 0.0);
            $job['Reason'] = $this->reasonFor($source, $signals);
            $jobs[] = $job;
        }

        return [
            'signals' => $signals,
            'total' => $total,
            'jobs' => $jobs,
        ];
    }

    public function recommendedCountFor(JobSeeker $jobSeeker): int
    {
        $signals = $this->aggregateSignals($jobSeeker);

        if (! $signals->hasSavedJobs()) {
            return 0;
        }

        $response = $this->client->search([
            'index' => $this->index(),
            'body' => (new RecommendedJobsQuery(
                nocCounts: $signals->nocCounts,
                employerCounts: $signals->employerCounts,
                titleCounts: $signals->titleCounts,
                city: $signals->city,
                equityFields: $signals->equityFields,
                ignoreJobIds: $signals->savedJobIds,
                page: 1,
                pageSize: 0,
            ))->build(),
        ]);

        return (int) ($response['hits']['total']['value'] ?? 0);
    }

    /**
     * @param  array<string, mixed>  $source
     */
    private function reasonFor(array $source, RecommendedJobSignals $signals): string
    {
        $clauses = [];

        $noc = (int) round((float) ($source['Noc2021'] ?? 0));
        $nocCount = $signals->nocCounts[$noc] ?? 0;
        if ($nocCount > 0) {
            $clauses[] = sprintf(
                'having the same NOC code as %s of your saved jobs',
                $this->countWord($nocCount),
            );
        }

        $employer = $this->normalizeText($source['EmployerName'] ?? null);
        $employerCount = $employer !== '' ? ($signals->employerCounts[$employer] ?? 0) : 0;
        if ($employerCount > 0) {
            $clauses[] = sprintf(
                'having the same employer as %s of your saved jobs',
                $this->countWord($employerCount),
            );
        }

        $title = $this->normalizeText($source['Title'] ?? null);
        $titleCount = $title !== '' ? ($signals->titleCounts[$title] ?? 0) : 0;
        if ($titleCount > 0) {
            $clauses[] = sprintf(
                'having the same job title as %s of your saved jobs',
                $this->countWord($titleCount),
            );
        }

        if ($signals->city !== null && $signals->city !== '') {
            $cities = $this->normalizeCities($source['City'] ?? null);
            if (in_array($signals->city, $cities, true)) {
                $clauses[] = 'being in your city';
            }
        }

        $equityMatches = [];
        foreach ($signals->equityFields as $field) {
            if (! array_key_exists($field, self::EQUITY_REASON_LABELS)) {
                continue;
            }

            if ((bool) ($source[$field] ?? false)) {
                $equityMatches[] = self::EQUITY_REASON_LABELS[$field];
            }
        }

        if ($clauses === [] && $equityMatches === []) {
            return 'Recommended based on your saved jobs and profile.';
        }

        $reason = 'Recommended based on '.implode(', ', $clauses).'.';

        if ($equityMatches !== []) {
            $reason .= ' This job also matches your profile as '.$this->joinWithAnd($equityMatches).'.';
        }

        return $reason;
    }

    /**
     * @return list<string>
     */
    private function normalizeCities(mixed $city): array
    {
        if (is_array($city)) {
            $cities = array_map(fn (mixed $value): string => $this->normalizeText($value), $city);

            return array_values(array_filter($cities, static fn (string $value): bool => $value !== ''));
        }

        $normalized = $this->normalizeText($city);

        return $normalized === '' ? [] : [$normalized];
    }

    private function normalizeText(mixed $value): string
    {
        $text = trim((string) ($value ?? ''));

        return $text === '' ? '' : mb_strtolower($text, 'UTF-8');
    }

    private function countWord(int $count): string
    {
        return match ($count) {
            1 => 'one',
            2 => 'two',
            3 => 'three',
            4 => 'four',
            5 => 'five',
            6 => 'six',
            7 => 'seven',
            8 => 'eight',
            9 => 'nine',
            10 => 'ten',
            default => (string) $count,
        };
    }

    /**
     * @param  list<string>  $parts
     */
    private function joinWithAnd(array $parts): string
    {
        $count = count($parts);

        if ($count <= 1) {
            return $parts[0] ?? '';
        }

        if ($count === 2) {
            return $parts[0].' and '.$parts[1];
        }

        $last = array_pop($parts);

        return implode(', ', $parts).', and '.$last;
    }

    private function index(): string
    {
        $key = app()->getLocale() === 'fr' ? 'fr' : 'en';

        return (string) config("opensearch.indexes.{$key}", config('opensearch.indexes.en'));
    }
}
