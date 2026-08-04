<?php

namespace Tests\Feature;

use Tests\TestCase;

class DeployVerifyCommandTest extends TestCase
{
    private ?string $originalAppEnv = null;

    private bool $devPreviewOriginallyExisted = false;

    private string $devPreviewPath = '';

    private ?string $devPreviewOriginalContent = null;

    protected function setUp(): void
    {
        parent::setUp();

        $current = getenv('APP_ENV');
        $this->originalAppEnv = $current === false ? null : $current;

        $this->devPreviewPath = base_path('routes/dev-preview.php');
        $this->devPreviewOriginallyExisted = file_exists($this->devPreviewPath);
        $this->devPreviewOriginalContent = $this->devPreviewOriginallyExisted
            ? file_get_contents($this->devPreviewPath) ?: ''
            : null;
    }

    protected function tearDown(): void
    {
        $this->restoreDevPreviewFile();
        $this->setAppEnv($this->originalAppEnv);

        parent::tearDown();
    }

    public function test_deploy_verify_exits_non_zero_when_app_env_is_local(): void
    {
        $this->ensureDevPreviewAbsent();
        $this->setAppEnv('local');

        $this->artisan('deploy:verify')
            ->expectsOutputToContain('APP_ENV must not be local for deploy targets.')
            ->assertExitCode(1);
    }

    public function test_deploy_verify_exits_non_zero_when_dev_preview_route_exists(): void
    {
        $this->setAppEnv('production');
        file_put_contents($this->devPreviewPath, "<?php\n");

        $this->artisan('deploy:verify')
            ->expectsOutputToContain('routes/dev-preview.php must not exist on deploy targets.')
            ->assertExitCode(1);
    }

    public function test_deploy_verify_exits_zero_when_not_local_and_dev_preview_absent(): void
    {
        $this->ensureDevPreviewAbsent();
        $this->setAppEnv('production');

        $this->artisan('deploy:verify')
            ->expectsOutputToContain('Deploy verification passed.')
            ->assertExitCode(0);
    }

    private function setAppEnv(?string $value): void
    {
        if ($value === null) {
            putenv('APP_ENV');
            unset($_ENV['APP_ENV'], $_SERVER['APP_ENV']);

            return;
        }

        putenv('APP_ENV='.$value);
        $_ENV['APP_ENV'] = $value;
        $_SERVER['APP_ENV'] = $value;
    }

    private function ensureDevPreviewAbsent(): void
    {
        if (file_exists($this->devPreviewPath)) {
            unlink($this->devPreviewPath);
        }
    }

    private function restoreDevPreviewFile(): void
    {
        if ($this->devPreviewOriginallyExisted) {
            file_put_contents($this->devPreviewPath, (string) $this->devPreviewOriginalContent);

            return;
        }

        if (file_exists($this->devPreviewPath)) {
            unlink($this->devPreviewPath);
        }
    }
}
