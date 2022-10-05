using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WorkBC.Data.Enums;

namespace WorkBC.Data.Model.JobBoard
{
    public class AdminUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(20)]
        public string SamAccountName { get; set; }

        [MaxLength(60)]
        public string DisplayName { get; set; }

        [MaxLength(40)]
        public string GivenName { get; set; }

        [MaxLength(40)]
        public string Surname { get; set; }

        [MaxLength(40)]
        public string Guid { get; set; }

        public DateTime DateUpdated { get; set; }

        public DateTime DateCreated { get; set; }

        public AdminLevel AdminLevel { get; set; }

        public bool Deleted { get; set; }

        public int? LockedByAdminUserId { get; set; }

        [JsonIgnore]
        public virtual AdminUser LockedByAdminUser { get; set; }

        public DateTime? DateLocked { get; set; }

        public int? ModifiedByAdminUserId { get; set; }

        [JsonIgnore]
        public virtual AdminUser ModifiedByAdminUser { get; set; }

        public DateTime? DateLastLogin { get; set; }
    }
}