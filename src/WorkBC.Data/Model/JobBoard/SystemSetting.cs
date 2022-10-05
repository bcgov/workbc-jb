using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkBC.Data.Enums;

namespace WorkBC.Data.Model.JobBoard
{
    public class SystemSetting
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(400)]
        [Key]
        public string Name { get; set; }

        public string Value { get; set; }

        public string DefaultValue { get; set; }

        public string Description { get; set; }

        public SystemSettingFieldType FieldType { get; set; }

        [ForeignKey("AdminUser")]
        public int ModifiedByAdminUserId { get; set; }

        public virtual AdminUser ModifiedByAdminUser { get; set; }

        public DateTime DateUpdated { get; set; }
    }
}