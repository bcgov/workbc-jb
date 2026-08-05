<?php

namespace Tests\Feature\Settings;

use App\Services\Settings\SystemSettingsService;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;
use Tests\TestCase;

class SystemSettingsServiceTest extends TestCase
{
    protected function setUp(): void
    {
        parent::setUp();
        $this->createFixture();
        app(SystemSettingsService::class)->invalidateCache();
    }

    protected function tearDown(): void
    {
        app(SystemSettingsService::class)->invalidateCache();
        $this->dropFixture();
        parent::tearDown();
    }

    public function test_get_many_reads_requested_settings_in_one_query_and_returns_missing_as_null(): void
    {
        DB::table('SystemSettings')->insert([
            ['Name' => 'one', 'Value' => 'first'],
            ['Name' => 'two', 'Value' => 'second'],
        ]);

        DB::enableQueryLog();

        $values = app(SystemSettingsService::class)->getMany(['one', 'two', 'missing']);

        $queries = DB::getQueryLog();

        $this->assertSame('first', $values['one']);
        $this->assertSame('second', $values['two']);
        $this->assertNull($values['missing']);
        $this->assertCount(1, $queries, 'Expected a single SystemSettings query for bulk read.');
    }

    public function test_get_uses_default_for_missing_value(): void
    {
        $value = app(SystemSettingsService::class)->get('does.not.exist', 'fallback');

        $this->assertSame('fallback', $value);
    }

    public function test_values_are_cached_until_explicit_invalidation(): void
    {
        DB::table('SystemSettings')->insert([
            'Name' => 'introText',
            'Value' => 'Initial value',
        ]);

        $service = app(SystemSettingsService::class);

        $this->assertSame('Initial value', $service->get('introText'));

        DB::table('SystemSettings')
            ->where('Name', 'introText')
            ->update(['Value' => 'Updated value']);

        $this->assertSame('Initial value', $service->get('introText'));

        $service->invalidateCache();

        $this->assertSame('Updated value', $service->get('introText'));
    }

    private function createFixture(): void
    {
        $this->dropFixture();

        Schema::create('SystemSettings', function (Blueprint $table): void {
            $table->string('Name')->primary();
            $table->text('Value')->nullable();
            $table->integer('FieldType')->default(0);
            $table->text('DefaultValue')->nullable();
            $table->integer('ModifiedByAdminUserId')->nullable();
            $table->dateTime('DateUpdated')->nullable();
        });
    }

    private function dropFixture(): void
    {
        Schema::dropIfExists('SystemSettings');
    }
}
