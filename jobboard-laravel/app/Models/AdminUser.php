<?php

namespace App\Models;

use App\Models\Casts\TolerantEnum;
use App\Models\Enums\AdminLevel;
use Filament\Models\Contracts\FilamentUser;
use Filament\Models\Contracts\HasName;
use Filament\Panel;
use Illuminate\Auth\Authenticatable;
use Illuminate\Contracts\Auth\Authenticatable as AuthenticatableContract;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;
use Illuminate\Database\Eloquent\Relations\HasMany;

/**
 * FND-6 (ADR-008): AdminUsers has NO credential column — admin identity has
 * always been federated (SamAccountName / Keycloak), never a local password.
 * This model is Authenticatable only so the `admin` guard can hold a session
 * for it; nothing here ever checks a password. The only way to authenticate is
 * a direct `Auth::guard('admin')->login($adminUser)` call from code that has
 * already verified identity some other way (today: the local dev-preview
 * route; from FND-6b: the Keycloak OIDC callback).
 */
final class AdminUser extends Model implements AuthenticatableContract, FilamentUser, HasName
{
    use Authenticatable;

    protected $table = 'AdminUsers';

    protected $primaryKey = 'Id';

    protected $keyType = 'int';

    public $incrementing = true;

    public $timestamps = false;

    protected $casts = [
        'AdminLevel' => TolerantEnum::class.':'.AdminLevel::class,
        'Deleted' => 'boolean',
        'DateLocked' => 'datetime',
        'DateCreated' => 'datetime',
        'DateUpdated' => 'datetime',
        'DateLastLogin' => 'datetime',
    ];

    public function lockedByAdminUser(): BelongsTo
    {
        return $this->belongsTo(self::class, 'LockedByAdminUserId', 'Id');
    }

    public function systemSettings(): HasMany
    {
        return $this->hasMany(SystemSetting::class, 'ModifiedByAdminUserId', 'Id');
    }

    /**
     * Gate on AdminLevel (never write-side; ADM-1's per-resource policies build
     * on this). Deleted or Disabled — or an unrecognized legacy code, which
     * {@see TolerantEnum} reads as null — fails closed.
     */
    public function canAccessPanel(Panel $panel): bool
    {
        if ((bool) $this->Deleted) {
            return false;
        }

        return match ($this->AdminLevel) {
            AdminLevel::Reporting, AdminLevel::Admin, AdminLevel::SuperAdmin => true,
            AdminLevel::Disabled, null => false,
        };
    }

    /**
     * Filament's topbar/avatar reads this (Filament\Models\Contracts\HasName).
     * DisplayName is NOT NULL on the real table.
     */
    public function getFilamentName(): string
    {
        return (string) $this->DisplayName;
    }

    /**
     * No password exists on this table — never used to authenticate (see class docblock).
     */
    public function getAuthPassword(): string
    {
        return '';
    }

    /**
     * "Remember me" is never offered for this guard (no UI passes remember=true),
     * so there is no remember_token column to touch — make that explicit.
     */
    public function getRememberTokenName(): string
    {
        return '';
    }
}
