<?php

namespace App\Services\Settings;

use App\Models\SystemSetting;
use Illuminate\Database\QueryException;
use Illuminate\Support\Facades\Cache;

final class SystemSettingsService
{
    private const CACHE_KEY = 'system-settings.read-through.v1';

    /**
     * @return array<string, string|null>
     */
    public function getMany(array $names): array
    {
        $requested = array_values(array_unique(array_filter(
            array_map(static fn (mixed $name): string => trim((string) $name), $names),
            static fn (string $name): bool => $name !== ''
        )));

        if ($requested === []) {
            return [];
        }

        $all = $this->allSettings();
        $resolved = [];

        foreach ($requested as $name) {
            $resolved[$name] = array_key_exists($name, $all)
                ? $all[$name]
                : null;
        }

        return $resolved;
    }

    public function get(string $name, ?string $default = null): ?string
    {
        $value = $this->getMany([$name])[$name] ?? null;

        return $value ?? $default;
    }

    public function invalidateCache(): void
    {
        Cache::forget(self::CACHE_KEY);
    }

    /**
     * @return array<string, string|null>
     */
    private function allSettings(): array
    {
        /** @var array<string, string|null> $settings */
        $settings = Cache::remember(self::CACHE_KEY, now()->addHours(24), static function (): array {
            try {
                return SystemSetting::query()
                    ->pluck('Value', 'Name')
                    ->map(static fn (mixed $value): ?string => $value !== null ? (string) $value : null)
                    ->all();
            } catch (QueryException) {
                return [];
            }
        });

        return $settings;
    }
}