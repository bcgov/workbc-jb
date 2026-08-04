<?php

namespace App\Models;

use App\Models\Concerns\SoftDeletesByFlag;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

/**
 * ACCT-6 — a job seeker's saved industry profile.
 *
 * `IndustryId` is a smallint FK to `Industries`. Note the legacy list joins
 * `Industries.TitleBC` (the B.C.-specific title), not `Title` — see
 * {@see \App\Services\JobSeeker\SavedProfileService::industryProfilesFor()}.
 */
final class SavedIndustryProfile extends Model
{
    use SoftDeletesByFlag;

    protected $table = 'SavedIndustryProfiles';

    protected $primaryKey = 'Id';

    protected $keyType = 'int';

    public $incrementing = true;

    public $timestamps = false;

    protected $casts = [
        'IsDeleted' => 'boolean',
        'IndustryId' => 'integer',
        'DateSaved' => 'datetime',
        'DateDeleted' => 'datetime',
    ];

    public function jobSeeker(): BelongsTo
    {
        return $this->belongsTo(JobSeeker::class, 'AspNetUserId', 'Id');
    }

    public function industry(): BelongsTo
    {
        return $this->belongsTo(Industry::class, 'IndustryId', 'Id');
    }
}
