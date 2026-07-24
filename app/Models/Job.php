<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;
use Illuminate\Database\Eloquent\Relations\HasMany;

final class Job extends Model
{
    protected $table = 'Jobs';

    protected $primaryKey = 'JobId';

    protected $keyType = 'string';

    public $incrementing = false;

    public $timestamps = false;

    protected $casts = [
        'FullTime' => 'boolean',
        'PartTime' => 'boolean',
        'LeadingToFullTime' => 'boolean',
        'Permanent' => 'boolean',
        'Temporary' => 'boolean',
        'Casual' => 'boolean',
        'Seasonal' => 'boolean',
        'IsActive' => 'boolean',
        'DatePosted' => 'datetime',
        'DateCreated' => 'datetime',
        'DateModified' => 'datetime',
        'ExpireDate' => 'datetime',
        'LastUpdated' => 'datetime',
        'Salary' => 'decimal:2',
    ];

    public function jobSource(): BelongsTo
    {
        return $this->belongsTo(JobSource::class, 'JobSourceId', 'Id');
    }

    public function location(): BelongsTo
    {
        return $this->belongsTo(Location::class, 'LocationId', 'LocationId');
    }

    // NOTE: Jobs has no RegionId column — a job's region is reached through its
    // Location ($job->location?->region; Locations.RegionId → Regions.Id), or read
    // from the OpenSearch Region field for search.

    public function industry(): BelongsTo
    {
        return $this->belongsTo(Industry::class, 'IndustryId', 'Id');
    }

    public function nocCode(): BelongsTo
    {
        return $this->belongsTo(NocCode::class, 'NocCodeId', 'Id');
    }

    public function nocCode2021(): BelongsTo
    {
        return $this->belongsTo(NocCode2021::class, 'NocCodeId2021', 'Id');
    }

    public function savedJobs(): HasMany
    {
        return $this->hasMany(SavedJob::class, 'JobId', 'JobId');
    }
}
