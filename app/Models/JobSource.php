<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\HasMany;

final class JobSource extends Model
{
    protected $table = 'JobSources';

    protected $primaryKey = 'Id';

    protected $keyType = 'int';

    public $incrementing = false;

    public $timestamps = false;

    public function jobs(): HasMany
    {
        return $this->hasMany(Job::class, 'JobSourceId', 'Id');
    }
}
