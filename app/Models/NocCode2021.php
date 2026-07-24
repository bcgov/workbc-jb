<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\HasMany;

final class NocCode2021 extends Model
{
    protected $table = 'NocCodes2021';

    protected $primaryKey = 'Id';

    protected $keyType = 'int';

    public $incrementing = false;

    public $timestamps = false;

    public function jobs(): HasMany
    {
        return $this->hasMany(Job::class, 'NocCodeId2021', 'Id');
    }
}
