<?php

namespace App\Filament\Widgets;

use Filament\Widgets\Widget;
use Illuminate\Support\Carbon;
use Illuminate\Support\Facades\DB;

/**
 * The WCAG 1.1.1 data-table alternative for {@see JobsPostedPerWeekChart}.
 */
class JobsPostedPerWeekTable extends Widget
{
    protected static ?int $sort = 4;

    protected static bool $isLazy = false;

    protected string $view = 'filament.widgets.simple-data-table';

    protected function getViewData(): array
    {
        $rows = DB::table('Jobs')
            ->selectRaw('date_trunc(\'week\', "DatePosted")::date as week, count(*) as jobs')
            ->where('DatePosted', '>=', now()->subWeeks(8))
            ->groupBy('week')
            ->orderBy('week')
            ->get();

        return [
            'caption' => 'Jobs posted per week — same data as the chart above',
            'columns' => ['Week of', 'Jobs posted'],
            'rows' => $rows->map(fn ($r) => [Carbon::parse($r->week)->format('M j, Y'), number_format((int) $r->jobs)])->all(),
        ];
    }
}
