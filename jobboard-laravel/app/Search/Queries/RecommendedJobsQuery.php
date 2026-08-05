<?php

namespace App\Search\Queries;

/**
 * Builds the OpenSearch request body for ACCT-5 recommended jobs as a
 * structured PHP array (never string-concatenated JSON).
 *
 * Read-model only (ADR-001 / Rule B): it filters and scores against derived
 * index fields and keeps the base ExpireDate >= now/d Vancouver filter.
 */
final class RecommendedJobsQuery
{
    private const TIME_ZONE = 'America/Vancouver';

    public const MAX_SAVED_JOBS = 200;

    private const WORKPLACE_VIRTUAL = 15141;

    private const BOOST_BASE_NOC_2021 = 1.0;

    private const BOOST_BASE_EMPLOYER = 1.0;

    private const BOOST_BASE_TITLE = 1.0;

    private const BOOST_BASE_CITY = 1.0;

    private const BOOST_BASE_EQUITY = 0.25;

    private const BOOST_REPEAT_BONUS = 0.01;

    /**
     * @param  array<int, int>  $nocCounts
     * @param  array<string, int>  $employerCounts
     * @param  array<string, int>  $titleCounts
     * @param  list<string>  $equityFields
     * @param  list<string>  $ignoreJobIds
     */
    public function __construct(
        private readonly array $nocCounts,
        private readonly array $employerCounts,
        private readonly array $titleCounts,
        private readonly ?string $city,
        private readonly array $equityFields,
        private readonly array $ignoreJobIds,
        private readonly int $page,
        private readonly int $pageSize,
    ) {}

    /**
     * @return array<string, mixed>
     */
    public function build(): array
    {
        $page = $this->page <= 0 ? 1 : $this->page;
        $pageSize = max(0, $this->pageSize);
        $from = ($page - 1) * $pageSize;

        $should = $this->shouldClauses();
        $mustNot = $this->mustNotClauses();

        return [
            'track_total_hits' => true,
            'size' => $pageSize,
            'from' => $from,
            '_source' => [
                'JobId',
                'Title',
                'EmployerName',
                'City',
                'Noc2021',
                'IsApprentice',
                'IsVeteran',
                'IsAboriginal',
                'IsMatureWorker',
                'IsNewcomer',
                'IsDisability',
                'IsStudent',
                'IsVismin',
                'IsYouth',
            ],
            'query' => [
                'bool' => [
                    'should' => $should,
                    'minimum_should_match' => 1,
                    'must_not' => $mustNot,
                    'filter' => [
                        'range' => [
                            'ExpireDate' => [
                                'gte' => 'now/d',
                                'time_zone' => self::TIME_ZONE,
                            ],
                        ],
                    ],
                ],
            ],
        ];
    }

    /**
     * @return array<int, array<string, mixed>>
     */
    private function shouldClauses(): array
    {
        $clauses = [];

        foreach ($this->nocCounts as $noc => $count) {
            $clauses[] = ['term' => ['Noc2021' => [
                'value' => (int) $noc,
                'boost' => $this->repeatBoost(self::BOOST_BASE_NOC_2021, $count),
            ]]];
        }

        foreach ($this->employerCounts as $employer => $count) {
            $clauses[] = ['term' => ['EmployerName.normalize' => [
                'value' => $employer,
                'boost' => $this->repeatBoost(self::BOOST_BASE_EMPLOYER, $count),
            ]]];
        }

        foreach ($this->titleCounts as $title => $count) {
            $clauses[] = ['term' => ['Title.normalize' => [
                'value' => $title,
                'boost' => $this->repeatBoost(self::BOOST_BASE_TITLE, $count),
            ]]];
        }

        if (is_string($this->city) && $this->city !== '') {
            $clauses[] = ['term' => ['City.normalize' => [
                'value' => $this->city,
                'boost' => self::BOOST_BASE_CITY,
            ]]];

            // Legacy parity: virtual jobs are kept with zero city boost.
            $clauses[] = ['term' => ['WorkplaceType.Id' => [
                'value' => self::WORKPLACE_VIRTUAL,
                'boost' => 0,
            ]]];
        }

        foreach ($this->equityFields as $field) {
            $clauses[] = ['term' => [$field => [
                'value' => true,
                'boost' => self::BOOST_BASE_EQUITY,
            ]]];
        }

        return $clauses;
    }

    /**
     * @return array<int, array<string, mixed>>
     */
    private function mustNotClauses(): array
    {
        if ($this->ignoreJobIds === []) {
            return [];
        }

        return [['terms' => ['JobId.keyword' => $this->ignoreJobIds]]];
    }

    private function repeatBoost(float $base, int $count): float
    {
        return $base + (self::BOOST_REPEAT_BONUS * $count);
    }
}
