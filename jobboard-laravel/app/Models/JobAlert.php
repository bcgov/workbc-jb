<?php

namespace App\Models;

use App\Models\Casts\TolerantEnum;
use App\Models\Concerns\SoftDeletesByFlag;
use App\Models\Enums\AlertFrequency;
use App\Search\Filters\JobSearchFiltersCast;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

final class JobAlert extends Model
{
    use SoftDeletesByFlag;

    protected $table = 'JobAlerts';

    protected $primaryKey = 'Id';

    protected $keyType = 'int';

    public $incrementing = true;

    public $timestamps = false;

    protected $casts = [
        'AlertFrequency' => TolerantEnum::class.':'.AlertFrequency::class,
        'JobSearchFilters' => JobSearchFiltersCast::class,
        'JobSearchFiltersVersion' => 'integer',
        'IsDeleted' => 'boolean',
        'DateCreated' => 'datetime',
        'DateModified' => 'datetime',
        'DateDeleted' => 'datetime',
    ];

    public function jobSeeker(): BelongsTo
    {
        return $this->belongsTo(JobSeeker::class, 'AspNetUserId', 'Id');
    }
}
