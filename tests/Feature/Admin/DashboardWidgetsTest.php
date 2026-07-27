<?php

namespace Tests\Feature\Admin;

use App\Filament\Widgets\ActiveJobsByIndustryChart;
use App\Filament\Widgets\ActiveJobsByIndustryTable;
use App\Filament\Widgets\JobsPostedPerWeekChart;
use App\Filament\Widgets\JobsPostedPerWeekTable;
use App\Models\AdminUser;
use App\Models\Enums\AdminLevel;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;
use Livewire\Livewire;
use Tests\TestCase;

/**
 * ADM-9 preview: the dashboard chart widgets and their WCAG 1.1.1 data-table
 * alternatives, reading real Jobs/Industries columns directly (Rule B —
 * ExpireDate/IndustryId/DatePosted are Jobs columns, only aggregated here,
 * never recomputed). Not the full ADM-9 story: no period/region/source
 * filters yet (those depend on the ADM-8 Reporting service).
 */
class DashboardWidgetsTest extends TestCase
{
    protected function setUp(): void
    {
        parent::setUp();
        $this->createFixture();

        DB::table('AdminUsers')->insert([
            'Id' => 1, 'AdminLevel' => AdminLevel::SuperAdmin->value, 'Deleted' => false,
            'DisplayName' => 'Test Admin', 'DateCreated' => now(), 'DateUpdated' => now(),
        ]);
        Auth::guard('admin')->login(AdminUser::query()->findOrFail(1));
    }

    protected function tearDown(): void
    {
        $this->dropFixture();
        parent::tearDown();
    }

    public function test_active_jobs_by_industry_chart_reflects_active_jobs_only(): void
    {
        Livewire::test(ActiveJobsByIndustryChart::class)
            ->assertSee('Active jobs by industry')
            ->assertSee('Construction') // 2 active Construction jobs seeded below
            ->assertDontSee('Expired Industry'); // its only job is expired
    }

    public function test_active_jobs_by_industry_table_is_the_accessible_alternative_with_real_counts(): void
    {
        Livewire::test(ActiveJobsByIndustryTable::class)
            ->assertSee('Construction')
            ->assertSee('2') // count for Construction
            ->assertDontSee('Expired Industry');
    }

    public function test_jobs_posted_per_week_chart_and_table_reflect_recent_postings(): void
    {
        Livewire::test(JobsPostedPerWeekChart::class)
            ->assertSee('Jobs posted per week');

        Livewire::test(JobsPostedPerWeekTable::class)
            ->assertSee('Jobs posted');
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

        Schema::create('Industries', function (Blueprint $table): void {
            $table->smallInteger('Id')->primary();
            $table->string('Title');
        });

        Schema::create('Jobs', function (Blueprint $table): void {
            $table->string('JobId')->primary();
            $table->smallInteger('IndustryId')->nullable();
            $table->dateTime('ExpireDate')->nullable();
            $table->dateTime('DatePosted')->nullable();
        });

        DB::table('Industries')->insert([
            ['Id' => 23, 'Title' => 'Construction'],
            ['Id' => 99, 'Title' => 'Expired Industry'],
        ]);

        DB::table('Jobs')->insert([
            ['JobId' => 'job-1', 'IndustryId' => 23, 'ExpireDate' => now()->addDays(10), 'DatePosted' => now()->subDays(3)],
            ['JobId' => 'job-2', 'IndustryId' => 23, 'ExpireDate' => now()->addDays(20), 'DatePosted' => now()->subDays(3)],
            ['JobId' => 'job-3', 'IndustryId' => 99, 'ExpireDate' => now()->subDays(5), 'DatePosted' => now()->subDays(10)], // expired
        ]);
    }

    private function dropFixture(): void
    {
        Schema::dropIfExists('Jobs');
        Schema::dropIfExists('Industries');
        Schema::dropIfExists('AdminUsers');
    }
}
