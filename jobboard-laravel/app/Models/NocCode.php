<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\HasMany;

final class NocCode extends Model
{
    protected $table = 'NocCodes';

    protected $primaryKey = 'Id';

    protected $keyType = 'int';

    public $incrementing = false;

    public $timestamps = false;

    public function jobs(): HasMany
    {
        return $this->hasMany(Job::class, 'NocCodeId', 'Id');
    }
}
