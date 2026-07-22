<?php

namespace Tests\Feature\Search;

use App\Services\Search\LocationService;
use Mockery;
use OpenSearch\Client;
use PHPUnit\Framework\Attributes\DataProvider;
use Tests\TestCase;

class LocationServiceTest extends TestCase
{
    /** @var array<int, array<string, mixed>> Captured search params, newest last. */
    private array $captured = [];

    /**
     * A Client whose search() branches on the request body: an `aggs.cities`
     * request returns the given buckets; a `City.normalize` term request returns
     * the given hit total; anything else returns an empty response.
     *
     * @param  string[]  $cityBuckets
     */
    private function fakeClient(array $cityBuckets = [], int $cityExistsTotal = 0): Client
    {
        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')->andReturnUsing(function (array $params) use ($cityBuckets, $cityExistsTotal) {
            $this->captured[] = $params;
            $body = $params['body'];

            if (isset($body['aggs']['cities'])) {
                return [
                    'aggregations' => [
                        'cities' => [
                            'buckets' => array_map(fn (string $c) => ['key' => $c, 'doc_count' => 1], $cityBuckets),
                        ],
                    ],
                ];
            }

            if (isset($body['query']['term']['City.normalize'])) {
                return ['hits' => ['total' => ['value' => $cityExistsTotal]]];
            }

            return ['hits' => ['total' => ['value' => 0]], 'aggregations' => []];
        });

        return $client;
    }

    /**
     * @return array<string, array{0: string, 1: bool}>
     */
    public static function postalCodes(): array
    {
        return [
            'valid no space' => ['V8W1P6', true],
            'valid with space' => ['V8W 1P6', true],
            'valid lowercase' => ['v8w 1p6', true],
            'city name' => ['Victoria', false],
            'invalid leading D' => ['D1A 1A1', false],
            'too short' => ['V8W 1P', false],
            'empty' => ['', false],
        ];
    }

    #[DataProvider('postalCodes')]
    public function test_is_postal_code_recognises_canadian_postal_codes(string $input, bool $expected): void
    {
        $service = new LocationService($this->fakeClient());

        $this->assertSame($expected, $service->isPostalCode($input));
    }

    public function test_suggest_cities_returns_prefix_matched_city_names(): void
    {
        $service = new LocationService($this->fakeClient(cityBuckets: ['Victoria', 'Victoria Harbour', 'Nanaimo']));

        $suggestions = $service->suggestCities('Vic');

        $this->assertSame(['Victoria', 'Victoria Harbour'], $suggestions);

        // It aggregates on City.keyword against a match_phrase_prefix on City.
        $body = $this->captured[0]['body'];
        $this->assertSame(0, $body['size']);
        $this->assertSame('Vic', $body['query']['match_phrase_prefix']['City']);
        $this->assertSame('City.keyword', $body['aggs']['cities']['terms']['field']);
    }

    public function test_suggest_cities_short_circuits_below_two_characters(): void
    {
        $service = new LocationService($this->fakeClient(cityBuckets: ['Victoria']));

        $this->assertSame([], $service->suggestCities('V'));
        $this->assertSame([], $this->captured, 'It must not hit OpenSearch for a one-character term.');
    }

    public function test_city_exists_is_true_when_the_index_has_a_match(): void
    {
        $service = new LocationService($this->fakeClient(cityExistsTotal: 3));

        $this->assertTrue($service->cityExists('Victoria'));

        $body = $this->captured[0]['body'];
        $this->assertSame('victoria', $body['query']['term']['City.normalize']);
        $this->assertSame(1, $body['terminate_after']);
    }

    public function test_city_exists_is_false_when_the_index_has_no_match(): void
    {
        $service = new LocationService($this->fakeClient(cityExistsTotal: 0));

        $this->assertFalse($service->cityExists('Nowherightsville'));
    }

    protected function tearDown(): void
    {
        Mockery::close();
        parent::tearDown();
    }
}
