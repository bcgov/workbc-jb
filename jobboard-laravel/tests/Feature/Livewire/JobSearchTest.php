<?php

namespace Tests\Feature\Livewire;

use App\Livewire\JobSearch;
use Livewire\Livewire;
use Mockery;
use OpenSearch\Client;
use Tests\TestCase;

/**
 * SRCH-1 Livewire component test: keyword + scope + sort + page produce the
 * expected JobSearchQuery body, and the mapped results render. A fake OpenSearch
 * client captures each request body and returns canned hits, so the test is
 * deterministic without a live cluster.
 */
class JobSearchTest extends TestCase
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

            return [
                'hits' => [
                    'total' => ['value' => 3],
                    'hits' => [
                        ['_source' => ['JobId' => '100', 'Title' => 'Software Engineer', 'EmployerName' => 'Acme Corp', 'City' => ['Victoria'], 'DatePosted' => '2026-06-01T00:00:00']],
                        ['_source' => ['JobId' => '200', 'Title' => 'Data Analyst', 'EmployerName' => 'BizCo', 'City' => ['Vancouver'], 'DatePosted' => '2026-05-01T00:00:00']],
                    ],
                ],
            ];
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

    public function test_it_renders_results_server_side_on_first_load(): void
    {
        Livewire::test(JobSearch::class)
            ->assertSee('Software Engineer')
            ->assertSee('Acme Corp')
            ->assertSeeHtml('aria-live="polite"');
    }

    public function test_default_query_uses_base_expiry_filter_and_dateposted_desc(): void
    {
        Livewire::test(JobSearch::class);

        $body = $this->lastBody();
        $this->assertTrue($body['track_total_hits']);
        $this->assertSame('now/d', $body['query']['bool']['filter']['range']['ExpireDate']['gte']);
        $this->assertSame('America/Vancouver', $body['query']['bool']['filter']['range']['ExpireDate']['time_zone']);
        // Default SortOrder 1 → DatePosted desc primary sort.
        $this->assertSame(['DatePosted' => 'desc'], $body['sort'][0]);
    }

    public function test_keyword_and_scope_build_a_simple_query_string(): void
    {
        Livewire::test(JobSearch::class)
            ->set('keyword', 'baker, cook')
            ->set('searchIn', 'title')
            ->call('applySearch');

        $body = $this->lastBody();
        $must = $body['query']['bool']['must'];

        $sqs = null;
        foreach ($must as $group) {
            $should = $group['bool']['should'] ?? [];
            foreach ($should as $clause) {
                if (isset($clause['simple_query_string'])) {
                    $sqs = $clause['simple_query_string'];
                }
            }
        }

        $this->assertNotNull($sqs, 'Expected a simple_query_string clause');
        $this->assertSame('baker|cook', $sqs['query']);
        $this->assertSame(['Title'], $sqs['fields']);
    }

    public function test_changing_sort_applies_new_order_and_resets_to_page_one(): void
    {
        Livewire::test(JobSearch::class)
            ->set('page', 4)
            ->set('sort', 3) // Title A–Z
            ->assertSet('page', 1);

        $this->assertSame(['Title.normalize' => 'asc'], $this->lastBody()['sort'][0]);
    }

    public function test_paging_sets_the_from_offset(): void
    {
        Livewire::test(JobSearch::class)
            ->call('gotoPage', 3)
            ->assertSet('page', 3);

        // page 3, pageSize 20 → from = 40
        $this->assertSame(40, $this->lastBody()['from']);
        $this->assertSame(20, $this->lastBody()['size']);
    }

    public function test_result_links_use_the_path_based_detail_route(): void
    {
        Livewire::test(JobSearch::class)
            ->assertSeeHtml('href="'.route('jobs.show', ['job' => \App\Support\JobSlug::path('100', 'Software Engineer')]).'"');
    }
}
