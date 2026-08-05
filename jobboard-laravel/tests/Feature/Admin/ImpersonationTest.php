<?php

namespace Tests\Feature\Admin;

use App\Models\AdminUser;
use App\Models\Enums\AdminLevel;
use App\Models\JobSeeker;
use App\Services\Admin\ImpersonationService;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;
use Tests\TestCase;

/**
 * FND-6 / ADM-4 scaffold: starting impersonation writes the ImpersonationLog
 * audit row (Token/AspNetUserId/AdminUserId/DateTokenCreated — no "ended"
 * column, confirmed against the real schema) and switches the `web` guard to
 * the seeker, while the `admin` guard's own session is left untouched.
 */
class ImpersonationTest extends TestCase
{
    private AdminUser $admin;

    private JobSeeker $seeker;

    protected function setUp(): void
    {
        parent::setUp();
        $this->createFixture();

        DB::table('AdminUsers')->insert([
            'Id' => 1, 'AdminLevel' => AdminLevel::SuperAdmin->value, 'Deleted' => false,
            'DateCreated' => now(), 'DateUpdated' => now(),
        ]);
        $this->admin = AdminUser::query()->findOrFail(1);

        DB::table('AspNetUsers')->insert([
            'Id' => 'seeker-1', 'Email' => 'seeker@example.com',
            'NormalizedEmail' => 'SEEKER@EXAMPLE.COM', 'PasswordHash' => 'x', 'SecurityStamp' => 'x',
        ]);
        $this->seeker = JobSeeker::query()->findOrFail('seeker-1');
    }

    protected function tearDown(): void
    {
        $this->dropFixture();
        parent::tearDown();
    }

    public function test_start_writes_audit_row_and_switches_the_web_guard_only(): void
    {
        Auth::guard('admin')->login($this->admin);

        $token = app(ImpersonationService::class)->start($this->admin, $this->seeker);

        $this->assertNotSame('', $token);

        $row = DB::table('ImpersonationLog')->where('Token', $token)->first();
        $this->assertNotNull($row);
        $this->assertSame('seeker-1', $row->AspNetUserId);
        $this->assertSame(1, (int) $row->AdminUserId);
        $this->assertNotNull($row->DateTokenCreated);

        $this->assertTrue(Auth::guard('web')->check());
        $this->assertSame('seeker-1', Auth::guard('web')->id());
        // The admin guard's own session is untouched by impersonating on `web`.
        $this->assertTrue(Auth::guard('admin')->check());
        $this->assertSame(1, Auth::guard('admin')->id());

        $this->assertTrue(app(ImpersonationService::class)->isActive());
        $this->assertSame(1, app(ImpersonationService::class)->impersonatingAdminId());
    }

    public function test_end_logs_out_web_guard_only_and_admin_session_survives(): void
    {
        Auth::guard('admin')->login($this->admin);
        app(ImpersonationService::class)->start($this->admin, $this->seeker);

        app(ImpersonationService::class)->end();

        $this->assertFalse(Auth::guard('web')->check());
        $this->assertFalse(app(ImpersonationService::class)->isActive());
        // The point of the whole design: ending impersonation never touches `admin`.
        $this->assertTrue(Auth::guard('admin')->check());
        $this->assertSame(1, Auth::guard('admin')->id());
    }

    public function test_end_impersonation_route_redirects_to_admin(): void
    {
        Auth::guard('admin')->login($this->admin);
        app(ImpersonationService::class)->start($this->admin, $this->seeker);

        $response = $this->post('/account/impersonation/end');

        $response->assertRedirect('/admin');
        $this->assertFalse(Auth::guard('web')->check());
        $this->assertTrue(Auth::guard('admin')->check());
    }

    public function test_impersonation_banner_shows_only_while_active(): void
    {
        Auth::guard('admin')->login($this->admin);
        app(ImpersonationService::class)->start($this->admin, $this->seeker);

        $this->get('/account')->assertOk()->assertSee('End impersonation');

        app(ImpersonationService::class)->end();
        Auth::guard('web')->login($this->seeker); // view the dashboard as the seeker directly this time

        $this->get('/account')->assertOk()->assertDontSee('End impersonation');
    }

    private function createFixture(): void
    {
        $this->dropFixture();

        Schema::create('AdminUsers', function (Blueprint $table): void {
            $table->integer('Id')->primary();
            $table->smallInteger('AdminLevel')->default(2);
            $table->string('DisplayName', 60)->default('Test Admin');
            $table->boolean('Deleted')->default(false);
            $table->dateTime('DateCreated')->nullable();
            $table->dateTime('DateUpdated')->nullable();
        });

        Schema::create('AspNetUsers', function (Blueprint $table): void {
            $table->string('Id')->primary();
            $table->string('Email')->nullable();
            $table->string('NormalizedEmail')->nullable();
            $table->string('PasswordHash')->nullable();
            $table->string('SecurityStamp')->nullable();
        });

        Schema::create('ImpersonationLog', function (Blueprint $table): void {
            $table->string('Token')->primary();
            $table->string('AspNetUserId')->nullable();
            $table->integer('AdminUserId');
            $table->dateTime('DateTokenCreated');
        });

        // Minimal support tables the dashboard's summary counts read.
        Schema::create('SavedJobs', function (Blueprint $table): void {
            $table->increments('Id');
            $table->string('JobId');
            $table->string('AspNetUserId');
            $table->boolean('IsDeleted')->default(false);
        });
        Schema::create('JobAlerts', function (Blueprint $table): void {
            $table->increments('Id');
            $table->string('AspNetUserId');
            $table->smallInteger('AlertFrequency')->default(1);
            $table->boolean('IsDeleted')->default(false);
        });
        Schema::create('SavedCareerProfiles', function (Blueprint $table): void {
            $table->increments('Id');
            $table->string('AspNetUserId');
            $table->boolean('IsDeleted')->default(false);
        });
        Schema::create('SavedIndustryProfiles', function (Blueprint $table): void {
            $table->increments('Id');
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
        Schema::dropIfExists('ImpersonationLog');
        Schema::dropIfExists('AspNetUsers');
        Schema::dropIfExists('AdminUsers');
    }
}
