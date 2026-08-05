<?php

namespace Tests\Feature\Account;

use App\Models\JobSeeker;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;
use Tests\TestCase;

class AccountNavigationTest extends TestCase
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

    public function test_persistent_navigation_renders_on_every_account_page(): void
    {
        $user = $this->createUser('user-a', 'Pat');
        Auth::guard('web')->login($user);

        foreach (['/account', '/account/saved-jobs', '/account/recommended', '/account/alerts', '/account/profiles', '/account/settings'] as $path) {
            $response = $this->get($path);

            $response->assertOk()
                ->assertSee('Hello, Pat')
                ->assertSee('Account profile')
                ->assertSee('Jobs')
                ->assertSee('Recommended jobs')
                ->assertSee('Careers &amp; industries', false)
                ->assertSee('Manage account');
        }
    }

    public function test_account_navigation_links_to_live_settings_page(): void
    {
        $user = $this->createUser('user-a', 'Pat');
        Auth::guard('web')->login($user);

        $response = $this->get('/account');

        $response->assertOk()
            ->assertSee('href="'.route('account.dashboard').'"', false)
            ->assertSee('href="'.route('account.saved-jobs').'"', false)
            ->assertSee('href="'.route('account.recommended').'"', false)
            ->assertSee('href="'.route('account.alerts').'"', false)
            ->assertSee('href="'.route('account.profiles').'"', false)
            ->assertSee('href="'.route('account.settings').'"', false)
            ->assertSee('Personal settings');

        foreach ([route('account.dashboard'), route('account.saved-jobs'), route('account.recommended'), route('account.alerts'), route('account.profiles'), route('account.settings')] as $link) {
            $this->get($link)->assertOk();
        }
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
            $table->dateTime('DateSaved')->nullable();
            $table->text('Note')->nullable();
            $table->dateTime('NoteUpdatedDate')->nullable();
            $table->boolean('IsDeleted')->default(false);
            $table->dateTime('DateDeleted')->nullable();
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

        Schema::create('JobAlerts', function (Blueprint $table): void {
            $table->id('Id');
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

        Schema::create('SavedCareerProfiles', function (Blueprint $table): void {
            $table->id('Id');
            $table->string('AspNetUserId');
            $table->integer('NocCodeId2021')->nullable();
            $table->dateTime('DateSaved')->nullable();
            $table->boolean('IsDeleted')->default(false);
            $table->dateTime('DateDeleted')->nullable();
        });

        Schema::create('SavedIndustryProfiles', function (Blueprint $table): void {
            $table->id('Id');
            $table->string('AspNetUserId');
            $table->smallInteger('IndustryId')->nullable();
            $table->dateTime('DateSaved')->nullable();
            $table->boolean('IsDeleted')->default(false);
            $table->dateTime('DateDeleted')->nullable();
        });

        Schema::create('NocCodes2021', function (Blueprint $table): void {
            $table->integer('Id')->primary();
            $table->string('Code', 5)->nullable();
            $table->string('Title', 150)->nullable();
        });

        Schema::create('Industries', function (Blueprint $table): void {
            $table->smallInteger('Id')->primary();
            $table->string('Title', 150)->nullable();
            $table->string('TitleBC', 150)->nullable();
        });

        Schema::create('Jobs', function (Blueprint $table): void {
            $table->string('JobId', 255)->primary();
            $table->integer('NocCodeId2021')->nullable();
            $table->string('Title')->nullable();
            $table->string('EmployerName')->nullable();
            $table->string('City')->nullable();
            $table->dateTime('ExpireDate')->nullable();
        });

        Schema::create('Countries', function (Blueprint $table): void {
            $table->integer('Id')->primary();
            $table->string('Name', 100);
        });

        Schema::create('Provinces', function (Blueprint $table): void {
            // Real schema keys on ProvinceId, not Id (unlike Countries).
            $table->integer('ProvinceId')->primary();
            $table->string('Name', 100);
        });

        Schema::create('Locations', function (Blueprint $table): void {
            $table->integer('LocationId')->primary();
            $table->string('City', 50)->nullable();
            $table->string('Label', 100)->nullable();
            $table->boolean('IsHidden')->default(false);
            $table->boolean('IsDuplicate')->default(false);
        });

        Schema::create('JobSeekerChangeLog', function (Blueprint $table): void {
            $table->increments('Id');
            $table->string('AspNetUserId')->nullable();
            $table->integer('ModifiedByAdminUserId')->nullable();
            $table->dateTime('DateUpdated');
            $table->text('Field')->nullable();
            $table->text('OldValue')->nullable();
            $table->text('NewValue')->nullable();
        });

        DB::table('Countries')->insert([
            ['Id' => 1, 'Name' => 'Canada'],
        ]);

        DB::table('Provinces')->insert([
            ['ProvinceId' => 2, 'Name' => 'British Columbia'],
        ]);

        DB::table('Locations')->insert([
            ['LocationId' => 10, 'City' => 'Vancouver', 'Label' => 'Vancouver, BC', 'IsHidden' => false, 'IsDuplicate' => false],
        ]);
    }

    private function dropFixture(): void
    {
        Schema::dropIfExists('JobSeekerChangeLog');
        Schema::dropIfExists('Locations');
        Schema::dropIfExists('Provinces');
        Schema::dropIfExists('Countries');
        Schema::dropIfExists('Jobs');
        Schema::dropIfExists('Industries');
        Schema::dropIfExists('NocCodes2021');
        Schema::dropIfExists('SavedIndustryProfiles');
        Schema::dropIfExists('SavedCareerProfiles');
        Schema::dropIfExists('JobSeekerFlags');
        Schema::dropIfExists('JobAlerts');
        Schema::dropIfExists('SavedJobs');
        Schema::dropIfExists('SystemSettings');
        Schema::dropIfExists('AspNetUsers');
    }

    private function createUser(string $id, string $firstName): JobSeeker
    {
        DB::table('AspNetUsers')->insert([
            'Id' => $id,
            'UserName' => $id.'@example.com',
            'FirstName' => $firstName,
            'NormalizedUserName' => mb_strtoupper($id.'@example.com', 'UTF-8'),
            'Email' => $id.'@example.com',
            'NormalizedEmail' => mb_strtoupper($id.'@example.com', 'UTF-8'),
            'PasswordHash' => 'unused-hash',
            'SecurityStamp' => 'stamp-'.$id,
            'EmailConfirmed' => true,
        ]);

        DB::table('JobSeekerFlags')->insert([
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
        ]);

        return JobSeeker::query()->findOrFail($id);
    }
}
