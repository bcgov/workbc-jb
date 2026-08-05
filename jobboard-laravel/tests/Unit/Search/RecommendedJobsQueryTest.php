<?php

namespace Tests\Unit\Search;

use App\Search\Queries\RecommendedJobsQuery;
use PHPUnit\Framework\TestCase;

class RecommendedJobsQueryTest extends TestCase
{
    /**
     * @param  array<int, int>  $nocCounts
     * @param  array<string, int>  $employerCounts
     * @param  array<string, int>  $titleCounts
     * @param  list<string>  $equityFields
     * @param  list<string>  $ignoreJobIds
     * @return array<string, mixed>
     */
    private function build(
        array $nocCounts,
        array $employerCounts,
        array $titleCounts,
        ?string $city,
        array $equityFields,
        array $ignoreJobIds,
    ): array {
        return (new RecommendedJobsQuery(
            nocCounts: $nocCounts,
            employerCounts: $employerCounts,
            titleCounts: $titleCounts,
            city: $city,
            equityFields: $equityFields,
            ignoreJobIds: $ignoreJobIds,
            page: 1,
            pageSize: 20,
        ))->build();
    }

    /**
     * @param  array<string, mixed>  $body
     * @return array<int, array<string, mixed>>
     */
    private function shouldEntries(array $body): array
    {
        return $body['query']['bool']['should'];
    }

    public function test_it_uses_exact_acct5_boost_values_and_exclusions(): void
    {
        $body = $this->build(
            nocCounts: [62100 => 2],
            employerCounts: ['acme ltd' => 2],
            titleCounts: ['cook' => 3],
            city: 'vancouver',
            equityFields: ['IsStudent', 'IsAboriginal'],
            ignoreJobIds: ['saved-1', 'saved-2'],
        );

        $this->assertSame(1, $body['query']['bool']['minimum_should_match']);
        $this->assertSame('now/d', $body['query']['bool']['filter']['range']['ExpireDate']['gte']);
        $this->assertSame('America/Vancouver', $body['query']['bool']['filter']['range']['ExpireDate']['time_zone']);

        $entries = $this->shouldEntries($body);

        $this->assertContains(['term' => ['Noc2021' => ['value' => 62100, 'boost' => 1.02]]], $entries);
        $this->assertContains(['term' => ['EmployerName.normalize' => ['value' => 'acme ltd', 'boost' => 1.02]]], $entries);
        $this->assertContains(['term' => ['Title.normalize' => ['value' => 'cook', 'boost' => 1.03]]], $entries);
        $this->assertContains(['term' => ['City.normalize' => ['value' => 'vancouver', 'boost' => 1.0]]], $entries);
        $this->assertContains(['term' => ['WorkplaceType.Id' => ['value' => 15141, 'boost' => 0]]], $entries);
        $this->assertContains(['term' => ['IsStudent' => ['value' => true, 'boost' => 0.25]]], $entries);
        $this->assertContains(['term' => ['IsAboriginal' => ['value' => true, 'boost' => 0.25]]], $entries);

        $this->assertSame([
            ['terms' => ['JobId.keyword' => ['saved-1', 'saved-2']]],
        ], $body['query']['bool']['must_not']);
    }

    public function test_repeat_saves_raise_boost_by_point_zero_one_per_save(): void
    {
        $once = $this->build(
            nocCounts: [],
            employerCounts: ['acme ltd' => 1],
            titleCounts: [],
            city: null,
            equityFields: [],
            ignoreJobIds: [],
        );

        $twice = $this->build(
            nocCounts: [],
            employerCounts: ['acme ltd' => 2],
            titleCounts: [],
            city: null,
            equityFields: [],
            ignoreJobIds: [],
        );

        $onceBoost = $once['query']['bool']['should'][0]['term']['EmployerName.normalize']['boost'];
        $twiceBoost = $twice['query']['bool']['should'][0]['term']['EmployerName.normalize']['boost'];

        $this->assertSame(1.01, $onceBoost);
        $this->assertSame(1.02, $twiceBoost);
    }
}
