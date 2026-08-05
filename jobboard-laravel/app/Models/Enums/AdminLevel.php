<?php

namespace App\Models\Enums;

/**
 * Mirror of WorkBC.Data/Enums/AdminLevel.cs (AdminUsers.AdminLevel).
 *
 * NOTE: the numeric order is ascending privilege (Reporting=1 … SuperAdmin=3).
 * Getting these values wrong is a privilege-escalation risk once admin auth
 * (FND-6) reads them, so they must match the .NET source exactly.
 */
enum AdminLevel: int
{
    case Disabled = 0;
    case Reporting = 1;
    case Admin = 2;
    case SuperAdmin = 3;
}
