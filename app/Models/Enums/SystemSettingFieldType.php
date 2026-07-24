<?php

namespace App\Models\Enums;

/**
 * Mirror of WorkBC.Data/Enums/SystemSettingFieldType.cs (SystemSettings.FieldType).
 */
enum SystemSettingFieldType: int
{
    case Unknown = 0;
    case SingleLineText = 1;
    case MultiLineText = 2;
    case Number = 3;
    case Boolean = 4;
    case Html = 5;
}
