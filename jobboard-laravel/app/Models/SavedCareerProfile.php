<?php

namespace App\Models;

use App\Models\Concerns\SoftDeletesByFlag;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

/**
 * ACCT-6 — a job seeker's saved career (NOC) profile.
 *
 * `NocCodeId2021` is the profile identity. The legacy
 * `EDM_CareerProfile_CareerProfileId` column is vestigial: the .NET
 * CareerProfilesController always wrote it as null and never looked anything up
 * by it, so it is mapped but never populated here either.
 */
final class SavedCareerProfile extends Model
{
    use SoftDeletesByFlag;

    protected $table = 'SavedCareerProfiles';

    protected $primaryKey = 'Id';

    protected $keyType = 'int';

    public $incrementing = true;

    public $timestamps = false;

    protected $casts = [
        'IsDeleted' => 'boolean',
        'NocCodeId2021' => 'integer',
        'DateSaved' => 'datetime',
        'DateDeleted' => 'datetime',
    ];

    public function jobSeeker(): BelongsTo
    {
        return $this->belongsTo(JobSeeker::class, 'AspNetUserId', 'Id');
    }

    public function nocCode(): BelongsTo
    {
        return $this->belongsTo(NocCode2021::class, 'NocCodeId2021', 'Id');
    }
}
