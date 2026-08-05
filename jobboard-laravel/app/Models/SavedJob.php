<?php

namespace App\Models;

use App\Models\Concerns\SoftDeletesByFlag;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

final class SavedJob extends Model
{
    use SoftDeletesByFlag;

    protected $table = 'SavedJobs';

    protected $primaryKey = 'Id';

    protected $keyType = 'int';

    public $incrementing = true;

    public $timestamps = false;

    protected $casts = [
        'IsDeleted' => 'boolean',
        'DateSaved' => 'datetime',
        'NoteUpdatedDate' => 'datetime',
        'DateDeleted' => 'datetime',
    ];

    public function jobSeeker(): BelongsTo
    {
        return $this->belongsTo(JobSeeker::class, 'AspNetUserId', 'Id');
    }

    public function job(): BelongsTo
    {
        return $this->belongsTo(Job::class, 'JobId', 'JobId');
    }
}
