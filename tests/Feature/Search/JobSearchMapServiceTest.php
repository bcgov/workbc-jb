<?php

namespace Tests\Feature\Search;

use App\Search\Filters\JobSearchFilters;
use App\Services\Search\JobSearchService;
use Mockery;
use OpenSearch\Client;
use Tests\Fakes\FakeGeocoder;
use Tests\TestCase;

/**
 * SRCH-9 service test: mapPins() runs the map query path against the derived
 * index (read-only) and reduces the hits to Google Maps pins.
 */
class JobSearchMapServiceTest extends TestCase
{
    /** @var array<string, mixed> */
    private array $captured = [];

    private function service(): JobSearchService
    {
        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')
            ->andReturnUsing(function (array $params): array {
                $this->captured = $params;

                return [
                    'hits' => [
                        'total' => ['value' => 2],
                        'hits' => [
                            ['_source' => [
                                'JobId' => '100', 'Title' => 'Baker',
                                'City' => ['Victoria'], 'Region' => ['Vancouver Island/Coast'],
                                'Location' => [['Lat' => '48.4', 'Lon' => '-123.3']],
                            ]],
                            ['_source' => [
                                'JobId' => '200', 'Title' => 'Cook',
                                'City' => ['Vancouver'], 'Region' => ['Mainland/Southwest'],
                                'Location' => [['Lat' => '49.2', 'Lon' => '-123.1']],
                            ]],
                        ],
                    ],
                ];
            });

        return new JobSearchService($client, new FakeGeocoder());
    }

    public function test_it_sends_the_map_query_body(): void
    {
        $this->service()->mapPins(JobSearchFilters::fromArray(['PageSize' => 20]));

        $body = $this->captured['body'];
        $this->assertSame(5000, $body['size']);
        $this->assertSame(['Location', 'JobId', 'City', 'Region', 'Title'], $body['_source']);
        $this->assertContains(
            ['exists' => ['field' => 'LocationGeo']],
            $body['query']['bool']['filter']['bool']['must'],
        );
    }

    public function test_it_queries_the_english_index_by_default(): void
    {
        $this->service()->mapPins(JobSearchFilters::fromArray(['PageSize' => 20]));

        $this->assertSame(config('opensearch.indexes.en'), $this->captured['index']);
    }

    public function test_it_projects_hits_into_pins(): void
    {
        $pins = $this->service()->mapPins(JobSearchFilters::fromArray(['PageSize' => 20]));

        $this->assertCount(2, $pins);
        $this->assertSame([
            'JobId' => '100',
            'Latitude' => '48.4',
            'Longitude' => '-123.3',
            'Title' => 'Baker',
        ], $pins[0]);
    }

    protected function tearDown(): void
    {
        Mockery::close();
        parent::tearDown();
    }
}
