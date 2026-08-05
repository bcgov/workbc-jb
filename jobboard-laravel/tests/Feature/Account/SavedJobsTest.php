<?php

namespace Tests\Feature\Account;

use App\Livewire\SavedJobsPage;
use App\Models\JobSeeker;
use App\Models\SavedJob;
use App\Services\JobSeeker\SavedJobService;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Carbon;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;
use Livewire\Livewire;
use Mockery;
use OpenSearch\Client;
use Tests\TestCase;

class SavedJobsTest extends TestCase
{
    protected function setUp(): void
    {
        parent::setUp();

        $this->createFixture();
    }

    protected function tearDown(): void
    {
        Carbon::setTestNow();
        Mockery::close();
        $this->dropFixture();

        parent::tearDown();
    }

    public function test_save_creates_single_row_and_is_idempotent_including_restore(): void
    {
        $user = $this->createUser('user-a');
        $this->insertJob('job-1', 'First Job');

        $service = app(SavedJobService::class);

        Carbon::setTestNow('2026-07-24 09:00:00');
        $service->save($user, 'job-1');

        $first = SavedJob::withTrashed()->where('AspNetUserId', 'user-a')->where('JobId', 'job-1')->firstOrFail();
        $this->assertFalse((bool) $first->IsDeleted);

        Carbon::setTestNow('2026-07-24 09:30:00');
        $service->save($user, 'job-1');

        $second = SavedJob::withTrashed()->where('AspNetUserId', 'user-a')->where('JobId', 'job-1')->firstOrFail();

        $this->assertSame($first->Id, $second->Id);
        $this->assertSame(1, SavedJob::withTrashed()->where('AspNetUserId', 'user-a')->where('JobId', 'job-1')->count());
        $this->assertSame('2026-07-24 09:30:00', $second->DateSaved?->format('Y-m-d H:i:s'));

        $service->unsave($user, 'job-1');

        $deleted = SavedJob::withTrashed()->where('AspNetUserId', 'user-a')->where('JobId', 'job-1')->firstOrFail();
        $this->assertTrue((bool) $deleted->IsDeleted);
        $this->assertNotNull($deleted->DateDeleted);

        Carbon::setTestNow('2026-07-24 10:00:00');
        $service->save($user, 'job-1');

        $restored = SavedJob::withTrashed()->where('AspNetUserId', 'user-a')->where('JobId', 'job-1')->firstOrFail();
        $this->assertSame($first->Id, $restored->Id);
        $this->assertFalse((bool) $restored->IsDeleted);
        $this->assertNull($restored->DateDeleted);
        $this->assertSame('2026-07-24 10:00:00', $restored->DateSaved?->format('Y-m-d H:i:s'));
    }

    public function test_legacy_duplicate_pairs_are_treated_as_one_membership(): void
    {
        // The real SavedJobs table has NO unique (AspNetUserId, JobId) constraint,
        // and the restored data contains duplicate pairs (incl. multiple ACTIVE
        // rows). Seed that directly, bypassing the service.
        $user = $this->createUser('user-a');
        $this->insertJob('job-1', 'Duplicated Job');

        DB::table('SavedJobs')->insert([
            ['JobId' => 'job-1', 'AspNetUserId' => 'user-a', 'DateSaved' => now(), 'IsDeleted' => false],
            ['JobId' => 'job-1', 'AspNetUserId' => 'user-a', 'DateSaved' => now(), 'IsDeleted' => false],
        ]);

        $service = app(SavedJobService::class);

        // Reported saved once; list shows the job a single time.
        $this->assertTrue($service->isSaved($user, 'job-1'));
        $this->assertCount(1, $service->listFor($user));

        // save() must not add a third active row.
        $service->save($user, 'job-1');
        $this->assertSame(2, SavedJob::query()->where('AspNetUserId', 'user-a')->where('JobId', 'job-1')->count());

        // unsave() clears ALL active rows, so the job is fully unsaved.
        $service->unsave($user, 'job-1');
        $this->assertFalse($service->isSaved($user, 'job-1'));
        $this->assertSame(0, SavedJob::query()->where('AspNetUserId', 'user-a')->where('JobId', 'job-1')->count());
        $this->assertSame(2, SavedJob::withTrashed()->where('AspNetUserId', 'user-a')->where('JobId', 'job-1')->count());
    }

    public function test_note_add_and_edit_updates_note_updated_date_and_over_limit_is_rejected(): void
    {
        $user = $this->createUser('user-a');
        $this->insertJob('job-1', 'Job With Note');

        app(SavedJobService::class)->save($user, 'job-1');

        $this->actingAs($user, 'web');

        Carbon::setTestNow('2026-07-24 11:00:00');
        Livewire::test(SavedJobsPage::class)
            ->call('startEditing', 'job-1')
            ->set('noteDraft', 'First note')
            ->call('saveNote', 'job-1')
            ->assertSee('First note');

        $afterFirst = SavedJob::query()->where('AspNetUserId', 'user-a')->where('JobId', 'job-1')->firstOrFail();
        $this->assertSame('First note', $afterFirst->Note);
        $this->assertSame('2026-07-24 11:00:00', $afterFirst->NoteUpdatedDate?->format('Y-m-d H:i:s'));

        Carbon::setTestNow('2026-07-24 11:30:00');
        Livewire::test(SavedJobsPage::class)
            ->call('startEditing', 'job-1')
            ->set('noteDraft', 'Updated note')
            ->call('saveNote', 'job-1');

        $afterSecond = SavedJob::query()->where('AspNetUserId', 'user-a')->where('JobId', 'job-1')->firstOrFail();
        $this->assertSame('Updated note', $afterSecond->Note);
        $this->assertSame('2026-07-24 11:30:00', $afterSecond->NoteUpdatedDate?->format('Y-m-d H:i:s'));

        Livewire::test(SavedJobsPage::class)
            ->call('startEditing', 'job-1')
            ->set('noteDraft', str_repeat('a', 801))
            ->call('saveNote', 'job-1')
            ->assertHasErrors(['noteDraft' => 'max']);

        $reloaded = SavedJob::query()->where('AspNetUserId', 'user-a')->where('JobId', 'job-1')->firstOrFail();
        $this->assertSame('Updated note', $reloaded->Note);
    }

    public function test_unsave_soft_deletes_and_saved_jobs_list_excludes_deleted_rows(): void
    {
        $user = $this->createUser('user-a');
        $this->insertJob('job-1', 'Soft Delete Job');

        app(SavedJobService::class)->save($user, 'job-1');

        $this->actingAs($user, 'web');

        $this->get('/account/saved-jobs')
            ->assertOk()
            ->assertSee('Soft Delete Job');

        Livewire::test(SavedJobsPage::class)
            ->call('unsave', 'job-1');

        $deleted = SavedJob::withTrashed()->where('AspNetUserId', 'user-a')->where('JobId', 'job-1')->firstOrFail();
        $this->assertTrue((bool) $deleted->IsDeleted);
        $this->assertNotNull($deleted->DateDeleted);

        $this->get('/account/saved-jobs')
            ->assertOk()
            ->assertDontSee('Soft Delete Job');
    }

    public function test_list_is_user_scoped_and_cannot_modify_another_users_saved_job(): void
    {
        $userA = $this->createUser('user-a');
        $userB = $this->createUser('user-b');

        $this->insertJob('job-a', 'User A Job');
        $this->insertJob('job-b', 'User B Job');

        $service = app(SavedJobService::class);
        $service->save($userA, 'job-a');
        $service->save($userB, 'job-b');

        $this->actingAs($userA, 'web');

        $this->get('/account/saved-jobs')
            ->assertOk()
            ->assertSee('User A Job')
            ->assertDontSee('User B Job');

        Livewire::test(SavedJobsPage::class)
            ->set('noteDraft', 'attempted overwrite')
            ->call('saveNote', 'job-b')
            ->call('unsave', 'job-b');

        $otherUsersRow = SavedJob::withTrashed()->where('AspNetUserId', 'user-b')->where('JobId', 'job-b')->firstOrFail();
        $this->assertFalse((bool) $otherUsersRow->IsDeleted);
        $this->assertNull($otherUsersRow->Note);
    }

    public function test_anonymous_search_remains_usable_and_save_toggle_is_not_shown(): void
    {
        $client = Mockery::mock(Client::class);
        $client->shouldReceive('search')->andReturn([
            'hits' => [
                'total' => ['value' => 1],
                'hits' => [
                    ['_source' => [
                        'JobId' => '777',
                        'Title' => 'Anonymous Search Job',
                        'EmployerName' => 'Public Employer',
                        'City' => ['Victoria'],
                        'DatePosted' => '2026-07-20T00:00:00',
                    ]],
                ],
            ],
        ]);
        $this->app->instance(Client::class, $client);

        $response = $this->get('/jobs');

        $response->assertOk();
        $response->assertSee('Anonymous Search Job');
        $response->assertSee('Sign in to save');
        $response->assertDontSee('data-testid="save-job-toggle-777"', false);
    }

    private function createFixture(): void
    {
        $this->dropFixture();

        Schema::create('AspNetUsers', function (Blueprint $table): void {
            $table->string('Id')->primary();
            $table->string('UserName')->nullable();
            $table->string('NormalizedUserName')->nullable();
            $table->string('Email')->nullable();
            $table->string('NormalizedEmail')->nullable();
            $table->string('PasswordHash')->nullable();
            $table->string('SecurityStamp')->nullable();
            $table->boolean('EmailConfirmed')->default(true);
        });

        Schema::create('Jobs', function (Blueprint $table): void {
            $table->string('JobId', 255)->primary();
            $table->string('Title')->nullable();
            $table->string('EmployerName')->nullable();
            $table->string('City')->nullable();
            $table->dateTime('ExpireDate')->nullable();
        });

        Schema::create('SavedJobs', function (Blueprint $table): void {
            $table->increments('Id');
            $table->string('JobId', 255);
            $table->string('AspNetUserId');
            $table->dateTime('DateSaved');
            $table->string('Note', 800)->nullable();
            $table->dateTime('NoteUpdatedDate')->nullable();
            $table->boolean('IsDeleted')->default(false);
            $table->dateTime('DateDeleted')->nullable();
        });
    }

    private function dropFixture(): void
    {
        Schema::dropIfExists('SavedJobs');
        Schema::dropIfExists('Jobs');
        Schema::dropIfExists('AspNetUsers');
    }

    private function createUser(string $id): JobSeeker
    {
        DB::table('AspNetUsers')->insert([
            'Id' => $id,
            'UserName' => $id.'@example.com',
            'NormalizedUserName' => mb_strtoupper($id.'@example.com', 'UTF-8'),
            'Email' => $id.'@example.com',
            'NormalizedEmail' => mb_strtoupper($id.'@example.com', 'UTF-8'),
            'PasswordHash' => 'unused-hash',
            'SecurityStamp' => 'stamp-'.$id,
            'EmailConfirmed' => true,
        ]);

        return JobSeeker::query()->findOrFail($id);
    }

    private function insertJob(string $jobId, string $title, ?string $expireDate = null): void
    {
        DB::table('Jobs')->insert([
            'JobId' => $jobId,
            'Title' => $title,
            'EmployerName' => 'Acme Employer',
            'City' => 'Vancouver',
            'ExpireDate' => $expireDate,
        ]);
    }
}
