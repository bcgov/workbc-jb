<?php

namespace Tests\Feature\Livewire;

use App\Livewire\JobSearch;
use App\Search\Support\SalaryRangeHelper;
use Livewire\Livewire;
use Mockery;
use OpenSearch\Client;
use Tests\TestCase;

/**
 * SRCH-4 — salary filter facet. The bound salary state maps to the FND-7
 * JobSearchQuery salary groups: fixed brackets and the custom range become
 * `Salary` range clauses (annualized via {@see SalaryRangeHelper}), unknown
 * salary becomes the SalarySort sentinel, and the benefit conditions become
 * `SalaryConditions.Description.keyword` terms. A fake OpenSearch client
 * captures each request body so the assertions are deterministic.
 */
class JobSearchSalaryTest extends TestCase
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
     * The should[] of the must-group that carries the Salary range / SalarySort
     * clauses (salaryGroup produces a single OR group), or null.
     *
     * @return array<int, array<string, mixed>>|null
     */
    private function salaryShould(): ?array
    {
        foreach ($this->lastBody()['query']['bool']['must'] as $group) {
            foreach ($group['bool']['should'] ?? [] as $clause) {
                if (isset($clause['range']['Salary']) || isset($clause['range']['SalarySort.Descending'])) {
                    return $group['bool']['should'];
                }
            }
        }

        return null;
    }

    /**
     * The `Salary` range clauses within a should group.
     *
     * @param  array<int, array<string, mixed>>  $should
     * @return array<int, array<string, mixed>>
     */
    private function salaryRanges(array $should): array
    {
        $out = [];
        foreach ($should as $clause) {
            if (isset($clause['range']['Salary'])) {
                $out[] = $clause['range']['Salary'];
            }
        }

        return $out;
    }

    /**
     * The should[] of the must-group that carries SalaryConditions terms, or null.
     *
     * @return array<int, array<string, mixed>>|null
     */
    private function conditionShould(): ?array
    {
        foreach ($this->lastBody()['query']['bool']['must'] as $group) {
            foreach ($group['bool']['should'] ?? [] as $clause) {
                if (isset($clause['term']['SalaryConditions.Description.keyword'])) {
                    return $group['bool']['should'];
                }
            }
        }

        return null;
    }

    public function test_fixed_bracket_maps_to_annualized_salary_range(): void
    {
        // Hourly (type 0), bracket 2 → $20–$30/hour annualized.
        Livewire::test(JobSearch::class)
            ->set('salaryType', 0)
            ->set('salaryBrackets', ['2']);

        $should = $this->salaryShould();
        $this->assertNotNull($should);

        [$min, $max] = SalaryRangeHelper::getAnnualRange(0, 2);
        $this->assertEquals(
            [['gte' => $min, 'lte' => $max]],
            $this->salaryRanges($should),
        );
    }

    public function test_multiple_brackets_are_or_within_the_group(): void
    {
        Livewire::test(JobSearch::class)
            ->set('salaryType', 4)
            ->set('salaryBrackets', ['1', '5']);

        $should = $this->salaryShould();
        $this->assertNotNull($should);
        $this->assertCount(2, $this->salaryRanges($should));
    }

    public function test_bracket_labels_reflect_the_selected_salary_type(): void
    {
        $component = Livewire::test(JobSearch::class);

        $hourly = $component->set('salaryType', 0)->instance()->salaryBracketLabels();
        $this->assertSame('Under $20', $hourly[1]);
        $this->assertSame('$50 or more', $hourly[5]);

        $annual = $component->set('salaryType', 4)->instance()->salaryBracketLabels();
        $this->assertSame('Under $40,000', $annual[1]);
        $this->assertSame('$100,000 or more', $annual[5]);
    }

    public function test_custom_range_annualizes_by_salary_type(): void
    {
        // Weekly (type 1): ×52. $1,000–$2,000/week → $52,000–$104,000/year.
        Livewire::test(JobSearch::class)
            ->set('salaryType', 1)
            ->set('salaryCustom', true)
            ->set('salaryMin', '1000')
            ->set('salaryMax', '2000');

        $should = $this->salaryShould();
        $this->assertNotNull($should);
        $this->assertEquals(
            [['gte' => 52000.0, 'lte' => 104000.0]],
            $this->salaryRanges($should),
        );
    }

    public function test_custom_range_min_only_is_unbounded_above(): void
    {
        // Monthly (type 3): ×12. $5,000/month → gte $60,000, no upper bound.
        Livewire::test(JobSearch::class)
            ->set('salaryType', 3)
            ->set('salaryCustom', true)
            ->set('salaryMin', '5000');

        $should = $this->salaryShould();
        $this->assertNotNull($should);
        $this->assertEquals(
            [['gte' => 60000.0]],
            $this->salaryRanges($should),
        );
    }

    public function test_unknown_salary_uses_the_salary_sort_sentinel(): void
    {
        Livewire::test(JobSearch::class)
            ->set('salaryUnknown', true);

        $should = $this->salaryShould();
        $this->assertNotNull($should);
        $this->assertContains(
            ['range' => ['SalarySort.Descending' => ['lte' => -99999999]]],
            $should,
        );
    }

    public function test_salary_conditions_map_to_keyword_terms_or_within(): void
    {
        Livewire::test(JobSearch::class)
            ->set('salaryConditions', ['Dental plan', 'RRSP benefits']);

        $should = $this->conditionShould();
        $this->assertNotNull($should);

        $values = array_map(
            static fn (array $clause) => $clause['term']['SalaryConditions.Description.keyword'],
            $should,
        );
        $this->assertEqualsCanonicalizing(['Dental plan', 'RRSP benefits'], $values);
    }

    public function test_salary_and_condition_groups_are_separate_must_groups(): void
    {
        Livewire::test(JobSearch::class)
            ->set('salaryType', 0)
            ->set('salaryBrackets', ['3'])
            ->set('salaryConditions', ['Bonus']);

        // AND across facets: the Salary range and the condition term live in
        // different must groups.
        $this->assertNotNull($this->salaryShould());
        $this->assertNotNull($this->conditionShould());
        $this->assertNotSame($this->salaryShould(), $this->conditionShould());
    }

    public function test_salary_type_alone_does_not_add_a_salary_group(): void
    {
        // Choosing only a pay unit (no bracket/custom/unknown) must not filter.
        Livewire::test(JobSearch::class)
            ->set('salaryType', 2);

        $this->assertNull($this->salaryShould());
    }

    public function test_unknown_brackets_and_conditions_are_discarded(): void
    {
        Livewire::test(JobSearch::class)
            ->set('salaryType', 0)
            ->set('salaryBrackets', ['9', '2'])
            ->set('salaryConditions', ['Bonus', 'Free spaceship']);

        // Only the valid bracket contributes a range.
        $should = $this->salaryShould();
        $this->assertNotNull($should);
        $this->assertCount(1, $this->salaryRanges($should));

        // Only the whitelisted condition contributes a term.
        $conditions = $this->conditionShould();
        $this->assertNotNull($conditions);
        $this->assertCount(1, $conditions);
    }

    public function test_clear_filters_resets_salary_state(): void
    {
        Livewire::test(JobSearch::class)
            ->set('salaryType', 3)
            ->set('salaryBrackets', ['2'])
            ->set('salaryCustom', true)
            ->set('salaryMin', '1000')
            ->set('salaryMax', '2000')
            ->set('salaryUnknown', true)
            ->set('salaryConditions', ['Bonus'])
            ->call('clearFilters')
            ->assertSet('salaryType', 0)
            ->assertSet('salaryBrackets', [])
            ->assertSet('salaryCustom', false)
            ->assertSet('salaryMin', '')
            ->assertSet('salaryMax', '')
            ->assertSet('salaryUnknown', false)
            ->assertSet('salaryConditions', []);

        $this->assertNull($this->salaryShould());
    }
}
