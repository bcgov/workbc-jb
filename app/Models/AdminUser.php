<?php

namespace App\Models;

use App\Models\Casts\TolerantEnum;
use App\Models\Enums\AdminLevel;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;
use Illuminate\Database\Eloquent\Relations\HasMany;

final class AdminUser extends Model
{
    protected $table = 'AdminUsers';

    protected $primaryKey = 'Id';

    protected $keyType = 'int';

    public $incrementing = true;

    public $timestamps = false;

    protected $casts = [
        'AdminLevel' => TolerantEnum::class.':'.AdminLevel::class,
        'DateLocked' => 'datetime',
        'DateAdded' => 'datetime',
    ];

    public function lockedByAdminUser(): BelongsTo
    {
        return $this->belongsTo(self::class, 'LockedByAdminUserId', 'Id');
    }

    public function systemSettings(): HasMany
    {
        return $this->hasMany(SystemSetting::class, 'ModifiedByAdminUserId', 'Id');
    }
}
