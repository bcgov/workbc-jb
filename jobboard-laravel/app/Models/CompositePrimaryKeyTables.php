<?php

namespace App\Models;

/**
 * Composite-PK reporting tables are intentionally query-builder only.
 *
 * Eloquent has no native composite primary-key support, so these tables must
 * not be modeled as standard Eloquent models:
 * - JobStats (WeeklyPeriodId, RegionId, JobSourceId)
 * - JobSeekerStats (WeeklyPeriodId, LabelKey, RegionId)
 * - ReportPersistenceControl (WeeklyPeriodId, TableName)
 */
final class CompositePrimaryKeyTables
{
    /** @var list<string> */
    public const TABLES = [
        'JobStats',
        'JobSeekerStats',
        'ReportPersistenceControl',
    ];
}
