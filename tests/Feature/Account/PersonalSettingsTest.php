<?php

namespace Tests\Feature\Account;

use App\Livewire\PersonalSettingsPage;
use App\Models\Enums\AccountStatus;
use App\Models\JobSeeker;
use App\Notifications\JobSeekerEmailVerificationNotification;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Hash;
use Illuminate\Support\Facades\Notification;
use Illuminate\Support\Facades\Schema;
use Livewire\Livewire;
use Tests\TestCase;

class PersonalSettingsTest extends TestCase
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

    public function test_settings_page_route_resolves_for_authenticated_users(): void
    {
        $user = $this->createUser('user-a', 'user-a@example.com', 'Current!123');
        Auth::guard('web')->login($user);

        $this->get('/account/settings')->assertOk()->assertSee('Personal settings');
    }

    public function test_profile_update_writes_one_summary_audit_row_with_labels(): void
    {
        $user = $this->createUser('user-a', 'user-a@example.com', 'Current!123', [
            'FirstName' => 'Alex',
            'LastName' => 'Lee',
            'CountryId' => 1,
            'ProvinceId' => 2,
            'LocationId' => 10,
            'City' => 'Vancouver',
        ]);

        Auth::guard('web')->login($user);

        Livewire::test(PersonalSettingsPage::class)
            ->set('firstName', 'Alicia')
            ->set('lastName', 'Lin')
            ->set('countryId', 2)
            ->set('provinceId', 2)
            ->set('locationId', 11)
            ->call('saveProfile')
            ->assertHasNoErrors()
            ->assertSet('profileStatus', 'Profile updated.');

        $updated = JobSeeker::query()->findOrFail('user-a');
        $this->assertSame('Alicia', (string) $updated->FirstName);
        $this->assertSame('Lin', (string) $updated->LastName);
        $this->assertSame(2, (int) $updated->CountryId);
        $this->assertSame(2, (int) $updated->ProvinceId);
        $this->assertSame(11, (int) $updated->LocationId);
        $this->assertSame('Victoria', (string) $updated->City);

        $rows = DB::table('JobSeekerChangeLog')->where('AspNetUserId', 'user-a')->orderBy('Id')->get();
        $this->assertCount(1, $rows);

        $row = $rows->first();
        $this->assertNotNull($row);
        $this->assertNull($row->ModifiedByAdminUserId);
        $this->assertStringContainsString('First name edited', (string) $row->Field);
        $this->assertStringContainsString('Last name edited', (string) $row->Field);
        $this->assertStringContainsString('Country edited', (string) $row->Field);
        $this->assertStringNotContainsString('Province edited', (string) $row->Field);
        $this->assertStringContainsString('City edited', (string) $row->Field);
        $this->assertStringContainsString('Country: Canada', (string) $row->OldValue);
        $this->assertStringContainsString('Country: United States', (string) $row->NewValue);
        $this->assertStringContainsString('City: Vancouver, BC', (string) $row->OldValue);
        $this->assertStringContainsString('City: Victoria, BC', (string) $row->NewValue);
    }

    public function test_non_bc_province_clears_location_and_city_server_side(): void
    {
        $user = $this->createUser('user-a', 'user-a@example.com', 'Current!123', [
            'ProvinceId' => 2,
            'LocationId' => 10,
            'City' => 'Vancouver',
        ]);

        Auth::guard('web')->login($user);

        Livewire::test(PersonalSettingsPage::class)
            ->set('provinceId', 3)
            ->set('locationId', 11)
            ->call('saveProfile')
            ->assertHasNoErrors();

        $updated = JobSeeker::query()->findOrFail('user-a');
        $this->assertSame(3, (int) $updated->ProvinceId);
        $this->assertNull($updated->LocationId);
        $this->assertNull($updated->City);
    }

    public function test_email_change_blocks_duplicates_and_requires_reverification(): void
    {
        Notification::fake();

        $user = $this->createUser('user-a', 'user-a@example.com', 'Current!123', [
            'EmailConfirmed' => true,
            'VerificationGuid' => '11111111-1111-1111-1111-111111111111',
        ]);
        $this->createUser('user-b', 'taken@example.com', 'Current!123');

        Auth::guard('web')->login($user);

        Livewire::test(PersonalSettingsPage::class)
            ->set('newEmail', 'taken@example.com')
            ->call('saveEmail')
            ->assertHasErrors(['newEmail']);

        $unchanged = JobSeeker::query()->findOrFail('user-a');
        $this->assertSame('user-a@example.com', (string) $unchanged->Email);

        Livewire::test(PersonalSettingsPage::class)
            ->set('newEmail', 'new-address@example.com')
            ->call('saveEmail')
            ->assertHasNoErrors()
            ->assertSet('emailStatus', 'Email updated. Please check your new inbox to verify this address.');

        $updated = JobSeeker::query()->findOrFail('user-a');
        $this->assertSame('new-address@example.com', (string) $updated->Email);
        $this->assertSame('new-address@example.com', (string) $updated->UserName);
        $this->assertSame('NEW-ADDRESS@EXAMPLE.COM', (string) $updated->NormalizedEmail);
        $this->assertSame('NEW-ADDRESS@EXAMPLE.COM', (string) $updated->NormalizedUserName);
        $this->assertFalse((bool) $updated->EmailConfirmed);
        $this->assertNotSame('11111111-1111-1111-1111-111111111111', (string) $updated->VerificationGuid);

        Notification::assertSentTo($updated, JobSeekerEmailVerificationNotification::class);

        $row = DB::table('JobSeekerChangeLog')->where('AspNetUserId', 'user-a')->orderByDesc('Id')->first();
        $this->assertNotNull($row);
        $this->assertStringContainsString('Email edited', (string) $row->Field);
    }

    public function test_password_change_requires_current_password_and_writes_safe_audit_row(): void
    {
        $user = $this->createUser('user-a', 'user-a@example.com', 'Current!123');
        Auth::guard('web')->login($user);

        Livewire::test(PersonalSettingsPage::class)
            ->set('currentPassword', 'Wrong!123')
            ->set('newPassword', 'NewPassword!456')
            ->set('newPasswordConfirmation', 'NewPassword!456')
            ->call('savePassword')
            ->assertHasErrors(['currentPassword']);

        $before = JobSeeker::query()->findOrFail('user-a');
        $oldStamp = (string) $before->SecurityStamp;

        Livewire::test(PersonalSettingsPage::class)
            ->set('currentPassword', 'Current!123')
            ->set('newPassword', 'NewPassword!456')
            ->set('newPasswordConfirmation', 'NewPassword!456')
            ->call('savePassword')
            ->assertHasNoErrors()
            ->assertSet('passwordStatus', 'Password changed.');

        $updated = JobSeeker::query()->findOrFail('user-a');
        $this->assertTrue(Hash::check('NewPassword!456', (string) $updated->PasswordHash));
        $this->assertNotSame($oldStamp, (string) $updated->SecurityStamp);

        $row = DB::table('JobSeekerChangeLog')->where('AspNetUserId', 'user-a')->orderByDesc('Id')->first();
        $this->assertNotNull($row);
        $this->assertSame('Password changed', (string) $row->Field);
        $this->assertSame('-', (string) $row->OldValue);
        $this->assertSame('-', (string) $row->NewValue);
        $this->assertStringNotContainsString('Current!123', (string) $row->Field.(string) $row->OldValue.(string) $row->NewValue);
        $this->assertStringNotContainsString('NewPassword!456', (string) $row->Field.(string) $row->OldValue.(string) $row->NewValue);
    }

    public function test_profile_validation_enforces_max_length_rules(): void
    {
        $user = $this->createUser('user-a', 'user-a@example.com', 'Current!123');
        Auth::guard('web')->login($user);

        Livewire::test(PersonalSettingsPage::class)
            ->set('firstName', str_repeat('A', 51))
            ->set('lastName', str_repeat('B', 51))
            ->call('saveProfile')
            ->assertHasErrors(['firstName' => 'max', 'lastName' => 'max']);
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
            $table->boolean('EmailConfirmed')->default(true);
            $table->string('PasswordHash')->nullable();
            $table->string('SecurityStamp')->nullable();
            $table->smallInteger('AccountStatus')->default(AccountStatus::Active->value);
            $table->string('FirstName', 50)->nullable();
            $table->string('LastName', 50)->nullable();
            $table->string('City', 50)->nullable();
            $table->integer('LocationId')->nullable();
            $table->integer('CountryId')->nullable();
            $table->integer('ProvinceId')->nullable();
            $table->uuid('VerificationGuid')->nullable();
            $table->dateTime('LastModified')->nullable();
        });

        Schema::create('Countries', function (Blueprint $table): void {
            $table->integer('Id')->primary();
            $table->string('Name', 100);
        });

        Schema::create('Provinces', function (Blueprint $table): void {
            // Mirrors the real schema: the PK is `ProvinceId`, NOT `Id` — unlike
            // Countries, which does use `Id`. A fixture with `Id` here agrees with
            // buggy code and hides a 500 in production (constraint #9).
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
            ['Id' => 2, 'Name' => 'United States'],
        ]);

        DB::table('Provinces')->insert([
            // ProvinceId 2 = British Columbia in the real data — the value the
            // "province is not B.C. clears city/location" rule keys on.
            ['ProvinceId' => 2, 'Name' => 'British Columbia'],
            ['ProvinceId' => 3, 'Name' => 'Alberta'],
        ]);

        DB::table('Locations')->insert([
            ['LocationId' => 10, 'City' => 'Vancouver', 'Label' => 'Vancouver, BC', 'IsHidden' => false, 'IsDuplicate' => false],
            ['LocationId' => 11, 'City' => 'Victoria', 'Label' => 'Victoria, BC', 'IsHidden' => false, 'IsDuplicate' => false],
        ]);
    }

    private function dropFixture(): void
    {
        Schema::dropIfExists('JobSeekerChangeLog');
        Schema::dropIfExists('Locations');
        Schema::dropIfExists('Provinces');
        Schema::dropIfExists('Countries');
        Schema::dropIfExists('AspNetUsers');
    }

    /**
     * @param  array<string, mixed>  $overrides
     */
    private function createUser(string $id, string $email, string $password, array $overrides = []): JobSeeker
    {
        DB::table('AspNetUsers')->insert(array_merge([
            'Id' => $id,
            'UserName' => $email,
            'NormalizedUserName' => mb_strtoupper($email, 'UTF-8'),
            'Email' => $email,
            'NormalizedEmail' => mb_strtoupper($email, 'UTF-8'),
            'EmailConfirmed' => true,
            'PasswordHash' => Hash::make($password),
            'SecurityStamp' => 'stamp-'.$id,
            'AccountStatus' => AccountStatus::Active->value,
            'FirstName' => 'Alex',
            'LastName' => 'Lee',
            'City' => null,
            'LocationId' => null,
            'CountryId' => 1,
            'ProvinceId' => 2,
            'VerificationGuid' => null,
            'LastModified' => now(),
        ], $overrides));

        return JobSeeker::query()->findOrFail($id);
    }
}
