<?php

namespace App\Search\Filters;

use Illuminate\Contracts\Database\Eloquent\CastsAttributes;
use Illuminate\Database\Eloquent\Model;

/**
 * Eloquent cast for JobSearchFilters (contracts §1). Stores the value object as
 * JSON on the model column and reads it back through the versioned deserializer,
 * pulling the shape version from a sibling column (default JobSearchFiltersVersion)
 * so old JobAlerts rows (version 0 or 1) keep deserializing.
 *
 * @implements CastsAttributes<JobSearchFilters, JobSearchFilters>
 */
final class JobSearchFiltersCast implements CastsAttributes
{
    public function __construct(private string $versionColumn = 'JobSearchFiltersVersion') {}

    /**
     * @param  array<string, mixed>  $attributes
     */
    public function get(Model $model, string $key, mixed $value, array $attributes): ?JobSearchFilters
    {
        if ($value === null || $value === '') {
            return null;
        }

        $data = is_array($value) ? $value : json_decode((string) $value, true);

        if (! is_array($data)) {
            throw new InvalidFilterException("Cannot decode JobSearchFilters from column [{$key}].");
        }

        $version = (int) ($attributes[$this->versionColumn] ?? JobSearchFilters::CURRENT_VERSION);

        return JobSearchFilters::fromArray($data, $version);
    }

    /**
     * @param  array<string, mixed>  $attributes
     * @return array<string, string>
     */
    public function set(Model $model, string $key, mixed $value, array $attributes): array
    {
        if ($value === null) {
            return [$key => ''];
        }

        if (! $value instanceof JobSearchFilters) {
            throw new InvalidFilterException(
                "Column [{$key}] expects a ".JobSearchFilters::class.' instance.'
            );
        }

        return [$key => json_encode($value->toArray(), JSON_UNESCAPED_SLASHES | JSON_UNESCAPED_UNICODE)];
    }
}
