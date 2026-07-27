<?php

namespace App\Filament\Widgets;

use Filament\Widgets\ChartWidget;
use Illuminate\Support\Facades\DB;

/**
 * ADM-9 preview — see {@see ActiveJobsByIndustryChart} docblock for scope
 * notes. Paired with {@see JobsPostedPerWeekTable}.
 */
class JobsPostedPerWeekChart extends ChartWidget
{
    protected static ?int $sort = 3;

    protected static bool $isLazy = false;

    protected ?string $heading = 'Jobs posted per week';

    protected ?string $description = 'Last 8 weeks, by DatePosted.';

    protected function getType(): string
    {
        return 'line';
    }

    protected function getData(): array
    {
        $rows = DB::table('Jobs')
            ->selectRaw('date_trunc(\'week\', "DatePosted")::date as week, count(*) as jobs')
            ->where('DatePosted', '>=', now()->subWeeks(8))
            ->groupBy('week')
            ->orderBy('week')
            ->get();

        return [
            'labels' => $rows->pluck('week')->map(fn ($d) => \Illuminate\Support\Carbon::parse($d)->format('M j'))->all(),
            'datasets' => [
                [
                    'label' => 'Jobs posted',
                    'data' => $rows->pluck('jobs')->map(fn ($n) => (int) $n)->all(),
                    'borderColor' => '#2E6AB0', // workbc-blue
                    'backgroundColor' => 'rgba(46, 106, 176, 0.15)',
                    'fill' => true,
                    'tension' => 0.25,
                ],
            ],
        ];
    }

    protected function getOptions(): array
    {
        return [
            'plugins' => ['legend' => ['display' => false]],
            'scales' => ['y' => ['ticks' => ['precision' => 0]]],
        ];
    }
}
