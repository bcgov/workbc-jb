<?php

namespace App\Filament\Widgets;

use App\Models\Job;
use Filament\Widgets\ChartWidget;

/**
 * ADM-9 preview (not the full story — no period/region/source filters yet;
 * those depend on the ADM-8 Reporting service). Real Jobs/Industries data,
 * read directly (Rule B: IndustryId/ExpireDate are Jobs columns, not
 * recomputed here — only aggregated). Paired with
 * {@see ActiveJobsByIndustryTable} as the WCAG 1.1.1 data alternative — the
 * canvas itself only carries Filament's built-in aria-label (heading text),
 * not the underlying numbers.
 */
class ActiveJobsByIndustryChart extends ChartWidget
{
    protected static ?int $sort = 1;

    protected static bool $isLazy = false;

    protected ?string $heading = 'Active jobs by industry';

    protected ?string $description = 'Top 8 industries by currently active job postings.';

    protected function getType(): string
    {
        return 'bar';
    }

    protected function getData(): array
    {
        $rows = Job::query()
            ->join('Industries', 'Industries.Id', '=', 'Jobs.IndustryId')
            ->where('Jobs.ExpireDate', '>=', now())
            ->selectRaw('"Industries"."Title" as title, count(*) as jobs')
            ->groupBy('Industries.Title')
            ->orderByDesc('jobs')
            ->limit(8)
            ->get();

        return [
            'labels' => $rows->pluck('title')->all(),
            'datasets' => [
                [
                    'label' => 'Active jobs',
                    'data' => $rows->pluck('jobs')->map(fn ($n) => (int) $n)->all(),
                    'backgroundColor' => '#2E6AB0', // workbc-blue
                ],
            ],
        ];
    }

    protected function getOptions(): array
    {
        return [
            'indexAxis' => 'y',
            'plugins' => ['legend' => ['display' => false]],
            'scales' => ['x' => ['ticks' => ['precision' => 0]]],
        ];
    }
}
