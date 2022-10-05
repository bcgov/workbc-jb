using System;
using System.Collections.Generic;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Admin.Areas.SystemSettings.Models
{
    public class SystemSettingManagementViewModel
    {
        public int TotalUsers { get; set; }
        public List<SystemSetting> Results { get; set; }
    }
}
