<?php

namespace App\Models;

use App\Models\Casts\TolerantEnum;
use App\Models\Enums\AccountStatus;
use Illuminate\Auth\Passwords\CanResetPassword;
use Illuminate\Contracts\Auth\CanResetPassword as CanResetPasswordContract;
use Illuminate\Database\Eloquent\Relations\HasMany;
use Illuminate\Database\Eloquent\Relations\HasOne;
use Illuminate\Foundation\Auth\User as Authenticatable;
use Illuminate\Notifications\Notifiable;

final class JobSeeker extends Authenticatable implements CanResetPasswordContract
{
    use CanResetPassword;
    use Notifiable;

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
        'LockoutEnd' => 'datetime',
        'AccountStatus' => TolerantEnum::class.':'.AccountStatus::class,
    ];

    public function getAuthPassword(): string
    {
        return (string) $this->PasswordHash;
    }

    public function getEmailForPasswordReset(): string
    {
        return (string) $this->Email;
    }

    public function routeNotificationForMail(): string
    {
        return (string) $this->Email;
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
