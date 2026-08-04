<?php

namespace Tests\Feature\Account;

use App\Models\JobSeeker;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;
use Mockery;
use OpenSearch\Client;
use Tests\TestCase;

class RecommendedJobsTest extends TestCase
{
    /** @var array<string, mixed>|null */
    private ?array $capturedBody = null;

    protected function setUp(): void
    {
        parent::setUp();

        $this->createFixture();
    }

    protected function tearDown(): void
    {
        Mockery::close();
        $this->dropFixture();

        parent::tearDown();
    }

    public function test_empty_state_when_seeker_has_no_saved_jobs(): void
    {
        $user = $this->createUser('user-a', city: 'Vancouver');

        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')->never();
        $this->app->instance(Client::class, $client);

        $this->actingAs($user, 'web')
            ->get('/account/recommended')
            ->assertOk()
            ->assertSee('No saved jobs yet')
            ->assertDontSee('No recommended jobs found');
    }

    public function test_empty_state_when_saved_jobs_have_no_matches_and_saved_jobs_are_excluded(): void
    {
        $user = $this->createUser('user-a', city: 'Vancouver');

        $this->insertJob('saved-1', 62100, 'Acme Ltd', 'Cook');
        $this->insertSavedJob('user-a', 'saved-1', '2026-08-01 10:00:00');

        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')
            ->once()
            ->andReturnUsing(function (array $params): array {
                $this->capturedBody = $params['body'];

                return [
                    'hits' => [
                        'total' => ['value' => 0],
                        'hits' => [],
                    ],
                ];
            });
        $this->app->instance(Client::class, $client);

        $this->actingAs($user, 'web')
            ->get('/account/recommended')
            ->assertOk()
            ->assertSee('No recommended jobs found')
            ->assertDontSee('No saved jobs yet');

        $this->assertNotNull($this->capturedBody);
        $this->assertSame(1, $this->capturedBody['query']['bool']['minimum_should_match']);
        $this->assertSame([
            ['terms' => ['JobId.keyword' => ['saved-1']]],
        ], $this->capturedBody['query']['bool']['must_not']);
    }

    public function test_recommended_results_include_reason_score_and_exact_boost_values(): void
    {
        $user = $this->createUser('user-a', city: 'Vancouver', flagOverrides: ['IsStudent' => true]);

        $this->insertJob('saved-1', 62100, 'Acme Ltd', 'Cook');
        $this->insertJob('saved-2', 62100, 'ACME LTD', 'Cook');
        $this->insertSavedJob('user-a', 'saved-1', '2026-08-01 10:00:00');
        $this->insertSavedJob('user-a', 'saved-2', '2026-08-01 11:00:00');

        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')
            ->once()
            ->andReturnUsing(function (array $params): array {
                $body = $params['body'];
                $this->capturedBody = $body;

                $should = $body['query']['bool']['should'];

                $this->assertContains(['term' => ['Noc2021' => ['value' => 62100, 'boost' => 1.02]]], $should);
                $this->assertContains(['term' => ['EmployerName.normalize' => ['value' => 'acme ltd', 'boost' => 1.02]]], $should);
                $this->assertContains(['term' => ['Title.normalize' => ['value' => 'cook', 'boost' => 1.02]]], $should);
                $this->assertContains(['term' => ['City.normalize' => ['value' => 'vancouver', 'boost' => 1.0]]], $should);
                $this->assertContains(['term' => ['WorkplaceType.Id' => ['value' => 15141, 'boost' => 0]]], $should);
                $this->assertContains(['term' => ['IsStudent' => ['value' => true, 'boost' => 0.25]]], $should);

                return [
                    'hits' => [
                        'total' => ['value' => 1],
                        'hits' => [
                            [
                                '_score' => 3.456,
                                '_source' => [
                                    'JobId' => 'rec-1',
                                    'Title' => 'Cook',
                                    'EmployerName' => 'Acme Ltd',
                                    'City' => ['Vancouver'],
                                    'Noc2021' => 62100,
                                    'IsStudent' => true,
                                ],
                            ],
                        ],
                    ],
                ];
            });
        $this->app->instance(Client::class, $client);

        $this->actingAs($user, 'web')
            ->get('/account/recommended')
            ->assertOk()
            ->assertSee('Recommended based on')
            ->assertSee('same NOC code as two of your saved jobs')
            ->assertSee('same employer as two of your saved jobs')
            ->assertSee('Relevance score: 3.46');
    }

    /**
     * @param  array<string, bool>  $flagOverrides
     */
    private function createUser(string $id, string $city, array $flagOverrides = []): JobSeeker
    {
        DB::table('AspNetUsers')->insert([
            'Id' => $id,
            'UserName' => $id.'@example.com',
            'FirstName' => 'Alex',
            'City' => $city,
            'NormalizedUserName' => mb_strtoupper($id.'@example.com', 'UTF-8'),
            'Email' => $id.'@example.com',
            'NormalizedEmail' => mb_strtoupper($id.'@example.com', 'UTF-8'),
            'PasswordHash' => 'unused-hash',
            'SecurityStamp' => 'stamp-'.$id,
            'EmailConfirmed' => true,
        ]);

        $flags = [
            'AspNetUserId' => $id,
            'IsApprentice' => false,
            'IsIndigenousPerson' => false,
            'IsMatureWorker' => false,
            'IsNewImmigrant' => false,
            'IsPersonWithDisability' => false,
            'IsStudent' => false,
            'IsVeteran' => false,
            'IsVisibleMinority' => false,
            'IsYouth' => false,
        ];

        foreach ($flagOverrides as $field => $value) {
            if (array_key_exists($field, $flags)) {
                $flags[$field] = $value;
            }
        }

        DB::table('JobSeekerFlags')->insert($flags);

        return JobSeeker::query()->findOrFail($id);
    }

    private function insertJob(string $jobId, int $noc2021, string $employerName, string $title): void
    {
        DB::table('Jobs')->insert([
            'JobId' => $jobId,
            'NocCodeId2021' => $noc2021,
            'EmployerName' => $employerName,
            'Title' => $title,
            'City' => 'Vancouver',
        ]);
    }

    private function insertSavedJob(string $userId, string $jobId, string $dateSaved): void
    {
        DB::table('SavedJobs')->insert([
            'AspNetUserId' => $userId,
            'JobId' => $jobId,
            'DateSaved' => $dateSaved,
            'IsDeleted' => false,
            'DateDeleted' => null,
        ]);
    }

    private function createFixture(): void
    {
        $this->dropFixture();

        Schema::create('AspNetUsers', function (Blueprint $table): void {
            $table->string('Id')->primary();
            $table->string('UserName')->nullable();
            $table->string('FirstName')->nullable();
            $table->string('City')->nullable();
            $table->string('NormalizedUserName')->nullable();
            $table->string('Email')->nullable();
            $table->string('NormalizedEmail')->nullable();
            $table->string('PasswordHash')->nullable();
            $table->string('SecurityStamp')->nullable();
            $table->boolean('EmailConfirmed')->default(true);
        });

        Schema::create('SavedJobs', function (Blueprint $table): void {
            $table->id('Id');
            $table->string('AspNetUserId');
            $table->string('JobId');
            $table->dateTime('DateSaved')->nullable();
            $table->boolean('IsDeleted')->default(false);
            $table->dateTime('DateDeleted')->nullable();
        });

        Schema::create('Jobs', function (Blueprint $table): void {
            $table->string('JobId')->primary();
            $table->integer('NocCodeId2021')->nullable();
            $table->string('EmployerName')->nullable();
            $table->string('Title')->nullable();
            $table->string('City')->nullable();
        });

        Schema::create('JobSeekerFlags', function (Blueprint $table): void {
            $table->id('Id');
            $table->string('AspNetUserId');
            $table->boolean('IsApprentice')->default(false);
            $table->boolean('IsIndigenousPerson')->default(false);
            $table->boolean('IsMatureWorker')->default(false);
            $table->boolean('IsNewImmigrant')->default(false);
            $table->boolean('IsPersonWithDisability')->default(false);
            $table->boolean('IsStudent')->default(false);
            $table->boolean('IsVeteran')->default(false);
            $table->boolean('IsVisibleMinority')->default(false);
            $table->boolean('IsYouth')->default(false);
        });
    }

    private function dropFixture(): void
    {
        Schema::dropIfExists('JobSeekerFlags');
        Schema::dropIfExists('Jobs');
        Schema::dropIfExists('SavedJobs');
        Schema::dropIfExists('AspNetUsers');
    }
}
