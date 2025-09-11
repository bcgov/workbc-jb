using System;
using System.Collections.Generic;
using WorkBC.Data.Enums;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Tests.Fixtures
{
    public static class SystemSettingsFixture
    {
        public static List<SystemSetting> systemSetting =>
         new List<SystemSetting>
            {
                new SystemSetting { Name="shared.settings.minimumWage", Value="17.85", Description="This is the minimum wage value that is used to filter data coming from the job bank.", FieldType= SystemSettingFieldType.SingleLineText, ModifiedByAdminUserId= 1, DateUpdated= DateTime.Now, DefaultValue="17.85"},
                new SystemSetting { Name="shared.settings.defaultSearchRadius", Value="15", Description="Default radius (km) for location searches. Valid values are 10, 15, 25, 50, 75 or 100. If you enter an invalid value then 15 will be used instead.", FieldType= SystemSettingFieldType.Number, ModifiedByAdminUserId= 1, DateUpdated= DateTime.Now, DefaultValue="15"}
            };

    }
}
