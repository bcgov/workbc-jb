using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WorkBC.Data.Enums;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Admin.Areas.SystemSettings.Models
{
    public class SystemSettingViewModel
    {
        public string Name { get; set; }
        public string ValueBoolean { get; set; }
        public string ValueSingleLine { get; set; }
        public string ValueMultiLine { get; set; }
        public string ValueHtml { get; set; }
        public string ValueNumber { get; set; }
        public string Description { get; set; }
        public SystemSettingFieldType FieldType { get; set; }

        public bool ValueIsOptional
        {
            get
            {
                switch (Name)
                {
                    case "jbAccount.dashboard.notification1Body":
                    case "jbAccount.dashboard.notification1Title":
                    case "jbAccount.dashboard.notification2Body":
                    case "jbAccount.dashboard.notification2Title":
                        return true;
                    default:
                        return false;
                }
            }
        }

        public string Value { get; set; }
    }
}
