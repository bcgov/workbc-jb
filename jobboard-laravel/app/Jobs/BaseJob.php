<?php

namespace App\Jobs;

use Closure;
use Illuminate\Bus\Queueable;
use Illuminate\Contracts\Queue\ShouldQueue;
use Illuminate\Foundation\Bus\Dispatchable;
use Illuminate\Queue\InteractsWithQueue;
use Illuminate\Queue\SerializesModels;
use Illuminate\Support\Facades\Cache;
use Illuminate\Support\Facades\Log;
use Throwable;

/**
 * FND-3 — base class for the app's OWN background work (alert emails, sitemap
 * regeneration, view-count flush). Feed import/indexing jobs are out of scope
 * (Rule C) — those stay in the existing .NET containers; this is only for
 * work this app dispatches itself.
 *
 * Subclasses implement {@see process()}; {@see handle()} wraps it with
 * lifecycle logging (started/finished/failed) so every job's behaviour is
 * observable in container logs without each subclass re-implementing it.
 *
 * Queues (architecture.md §9): dispatch onto 'notifications' for anything
 * user-facing (alert emails) via `$this->onQueue('notifications')` in the
 * subclass constructor; everything else defaults to 'default'.
 *
 * Idempotency: Laravel's {@see \Illuminate\Contracts\Queue\ShouldBeUnique}
 * (implemented directly by a subclass + its own `uniqueId()`) prevents the
 * SAME job from being enqueued twice concurrently. {@see idempotent()} is the
 * complementary runtime guard — it prevents a RETRY of an already-completed
 * attempt from repeating a side effect (e.g. re-sending an alert email after
 * a late failure on an unrelated step).
 */
abstract class BaseJob implements ShouldQueue
{
    use Dispatchable;
    use InteractsWithQueue;
    use Queueable;
    use SerializesModels;

    public int $tries = 3;

    public bool $failOnTimeout = true;

    /**
     * Exponential backoff in seconds between retries (one entry per attempt).
     *
     * @return array<int, int>
     */
    public function backoff(): array
    {
        return [10, 30, 90];
    }

    final public function handle(): void
    {
        $start = microtime(true);
        $this->log('started');

        try {
            $this->process();
        } catch (Throwable $e) {
            $this->log('failed', ['error' => $e->getMessage()]);

            throw $e;
        }

        $this->log('finished', ['duration_ms' => (int) ((microtime(true) - $start) * 1000)]);
    }

    /**
     * The job's actual work. Throw to trigger a retry (per $tries/backoff).
     */
    abstract protected function process(): void;

    /**
     * Run $callback at most once per $key, across retries and re-dispatches.
     * Guards a side effect (e.g. "send this email") that must not repeat if a
     * retried job reaches it a second time. Returns null (and skips
     * $callback) when $key has already run within $ttlSeconds.
     */
    protected function idempotent(string $key, Closure $callback, int $ttlSeconds = 86400): mixed
    {
        $lockKey = 'idempotent:' . static::class . ':' . $key;

        if (Cache::has($lockKey)) {
            $this->log('skipped-duplicate', ['key' => $key]);

            return null;
        }

        $result = $callback();
        Cache::put($lockKey, true, $ttlSeconds);

        return $result;
    }

    /**
     * Structured lifecycle logging to stderr — container log collectors
     * (k8s/Docker) capture stderr identically to stdout, and keeping this on
     * its own channel means job logs stay structured regardless of the app's
     * general LOG_CHANNEL.
     *
     * @param  array<string, mixed>  $context
     */
    private function log(string $status, array $context = []): void
    {
        Log::channel('stderr')->info(static::class . " {$status}", array_merge([
            'job' => static::class,
            'status' => $status,
            'attempt' => $this->attempts(),
        ], $context));
    }
}
