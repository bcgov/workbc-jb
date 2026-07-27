<?php

declare(strict_types=1);

namespace WorkBC\JobAlertNotifier\Search;

/**
 * Port of WorkBC.ElasticSearch.Models.Helpers.SalaryRangeHelper.
 *
 * The C# version does decimal math: annualMin = multiplier * min,
 * annualMax = multiplier * max - 0.01m, both rendered with ToString("0.00").
 * PHP floats would drift on the *.999999 bracket bounds, so the math here is
 * done in integer millionths of a dollar and rounded to cents at the end.
 */
final class SalaryRangeHelper
{
    public const HOURLY = 0;
    public const WEEKLY = 1;
    public const BI_WEEKLY = 2;
    public const MONTHLY = 3;
    public const ANNUALLY = 4;
    public const NONE = 5;

    /** Bracket bounds in millionths of a dollar, per salary type. */
    private const RANGES = [
        self::HOURLY => [
            [0, 19999999],
            [20000000, 29999999],
            [30000000, 39999999],
            [40000000, 49999999],
            [50000000, 50000000000],
        ],
        self::WEEKLY => [
            [0, 799999999],
            [800000000, 1199999999],
            [1200000000, 1599999999],
            [1600000000, 1999999999],
            [2000000000, 200000000000],
        ],
        self::BI_WEEKLY => [
            [0, 1599999999],
            [1600000000, 2399999999],
            [2400000000, 3199999999],
            [3200000000, 3999999999],
            [4000000000, 400000000000],
        ],
        self::MONTHLY => [
            [0, 3999999999],
            [4000000000, 5999999999],
            [6000000000, 7999999999],
            [8000000000, 9999999999],
            [10000000000, 10000000000000],
        ],
        self::ANNUALLY => [
            [0, 39999999999],
            [40000000000, 59999999999],
            [60000000000, 79999999999],
            [80000000000, 99999999999],
            [100000000000, 100000000000000],
        ],
    ];

    private const MULTIPLIERS = [
        self::HOURLY => 2080,
        self::WEEKLY => 52,
        self::BI_WEEKLY => 26,
        self::MONTHLY => 12,
    ];

    /**
     * Returns [minAnnual, maxAnnual] as "0.00"-formatted strings, matching
     * the C# KeyValuePair<string,string> exactly.
     *
     * @return array{0: string, 1: string}
     */
    public static function getAnnualRange(int $salaryType, int $bracket): array
    {
        if ($bracket > 5 || $bracket < 1) {
            throw new \OutOfRangeException('Salary bracket must be between 1 and 5');
        }

        // Unknown/NONE/ANNUALLY all fall back to the annual table with
        // multiplier 1, matching the C# switch default.
        $multiplier = self::MULTIPLIERS[$salaryType] ?? 1;
        $ranges = self::RANGES[$salaryType] ?? self::RANGES[self::ANNUALLY];
        [$minMicro, $maxMicro] = $ranges[$bracket - 1];

        // maxAnnual = multiplier * max - 0.01 (0.01 dollars = 10,000 millionths)
        return [
            self::microToMoney($multiplier * $minMicro),
            self::microToMoney($multiplier * $maxMicro - 10000),
        ];
    }

    /** Millionths of a dollar → "0.00" string (round half away from zero, like decimal.ToString). */
    private static function microToMoney(int $micro): string
    {
        $cents = intdiv($micro, 10000);
        if (($micro % 10000) * 2 >= 10000) {
            $cents++;
        }
        return sprintf('%d.%02d', intdiv($cents, 100), $cents % 100);
    }
}
