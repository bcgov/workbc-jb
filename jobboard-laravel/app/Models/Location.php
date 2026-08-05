<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;
use Illuminate\Database\Eloquent\Relations\HasMany;

final class Location extends Model
{
    protected $table = 'Locations';

    protected $primaryKey = 'LocationId';

    protected $keyType = 'int';

    public $incrementing = false;

    public $timestamps = false;

    protected $casts = [
        'IsHidden' => 'boolean',
        'IsDuplicate' => 'boolean',
    ];

    public function region(): BelongsTo
    {
        return $this->belongsTo(Region::class, 'RegionId', 'Id');
    }

    public function jobs(): HasMany
    {
        return $this->hasMany(Job::class, 'LocationId', 'LocationId');
    }
}
