<?php

namespace App\Models\Casts;

use BackedEnum;
use Illuminate\Contracts\Database\Eloquent\CastsAttributes;
use Illuminate\Database\Eloquent\Model;

/**
 * Casts an integer column to a backed enum, tolerating legacy/unknown codes: a
 * value with no matching case reads as null instead of throwing. The pre-Laravel
 * data contains occasional out-of-range codes (e.g. a stray AspNetUsers
 * AccountStatus outside the .NET enum), and a strict enum cast would 500 the whole
 * read for one dirty row. Known values still map to the exact case.
 *
 * @implements CastsAttributes<BackedEnum|null, BackedEnum|int|null>
 */
class TolerantEnum implements CastsAttributes
{
    /** @param  class-string<BackedEnum>  $enum */
    public function __construct(private string $enum) {}

    public function get(Model $model, string $key, mixed $value, array $attributes): ?BackedEnum
    {
        if ($value === null || $value === '') {
            return null;
        }

        return $this->enum::tryFrom((int) $value);
    }

    public function set(Model $model, string $key, mixed $value, array $attributes): ?int
    {
        if ($value === null) {
            return null;
        }

        return $value instanceof BackedEnum ? (int) $value->value : (int) $value;
    }
}
