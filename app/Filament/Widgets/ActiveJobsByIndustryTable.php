<?php

namespace App\Filament\Widgets;

use App\Models\Job;
use Filament\Widgets\Widget;

/**
 * The WCAG 1.1.1 data-table alternative for {@see ActiveJobsByIndustryChart}
 * (a bare <canvas> only carries a short aria-label, not the underlying
 * numbers) — same query, rendered as a real table.
 */
class ActiveJobsByIndustryTable extends Widget
{
    protected static ?int $sort = 2;

    protected static bool $isLazy = false;

    protected string $view = 'filament.widgets.simple-data-table';

    protected function getViewData(): array
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
            'caption' => 'Active jobs by industry — same data as the chart above',
            'columns' => ['Industry', 'Active jobs'],
            'rows' => $rows->map(fn ($r) => [$r->title, number_format((int) $r->jobs)])->all(),
        ];
    }
}
