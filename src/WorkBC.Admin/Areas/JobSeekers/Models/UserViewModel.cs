using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using WorkBC.Data.Enums;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Admin.Areas.JobSeekers.Models
{
    public class UserViewModel
    {
        [MaxLength(50)]
        [Required(ErrorMessage = "Please complete the firstname")]
        public string FirstName { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Please complete the lastname")]
        public string LastName { get; set; }

        public List<SelectListItem> Cities { get; set; }
        public int? CountryId { get; set; }
        public string City { get; set; }
        public int? ProvinceId { get; set; }
        public int? LocationId { get; set; }
        public Location Location { get; set; }
        public JobSeekerFlags JobSeekerFlags { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public int? RegionId { get; set; }
        public string Password { get; set; }
        public DateTime DateRegistered { get; set; }
        public DateTime LastModified { get; set; }
        public string UserId { get; set; }
        public AccountStatus AccountStatus { get; set; }
        public bool? IsDuplicateLocation { get; set; }
        public bool ReadOnlyMode { get; set; }
        public string UserName { get; set; }

        public int? SecurityQuestionId { get; set; }

        [MaxLength(50)]
        public string SecurityAnswer { get; set; }
    }
}