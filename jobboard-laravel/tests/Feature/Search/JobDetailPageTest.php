<?php

namespace Tests\Feature\Search;

use App\Models\JobView;
use App\Support\JobSlug;
use Illuminate\Support\Facades\Schema;
use Mockery;
use OpenSearch\Client;
use Tests\TestCase;
use Throwable;

/**
 * SRCH-7 job-detail page. The page is server-rendered Blade (SEO-critical),
 * fetched by id from the derived OpenSearch read model with NO ExpireDate
 * filter, emits schema.org/JobPosting JSON-LD, and increments the federal-job
 * view counter as a fire-and-forget side effect.
 *
 * The view counter is exercised against a throwaway in-memory SQLite `JobViews`
 * table (never the production PascalCase schema — map, don't create).
 */
class JobDetailPageTest extends TestCase
{
    private bool $ready = false;

    protected function setUp(): void
    {
        parent::setUp();

        config([
            'database.default' => 'jobviews_mem',
            'database.connections.jobviews_mem' => [
                'driver' => 'sqlite',
                'database' => ':memory:',
                'prefix' => '',
                'foreign_key_constraints' => false,
            ],
        ]);

        try {
            Schema::create('JobViews', function ($table): void {
                $table->string('JobId')->primary();
                $table->integer('Views')->nullable();
                $table->dateTime('DateLastViewed')->nullable();
            });
            $this->ready = true;
        } catch (Throwable $e) {
            $this->markTestSkipped('SQLite not available for the job-view test: '.$e->getMessage());
        }
    }

    protected function tearDown(): void
    {
        Mockery::close();
        parent::tearDown();
    }

    /**
     * @param  array<string, mixed>|null  $source  the OpenSearch _source, or null for "no hits".
     */
    private function bindClient(?array $source): void
    {
        $hits = $source === null ? [] : [['_id' => $source['JobId'] ?? 'x', '_source' => $source]];

        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')->andReturn([
            'hits' => [
                'total' => ['value' => count($hits)],
                'hits' => $hits,
            ],
        ]);

        $this->app->instance(Client::class, $client);
    }

    /**
     * @return array<string, mixed>
     */
    private function federalJob(array $overrides = []): array
    {
        return array_merge([
            'JobId' => 'fed001',
            'Title' => 'Policy Analyst',
            'EmployerName' => 'Government of Canada',
            'City' => ['Victoria'],
            'Province' => 'BC',
            'DatePosted' => '2026-06-01T00:00:00',
            'ExpireDate' => '2099-01-01T00:00:00',
            'Salary' => 85000,
            'SalarySummary' => '$85,000.00 yearly',
            'IsFederalJob' => true,
            'HoursOfWork' => ['Description' => ['Full-time']],
            'Location' => [['Lat' => '48.4284', 'Lon' => '-123.3656']],
        ], $overrides);
    }

    private function detailPath(string $jobId, string $title): string
    {
        return '/jobs/'.JobSlug::path($jobId, $title);
    }

    /**
     * @return array<string, mixed>
     */
    private function externalJob(array $overrides = []): array
    {
        return array_merge([
            'JobId' => 'ext777',
            'Title' => 'Barista',
            'EmployerName' => 'Cafe Aroma',
            'City' => ['Vancouver'],
            'Province' => 'British Columbia',
            'DatePosted' => '2026-06-01T00:00:00',
            'ExpireDate' => '2099-01-01T00:00:00',
            'IsFederalJob' => false,
            'JobDescription' => "<p>Great <b>coffee</b> job.</p><ul><li>Make espresso</li><li>Smile</li></ul><script>alert('xss')</script>",
            'ExternalSource' => ['Source' => [['Url' => 'https://innovibe.example/jobs/777', 'Source' => 'Innovibe']]],
            'ApplyWebsite' => 'https://apply.example/777',
        ], $overrides);
    }

    public function test_it_renders_the_job_in_raw_server_html(): void
    {
        $this->bindClient($this->federalJob());

        $response = $this->get($this->detailPath('fed001', 'Policy Analyst'));

        $response->assertOk();
        $response->assertSee('Policy Analyst');
        $response->assertSee('Government of Canada');
    }

    public function test_it_emits_valid_jobposting_json_ld(): void
    {
        $this->bindClient($this->federalJob());

        $response = $this->get($this->detailPath('fed001', 'Policy Analyst'));
        $response->assertOk();

        $this->assertMatchesRegularExpression('/type="application\/ld\+json"/', $response->getContent());

        preg_match('/<script type="application\/ld\+json">(.*?)<\/script>/s', $response->getContent(), $m);
        $this->assertNotEmpty($m[1] ?? null, 'JSON-LD block not found in the response.');

        $data = json_decode(trim($m[1]), true);
        $this->assertIsArray($data, 'JSON-LD is not valid JSON.');
        $this->assertSame('JobPosting', $data['@type']);
        $this->assertSame('Policy Analyst', $data['title']);
        $this->assertSame('Government of Canada', $data['hiringOrganization']['name']);
        $this->assertSame('2026-06-01T00:00:00', $data['datePosted']);
        $this->assertSame('2099-01-01T00:00:00', $data['validThrough']);
        $this->assertSame('CAD', $data['baseSalary']['currency']);
        $this->assertEquals(85000, $data['baseSalary']['value']['value']);
    }

    public function test_it_sets_the_title_meta_description_and_canonical(): void
    {
        $this->bindClient($this->federalJob());

        $response = $this->get($this->detailPath('fed001', 'Policy Analyst'));

        $response->assertOk();
        $response->assertSee('Policy Analyst — Government of Canada | WorkBC Job Board');
        $response->assertSee('name="description"', false);
        $response->assertSee('rel="canonical"', false);
        $response->assertSee('hreflang="fr"', false);
    }

    public function test_it_increments_the_view_count_for_federal_jobs(): void
    {
        $this->bindClient($this->federalJob());
        $path = $this->detailPath('fed001', 'Policy Analyst');

        $this->get($path)->assertOk()->assertSee('1 view');
        $this->assertSame(1, JobView::find('fed001')?->Views);

        $this->get($path)->assertOk()->assertSee('2 views');
        $this->assertSame(2, JobView::find('fed001')?->Views);
    }

    public function test_it_does_not_record_views_for_external_jobs(): void
    {
        $this->bindClient($this->federalJob([
            'JobId' => 'ext001',
            'Title' => 'Barista',
            'IsFederalJob' => false,
        ]));

        $this->get($this->detailPath('ext001', 'Barista'))->assertOk();

        $this->assertNull(JobView::find('ext001'));
    }

    public function test_a_language_toggle_does_not_double_count_a_view(): void
    {
        $this->bindClient($this->federalJob());
        $path = $this->detailPath('fed001', 'Policy Analyst');

        $this->get($path.'?toggle=1')->assertOk();

        $this->assertNull(JobView::find('fed001'));
    }

    public function test_an_expired_job_still_renders(): void
    {
        $this->bindClient($this->federalJob(['ExpireDate' => '2000-01-01T00:00:00']));

        $response = $this->get($this->detailPath('fed001', 'Policy Analyst'));

        $response->assertOk();
        $response->assertSee('expired');
    }

    public function test_an_unknown_job_returns_404(): void
    {
        $this->bindClient(null);

        $this->get('/jobs/missing-nope999')->assertNotFound();
    }

    public function test_a_bare_id_url_redirects_to_the_canonical_slug(): void
    {
        $this->bindClient($this->federalJob());

        $this->get('/jobs/fed001')
            ->assertRedirect($this->detailPath('fed001', 'Policy Analyst'));

        // The redirect must not have counted a view.
        $this->assertNull(JobView::find('fed001'));
    }

    // --- SRCH-7b: external (Innovibe) job rendering ---

    public function test_an_external_job_renders_its_description_facts_and_apply_button(): void
    {
        $this->bindClient($this->externalJob());

        $response = $this->get($this->detailPath('ext777', 'Barista'));

        $response->assertOk();
        // Facts.
        $response->assertSee('Barista');
        $response->assertSee('Cafe Aroma');
        $response->assertSee('Vancouver');
        // Sanitized description content (rendered on our page, not a redirect).
        $response->assertSee('Great coffee job.');
        $response->assertSee('Make espresso');
        // "via {source}" attribution + apply-to-source button linking to the original URL.
        $response->assertSee('Posted via Innovibe.');
        $response->assertSee('Apply on Innovibe');
        $response->assertSee('href="https://innovibe.example/jobs/777"', false);
    }

    public function test_an_external_description_is_sanitized_against_xss(): void
    {
        $this->bindClient($this->externalJob());

        $response = $this->get($this->detailPath('ext777', 'Barista'));

        $response->assertOk();
        // No raw markup from the source description survives into the document.
        $response->assertDontSee('<script>alert', false);
        $response->assertDontSee('<b>coffee</b>', false);
        $response->assertDontSee('<p>Great', false);
    }

    public function test_an_external_job_is_self_canonical(): void
    {
        $this->bindClient($this->externalJob());

        $response = $this->get($this->detailPath('ext777', 'Barista'));

        $response->assertOk();
        $canonical = route('jobs.show', ['job' => JobSlug::path('ext777', 'Barista')]);
        $response->assertSee('<link rel="canonical" href="'.$canonical.'"', false);
    }

    public function test_an_expired_external_job_renders_with_a_dead_link_note(): void
    {
        $this->bindClient($this->externalJob(['ExpireDate' => '2000-01-01T00:00:00']));

        $response = $this->get($this->detailPath('ext777', 'Barista'));

        $response->assertOk();
        $response->assertSee('Great coffee job.');
        $response->assertSee('may no longer be available');
    }

    public function test_an_external_job_emits_jobposting_json_ld_with_the_description(): void
    {
        $this->bindClient($this->externalJob());

        $response = $this->get($this->detailPath('ext777', 'Barista'));
        $response->assertOk();

        preg_match('/<script type="application\/ld\+json">(.*?)<\/script>/s', $response->getContent(), $m);
        $data = json_decode(trim($m[1] ?? ''), true);

        $this->assertIsArray($data);
        $this->assertSame('JobPosting', $data['@type']);
        $this->assertStringContainsString('Great coffee job.', $data['description']);
        $this->assertStringNotContainsString('<script>', $data['description']);
    }
}
