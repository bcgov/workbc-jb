<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

final class JobSeekerFlags extends Model
{
    protected $table = 'JobSeekerFlags';

    protected $primaryKey = 'Id';

    protected $keyType = 'int';

    public $incrementing = true;

    public $timestamps = false;

    protected $casts = [
        'IsApprentice' => 'boolean',
        'IsIndigenousPerson' => 'boolean',
        'IsMatureWorker' => 'boolean',
        'IsNewImmigrant' => 'boolean',
        'IsPersonWithDisability' => 'boolean',
        'IsStudent' => 'boolean',
        'IsVeteran' => 'boolean',
        'IsVisibleMinority' => 'boolean',
        'IsYouth' => 'boolean',
    ];

    public function jobSeeker(): BelongsTo
    {
        return $this->belongsTo(JobSeeker::class, 'AspNetUserId', 'Id');
    }
}
