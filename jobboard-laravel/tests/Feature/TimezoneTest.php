<?php

namespace Tests\Feature;

use Tests\TestCase;

/**
 * Rule F guard: the application timezone must stay America/Vancouver. All date logic
 * (job expiry, date-range facets, alerts) depends on it — a change here silently shifts
 * every boundary. This test fails if the timezone is ever changed (FND-1 acceptance).
 */
class TimezoneTest extends TestCase
{
    public function test_application_timezone_is_america_vancouver(): void
    {
        $this->assertSame('America/Vancouver', config('app.timezone'));
        $this->assertSame('America/Vancouver', date_default_timezone_get());
    }
}
