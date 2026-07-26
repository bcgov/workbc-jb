<?php

namespace Tests\Feature\Admin;

use App\Models\AdminUser;
use App\Models\Enums\AdminLevel;
use App\Models\JobSeeker;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;
use Tests\TestCase;

/**
 * FND-6 / ADR-008: the `admin` guard has no credential to check (AdminUsers has
 * no password column) — it only ever holds a session via a direct login() call.
 * Access to the Filament panel is gated by AdminUser::canAccessPanel(), driven
 * by AdminLevel. The job-seeker `web` guard (FND-5) must stay unaffected.
 */
class AdminAuthTest extends TestCase
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

    public function test_admin_guard_resolves_a_real_admin_user_via_direct_login(): void
    {
        $admin = $this->createAdmin(1, AdminLevel::SuperAdmin);

        Auth::guard('admin')->login($admin);

        $this->assertTrue(Auth::guard('admin')->check());
        $this->assertSame(1, Auth::guard('admin')->id());
        $this->assertInstanceOf(AdminUser::class, Auth::guard('admin')->user());
    }

    public function test_can_access_panel_is_true_for_active_reporting_admin_and_superadmin(): void
    {
        $this->assertTrue($this->createAdmin(1, AdminLevel::Reporting)->canAccessPanel($this->panel()));
        $this->assertTrue($this->createAdmin(2, AdminLevel::Admin)->canAccessPanel($this->panel()));
        $this->assertTrue($this->createAdmin(3, AdminLevel::SuperAdmin)->canAccessPanel($this->panel()));
    }

    public function test_can_access_panel_is_false_for_disabled_deleted_or_unrecognized_level(): void
    {
        $this->assertFalse($this->createAdmin(1, AdminLevel::Disabled)->canAccessPanel($this->panel()));
        $this->assertFalse($this->createAdmin(2, AdminLevel::SuperAdmin, deleted: true)->canAccessPanel($this->panel()));

        // A legacy/out-of-enum AdminLevel code reads as null via TolerantEnum — fails closed.
        DB::table('AdminUsers')->insert(['Id' => 3, 'AdminLevel' => 99, 'Deleted' => false, 'DateCreated' => now(), 'DateUpdated' => now()]);
        $this->assertFalse(AdminUser::query()->findOrFail(3)->canAccessPanel($this->panel()));
    }

    public function test_unauthenticated_visitor_cannot_reach_the_admin_panel(): void
    {
        $response = $this->get('/admin');

        $response->assertStatus(302);
        $this->assertFalse(Auth::guard('admin')->check());
    }

    public function test_job_seeker_web_guard_is_unaffected_by_the_admin_guard(): void
    {
        Schema::create('AspNetUsers', function (Blueprint $table): void {
            $table->string('Id')->primary();
            $table->string('Email')->nullable();
            $table->string('NormalizedEmail')->nullable();
            $table->string('PasswordHash')->nullable();
            $table->string('SecurityStamp')->nullable();
        });

        DB::table('AspNetUsers')->insert([
            'Id' => 'seeker-1', 'Email' => 'seeker@example.com',
            'NormalizedEmail' => 'SEEKER@EXAMPLE.COM', 'PasswordHash' => 'x', 'SecurityStamp' => 'x',
        ]);
        $seeker = JobSeeker::query()->findOrFail('seeker-1');
        $admin = $this->createAdmin(1, AdminLevel::SuperAdmin);

        Auth::guard('admin')->login($admin);
        Auth::guard('web')->login($seeker);

        $this->assertTrue(Auth::guard('admin')->check());
        $this->assertTrue(Auth::guard('web')->check());
        $this->assertSame('seeker-1', Auth::guard('web')->id());
        $this->assertSame(1, Auth::guard('admin')->id());

        Schema::dropIfExists('AspNetUsers');
    }

    private function panel(): \Filament\Panel
    {
        return \Filament\Facades\Filament::getPanel('admin');
    }

    private function createAdmin(int $id, AdminLevel $level, bool $deleted = false): AdminUser
    {
        DB::table('AdminUsers')->insert([
            'Id' => $id,
            'AdminLevel' => $level->value,
            'Deleted' => $deleted,
            'DateCreated' => now(),
            'DateUpdated' => now(),
        ]);

        return AdminUser::query()->findOrFail($id);
    }

    private function createFixture(): void
    {
        $this->dropFixture();

        Schema::create('AdminUsers', function (Blueprint $table): void {
            $table->integer('Id')->primary();
            $table->smallInteger('AdminLevel')->default(2);
            $table->string('DisplayName', 60)->default('Test Admin');
            $table->boolean('Deleted')->default(false);
            $table->integer('LockedByAdminUserId')->nullable();
            $table->dateTime('DateLocked')->nullable();
            $table->dateTime('DateCreated')->nullable();
            $table->dateTime('DateUpdated')->nullable();
            $table->dateTime('DateLastLogin')->nullable();
        });
    }

    private function dropFixture(): void
    {
        Schema::dropIfExists('AdminUsers');
    }
}
