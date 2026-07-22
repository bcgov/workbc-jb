<?php

namespace App\Search\Support;

/**
 * Maps the fixed salary brackets (1–5) to annualized min/max ranges, per
 * SalaryType. Direct port of the C# SalaryRangeHelper
 * (WorkBC.ElasticSearch.Models/Helpers/SalaryRangeHelper.cs).
 *
 * Salaries are always ANNUALIZED in the index (Rule B: the indexer computes
 * Salary; the app only reads it), so the bracket ranges are multiplied up to
 * annual figures here to match the stored Salary field.
 */
final class SalaryRangeHelper
{
    public const HOURLY = 0;
    public const WEEKLY = 1;
    public const BI_WEEKLY = 2;
    public const MONTHLY = 3;
    public const ANNUALLY = 4;
    public const NONE = 5;

    /** @var array<int, array<int, array{0: float, 1: float}>> [min, max] per bracket */
    private const RANGES = [
        self::HOURLY => [
            [0.0, 19.999999], [20.0, 29.999999], [30.0, 39.999999], [40.0, 49.999999], [50.0, 50000.0],
        ],
        self::WEEKLY => [
            [0.0, 799.999999], [800.0, 1199.999999], [1200.0, 1599.999999], [1600.0, 1999.999999], [2000.0, 200000.0],
        ],
        self::BI_WEEKLY => [
            [0.0, 1599.999999], [1600.0, 2399.999999], [2400.0, 3199.999999], [3200.0, 3999.999999], [4000.0, 400000.0],
        ],
        self::MONTHLY => [
            [0.0, 3999.999999], [4000.0, 5999.999999], [6000.0, 7999.999999], [8000.0, 9999.999999], [10000.0, 10000000.0],
        ],
        self::ANNUALLY => [
            [0.0, 39999.999999], [40000.0, 59999.999999], [60000.0, 79999.999999], [80000.0, 99999.999999], [100000.0, 100000000.0],
        ],
    ];

    /** @var array<int, int> annualizing multiplier per SalaryType */
    private const MULTIPLIERS = [
        self::HOURLY => 2080,
        self::WEEKLY => 52,
        self::BI_WEEKLY => 26,
        self::MONTHLY => 12,
        self::ANNUALLY => 1,
    ];

    /**
     * Annualized [min, max] for a bracket (1–5), rounded to 2 decimals.
     *
     * @return array{0: float, 1: float}
     */
    public static function getAnnualRange(int $salaryType, int $bracket): array
    {
        if ($bracket < 1 || $bracket > 5) {
            throw new \OutOfRangeException('Salary bracket must be between 1 and 5');
        }

        $type = array_key_exists($salaryType, self::RANGES) ? $salaryType : self::ANNUALLY;
        $multiplier = self::MULTIPLIERS[$type];
        [$min, $max] = self::RANGES[$type][$bracket - 1];

        return [
            round($multiplier * $min, 2),
            round($multiplier * $max - 0.01, 2),
        ];
    }

    /** Annualizing multiplier for a SalaryType (used for custom bracket 6). */
    public static function multiplier(int $salaryType): int
    {
        return self::MULTIPLIERS[$salaryType] ?? 1;
    }
}
