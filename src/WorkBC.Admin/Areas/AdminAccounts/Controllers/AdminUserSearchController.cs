using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBC.Admin.Controllers;
using WorkBC.Data;
using System.Linq;
using WorkBC.Admin.Areas.AdminAccounts.Models;
using WorkBC.Admin.Services;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Admin.Areas.AdminAccounts.Controllers
{
    [Authorize(Roles = MinAccessLevel.SuperAdmin)]
    [Area("AdminAccounts")]
    [Route("admin-accounts/[controller]/[action]")]
    public class AdminUserSearchController : AdminControllerBase
    {
        private readonly JobBoardContext _jobBoardContext;
        private readonly int _adminLockHours;

        public AdminUserSearchController(JobBoardContext jobBoardContext, IConfiguration configuration)
        {
            _jobBoardContext = jobBoardContext;
            _adminLockHours = int.Parse(configuration["AppSettings:AdminLockHours"]);
        }

        public IActionResult Index([FromQuery] string searchQuery)
        {
            var allUsers = _jobBoardContext.AdminUsers.ToList();
            var lstUsers = new List<AdminUserRowViewModel>();
            
            foreach (AdminUser user in allUsers)
            {
                var continueEditing = user.LockedByAdminUserId.HasValue && user.LockedByAdminUserId.Value == base.CurrentAdminUserId && user.DateLocked > DateTime.Now.AddHours(-_adminLockHours);

                lstUsers.Add(new AdminUserRowViewModel()
                {
                    AdminUser = user,
                    ContinueEditing = continueEditing
                });
            }

            var result = new AdminUserSearchViewModel()
            {
                Results = new List<AdminUserRowViewModel>(),
                TotalUsers = allUsers.Count
            };
            result.Results = lstUsers;

            return View("AdminUserSearch", result);
        }
    }
}
