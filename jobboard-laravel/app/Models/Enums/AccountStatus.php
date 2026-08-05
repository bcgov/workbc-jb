<?php

namespace App\Models\Enums;

/**
 * Mirror of WorkBC.Data/Enums/AccountStatus.cs (the AspNetUsers.AccountStatus
 * codes). Legacy/dirty values outside this set read as null via
 * {@see \App\Models\Casts\TolerantEnum}.
 */
enum AccountStatus: int
{
    case InvalidStatusZero = 0;
    case Active = 1;
    case Deactivated = 3;
    case Pending = 4; // awaiting email activation
    case Deleted = 99;
}
