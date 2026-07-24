<?php

namespace Tests\Feature\Livewire;

use App\Livewire\JobSearch;
use App\Search\Contracts\Geocoder;
use App\Search\Support\GeoPoint;
use Livewire\Livewire;
use Mockery;
use OpenSearch\Client;
use Tests\Concerns\InteractsWithLocationsTable;
use Tests\Fakes\FakeGeocoder;
use Tests\TestCase;

/**
 * SRCH-2 Livewire test: the location facet turns city/postal input into the
 * FND-7 JobSearchQuery location clauses, validates cities against the curated
 * `Locations` table (via LocationService), and drives radius search through the
 * injected Geocoder — deterministically (mocked OpenSearch client for the search
 * itself, a Locations fixture for suggestions/validation, and a fake Geocoder).
 */
class JobSearchLocationTest extends TestCase
{
    use InteractsWithLocationsTable;

    /** @var array<int, array<string, mixed>> Captured main-search bodies, newest last. */
    private array $bodies = [];

    protected function setUp(): void
    {
        parent::setUp();

        $this->createLocationsFixture();
        $this->bodies = [];

        // LocationService now reads the Locations table, so the OpenSearch client
        // only serves the main results query here.
        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')->andReturnUsing(function (array $params): array {
            $this->bodies[] = $params['body'];

            return [
                'hits' => [
                    'total' => ['value' => 1],
                    'hits' => [
                        ['_source' => ['JobId' => '100', 'Title' => 'Nurse', 'EmployerName' => 'Island Health', 'City' => ['Victoria'], 'DatePosted' => '2026-06-01T00:00:00']],
                    ],
                ],
            ];
        });

        $this->app->instance(Client::class, $client);
    }

    protected function tearDown(): void
    {
        $this->dropLocationsFixture();
        Mockery::close();
        parent::tearDown();
    }

    private function bindGeocoder(?GeoPoint $point = null): void
    {
        $this->app->instance(Geocoder::class, new FakeGeocoder($point));
    }

    /**
     * @return array<string, mixed>
     */
    private function lastBody(): array
    {
        return $this->bodies[array_key_last($this->bodies)];
    }

    /**
     * Recursively find the first sub-array matching a predicate.
     *
     * @param  array<mixed>  $haystack
     */
    private function findClause(array $haystack, callable $predicate): ?array
    {
        if ($predicate($haystack)) {
            return $haystack;
        }
        foreach ($haystack as $value) {
            if (is_array($value)) {
                $found = $this->findClause($value, $predicate);
                if ($found !== null) {
                    return $found;
                }
            }
        }

        return null;
    }

    public function test_adding_a_valid_city_builds_an_exact_normalized_term_with_virtual_jobs(): void
    {
        $this->bindGeocoder();

        Livewire::test(JobSearch::class)
            ->set('locationInput', 'Victoria')
            ->call('addLocation')
            ->assertSet('locations', [['City' => 'Victoria']])
            ->assertSet('locationInput', '')
            ->assertSet('locationError', null);

        $body = $this->lastBody();

        $cityTerm = $this->findClause($body, fn ($c) => isset($c['term']['City.normalize']));
        $this->assertNotNull($cityTerm, 'Exact city search must use a City.normalize term.');
        $this->assertSame('victoria', $cityTerm['term']['City.normalize']['value'] ?? $cityTerm['term']['City.normalize']);

        // Virtual (province-wide) jobs are always OR-ed into a location group.
        $virtual = $this->findClause($body, fn ($c) => isset($c['term']['WorkplaceType.Id']));
        $this->assertNotNull($virtual, 'Location groups must include virtual jobs (WorkplaceType.Id 15141).');
        $this->assertSame(15141, $virtual['term']['WorkplaceType.Id']['value']);
        $this->assertSame(0, $virtual['term']['WorkplaceType.Id']['boost']);
    }

    public function test_city_with_radius_uses_geo_distance_and_geo_sort(): void
    {
        $this->bindGeocoder(new GeoPoint(48.4284, -123.3656));

        Livewire::test(JobSearch::class)
            ->set('locationInput', 'Victoria')
            ->call('addLocation')
            ->set('distance', 25);

        $body = $this->lastBody();

        $geo = $this->findClause($body, fn ($c) => isset($c['geo_distance']));
        $this->assertNotNull($geo, 'Radius search must add a geo_distance clause.');
        $this->assertSame('25km', $geo['geo_distance']['distance']);

        $geoSort = $this->findClause($body['sort'], fn ($c) => isset($c['_geo_distance']));
        $this->assertNotNull($geoSort, 'Radius search must sort by distance.');
    }

    public function test_postal_code_input_is_accepted_and_normalized_to_an_exact_term(): void
    {
        $this->bindGeocoder();

        Livewire::test(JobSearch::class)
            ->set('locationInput', 'v8w 1p6')
            ->call('addLocation')
            ->assertSet('locations', [['Postal' => 'v8w 1p6']])
            ->assertSet('locationError', null);

        $body = $this->lastBody();
        $postal = $this->findClause($body, fn ($c) => isset($c['term']['PostalCode.keyword']));
        $this->assertNotNull($postal, 'Exact postal search must use a PostalCode.keyword term.');
        $this->assertSame('V8W1P6', $postal['term']['PostalCode.keyword']['value'] ?? $postal['term']['PostalCode.keyword']);
    }

    public function test_unknown_city_sets_an_accessible_error_and_adds_no_location(): void
    {
        $this->bindGeocoder();

        Livewire::test(JobSearch::class)
            ->set('locationInput', 'Nowherightsville')
            ->call('addLocation')
            ->assertSet('locations', [])
            ->assertSet('locationError', fn ($v) => is_string($v) && str_contains($v, 'Nowherightsville'));
    }

    public function test_typing_populates_city_suggestions(): void
    {
        $this->bindGeocoder();

        Livewire::test(JobSearch::class)
            ->set('locationInput', 'Vic')
            ->assertSet('suggestions', ['Victoria', 'Victoria Harbour']);
    }

    public function test_selecting_a_suggestion_commits_the_city(): void
    {
        $this->bindGeocoder();

        Livewire::test(JobSearch::class)
            ->call('selectSuggestion', 'Nanaimo')
            ->assertSet('locations', [['City' => 'Nanaimo']])
            ->assertSet('suggestions', []);
    }

    public function test_removing_a_location_clears_it(): void
    {
        $this->bindGeocoder();

        Livewire::test(JobSearch::class)
            ->set('locationInput', 'Victoria')
            ->call('addLocation')
            ->assertSet('locations', [['City' => 'Victoria']])
            ->call('removeLocation', 0)
            ->assertSet('locations', []);
    }
}
