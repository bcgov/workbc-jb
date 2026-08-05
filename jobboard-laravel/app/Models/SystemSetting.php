<?php

namespace App\Models;

use App\Models\Casts\TolerantEnum;
use App\Models\Enums\SystemSettingFieldType;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

final class SystemSetting extends Model
{
    protected $table = 'SystemSettings';

    protected $primaryKey = 'Name';

    protected $keyType = 'string';

    public $incrementing = false;

    public $timestamps = false;

    protected $casts = [
        'FieldType' => TolerantEnum::class.':'.SystemSettingFieldType::class,
        'DateUpdated' => 'datetime',
    ];

    public function modifiedByAdminUser(): BelongsTo
    {
        return $this->belongsTo(AdminUser::class, 'ModifiedByAdminUserId', 'Id');
    }
}
