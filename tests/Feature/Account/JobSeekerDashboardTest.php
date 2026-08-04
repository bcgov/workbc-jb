<?php

namespace Tests\Feature\Account;

use App\Models\Enums\AlertFrequency;
use App\Models\JobSeeker;
use App\Services\Settings\SystemSettingsService;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;
use Tests\TestCase;

class JobSeekerDashboardTest extends TestCase
{
    protected function setUp(): void
    {
        parent::setUp();

        $this->createFixture();
    }

    protected function tearDown(): void
    {
        $this->dropFixture();

        parent::tearDown();
    }

    public function test_dashboard_shows_only_authenticated_users_counts(): void
    {
        $this->seedUsers();

        $this->seedSavedJobs('user-a', 2, 1);
        $this->seedSavedJobs('user-b', 7, 0);

        $this->seedJobAlerts('user-a', [
            ['frequency' => AlertFrequency::Daily->value, 'deleted' => false],
            ['frequency' => AlertFrequency::Never->value, 'deleted' => false],
            ['frequency' => AlertFrequency::Weekly->value, 'deleted' => true],
        ]);
        $this->seedJobAlerts('user-b', [
            ['frequency' => AlertFrequency::Daily->value, 'deleted' => false],
            ['frequency' => AlertFrequency::Monthly->value, 'deleted' => false],
            ['frequency' => AlertFrequency::Never->value, 'deleted' => false],
            ['frequency' => AlertFrequency::BiWeekly->value, 'deleted' => false],
        ]);

        $this->seedSavedCareerProfiles('user-a', 1, 2);
        $this->seedSavedCareerProfiles('user-b', 9, 0);

        $this->seedSavedIndustryProfiles('user-a', 3, 1);
        $this->seedSavedIndustryProfiles('user-b', 8, 0);

        $userA = JobSeeker::query()->findOrFail('user-a');

        $response = $this->actingAs($userA, 'web')->get('/account');

        $response->assertOk();

        $html = $response->getContent();

        $this->assertMetricCount($html, 'saved-jobs-count', 2);
        $this->assertMetricCount($html, 'active-alerts-count', 1);
        $this->assertMetricCount($html, 'saved-career-profiles-count', 1);
        $this->assertMetricCount($html, 'saved-industry-profiles-count', 3);

        // Ensure distinct counts from another user are not rendered for user-a.
        $this->assertMetricCount($html, 'saved-jobs-count', 7, false);
        $this->assertMetricCount($html, 'active-alerts-count', 4, false);
        $this->assertMetricCount($html, 'saved-career-profiles-count', 9, false);
        $this->assertMetricCount($html, 'saved-industry-profiles-count', 8, false);
    }

    public function test_dashboard_requires_authentication(): void
    {
        $response = $this->get('/account');

        $response->assertRedirect('/login');
    }

    public function test_dashboard_groups_counts_as_links_and_has_no_dead_account_settings_link(): void
    {
        $this->seedUsers();

        $user = JobSeeker::query()->findOrFail('user-a');

        $this->seedSavedJobs('user-a', 2, 0);
        $this->seedJobAlerts('user-a', [['frequency' => AlertFrequency::Daily->value, 'deleted' => false]]);
        $this->seedSavedCareerProfiles('user-a', 4, 0);
        $this->seedSavedIndustryProfiles('user-a', 5, 0);

        $response = $this->actingAs($user, 'web')->get('/account');

        $response->assertOk()
            ->assertSee('Jobs')
            ->assertSee('Careers &amp; industries', false)
            ->assertSee('Manage account')
            ->assertSee('href="'.route('account.saved-jobs').'"', false)
            ->assertSee('href="'.route('account.alerts').'"', false)
            ->assertSee('href="'.route('account.profiles').'"', false)
            ->assertDontSee('/account/settings', false)
            ->assertSee('Personal settings (coming soon)');

        $html = $response->getContent();

        $this->assertMetricCount($html, 'saved-jobs-count', 2);
        $this->assertMetricCount($html, 'active-alerts-count', 1);
        $this->assertMetricCount($html, 'saved-career-profiles-count', 4);
        $this->assertMetricCount($html, 'saved-industry-profiles-count', 5);
    }

    public function test_dashboard_reads_copy_from_system_settings_and_caches_until_invalidation(): void
    {
        $this->seedUsers();
        $this->seedDashboardSettings([
            'introText' => '<p>Intro <strong>copy</strong><script>alert(1)</script></p>',
            'jobsDescription' => '<p>Jobs area description.</p>',
            'careersDescription' => '<p>Careers area description.</p>',
            'accountDescription' => '<p>Account area description.</p>',
            'newAccountMessageTitle' => 'Welcome to your account',
            'newAccountMessageBody' => '<p>This is your first visit.</p>',
            'notification1Title' => 'Service update',
            'notification1Body' => '<p>Maintenance this weekend.</p>',
            'notification1Enabled' => '1',
            'notification2Title' => 'Hidden notification',
            'notification2Body' => '<p>Should not render.</p>',
            'notification2Enabled' => '0',
            'resource1Title' => 'High opportunity occupations',
            'resource1Body' => '<p>Explore labour market insights.</p>',
            'resource1Url' => '/research-labour-market/high-opportunity-occupations',
        ]);

        $user = JobSeeker::query()->findOrFail('user-a');

        $first = $this->actingAs($user, 'web')->get('/account');

        $first->assertOk()
            ->assertSee('Welcome to your account')
            ->assertSee('Service update')
            ->assertDontSee('Hidden notification')
            ->assertSee('Intro', false)
            ->assertSee('Jobs area description.', false)
            ->assertSee('High opportunity occupations')
            ->assertSee('https://www.workbc.ca/research-labour-market/high-opportunity-occupations', false)
            ->assertDontSee('alert(1)');

        DB::table('SystemSettings')
            ->where('Name', 'jbAccount.dashboard.introText')
            ->update(['Value' => '<p>Updated intro after first render.</p>']);

        $cached = $this->actingAs($user, 'web')->get('/account');
        $cached->assertOk()
            ->assertSee('Intro', false)
            ->assertDontSee('Updated intro after first render.', false);

        app(SystemSettingsService::class)->invalidateCache();

        $fresh = $this->actingAs($user, 'web')->get('/account');
        $fresh->assertOk()
            ->assertSee('Updated intro after first render.', false);
    }

    private function assertMetricCount(string $html, string $testId, int $count, bool $shouldExist = true): void
    {
        $pattern = '/data-testid="'.preg_quote($testId, '/').'"[^>]*>\s*'.preg_quote((string) $count, '/').'\s*</';
        $matches = preg_match($pattern, $html) === 1;

        if ($shouldExist) {
            $this->assertTrue($matches, "Expected {$testId} to show {$count}.");

            return;
        }

        $this->assertFalse($matches, "Did not expect {$testId} to show {$count}.");
    }

    private function createFixture(): void
    {
        $this->dropFixture();

        Schema::create('AspNetUsers', function (Blueprint $table): void {
            $table->string('Id')->primary();
            $table->string('UserName')->nullable();
            $table->string('FirstName')->nullable();
            $table->string('NormalizedUserName')->nullable();
            $table->string('Email')->nullable();
            $table->string('NormalizedEmail')->nullable();
            $table->string('PasswordHash')->nullable();
            $table->string('SecurityStamp')->nullable();
            $table->boolean('EmailConfirmed')->default(true);
        });

        Schema::create('SystemSettings', function (Blueprint $table): void {
            $table->string('Name')->primary();
            $table->text('Value')->nullable();
            $table->integer('FieldType')->default(0);
            $table->text('DefaultValue')->nullable();
            $table->integer('ModifiedByAdminUserId')->nullable();
            $table->dateTime('DateUpdated')->nullable();
        });

        Schema::create('SavedJobs', function (Blueprint $table): void {
            $table->id('Id');
            $table->string('AspNetUserId');
            $table->string('JobId')->nullable();
            $table->boolean('IsDeleted')->default(false);
            $table->dateTime('DateDeleted')->nullable();
        });

        Schema::create('JobAlerts', function (Blueprint $table): void {
            $table->id('Id');
            $table->string('AspNetUserId');
            $table->unsignedTinyInteger('AlertFrequency')->default(AlertFrequency::Daily->value);
            $table->boolean('IsDeleted')->default(false);
            $table->dateTime('DateDeleted')->nullable();
        });

        Schema::create('SavedCareerProfiles', function (Blueprint $table): void {
            $table->id('Id');
            $table->string('AspNetUserId');
            $table->boolean('IsDeleted')->default(false);
        });

        Schema::create('SavedIndustryProfiles', function (Blueprint $table): void {
            $table->id('Id');
            $table->string('AspNetUserId');
            $table->boolean('IsDeleted')->default(false);
        });
    }

    private function dropFixture(): void
    {
        Schema::dropIfExists('SavedIndustryProfiles');
        Schema::dropIfExists('SavedCareerProfiles');
        Schema::dropIfExists('JobAlerts');
        Schema::dropIfExists('SavedJobs');
        Schema::dropIfExists('SystemSettings');
        Schema::dropIfExists('AspNetUsers');
    }

    private function seedUsers(): void
    {
        foreach (['user-a', 'user-b'] as $id) {
            DB::table('AspNetUsers')->insert([
                'Id' => $id,
                'UserName' => $id.'@example.com',
                'FirstName' => $id === 'user-a' ? 'Alex' : 'Bryn',
                'NormalizedUserName' => mb_strtoupper($id.'@example.com', 'UTF-8'),
                'Email' => $id.'@example.com',
                'NormalizedEmail' => mb_strtoupper($id.'@example.com', 'UTF-8'),
                'PasswordHash' => 'unused-hash',
                'SecurityStamp' => 'stamp-'.$id,
                'EmailConfirmed' => true,
            ]);
        }
    }

    /**
     * @param  array<string, string>  $values
     */
    private function seedDashboardSettings(array $values): void
    {
        foreach ($values as $name => $value) {
            DB::table('SystemSettings')->insert([
                'Name' => 'jbAccount.dashboard.'.$name,
                'Value' => $value,
                'FieldType' => 5,
                'DefaultValue' => null,
                'ModifiedByAdminUserId' => null,
                'DateUpdated' => now(),
            ]);
        }
    }

    private function seedSavedJobs(string $userId, int $active, int $deleted): void
    {
        for ($i = 0; $i < $active; $i++) {
            DB::table('SavedJobs')->insert([
                'AspNetUserId' => $userId,
                'JobId' => $userId.'-active-'.$i,
                'IsDeleted' => false,
                'DateDeleted' => null,
            ]);
        }

        for ($i = 0; $i < $deleted; $i++) {
            DB::table('SavedJobs')->insert([
                'AspNetUserId' => $userId,
                'JobId' => $userId.'-deleted-'.$i,
                'IsDeleted' => true,
                'DateDeleted' => now(),
            ]);
        }
    }

    /**
     * @param  list<array{frequency:int,deleted:bool}>  $alerts
     */
    private function seedJobAlerts(string $userId, array $alerts): void
    {
        foreach ($alerts as $alert) {
            DB::table('JobAlerts')->insert([
                'AspNetUserId' => $userId,
                'AlertFrequency' => $alert['frequency'],
                'IsDeleted' => $alert['deleted'],
                'DateDeleted' => $alert['deleted'] ? now() : null,
            ]);
        }
    }

    private function seedSavedCareerProfiles(string $userId, int $active, int $deleted): void
    {
        for ($i = 0; $i < $active; $i++) {
            DB::table('SavedCareerProfiles')->insert([
                'AspNetUserId' => $userId,
                'IsDeleted' => false,
            ]);
        }

        for ($i = 0; $i < $deleted; $i++) {
            DB::table('SavedCareerProfiles')->insert([
                'AspNetUserId' => $userId,
                'IsDeleted' => true,
            ]);
        }
    }

    private function seedSavedIndustryProfiles(string $userId, int $active, int $deleted): void
    {
        for ($i = 0; $i < $active; $i++) {
            DB::table('SavedIndustryProfiles')->insert([
                'AspNetUserId' => $userId,
                'IsDeleted' => false,
            ]);
        }

        for ($i = 0; $i < $deleted; $i++) {
            DB::table('SavedIndustryProfiles')->insert([
                'AspNetUserId' => $userId,
                'IsDeleted' => true,
            ]);
        }
    }
}
