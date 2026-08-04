<?php

namespace Tests\Feature\Account;

use App\Livewire\SavedProfilesPage;
use App\Models\JobSeeker;
use App\Models\SavedCareerProfile;
use App\Models\SavedIndustryProfile;
use App\Services\JobSeeker\SavedProfileService;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Carbon;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;
use Livewire\Livewire;
use Tests\TestCase;

/**
 * ACCT-6 — saved career (NOC) and industry profiles: the owning service, the
 * browser-called save/status endpoints (ADR-009 session auth), and the account
 * listing page.
 */
class SavedProfilesTest extends TestCase
{
    protected function setUp(): void
    {
        parent::setUp();
        $this->createFixture();
    }

    protected function tearDown(): void
    {
        Carbon::setTestNow();
        $this->dropFixture();
        parent::tearDown();
    }

    // --- Service semantics ---------------------------------------------------

    public function test_saving_is_insert_if_absent_and_never_creates_a_second_active_row(): void
    {
        $user = $this->createUser('user-a');
        $service = app(SavedProfileService::class);

        Carbon::setTestNow('2026-07-30 09:00:00');
        $first = $service->saveCareerProfile($user, 21001);

        Carbon::setTestNow('2026-07-30 10:00:00');
        $second = $service->saveCareerProfile($user, 21001);

        $this->assertSame($first->Id, $second->Id, 'a repeat save must reuse the existing row');
        $this->assertSame(1, SavedCareerProfile::withTrashed()->where('AspNetUserId', 'user-a')->count());
        $this->assertTrue($service->hasCareerProfile($user, 21001));
    }

    public function test_removing_soft_deletes_and_a_later_save_restores_the_same_row(): void
    {
        $user = $this->createUser('user-a');
        $service = app(SavedProfileService::class);

        $created = $service->saveCareerProfile($user, 21001);
        $this->assertTrue($service->removeCareerProfile($user, 21001));

        // Soft-deleted, not gone: the row survives with the legacy flag pair set.
        $this->assertFalse($service->hasCareerProfile($user, 21001));
        $trashed = SavedCareerProfile::withTrashed()->findOrFail($created->Id);
        $this->assertTrue((bool) $trashed->IsDeleted);
        $this->assertNotNull($trashed->DateDeleted);

        $restored = $service->saveCareerProfile($user, 21001);
        $this->assertSame($created->Id, $restored->Id);
        $this->assertSame(1, SavedCareerProfile::withTrashed()->where('AspNetUserId', 'user-a')->count());
    }

    public function test_removing_a_profile_that_was_never_saved_reports_false(): void
    {
        $service = app(SavedProfileService::class);

        $this->assertFalse($service->removeCareerProfile($this->createUser('user-a'), 99999));
    }

    public function test_profiles_are_scoped_to_their_owner(): void
    {
        $a = $this->createUser('user-a');
        $b = $this->createUser('user-b');
        $service = app(SavedProfileService::class);

        $service->saveCareerProfile($a, 21001);
        $service->saveIndustryProfile($a, 23);

        $this->assertFalse($service->hasCareerProfile($b, 21001));
        $this->assertFalse($service->hasIndustryProfile($b, 23));
        $this->assertCount(0, $service->careerProfilesFor($b));
        $this->assertCount(0, $service->industryProfilesFor($b));
    }

    public function test_listing_joins_noc_titles_and_the_bc_industry_title(): void
    {
        $user = $this->createUser('user-a');
        $service = app(SavedProfileService::class);

        $service->saveCareerProfile($user, 21001);
        $service->saveIndustryProfile($user, 23);

        $career = $service->careerProfilesFor($user)->first();
        $this->assertSame('Financial auditors and accountants', $career['title']);
        $this->assertSame('11100', $career['code']);

        // The legacy list uses Industries.TitleBC, not Title.
        $industry = $service->industryProfilesFor($user)->first();
        $this->assertSame('Construction (B.C.)', $industry['title']);
    }

    public function test_soft_deleted_rows_are_excluded_from_listings(): void
    {
        $user = $this->createUser('user-a');
        $service = app(SavedProfileService::class);

        $service->saveIndustryProfile($user, 23);
        $service->removeIndustryProfile($user, 23);

        $this->assertCount(0, $service->industryProfilesFor($user));
        $this->assertSame(1, SavedIndustryProfile::withTrashed()->where('AspNetUserId', 'user-a')->count());
    }

    // --- Browser-called endpoints (ADR-009) ----------------------------------

    public function test_endpoints_reject_anonymous_callers_with_401_not_a_redirect(): void
    {
        // Drupal's JS sends Accept: */*, so `auth:web` would 302 to the login page
        // and the caller would read login HTML as success. The contract is 401.
        foreach ([
            ['get', '/api/career-profiles/status/21001'],
            ['post', '/api/career-profiles/save/21001'],
            ['get', '/api/industry-profiles/status/23'],
            ['post', '/api/industry-profiles/save/23'],
        ] as [$method, $uri]) {
            $this->withHeaders(['Accept' => '*/*'])->{$method}($uri)
                ->assertStatus(401)
                ->assertJson(['message' => 'Unauthenticated.']);
        }
    }

    public function test_status_reports_saved_state_and_returns_a_csrf_token(): void
    {
        $user = $this->createUser('user-a');
        Auth::guard('web')->login($user);

        $this->getJson('/api/career-profiles/status/21001')
            ->assertOk()
            ->assertJson(['saved' => false])
            ->assertJsonStructure(['saved', 'csrf']);

        $this->postJson('/api/career-profiles/save/21001')->assertOk()->assertJson(['saved' => true]);

        $response = $this->getJson('/api/career-profiles/status/21001')->assertOk()->assertJson(['saved' => true]);

        // The token must be in the body: Drupal's JS is cross-origin and cannot
        // read our XSRF-TOKEN cookie, so the double-submit pattern can't work.
        $this->assertNotEmpty($response->json('csrf'));
    }

    public function test_industry_endpoints_work_the_same_way(): void
    {
        $user = $this->createUser('user-a');
        Auth::guard('web')->login($user);

        $this->getJson('/api/industry-profiles/status/23')->assertOk()->assertJson(['saved' => false]);
        $this->postJson('/api/industry-profiles/save/23')->assertOk()->assertJson(['saved' => true]);
        $this->getJson('/api/industry-profiles/status/23')->assertOk()->assertJson(['saved' => true]);

        $this->deleteJson('/api/industry-profiles/23')->assertOk()->assertJson(['saved' => false]);
        $this->getJson('/api/industry-profiles/status/23')->assertOk()->assertJson(['saved' => false]);
    }

    public function test_one_users_endpoint_calls_never_affect_another(): void
    {
        $a = $this->createUser('user-a');
        $b = $this->createUser('user-b');

        Auth::guard('web')->login($a);
        $this->postJson('/api/career-profiles/save/21001')->assertOk();

        Auth::guard('web')->login($b);
        $this->getJson('/api/career-profiles/status/21001')->assertOk()->assertJson(['saved' => false]);
    }

    // --- Account page --------------------------------------------------------

    public function test_the_page_lists_both_profile_types_and_removes_them(): void
    {
        $user = $this->createUser('user-a');
        Auth::guard('web')->login($user);

        $service = app(SavedProfileService::class);
        $service->saveCareerProfile($user, 21001);
        $service->saveIndustryProfile($user, 23);

        Livewire::test(SavedProfilesPage::class)
            ->assertSee('Financial auditors and accountants')
            ->assertSee('Construction (B.C.)')
            ->call('removeCareerProfile', 21001)
            ->assertDontSee('Financial auditors and accountants')
            ->assertSee('Construction (B.C.)');

        $this->assertFalse($service->hasCareerProfile($user, 21001));
        $this->assertTrue($service->hasIndustryProfile($user, 23));
    }

    public function test_the_page_shows_empty_states(): void
    {
        Auth::guard('web')->login($this->createUser('user-a'));

        Livewire::test(SavedProfilesPage::class)
            ->assertSee('No saved career profiles yet')
            ->assertSee('No saved industry profiles yet');
    }

    /** The ACCT-1 dashboard links here; it used to 404. */
    public function test_the_account_profiles_route_resolves(): void
    {
        Auth::guard('web')->login($this->createUser('user-a'));

        $this->get('/account/profiles')->assertOk()->assertSee('Saved profiles');
    }

    // --- Fixture -------------------------------------------------------------

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

        Schema::create('SavedCareerProfiles', function (Blueprint $table): void {
            $table->increments('Id');
            $table->integer('EDM_CareerProfile_CareerProfileId')->nullable();
            $table->string('AspNetUserId', 450)->nullable();
            $table->dateTime('DateSaved');
            $table->dateTime('DateDeleted')->nullable();
            $table->boolean('IsDeleted')->default(false);
            $table->integer('NocCodeId2021')->nullable();
        });

        Schema::create('SavedIndustryProfiles', function (Blueprint $table): void {
            $table->increments('Id');
            $table->string('AspNetUserId', 450)->nullable();
            $table->dateTime('DateSaved');
            $table->dateTime('DateDeleted')->nullable();
            $table->boolean('IsDeleted')->default(false);
            $table->smallInteger('IndustryId')->nullable();
        });

        DB::table('NocCodes2021')->insert([
            'Id' => 21001, 'Code' => '11100', 'Title' => 'Financial auditors and accountants',
        ]);

        // Title vs TitleBC deliberately differ so the join can be asserted.
        DB::table('Industries')->insert([
            'Id' => 23, 'Title' => 'Construction', 'TitleBC' => 'Construction (B.C.)',
        ]);
    }

    private function dropFixture(): void
    {
        Schema::dropIfExists('SavedIndustryProfiles');
        Schema::dropIfExists('SavedCareerProfiles');
        Schema::dropIfExists('Industries');
        Schema::dropIfExists('NocCodes2021');
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
