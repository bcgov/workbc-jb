<?php

namespace App\Models\Enums;

/**
 * Mirror of WorkBC.Data/Enums/JobAlertFrequency.cs (JobAlerts.AlertFrequency).
 * Drives the alert-sender fan-out (daily always; weekly Monday; biweekly 15th +
 * month-end; monthly the 1st; never = disabled).
 */
enum AlertFrequency: int
{
    case Daily = 1;
    case Weekly = 2;
    case BiWeekly = 3;
    case Monthly = 4;
    case Never = 5;
}
