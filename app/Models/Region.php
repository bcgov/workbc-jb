<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\HasMany;

final class Region extends Model
{
    protected $table = 'Regions';

    protected $primaryKey = 'Id';

    protected $keyType = 'int';

    public $incrementing = false;

    public $timestamps = false;

    public function locations(): HasMany
    {
        return $this->hasMany(Location::class, 'RegionId', 'Id');
    }

    public function jobs(): HasMany
    {
        return $this->hasMany(Job::class, 'RegionId', 'Id');
    }
}
