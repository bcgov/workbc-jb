using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WorkBC.Data.Enums;

namespace WorkBC.Admin.Areas.AdminAccounts.Models
{
    public class AdminUserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please complete the IDIR Username")]
        [DisplayName("IDIR Username:")]
        public string SamAccountName { get; set; }

        [Required(ErrorMessage = "Please complete the IDIR Name")]
        [DisplayName("IDIR Name:")]
        public string DisplayName { get; set; }

        [DisplayName("Unique Identifier:")]
        public string Guid { get; set; }

        public DateTime DateUpdated { get; set; }

        public DateTime DateCreated { get; set; }

        [Required(ErrorMessage = "Please complete the account type")]
        [DisplayName("Account Type:")]
        public AdminLevel AdminLevel { get; set; }

        public bool Deleted { get; set; }

        public int? LockedByAdminUserId { get; set; }

        public DateTime? DateLocked { get; set; }

        public bool ReadOnlyMode { get; set; }

        public DateTime? DateLastLogin { get; set; }
    }
}