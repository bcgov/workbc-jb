<?php

use Illuminate\Foundation\Inspiring;
use Illuminate\Support\Facades\Artisan;

Artisan::command('inspire', function () {
    $this->comment(Inspiring::quote());
})->purpose('Display an inspiring quote');

Artisan::command('deploy:verify', function (): int {
    $errors = [];
    $appEnv = strtolower(trim((string) env('APP_ENV', app()->environment())));

    if ($appEnv === 'local') {
        $errors[] = 'APP_ENV must not be local for deploy targets.';
    }

    if (file_exists(base_path('routes/dev-preview.php'))) {
        $errors[] = 'routes/dev-preview.php must not exist on deploy targets.';
    }

    if ($errors !== []) {
        foreach ($errors as $error) {
            $this->error($error);
        }

        return 1;
    }

    $this->info('Deploy verification passed.');

    return 0;
})->purpose('Fail deploys when local-only safety controls are violated');
