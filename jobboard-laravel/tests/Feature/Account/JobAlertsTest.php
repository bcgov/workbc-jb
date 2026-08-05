<?php

namespace Tests\Feature\Account;

use App\Livewire\JobAlertsList;
use App\Models\Enums\AlertFrequency;
use App\Models\JobAlert;
use App\Models\JobSeeker;
use App\Search\Filters\JobSearchFilters;
use App\Services\JobSeeker\JobAlertsService;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Carbon;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;
use Livewire\Livewire;
use Mockery;
use OpenSearch\Client;
use Tests\TestCase;

/**
 * ACCT-3 — job alerts: create/edit/list/delete, JobSearchFilters JSON round-trip,
 * the live match count, the JobSeekerChangeLog audit side-effect, and per-user
 * scoping.
 */
class JobAlertsTest extends TestCase
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

    private function filters(): JobSearchFilters
    {
        return JobSearchFilters::fromArray([
            'Keyword' => 'nurse',
            'SearchJobTypeFullTime' => true,
        ]);
    }

    public function test_create_stores_row_with_round_tripped_filters_and_audit(): void
    {
        $user = $this->createUser('user-a');
        Carbon::setTestNow('2026-07-24 09:00:00');

        $alert = app(JobAlertsService::class)->save($user, null, 'Nursing jobs', AlertFrequency::Daily, $this->filters());

        $this->assertSame('Nursing jobs', $alert->Title);
        $this->assertSame(AlertFrequency::Daily, $alert->AlertFrequency);
        $this->assertSame(1, (int) $alert->JobSearchFiltersVersion);
        $this->assertNotSame('', (string) $alert->UrlParameters);
        $this->assertFalse((bool) $alert->IsDeleted);

        // Stored JSON round-trips back through the model cast to a VO.
        $reloaded = JobAlert::query()->findOrFail($alert->Id);
        $this->assertInstanceOf(JobSearchFilters::class, $reloaded->JobSearchFilters);
        $this->assertSame('nurse', $reloaded->JobSearchFilters->Keyword);
        $this->assertTrue($reloaded->JobSearchFilters->SearchJobTypeFullTime);

        // Audit row (matches the .NET convention).
        $log = DB::table('JobSeekerChangeLog')->where('AspNetUserId', 'user-a')->first();
        $this->assertNotNull($log);
        $this->assertSame("Job alert 'Nursing jobs' created", $log->Field);
        $this->assertNull($log->ModifiedByAdminUserId);
        $this->assertSame('-', $log->OldValue);
    }

    public function test_edit_updates_fields_and_writes_updated_audit(): void
    {
        $user = $this->createUser('user-a');
        $service = app(JobAlertsService::class);
        $alert = $service->save($user, null, 'Original', AlertFrequency::Daily, $this->filters());

        Carbon::setTestNow('2026-07-24 12:00:00');
        $updated = $service->save($user, (int) $alert->Id, 'Renamed', AlertFrequency::Weekly, $this->filters());

        $this->assertSame((int) $alert->Id, (int) $updated->Id);
        $this->assertSame('Renamed', $updated->Title);
        $this->assertSame(AlertFrequency::Weekly, $updated->AlertFrequency);
        $this->assertSame('2026-07-24 12:00:00', $updated->DateModified?->format('Y-m-d H:i:s'));
        $this->assertSame(1, JobAlert::query()->where('AspNetUserId', 'user-a')->count());

        $this->assertDatabaseHas('JobSeekerChangeLog', [
            'AspNetUserId' => 'user-a',
            'Field' => "Job alert 'Renamed' updated",
        ]);
    }

    public function test_preview_total_runs_filters_with_size_zero(): void
    {
        $client = Mockery::mock(Client::class);
        $captured = null;
        $client->shouldReceive('search')->andReturnUsing(function (array $params) use (&$captured): array {
            $captured = $params['body'];

            return ['hits' => ['total' => ['value' => 4242], 'hits' => []]];
        });
        $this->app->instance(Client::class, $client);

        $total = app(JobAlertsService::class)->previewTotal($this->filters());

        $this->assertSame(4242, $total);
        $this->assertSame(0, $captured['size']);
    }

    public function test_delete_soft_deletes_and_writes_deleted_audit(): void
    {
        $user = $this->createUser('user-a');
        $service = app(JobAlertsService::class);
        $alert = $service->save($user, null, 'To delete', AlertFrequency::Daily, $this->filters());

        $this->assertTrue($service->delete($user, (int) $alert->Id));

        $row = JobAlert::withTrashed()->findOrFail($alert->Id);
        $this->assertTrue((bool) $row->IsDeleted);
        $this->assertNotNull($row->DateDeleted);
        $this->assertCount(0, $service->listFor($user));

        $this->assertDatabaseHas('JobSeekerChangeLog', [
            'AspNetUserId' => 'user-a',
            'Field' => "Job alert 'To delete' deleted",
            'ModifiedByAdminUserId' => null,
        ]);
    }

    public function test_list_and_edit_and_delete_are_user_scoped(): void
    {
        $userA = $this->createUser('user-a');
        $userB = $this->createUser('user-b');
        $service = app(JobAlertsService::class);

        $service->save($userA, null, 'Alert A', AlertFrequency::Daily, $this->filters());
        $bAlert = $service->save($userB, null, 'Alert B', AlertFrequency::Daily, $this->filters());

        // findFor / delete never reach another user's alert.
        $this->assertNull($service->findFor($userA, (int) $bAlert->Id));
        $this->assertFalse($service->delete($userA, (int) $bAlert->Id));
        $this->assertFalse((bool) JobAlert::withTrashed()->findOrFail($bAlert->Id)->IsDeleted);

        // List page shows only the current user's alerts.
        $this->actingAs($userA, 'web');
        $this->get('/account/alerts')->assertOk()->assertSee('Alert A')->assertDontSee('Alert B');

        // Editing another user's alert 404s (JobSearch mount -> findFor null).
        $this->get('/account/alerts/'.$bAlert->Id.'/edit')->assertNotFound();

        // Delete from the list is scoped to the current user.
        Livewire::test(JobAlertsList::class)->call('delete', (int) $bAlert->Id);
        $this->assertFalse((bool) JobAlert::withTrashed()->findOrFail($bAlert->Id)->IsDeleted);
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

        Schema::create('JobAlerts', function (Blueprint $table): void {
            $table->increments('Id');
            $table->string('AspNetUserId');
            $table->string('Title')->nullable();
            $table->smallInteger('AlertFrequency')->default(1);
            $table->text('JobSearchFilters')->nullable();
            $table->integer('JobSearchFiltersVersion')->default(0);
            $table->text('UrlParameters')->nullable();
            $table->boolean('IsDeleted')->default(false);
            $table->dateTime('DateCreated')->nullable();
            $table->dateTime('DateModified')->nullable();
            $table->dateTime('DateDeleted')->nullable();
        });

        Schema::create('JobSeekerChangeLog', function (Blueprint $table): void {
            $table->increments('Id');
            $table->string('AspNetUserId')->nullable();
            $table->string('Field')->nullable();
            $table->text('OldValue')->nullable();
            $table->text('NewValue')->nullable();
            $table->integer('ModifiedByAdminUserId')->nullable();
            $table->dateTime('DateUpdated');
        });
    }

    private function dropFixture(): void
    {
        Schema::dropIfExists('JobSeekerChangeLog');
        Schema::dropIfExists('JobAlerts');
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
}
