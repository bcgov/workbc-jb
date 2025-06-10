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
                new SystemSetting { Name="shared.settings.minimumWage", Value="17.85", Description="shared.settings.minimumWage", FieldType= SystemSettingFieldType.SingleLineText, ModifiedByAdminUserId= 1, DateUpdated= DateTime.Now, DefaultValue="17.85"},
                new SystemSetting { Name="shared.settings.defaultSearchRadius", Value="15", Description="Default Minimum Wage", FieldType= SystemSettingFieldType.Number, ModifiedByAdminUserId= 1, DateUpdated= DateTime.Now, DefaultValue="15"}
            };

    }
}
