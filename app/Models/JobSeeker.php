<?php

namespace App\Models;

use App\Models\Casts\TolerantEnum;
use App\Models\Enums\AccountStatus;
use Illuminate\Database\Eloquent\Relations\HasMany;
use Illuminate\Database\Eloquent\Relations\HasOne;
use Illuminate\Foundation\Auth\User as Authenticatable;

final class JobSeeker extends Authenticatable
{
    protected $table = 'AspNetUsers';

    protected $primaryKey = 'Id';

    protected $keyType = 'string';

    public $incrementing = false;

    public $timestamps = false;

    protected $casts = [
        'EmailConfirmed' => 'boolean',
        'PhoneNumberConfirmed' => 'boolean',
        'TwoFactorEnabled' => 'boolean',
        'LockoutEnabled' => 'boolean',
        'DateRegistered' => 'datetime',
        'LastLogon' => 'datetime',
        'LastModified' => 'datetime',
        'DateLocked' => 'datetime',
        'AccountStatus' => TolerantEnum::class.':'.AccountStatus::class,
    ];

    public function getAuthPassword(): string
    {
        return (string) $this->PasswordHash;
    }

    public function savedJobs(): HasMany
    {
        return $this->hasMany(SavedJob::class, 'AspNetUserId', 'Id');
    }

    public function jobAlerts(): HasMany
    {
        return $this->hasMany(JobAlert::class, 'AspNetUserId', 'Id');
    }

    public function flags(): HasOne
    {
        return $this->hasOne(JobSeekerFlags::class, 'AspNetUserId', 'Id');
    }
}
