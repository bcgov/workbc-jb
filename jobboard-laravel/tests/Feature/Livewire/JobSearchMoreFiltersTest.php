<?php

namespace Tests\Feature\Livewire;

use App\Livewire\JobSearch;
use Livewire\Livewire;
use Mockery;
use OpenSearch\Client;
use Tests\TestCase;

/**
 * SRCH-5 — the "More" filter facet. The bound state maps to the FND-7
 * JobSearchQuery groups: equity groups → the index's Is* terms (OR within),
 * posting language English+French → IsFederalJob, NOC → the Noc2021 term, the
 * job source enum → federal flag / non-federal / nested ExternalSource groups,
 * and the placement-agency exclusion → a must_not. A fake OpenSearch client
 * captures each request body so the assertions are deterministic.
 */
class JobSearchMoreFiltersTest extends TestCase
{
    /** @var array<int, array<string, mixed>> */
    private array $bodies = [];

    protected function setUp(): void
    {
        parent::setUp();

        $this->bodies = [];

        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')->andReturnUsing(function (array $params): array {
            $this->bodies[] = $params['body'];

            return ['hits' => ['total' => ['value' => 0], 'hits' => []]];
        });

        $this->app->instance(Client::class, $client);
    }

    protected function tearDown(): void
    {
        Mockery::close();
        parent::tearDown();
    }

    /**
     * @return array<string, mixed>
     */
    private function lastBody(): array
    {
        return $this->bodies[array_key_last($this->bodies)];
    }

    /**
     * @return array<int, array<string, mixed>>
     */
    private function mustGroups(): array
    {
        return $this->lastBody()['query']['bool']['must'];
    }

    /**
     * The should[] of the must-group that contains a term on $field, or null.
     *
     * @return array<int, array<string, mixed>>|null
     */
    private function shouldGroupFor(string $field): ?array
    {
        foreach ($this->mustGroups() as $group) {
            foreach ($group['bool']['should'] ?? [] as $clause) {
                if (isset($clause['term'][$field])) {
                    return $group['bool']['should'];
                }
            }
        }

        return null;
    }

    private function termExists(string $field, mixed $value): bool
    {
        foreach ($this->mustGroups() as $group) {
            foreach ($group['bool']['should'] ?? [] as $clause) {
                if (isset($clause['term'][$field]) && $clause['term'][$field] === $value) {
                    return true;
                }
            }
        }

        return false;
    }

    /**
     * @param  array<string, mixed>  $clause
     */
    private function clauseInSomeGroup(array $clause): bool
    {
        foreach ($this->mustGroups() as $group) {
            foreach ($group['bool']['should'] ?? [] as $candidate) {
                if ($candidate === $clause) {
                    return true;
                }
            }
        }

        return false;
    }

    /**
     * The nested ExternalSource match_phrase values, keyed by field.
     *
     * @return array<string, array<int, string>>
     */
    private function externalSourceMatchPhrases(): array
    {
        $out = [];
        foreach ($this->mustGroups() as $group) {
            foreach ($group['bool']['should'] ?? [] as $clause) {
                if (($clause['nested']['path'] ?? null) !== 'ExternalSource') {
                    continue;
                }
                foreach ($clause['nested']['query']['bool']['should'] ?? [] as $phrase) {
                    foreach ($phrase['match_phrase'] ?? [] as $field => $value) {
                        $out[$field][] = $value;
                    }
                }
            }
        }

        return $out;
    }

    /**
     * @return array<int, array<string, mixed>>
     */
    private function mustNot(): array
    {
        return $this->lastBody()['query']['bool']['must_not'] ?? [];
    }

    public function test_equity_groups_map_to_index_terms_or_within(): void
    {
        Livewire::test(JobSearch::class)
            ->set('equityGroups', ['Apprentice', 'Veterans', 'Indigenous']);

        // A single OR group carries every selected equity term (mapped names).
        $should = $this->shouldGroupFor('IsApprentice');
        $this->assertNotNull($should);
        $this->assertTrue($this->termExists('IsApprentice', true));
        $this->assertTrue($this->termExists('IsVeteran', true));
        $this->assertTrue($this->termExists('IsAboriginal', true));
    }

    public function test_posting_language_english_french_flags_federal_jobs(): void
    {
        Livewire::test(JobSearch::class)
            ->set('postingLanguage', '2');

        $this->assertTrue($this->termExists('IsFederalJob', true));
    }

    public function test_noc_code_maps_to_noc2021_term(): void
    {
        Livewire::test(JobSearch::class)
            ->set('nocCode', '21231');

        $this->assertTrue($this->termExists('Noc2021', '21231'));
    }

    public function test_noc_code_extracts_the_digits_from_a_name(): void
    {
        Livewire::test(JobSearch::class)
            ->set('nocCode', 'NOC 21231 - Software engineers');

        $this->assertTrue($this->termExists('Noc2021', '21231'));
    }

    public function test_job_source_workbc_is_federal_true(): void
    {
        Livewire::test(JobSearch::class)
            ->set('jobSource', '1');

        $this->assertTrue($this->termExists('IsFederalJob', true));
    }

    public function test_job_source_external_is_federal_false(): void
    {
        Livewire::test(JobSearch::class)
            ->set('jobSource', '2');

        $this->assertTrue($this->termExists('IsFederalJob', false));
    }

    public function test_job_source_federal_government_uses_nested_external_source(): void
    {
        Livewire::test(JobSearch::class)
            ->set('jobSource', '3');

        $this->assertEquals(
            ['https://emploisfp-psjobs.cfp-psc.gc.ca'],
            $this->externalSourceMatchPhrases()['ExternalSource.Source.Url'] ?? [],
        );
    }

    public function test_job_source_municipal_uses_employer_type_and_nested_sources(): void
    {
        Livewire::test(JobSearch::class)
            ->set('jobSource', '4');

        $this->assertTrue($this->clauseInSomeGroup(['term' => ['EmployerTypeId' => ['value' => '4']]]));
        $this->assertEqualsCanonicalizing(
            ['CivicInfoBC', 'CivicJobs.ca'],
            $this->externalSourceMatchPhrases()['ExternalSource.Source.Source'] ?? [],
        );
    }

    public function test_job_source_provincial_uses_nested_external_source(): void
    {
        Livewire::test(JobSearch::class)
            ->set('jobSource', '5');

        $this->assertEquals(
            ['https://bcpublicservice.hua.hrsmart.com'],
            $this->externalSourceMatchPhrases()['ExternalSource.Source.Url'] ?? [],
        );
    }

    public function test_exclude_placement_agency_uses_must_not(): void
    {
        Livewire::test(JobSearch::class)
            ->set('excludePlacementAgency', true);

        $this->assertContains(['term' => ['EmployerTypeId' => 1]], $this->mustNot());
    }

    public function test_unknown_equity_keys_and_invalid_job_source_are_discarded(): void
    {
        Livewire::test(JobSearch::class)
            ->set('equityGroups', ['Apprentice', 'Wizards'])
            ->set('jobSource', '99');

        $this->assertTrue($this->termExists('IsApprentice', true));
        // The junk equity key contributes no term…
        $should = $this->shouldGroupFor('IsApprentice');
        $this->assertNotNull($should);
        $this->assertCount(1, $should);
        // …and the invalid source adds neither a federal flag nor a nested group.
        $this->assertFalse($this->termExists('IsFederalJob', true));
        $this->assertFalse($this->termExists('IsFederalJob', false));
        $this->assertSame([], $this->externalSourceMatchPhrases());
    }

    public function test_defaults_add_no_more_filters(): void
    {
        Livewire::test(JobSearch::class);

        $this->assertNull($this->shouldGroupFor('IsFederalJob'));
        $this->assertNull($this->shouldGroupFor('IsApprentice'));
        $this->assertNull($this->shouldGroupFor('Noc2021'));
        $this->assertSame([], $this->mustNot());
    }

    public function test_clear_filters_resets_more_state(): void
    {
        Livewire::test(JobSearch::class)
            ->set('equityGroups', ['Apprentice'])
            ->set('postingLanguage', '2')
            ->set('nocCode', '21231')
            ->set('jobSource', '4')
            ->set('excludePlacementAgency', true)
            ->call('clearFilters')
            ->assertSet('equityGroups', [])
            ->assertSet('postingLanguage', '1')
            ->assertSet('nocCode', '')
            ->assertSet('jobSource', '0')
            ->assertSet('excludePlacementAgency', false);

        $this->assertNull($this->shouldGroupFor('IsApprentice'));
        $this->assertSame([], $this->mustNot());
    }
}
