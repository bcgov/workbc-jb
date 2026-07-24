<?php

namespace App\Models\Concerns;

use Illuminate\Database\Eloquent\Builder;

/**
 * Shared soft-delete behavior for legacy tables that use IsDeleted + DateDeleted.
 *
 * This is intentionally NOT Laravel SoftDeletes: existing schema and columns are
 * fixed, and deletes should only flip the legacy flags.
 */
trait SoftDeletesByFlag
{
    protected static function bootSoftDeletesByFlag(): void
    {
        static::addGlobalScope('notDeleted', static fn (Builder $query): Builder => $query->where('IsDeleted', false));
    }

    public function delete(): ?bool
    {
        $this->IsDeleted = true;
        $this->DateDeleted = now();

        return $this->save();
    }

    public static function withTrashed(): Builder
    {
        return static::withoutGlobalScope('notDeleted');
    }

    public static function onlyTrashed(): Builder
    {
        return static::withTrashed()->where('IsDeleted', true);
    }

    public function restore(): bool
    {
        $this->IsDeleted = false;
        $this->DateDeleted = null;

        return $this->save();
    }
}
