<?php

namespace Tests\Unit\Jobs;

use App\Jobs\BaseJob;
use Illuminate\Support\Facades\Log;
use Monolog\Handler\TestHandler;
use Tests\TestCase;

/**
 * FND-3 — BaseJob's shared lifecycle logging and idempotency helper. Uses two
 * tiny job subclasses defined below rather than a real queued job, since none
 * of the app's own background work (ACCT-4 alert sender, etc.) exists yet.
 */
class BaseJobTest extends TestCase
{
    public function test_a_job_logs_started_and_finished(): void
    {
        $handler = new TestHandler();
        Log::channel('stderr')->getLogger()->pushHandler($handler);

        BaseJobTestSampleJob::dispatch();

        $messages = array_map(
            static fn (\Monolog\LogRecord $record): string => $record->message,
            $handler->getRecords(),
        );

        $this->assertContains(BaseJobTestSampleJob::class . ' started', $messages);
        $this->assertContains(BaseJobTestSampleJob::class . ' finished', $messages);
    }

    public function test_idempotent_runs_the_callback_only_once_per_key(): void
    {
        BaseJobTestIdempotentJob::$runs = 0;

        BaseJobTestIdempotentJob::dispatch();
        BaseJobTestIdempotentJob::dispatch();

        $this->assertSame(1, BaseJobTestIdempotentJob::$runs);
    }

    public function test_idempotent_runs_again_once_a_different_key_is_used(): void
    {
        BaseJobTestIdempotentJob::$runs = 0;

        BaseJobTestIdempotentJob::dispatch('key-a');
        BaseJobTestIdempotentJob::dispatch('key-b');

        $this->assertSame(2, BaseJobTestIdempotentJob::$runs);
    }
}

class BaseJobTestSampleJob extends BaseJob
{
    protected function process(): void
    {
        // No-op: this job only exists to exercise BaseJob's own lifecycle logging.
    }
}

class BaseJobTestIdempotentJob extends BaseJob
{
    public static int $runs = 0;

    public function __construct(private readonly string $key = 'fixed-key') {}

    protected function process(): void
    {
        $this->idempotent($this->key, static function (): void {
            BaseJobTestIdempotentJob::$runs++;
        });
    }
}
