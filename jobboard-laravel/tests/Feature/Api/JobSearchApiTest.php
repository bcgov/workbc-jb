<?php

namespace Tests\Feature\Api;

use Mockery;
use OpenSearch\Client;
use Tests\TestCase;

/**
 * SRCH-10 — POST /api/Search/JobSearch (contracts.md §2.1): response shape +
 * casing for a federal and an external job, strict unknown-field rejection,
 * and the profile-sidebar federal-first heuristic.
 */
class JobSearchApiTest extends TestCase
{
    /** @var array<int, array<string, mixed>> */
    private array $bodies = [];

    private function bindClient(callable $responder): void
    {
        $this->bodies = [];

        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')
            ->andReturnUsing(function (array $params) use ($responder): array {
                $this->bodies[] = $params['body'];

                return $responder($params);
            });

        $this->app->instance(Client::class, $client);
    }

    public function test_response_shape_and_casing_for_a_federal_job(): void
    {
        $this->bindClient(fn (): array => [
            'hits' => [
                'total' => ['value' => 1],
                'hits' => [
                    ['_source' => [
                        'JobId' => 'fed-1',
                        'Title' => 'Policy Analyst',
                        'EmployerName' => 'Government of Canada',
                        'DatePosted' => '2026-06-09T13:58:07',
                        'ExpireDate' => '2026-09-07T13:58:07',
                        'City' => ['Victoria'],
                        'Province' => 'British Columbia',
                        'Region' => ['Vancouver Island/Coast'],
                        'Location' => [['Lat' => '48.4', 'Lon' => '-123.3']],
                        'Noc2021' => 10.0,
                        'NocGroup' => 'Policy analysts (10)',
                        'Salary' => 90000.0,
                        'SalarySummary' => '$90,000 annually',
                        'IsFederalJob' => true,
                        'HoursOfWork' => ['Description' => ['Full-time']],
                        'WorkplaceType' => ['Id' => 0, 'Description' => 'On-site only'],
                        'ApplyWebsite' => 'https://canada.ca/apply',
                        'EduLevel' => ['University'], // filter-only index field
                    ]],
                ],
            ],
        ]);

        $response = $this->postJson('/api/Search/JobSearch', ['PageSize' => 20]);

        $response->assertOk();
        $response->assertJsonStructure(['count', 'result', 'pageNumber', 'pageSize']);
        $response->assertJson([
            'count' => 1,
            'pageNumber' => 1,
            'pageSize' => 20,
        ]);

        $item = $response->json('result.0');
        $this->assertSame('fed-1', $item['JobId']);
        $this->assertSame('Policy Analyst', $item['Title']);
        $this->assertSame('Victoria', $item['City']); // CSV string, not an array
        $this->assertSame('00010', $item['Noc2021']); // zero-padded 5-char string
        $this->assertTrue($item['IsFederalJob']);
        $this->assertSame(['Description' => ['Full-time']], $item['HoursOfWork']);
        $this->assertArrayNotHasKey('ExternalSource', $item); // omitted when absent
        $this->assertArrayNotHasKey('EduLevel', $item); // filter-only field never returned
    }

    public function test_response_shape_and_casing_for_an_external_job(): void
    {
        $this->bindClient(fn (): array => [
            'hits' => [
                'total' => ['value' => 1],
                'hits' => [
                    ['_source' => [
                        'JobId' => 'ext-1',
                        'Title' => 'Senior Analyst, Sales & Trading',
                        'EmployerName' => 'Hiive',
                        'City' => ['Vancouver'],
                        'IsFederalJob' => false,
                        'ExternalSource' => ['Source' => [['Url' => 'https://jobs.example/1', 'Source' => 'jobs.example']]],
                    ]],
                ],
            ],
        ]);

        $response = $this->postJson('/api/Search/JobSearch', []);

        $response->assertOk();
        $item = $response->json('result.0');

        $this->assertSame('ext-1', $item['JobId']);
        $this->assertFalse($item['IsFederalJob']);
        $this->assertSame('https://jobs.example/1', $item['ExternalSource']['Source'][0]['Url']);
        $this->assertArrayNotHasKey('WageClass', $item); // federal-only field omitted for external
        $this->assertArrayNotHasKey('Noc2021', $item); // absent/null fields are omitted entirely
    }

    public function test_unknown_field_is_rejected_with_http_400(): void
    {
        $this->bindClient(fn (): array => ['hits' => ['total' => ['value' => 0], 'hits' => []]]);

        $response = $this->postJson('/api/Search/JobSearch', ['NotARealField' => 'x']);

        $response->assertStatus(400);
    }

    /**
     * The IsFederalJob term the jobSourceGroup added to this query body, if any.
     *
     * @param  array<string, mixed>  $body
     */
    private function isFederalJobTerm(array $body): ?bool
    {
        foreach ($body['query']['bool']['must'] as $group) {
            $source = $group['bool']['should'][0]['term']['IsFederalJob'] ?? null;
            if ($source !== null) {
                return (bool) $source;
            }
        }

        return null;
    }

    public function test_profile_sidebar_heuristic_returns_federal_jobs_first(): void
    {
        $this->bindClient(function (array $params): array {
            $source = $this->isFederalJobTerm($params['body']);

            // The first (federal) attempt matches jobs; a federal-first response
            // must never fall through to an external re-query.
            if ($source === true) {
                return [
                    'hits' => [
                        'total' => ['value' => 1],
                        'hits' => [['_source' => ['JobId' => 'fed-2', 'Title' => 'Baker', 'IsFederalJob' => true]]],
                    ],
                ];
            }

            return ['hits' => ['total' => ['value' => 99], 'hits' => [['_source' => ['JobId' => 'ext-2', 'IsFederalJob' => false]]]]];
        });

        // NOC filter + PageSize <= 10 + no pinned source => federal-first.
        $response = $this->postJson('/api/Search/JobSearch', [
            'SearchNocField' => '62100',
            'PageSize' => 10,
        ]);

        $response->assertOk();
        $this->assertCount(1, $this->bodies, 'Federal jobs matched — must not fall back to a second (external) query.');
        $this->assertSame('fed-2', $response->json('result.0.JobId'));
        $this->assertTrue($response->json('result.0.IsFederalJob'));
    }

    public function test_profile_sidebar_heuristic_falls_back_to_external_when_no_federal_jobs_match(): void
    {
        $this->bindClient(function (array $params): array {
            $source = $this->isFederalJobTerm($params['body']);

            if ($source === true) {
                return ['hits' => ['total' => ['value' => 0], 'hits' => []]];
            }

            return [
                'hits' => [
                    'total' => ['value' => 1],
                    'hits' => [['_source' => ['JobId' => 'ext-3', 'Title' => 'Cook', 'IsFederalJob' => false]]],
                ],
            ];
        });

        $response = $this->postJson('/api/Search/JobSearch', [
            'SearchNocField' => '65201',
            'PageSize' => 5,
        ]);

        $response->assertOk();
        $this->assertCount(2, $this->bodies, 'Zero federal jobs — must fall back to a second (external) query.');
        $this->assertSame('ext-3', $response->json('result.0.JobId'));
        $this->assertFalse($response->json('result.0.IsFederalJob'));
    }

    public function test_no_federal_first_when_a_source_is_already_pinned(): void
    {
        $this->bindClient(fn (): array => ['hits' => ['total' => ['value' => 0], 'hits' => []]]);

        $this->postJson('/api/Search/JobSearch', [
            'SearchNocField' => '62100',
            'PageSize' => 10,
            'SearchJobSource' => '2',
        ]);

        // Only ONE query — the heuristic must not trigger when a source is pinned.
        $this->assertCount(1, $this->bodies);
    }

    public function test_optional_language_segment_selects_the_french_index(): void
    {
        $this->bindClient(fn (): array => ['hits' => ['total' => ['value' => 0], 'hits' => []]]);

        $this->postJson('/api/Search/JobSearch/fr', []);

        $this->assertSame('fr', app()->getLocale());
    }

    protected function tearDown(): void
    {
        Mockery::close();
        parent::tearDown();
    }
}
