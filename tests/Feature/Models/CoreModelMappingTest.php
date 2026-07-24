<?php

namespace Tests\Feature\Models;

use App\Models\AdminUser;
use App\Models\Enums\AccountStatus;
use App\Models\Enums\AdminLevel;
use App\Models\Enums\AlertFrequency;
use App\Models\Enums\SystemSettingFieldType;
use App\Models\Industry;
use App\Models\Job;
use App\Models\JobAlert;
use App\Models\JobSeeker;
use App\Models\JobSeekerFlags;
use App\Models\JobSource;
use App\Models\Location;
use App\Models\NocCode;
use App\Models\NocCode2021;
use App\Models\Region;
use App\Models\SavedJob;
use App\Models\SystemSetting;
use App\Search\Filters\JobSearchFilters;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;
use Tests\TestCase;

class CoreModelMappingTest extends TestCase
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

    public function test_job_mapping_casts_and_relationship(): void
    {
        $job = Job::query()->findOrFail('job-1');

        $this->assertSame('Jobs', $job->getTable());
        $this->assertSame('JobId', $job->getKeyName());
        $this->assertFalse($job->getIncrementing());
        $this->assertSame('string', $job->getKeyType());
        $this->assertTrue($job->IsActive);
        $this->assertSame('120000.50', $job->Salary);
        $this->assertSame('2026-08-20 10:00:00', $job->ExpireDate?->format('Y-m-d H:i:s'));
        $this->assertSame('Federal', $job->jobSource?->Name);
        // Jobs has no RegionId — region is reached through the job's Location.
        $this->assertSame('Mainland / Southwest', $job->location?->region?->Name);
    }

    public function test_job_seeker_mapping_casts_and_relationship(): void
    {
        $seeker = JobSeeker::query()->findOrFail('user-1');

        $this->assertSame('AspNetUsers', $seeker->getTable());
        $this->assertSame('Id', $seeker->getKeyName());
        $this->assertFalse($seeker->getIncrementing());
        $this->assertSame('string', $seeker->getKeyType());
        $this->assertSame(AccountStatus::Active, $seeker->AccountStatus);
        $this->assertTrue($seeker->EmailConfirmed);
        $this->assertSame('hash-1', $seeker->getAuthPassword());
        $this->assertTrue($seeker->flags?->IsApprentice);
    }

    public function test_saved_job_mapping_casts_soft_delete_and_relationship(): void
    {
        $savedJob = SavedJob::query()->findOrFail(101);

        $this->assertSame('SavedJobs', $savedJob->getTable());
        $this->assertSame('Id', $savedJob->getKeyName());
        $this->assertTrue($savedJob->getIncrementing());
        $this->assertSame('int', $savedJob->getKeyType());
        $this->assertFalse($savedJob->IsDeleted);
        $this->assertSame('2026-07-24 10:15:00', $savedJob->DateSaved?->format('Y-m-d H:i:s'));
        $this->assertSame('job-1', $savedJob->job?->JobId);
        $this->assertSame(1, SavedJob::query()->count());
        $this->assertSame(2, SavedJob::withTrashed()->count());
    }

    public function test_job_alert_mapping_casts_soft_delete_and_relationship(): void
    {
        $alert = JobAlert::query()->findOrFail(201);

        $this->assertSame('JobAlerts', $alert->getTable());
        $this->assertSame('Id', $alert->getKeyName());
        $this->assertTrue($alert->getIncrementing());
        $this->assertSame('int', $alert->getKeyType());
        $this->assertSame(AlertFrequency::Daily, $alert->AlertFrequency);
        $this->assertFalse($alert->IsDeleted);
        $this->assertInstanceOf(JobSearchFilters::class, $alert->JobSearchFilters);
        $this->assertSame(2, $alert->JobSearchFilters?->Page);
        $this->assertSame('2026-07-24 09:00:00', $alert->DateCreated?->format('Y-m-d H:i:s'));
        $this->assertSame('user-1', $alert->jobSeeker?->Id);
        $this->assertSame(1, JobAlert::query()->count());
        $this->assertSame(2, JobAlert::withTrashed()->count());
    }

    public function test_job_source_mapping_and_relationship(): void
    {
        $source = JobSource::query()->findOrFail(1);

        $this->assertSame('JobSources', $source->getTable());
        $this->assertSame('Id', $source->getKeyName());
        $this->assertFalse($source->getIncrementing());
        $this->assertSame('int', $source->getKeyType());
        $this->assertSame('job-1', $source->jobs->first()?->JobId);
    }

    public function test_location_mapping_casts_and_relationship(): void
    {
        $location = Location::query()->findOrFail(10);

        $this->assertSame('Locations', $location->getTable());
        $this->assertSame('LocationId', $location->getKeyName());
        $this->assertFalse($location->getIncrementing());
        $this->assertSame('int', $location->getKeyType());
        $this->assertFalse($location->IsHidden);
        $this->assertSame('Mainland / Southwest', $location->region?->Name);
    }

    public function test_region_mapping_and_relationship(): void
    {
        $region = Region::query()->findOrFail(2);

        $this->assertSame('Regions', $region->getTable());
        $this->assertSame('Id', $region->getKeyName());
        $this->assertFalse($region->getIncrementing());
        $this->assertSame('int', $region->getKeyType());
        $this->assertSame(10, $region->locations->first()?->LocationId);
    }

    public function test_industry_mapping_and_relationship(): void
    {
        $industry = Industry::query()->findOrFail(21);

        $this->assertSame('Industries', $industry->getTable());
        $this->assertSame('Id', $industry->getKeyName());
        $this->assertFalse($industry->getIncrementing());
        $this->assertSame('int', $industry->getKeyType());
        $this->assertSame('job-1', $industry->jobs->first()?->JobId);
    }

    public function test_noc_code_mapping_and_relationship(): void
    {
        $noc = NocCode::query()->findOrFail(12);

        $this->assertSame('NocCodes', $noc->getTable());
        $this->assertSame('Id', $noc->getKeyName());
        $this->assertFalse($noc->getIncrementing());
        $this->assertSame('int', $noc->getKeyType());
        $this->assertSame('job-1', $noc->jobs->first()?->JobId);
    }

    public function test_noc_code_2021_mapping_and_relationship(): void
    {
        $noc = NocCode2021::query()->findOrFail(62010);

        $this->assertSame('NocCodes2021', $noc->getTable());
        $this->assertSame('Id', $noc->getKeyName());
        $this->assertFalse($noc->getIncrementing());
        $this->assertSame('int', $noc->getKeyType());
        $this->assertSame('job-1', $noc->jobs->first()?->JobId);
    }

    public function test_system_setting_mapping_casts_and_relationship(): void
    {
        $setting = SystemSetting::query()->findOrFail('SiteTitle');

        $this->assertSame('SystemSettings', $setting->getTable());
        $this->assertSame('Name', $setting->getKeyName());
        $this->assertFalse($setting->getIncrementing());
        $this->assertSame('string', $setting->getKeyType());
        $this->assertSame('Job Board', $setting->Value);
        $this->assertInstanceOf(AdminUser::class, $setting->modifiedByAdminUser);
        $this->assertSame('2026-07-24 08:00:00', $setting->DateUpdated?->format('Y-m-d H:i:s'));
    }

    public function test_admin_user_mapping_casts_and_relationship(): void
    {
        $admin = AdminUser::query()->findOrFail(500);

        $this->assertSame('AdminUsers', $admin->getTable());
        $this->assertSame('Id', $admin->getKeyName());
        $this->assertTrue($admin->getIncrementing());
        $this->assertSame('int', $admin->getKeyType());
        $this->assertSame(AdminLevel::Admin, $admin->AdminLevel);
        $this->assertSame('SiteTitle', $admin->systemSettings->first()?->Name);
        $this->assertSame('2026-07-20 12:00:00', $admin->DateAdded?->format('Y-m-d H:i:s'));
    }

    public function test_job_seeker_flags_mapping_casts_and_relationship(): void
    {
        $flags = JobSeekerFlags::query()->findOrFail(900);

        $this->assertSame('JobSeekerFlags', $flags->getTable());
        $this->assertSame('Id', $flags->getKeyName());
        $this->assertTrue($flags->getIncrementing());
        $this->assertSame('int', $flags->getKeyType());
        $this->assertTrue($flags->IsApprentice);
        $this->assertFalse($flags->IsVeteran);
        $this->assertSame('user-1', $flags->jobSeeker?->Id);
    }

    /**
     * The enum casts must mirror the .NET source (WorkBC.Data/Enums/*) exactly —
     * wrong values would mis-label roles/statuses (AdminLevel is a privilege
     * ordering) — and tolerate legacy codes outside the enum without throwing.
     */
    public function test_enum_casts_mirror_dotnet_source_and_tolerate_unknown_codes(): void
    {
        DB::table('AspNetUsers')->insert([
            ['Id' => 'user-pending', 'PasswordHash' => 'h', 'AccountStatus' => 4],
            ['Id' => 'user-dirty', 'PasswordHash' => 'h', 'AccountStatus' => 7], // not in the .NET enum
        ]);
        // Fixture admin 501 already has AdminLevel = 1 (Reporting); add a SuperAdmin (3).
        DB::table('AdminUsers')->insert([
            ['Id' => 502, 'AdminLevel' => 3],
        ]);
        DB::table('SystemSettings')->insert([
            ['Name' => 'HtmlSetting', 'Value' => 'x', 'FieldType' => 5],
            ['Name' => 'TextSetting', 'Value' => 'y', 'FieldType' => 1],
        ]);
        DB::table('JobAlerts')->insert([
            ['Id' => 203, 'AspNetUserId' => 'user-1', 'AlertFrequency' => 5, 'JobSearchFiltersVersion' => 0, 'IsDeleted' => false],
        ]);

        // AccountStatus: real code maps; an out-of-enum legacy code reads as null (no 500).
        $this->assertSame(AccountStatus::Pending, JobSeeker::query()->findOrFail('user-pending')->AccountStatus);
        $this->assertNull(JobSeeker::query()->findOrFail('user-dirty')->AccountStatus);

        // AdminLevel: ascending privilege — Reporting=1, SuperAdmin=3 (must NOT be inverted).
        $this->assertSame(AdminLevel::Reporting, AdminUser::query()->findOrFail(501)->AdminLevel);
        $this->assertSame(AdminLevel::SuperAdmin, AdminUser::query()->findOrFail(502)->AdminLevel);

        // SystemSettingFieldType: the full .NET set (1=SingleLineText … 5=Html).
        $this->assertSame(SystemSettingFieldType::Html, SystemSetting::query()->findOrFail('HtmlSetting')->FieldType);
        $this->assertSame(SystemSettingFieldType::SingleLineText, SystemSetting::query()->findOrFail('TextSetting')->FieldType);

        // AlertFrequency: Never=5 exists.
        $this->assertSame(AlertFrequency::Never, JobAlert::query()->findOrFail(203)->AlertFrequency);
    }

    private function createFixture(): void
    {
        $this->dropFixture();

        Schema::create('JobSources', function (Blueprint $table): void {
            $table->smallInteger('Id')->primary();
            $table->string('Name');
        });

        Schema::create('Regions', function (Blueprint $table): void {
            $table->integer('Id')->primary();
            $table->string('Name');
        });

        Schema::create('Locations', function (Blueprint $table): void {
            $table->integer('LocationId')->primary();
            $table->integer('RegionId')->nullable();
            $table->string('City')->nullable();
            $table->boolean('IsHidden')->default(false);
            $table->boolean('IsDuplicate')->default(false);
        });

        Schema::create('Industries', function (Blueprint $table): void {
            $table->smallInteger('Id')->primary();
            $table->string('Name');
        });

        Schema::create('NocCodes', function (Blueprint $table): void {
            $table->smallInteger('Id')->primary();
            $table->string('Code')->nullable();
        });

        Schema::create('NocCodes2021', function (Blueprint $table): void {
            $table->integer('Id')->primary();
            $table->string('Code')->nullable();
        });

        Schema::create('Jobs', function (Blueprint $table): void {
            $table->string('JobId')->primary();
            $table->smallInteger('JobSourceId');
            $table->integer('LocationId')->nullable();
            $table->smallInteger('IndustryId')->nullable();
            $table->smallInteger('NocCodeId')->nullable();
            $table->integer('NocCodeId2021')->nullable();
            $table->boolean('FullTime')->default(false);
            $table->boolean('PartTime')->default(false);
            $table->boolean('LeadingToFullTime')->default(false);
            $table->boolean('Permanent')->default(false);
            $table->boolean('Temporary')->default(false);
            $table->boolean('Casual')->default(false);
            $table->boolean('Seasonal')->default(false);
            $table->boolean('IsActive')->default(true);
            $table->decimal('Salary', 10, 2)->nullable();
            $table->dateTime('DatePosted')->nullable();
            $table->dateTime('DateCreated')->nullable();
            $table->dateTime('DateModified')->nullable();
            $table->dateTime('ExpireDate')->nullable();
            $table->dateTime('LastUpdated')->nullable();
        });

        Schema::create('AspNetUsers', function (Blueprint $table): void {
            $table->string('Id')->primary();
            $table->string('PasswordHash')->nullable();
            $table->smallInteger('AccountStatus')->default(1);
            $table->boolean('EmailConfirmed')->default(false);
            $table->boolean('PhoneNumberConfirmed')->default(false);
            $table->boolean('TwoFactorEnabled')->default(false);
            $table->boolean('LockoutEnabled')->default(false);
            $table->dateTime('DateRegistered')->nullable();
            $table->dateTime('LastLogon')->nullable();
            $table->dateTime('LastModified')->nullable();
            $table->dateTime('DateLocked')->nullable();
        });

        Schema::create('SavedJobs', function (Blueprint $table): void {
            $table->integer('Id')->primary();
            $table->string('JobId');
            $table->string('AspNetUserId');
            $table->dateTime('DateSaved')->nullable();
            $table->text('Note')->nullable();
            $table->dateTime('NoteUpdatedDate')->nullable();
            $table->boolean('IsDeleted')->default(false);
            $table->dateTime('DateDeleted')->nullable();
        });

        Schema::create('JobAlerts', function (Blueprint $table): void {
            $table->integer('Id')->primary();
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

        Schema::create('AdminUsers', function (Blueprint $table): void {
            $table->integer('Id')->primary();
            $table->smallInteger('AdminLevel')->default(2);
            $table->integer('LockedByAdminUserId')->nullable();
            $table->dateTime('DateLocked')->nullable();
            $table->dateTime('DateAdded')->nullable();
        });

        Schema::create('SystemSettings', function (Blueprint $table): void {
            $table->string('Name')->primary();
            $table->text('Value')->nullable();
            $table->integer('FieldType')->default(0);
            $table->text('DefaultValue')->nullable();
            $table->integer('ModifiedByAdminUserId')->nullable();
            $table->dateTime('DateUpdated')->nullable();
        });

        Schema::create('JobSeekerFlags', function (Blueprint $table): void {
            $table->integer('Id')->primary();
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

        DB::table('JobSources')->insert([
            ['Id' => 1, 'Name' => 'Federal'],
            ['Id' => 2, 'Name' => 'External'],
        ]);

        DB::table('Regions')->insert([
            ['Id' => -1, 'Name' => 'Outside BC'],
            ['Id' => 2, 'Name' => 'Mainland / Southwest'],
        ]);

        DB::table('Locations')->insert([
            ['LocationId' => 10, 'RegionId' => 2, 'City' => 'Vancouver', 'IsHidden' => false, 'IsDuplicate' => false],
            ['LocationId' => 11, 'RegionId' => -1, 'City' => 'Remote', 'IsHidden' => false, 'IsDuplicate' => false],
        ]);

        DB::table('Industries')->insert([
            ['Id' => 21, 'Name' => 'Mining, quarrying, and oil and gas extraction'],
            ['Id' => 23, 'Name' => 'Construction'],
        ]);

        DB::table('NocCodes')->insert([
            ['Id' => 12, 'Code' => '0012'],
            ['Id' => 13, 'Code' => '0013'],
        ]);

        DB::table('NocCodes2021')->insert([
            ['Id' => 62010, 'Code' => '62010'],
            ['Id' => 62020, 'Code' => '62020'],
        ]);

        DB::table('Jobs')->insert([
            [
                'JobId' => 'job-1',
                'JobSourceId' => 1,
                'LocationId' => 10,
                'IndustryId' => 21,
                'NocCodeId' => 12,
                'NocCodeId2021' => 62010,
                'FullTime' => true,
                'PartTime' => false,
                'LeadingToFullTime' => false,
                'Permanent' => true,
                'Temporary' => false,
                'Casual' => false,
                'Seasonal' => false,
                'IsActive' => true,
                'Salary' => 120000.50,
                'DatePosted' => '2026-07-20 08:30:00',
                'DateCreated' => '2026-07-20 08:30:00',
                'DateModified' => '2026-07-20 09:30:00',
                'ExpireDate' => '2026-08-20 10:00:00',
                'LastUpdated' => '2026-07-20 09:30:00',
            ],
            [
                'JobId' => 'job-2',
                'JobSourceId' => 2,
                'LocationId' => 11,
                'IndustryId' => 23,
                'NocCodeId' => 13,
                'NocCodeId2021' => 62020,
                'FullTime' => false,
                'PartTime' => true,
                'LeadingToFullTime' => false,
                'Permanent' => false,
                'Temporary' => true,
                'Casual' => false,
                'Seasonal' => false,
                'IsActive' => true,
                'Salary' => 60000.00,
                'DatePosted' => '2026-07-19 08:30:00',
                'DateCreated' => '2026-07-19 08:30:00',
                'DateModified' => '2026-07-19 09:30:00',
                'ExpireDate' => '2026-08-19 10:00:00',
                'LastUpdated' => '2026-07-19 09:30:00',
            ],
        ]);

        DB::table('AspNetUsers')->insert([
            [
                'Id' => 'user-1',
                'PasswordHash' => 'hash-1',
                'AccountStatus' => 1,
                'EmailConfirmed' => true,
                'PhoneNumberConfirmed' => false,
                'TwoFactorEnabled' => false,
                'LockoutEnabled' => true,
                'DateRegistered' => '2026-07-01 09:00:00',
                'LastLogon' => '2026-07-24 08:00:00',
                'LastModified' => '2026-07-24 08:05:00',
                'DateLocked' => null,
            ],
            [
                'Id' => 'user-2',
                'PasswordHash' => 'hash-2',
                'AccountStatus' => 1,
                'EmailConfirmed' => false,
                'PhoneNumberConfirmed' => false,
                'TwoFactorEnabled' => false,
                'LockoutEnabled' => false,
                'DateRegistered' => '2026-07-10 09:00:00',
                'LastLogon' => null,
                'LastModified' => null,
                'DateLocked' => null,
            ],
        ]);

        DB::table('SavedJobs')->insert([
            [
                'Id' => 101,
                'JobId' => 'job-1',
                'AspNetUserId' => 'user-1',
                'DateSaved' => '2026-07-24 10:15:00',
                'Note' => 'favorite',
                'NoteUpdatedDate' => '2026-07-24 10:16:00',
                'IsDeleted' => false,
                'DateDeleted' => null,
            ],
            [
                'Id' => 102,
                'JobId' => 'job-2',
                'AspNetUserId' => 'user-1',
                'DateSaved' => '2026-07-24 11:15:00',
                'Note' => null,
                'NoteUpdatedDate' => null,
                'IsDeleted' => true,
                'DateDeleted' => '2026-07-24 12:00:00',
            ],
        ]);

        DB::table('JobAlerts')->insert([
            [
                'Id' => 201,
                'AspNetUserId' => 'user-1',
                'Title' => 'Daily alert',
                'AlertFrequency' => 1,
                'JobSearchFilters' => '{"Page":2,"SearchIsPostingsInEnglish":true}',
                'JobSearchFiltersVersion' => 0,
                'UrlParameters' => 'Page=2',
                'IsDeleted' => false,
                'DateCreated' => '2026-07-24 09:00:00',
                'DateModified' => '2026-07-24 09:05:00',
                'DateDeleted' => null,
            ],
            [
                'Id' => 202,
                'AspNetUserId' => 'user-2',
                'Title' => 'Weekly alert',
                'AlertFrequency' => 2,
                'JobSearchFilters' => '{"Page":1}',
                'JobSearchFiltersVersion' => 0,
                'UrlParameters' => 'Page=1',
                'IsDeleted' => true,
                'DateCreated' => '2026-07-23 09:00:00',
                'DateModified' => null,
                'DateDeleted' => '2026-07-23 09:30:00',
            ],
        ]);

        DB::table('AdminUsers')->insert([
            ['Id' => 500, 'AdminLevel' => 2, 'LockedByAdminUserId' => null, 'DateLocked' => null, 'DateAdded' => '2026-07-20 12:00:00'],
            ['Id' => 501, 'AdminLevel' => 1, 'LockedByAdminUserId' => 500, 'DateLocked' => '2026-07-21 12:00:00', 'DateAdded' => '2026-07-19 12:00:00'],
        ]);

        DB::table('SystemSettings')->insert([
            [
                'Name' => 'SiteTitle',
                'Value' => 'Job Board',
                'FieldType' => 0,
                'DefaultValue' => 'WorkBC Job Board',
                'ModifiedByAdminUserId' => 500,
                'DateUpdated' => '2026-07-24 08:00:00',
            ],
            [
                'Name' => 'MaxSavedJobs',
                'Value' => '100',
                'FieldType' => 1,
                'DefaultValue' => '50',
                'ModifiedByAdminUserId' => 501,
                'DateUpdated' => '2026-07-24 08:10:00',
            ],
        ]);

        DB::table('JobSeekerFlags')->insert([
            [
                'Id' => 900,
                'AspNetUserId' => 'user-1',
                'IsApprentice' => true,
                'IsIndigenousPerson' => false,
                'IsMatureWorker' => false,
                'IsNewImmigrant' => true,
                'IsPersonWithDisability' => false,
                'IsStudent' => false,
                'IsVeteran' => false,
                'IsVisibleMinority' => true,
                'IsYouth' => false,
            ],
            [
                'Id' => 901,
                'AspNetUserId' => 'user-2',
                'IsApprentice' => false,
                'IsIndigenousPerson' => true,
                'IsMatureWorker' => false,
                'IsNewImmigrant' => false,
                'IsPersonWithDisability' => false,
                'IsStudent' => true,
                'IsVeteran' => false,
                'IsVisibleMinority' => false,
                'IsYouth' => true,
            ],
        ]);
    }

    private function dropFixture(): void
    {
        Schema::dropIfExists('JobSeekerFlags');
        Schema::dropIfExists('SystemSettings');
        Schema::dropIfExists('AdminUsers');
        Schema::dropIfExists('JobAlerts');
        Schema::dropIfExists('SavedJobs');
        Schema::dropIfExists('AspNetUsers');
        Schema::dropIfExists('Jobs');
        Schema::dropIfExists('NocCodes2021');
        Schema::dropIfExists('NocCodes');
        Schema::dropIfExists('Industries');
        Schema::dropIfExists('Locations');
        Schema::dropIfExists('Regions');
        Schema::dropIfExists('JobSources');
    }
}
