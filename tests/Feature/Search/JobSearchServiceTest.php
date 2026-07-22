<?php

namespace Tests\Feature\Search;

use App\Search\Filters\JobSearchFilters;
use App\Services\Search\JobSearchService;
use Mockery;
use OpenSearch\Client;
use Tests\Fakes\FakeGeocoder;
use Tests\TestCase;

class JobSearchServiceTest extends TestCase
{
    /** @var array<string, mixed> */
    private array $captured = [];

    private function service(int $total = 2): JobSearchService
    {
        return new JobSearchService($this->fakeClient($total), new FakeGeocoder());
    }

    private function fakeClient(int $total = 2): Client
    {
        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')
            ->andReturnUsing(function (array $params) use ($total): array {
                $this->captured = $params;

                return [
                    'hits' => [
                        'total' => ['value' => $total],
                        'hits' => [
                            ['_source' => ['JobId' => '100', 'Title' => 'Baker', 'EmployerName' => 'Acme', 'City' => ['Victoria']]],
                            ['_source' => ['JobId' => '200', 'Title' => 'Cook', 'EmployerName' => 'BizCo', 'City' => ['Vancouver']]],
                        ],
                    ],
                ];
            });

        return $client;
    }

    public function test_it_queries_the_english_index_by_default(): void
    {
        $service = $this->service();

        $service->search(JobSearchFilters::fromArray(['PageSize' => 20]));

        $this->assertSame(config('opensearch.indexes.en'), $this->captured['index']);
    }

    public function test_it_sends_the_structured_query_body_with_the_base_expiry_filter(): void
    {
        $service = $this->service();

        $service->search(JobSearchFilters::fromArray(['PageSize' => 20]));

        $body = $this->captured['body'];
        $this->assertTrue($body['track_total_hits']);
        $this->assertSame('now/d', $body['query']['bool']['filter']['range']['ExpireDate']['gte']);
        $this->assertSame('America/Vancouver', $body['query']['bool']['filter']['range']['ExpireDate']['time_zone']);
    }

    public function test_it_projects_hits_to_the_search_result_dto(): void
    {
        $service = $this->service(total: 42);

        $result = $service->search(JobSearchFilters::fromArray(['Page' => 3, 'PageSize' => 20]));

        $this->assertSame(42, $result->count);
        $this->assertSame(3, $result->pageNumber);
        $this->assertSame(20, $result->pageSize);
        $this->assertCount(2, $result->results);
        $this->assertSame('Baker', $result->results[0]->toArray()['Title']);
    }

    protected function tearDown(): void
    {
        Mockery::close();
        parent::tearDown();
    }
}
