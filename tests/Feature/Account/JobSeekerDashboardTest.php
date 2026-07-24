<?php

namespace Tests\Feature\Account;

use App\Models\Enums\AlertFrequency;
use App\Models\JobSeeker;
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
        Schema::dropIfExists('AspNetUsers');
    }

    private function seedUsers(): void
    {
        foreach (['user-a', 'user-b'] as $id) {
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
