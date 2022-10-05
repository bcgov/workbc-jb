using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using WorkBC.Data.Enums;

namespace WorkBC.Data.Model.JobBoard
{
    /// <summary>
    ///     Extends ASP.NET Core Identity User
    /// </summary>
    public class JobSeeker : IdentityUser
    {
        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        [MaxLength(50)]
        public string City { get; set; }

        [ForeignKey("Country")]
        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }

        [ForeignKey("Province")]
        public int? ProvinceId { get; set; }

        public virtual Province Province { get; set; }

        [ForeignKey("Location")]
        public int? LocationId { get; set; }

        public virtual Location Location { get; set; }

        // reference to old schema for imported users
        public int? LegacyWebUserId { get; set; }

        [Column(TypeName = "smallint")]
        public AccountStatus AccountStatus { get; set; }

        public Guid? VerificationGuid { get; set; }

        public virtual SecurityQuestion SecurityQuestion { get; set; }

        [ForeignKey("SecurityQuestion")]
        public int? SecurityQuestionId { get; set; }

        [MaxLength(50)]
        public string SecurityAnswer { get; set; }

        public DateTime DateRegistered { get; set; }

        public DateTime LastModified { get; set; }

        public DateTime? LastLogon { get; set; }

        public virtual JobSeekerFlags JobSeekerFlags { get; set; }

        public int? LockedByAdminUserId { get; set; }

        [ForeignKey("LockedByAdminUserId")]
        [JsonIgnore]
        public virtual AdminUser LockedByAdminUser { get; set; }

        public DateTime? DateLocked { get; set; }
    }
}