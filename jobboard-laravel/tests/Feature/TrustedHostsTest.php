<?php

namespace Tests\Feature;

use Tests\TestCase;

/**
 * FND-8 app-hardening: host restrictions are env-driven and inert by default.
 */
class TrustedHostsTest extends TestCase
{
    private ?string $originalTrustedHostPatterns = null;

    private ?string $originalAppEnv = null;

    protected function setUp(): void
    {
        parent::setUp();

        $current = getenv('TRUSTED_HOST_PATTERNS');
        $this->originalTrustedHostPatterns = $current === false ? null : $current;

        $appEnv = getenv('APP_ENV');
        $this->originalAppEnv = $appEnv === false ? null : $appEnv;
    }

    protected function tearDown(): void
    {
        $this->setTrustedHostPatterns($this->originalTrustedHostPatterns);
        $this->setAppEnv($this->originalAppEnv);

        parent::tearDown();
    }

    public function test_forged_forwarded_host_is_rejected_when_restriction_is_enabled(): void
    {
        $this->setAppEnv('production');
        $this->setTrustedHostPatterns('^(.+\.)?workbc\.ca$');

        $this->get('/sitemap.xml', [
            'X-Forwarded-Host' => 'attacker.example',
            'X-Forwarded-Proto' => 'https',
        ])->assertStatus(400);
    }

    public function test_workbc_host_is_accepted_when_restriction_is_enabled(): void
    {
        $this->setAppEnv('production');
        $this->setTrustedHostPatterns('^(.+\.)?workbc\.ca$');

        $this->get('/sitemap.xml', [
            'X-Forwarded-Host' => 'api-jobboard.workbc.ca',
            'X-Forwarded-Proto' => 'https',
        ])->assertOk()
            ->assertSee('https://api-jobboard.workbc.ca/sitemap-en.xml', escape: false);
    }

    public function test_restriction_is_inert_by_default_when_patterns_are_empty(): void
    {
        $this->setAppEnv('production');
        $this->setTrustedHostPatterns('');

        $this->get('/sitemap.xml', [
            'X-Forwarded-Host' => 'attacker.example',
            'X-Forwarded-Proto' => 'https',
        ])->assertOk()
            ->assertSee('https://attacker.example/sitemap-en.xml', escape: false);
    }

    private function setTrustedHostPatterns(?string $value): void
    {
        if ($value === null) {
            putenv('TRUSTED_HOST_PATTERNS');
            unset($_ENV['TRUSTED_HOST_PATTERNS'], $_SERVER['TRUSTED_HOST_PATTERNS']);
        } else {
            putenv('TRUSTED_HOST_PATTERNS='.$value);
            $_ENV['TRUSTED_HOST_PATTERNS'] = $value;
            $_SERVER['TRUSTED_HOST_PATTERNS'] = $value;
        }

        $this->refreshApplication();
    }

    private function setAppEnv(?string $value): void
    {
        if ($value === null) {
            putenv('APP_ENV');
            unset($_ENV['APP_ENV'], $_SERVER['APP_ENV']);

            $this->refreshApplication();

            return;
        }

        putenv('APP_ENV='.$value);
        $_ENV['APP_ENV'] = $value;
        $_SERVER['APP_ENV'] = $value;

        $this->refreshApplication();
    }
}
