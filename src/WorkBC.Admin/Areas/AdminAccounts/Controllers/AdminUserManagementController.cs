using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBC.Admin.Areas.AdminAccounts.Models;
using WorkBC.Admin.Controllers;
using WorkBC.Admin.Services;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;
using WorkBC.Shared.Extensions;

namespace WorkBC.Admin.Areas.AdminAccounts.Controllers
{
    [Authorize(Roles = MinAccessLevel.SuperAdmin)]
    [Area("AdminAccounts")]
    [Route("admin-accounts/[controller]/[action]/{id?}")]
    public class AdminUserManagementController : AdminControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly JobBoardContext _dbContext;
        private readonly ILogger<AdminUserManagementController> _logger;
        private readonly string _environment;
        private readonly int _adminLockHours;

        public AdminUserManagementController(IConfiguration configuration, JobBoardContext dbContext,
            ILogger<AdminUserManagementController> logger)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _logger = logger;
            _environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production";
            _adminLockHours = int.Parse(configuration["AppSettings:AdminLockHours"]);
        }

        [HttpGet]
        public IActionResult EditAdminUser(int id)
        {
            //find admin user with ID
            AdminUser au = _dbContext.AdminUsers.FirstOrDefault(user => user.Id == id);

            //Set ViewModel
            var model = new AdminUserViewModel
            {
                DisplayName = au.DisplayName,
                Guid = au.Guid,
                AdminLevel = au.AdminLevel,
                Deleted = au.Deleted,
                LockedByAdminUserId = au.LockedByAdminUserId,
                DateLocked = au.DateLocked,
                DateCreated = au.DateCreated,
                DateUpdated = au.DateUpdated,
                Id = au.Id,
                SamAccountName = au.SamAccountName,
                ReadOnlyMode = false,
                DateLastLogin = au.DateLastLogin
            };

            //If the logged-in user is different from the locked user, the user should be in read-only mode
            if (au.LockedByAdminUserId.HasValue && au.LockedByAdminUserId.Value != base.CurrentAdminUserId && au.DateLocked > DateTime.Now.AddHours(-_adminLockHours))
            {
                model.ReadOnlyMode = true;
            }
            else if (au.LockedByAdminUserId != CurrentAdminUserId)
            {
                //lock current user
                au.LockedByAdminUserId = CurrentAdminUserId;
                au.DateLocked = DateTime.Now;
            }

            _dbContext.AdminUsers.Update(au);
            _dbContext.SaveChanges();

            //return model to view
            return View("EditAdminUser", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAdminUser(AdminUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditAdminUser", model);
            }

            try
            {
                //Find admin user with ID
                AdminUser user = _dbContext.AdminUsers.FirstOrDefault(usr => usr.Id == model.Id);

                if (user != null)
                {
                    //Update admin user with new values
                    user.DateUpdated = DateTime.Now;
                    user.AdminLevel = model.AdminLevel;
                    user.ModifiedByAdminUserId = CurrentAdminUserId;

                    user.DateLocked = null;
                    user.LockedByAdminUser = null;

                    // Don't update these non-editable fields
                    // - Guid 
                    // - SamAccountName 
                    // - Deleted
                    // - Email
                    // - DisplayName

                    //Update admin user
                    _dbContext.AdminUsers.Update(user);
                    _dbContext.SaveChanges();

                    //Go back to the listing page
                    return RedirectToAction("Index", "AdminUserSearch");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                ModelState.AddModelError("error", "Unexpected error: " + ex.Message);
            }

            //go back to edit screen with errors on screen (invalid values for the fields)
            return View("EditAdminUser", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(int userId)
        {
            //find the admin user by ID
            AdminUser user = _dbContext.AdminUsers.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                //Delete user
                user.Deleted = true;
                user.ModifiedByAdminUserId = CurrentAdminUserId;
                user.DateUpdated = DateTime.Now;
                user.LockedByAdminUserId = null;
                user.DateLocked = null;
                _dbContext.AdminUsers.Update(user);
                _dbContext.SaveChanges();
            }

            //Go back to listing page (refresh page)
            return RedirectToAction("Index", "AdminUserSearch");
        }

        public IActionResult AddAdminUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAdminUser(AdminUserViewModel model)
        {
            // check for duplicate users
            string guid = model.Guid.Replace("-", "").ToUpper();
            if (_dbContext.AdminUsers.Any(u => u.Guid == guid && !u.Deleted))
            {
                ModelState.AddModelError("SamAccountName", "User already exists");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                //Create new admin user from view model
                var au = new AdminUser
                {
                    AdminLevel = model.AdminLevel,
                    DisplayName = model.DisplayName,
                    Guid = (model.Guid ?? "").Replace("-", "").ToUpper(),
                    SamAccountName = model.SamAccountName.ToUpper(),
                    GivenName = ParseGivenName(model.DisplayName),
                    Surname = ParseSurname(model.DisplayName),
                    ModifiedByAdminUserId = CurrentAdminUserId,
                    DateUpdated = DateTime.Now,
                    DateCreated = DateTime.Now,
                    Deleted = false
                };

                //Add to database
                _dbContext.AdminUsers.Add(au);
                _dbContext.SaveChanges();

                //Go to listing page
                return RedirectToAction("Index", "AdminUserSearch");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                ModelState.AddModelError("error", "Unexpected error: " + ex.Message);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserInfo(string id)
        {
            string username = id;

            if (_environment == "Development" && _configuration["Keycloak:DevModeBypassEnabled"] == "true")
            {
                return Ok(new
                {
                    Guid = GetFakeUserGuid(username),
                    SamAccountName = username,
                    DisplayName = $"XT:{username.ToUpper()} AEST:EX"
                });
            }

            var tenantId = _configuration["AzureAdSettings:TenantId"];
            var clientId = _configuration["AzureAdSettings:ClientId"];
            //var clientSecret = _configuration["AzureAdSettings:ClientSecret"];
            var clientCert = _configuration["AzureAdSettings:ClientCertificate"];
            byte[] data = Convert.FromBase64String(clientCert);
            string decodedString = System.Text.Encoding.UTF8.GetString(data);

            var creds = new ClientSecretCredential(tenantId, clientId, clientCert);

            GraphServiceClient graphClient = new GraphServiceClient(creds);

            var queryOptions = new List<QueryOption>()
            {
                new QueryOption("$count", "true"),
                new QueryOption("$filter", $"onPremisesSamAccountName eq '{username}' or userPrincipalName eq '{username}@gov.bc.ca'"),
            };

            var users = await graphClient.Users
                .Request(queryOptions)
                .Header("ConsistencyLevel", "eventual")
                .Select(x => new {
                    x.OnPremisesImmutableId,
                    x.OnPremisesSamAccountName,
                    x.DisplayName
                })
                .GetAsync();

            if (!users.Any())
            {
                return NotFound("The IDIR entered is invalid. Please enter a valid Azure AD IDIR.");
            }

            User user = users.FirstOrDefault();
            string samName = (user.OnPremisesSamAccountName ?? "").Trim().ToUpper();
            if (_dbContext.AdminUsers.Any(u => u.SamAccountName == samName && !u.Deleted))
            {
                return Conflict("This IDIR username already exists.");
            }

            return Ok(new
            {
                Guid = new Guid(Convert.FromBase64String(user.OnPremisesImmutableId)),
                SamAccountName = user.OnPremisesSamAccountName,
                user.DisplayName
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnlockAdminUser(int currentUserId)
        {
            //find admin user
            AdminUser adminUser = _dbContext.AdminUsers.FirstOrDefault(user => user.Id == currentUserId);

            if (adminUser != null)
            {
                //remove lock from admin user
                adminUser.DateLocked = null;
                adminUser.LockedByAdminUserId = null;
                adminUser.DateUpdated = DateTime.Now;
                adminUser.ModifiedByAdminUserId = CurrentAdminUserId;

                //update DB
                _dbContext.AdminUsers.Update(adminUser);
                _dbContext.SaveChanges();
            }

            //refresh list of admin users
            return RedirectToAction("Index", "AdminUserSearch");
        }

        public ActionResult BackToList(int id)
        {
            //remove lock from admin user
            AdminUser au = _dbContext.AdminUsers.FirstOrDefault(user => user.Id == id);

            if (au != null && au.LockedByAdminUserId.HasValue && au.LockedByAdminUserId.Value == base.CurrentAdminUserId)
            {
                try
                {
                    //unlock user
                    au.DateLocked = null;
                    au.LockedByAdminUserId = null;

                    _dbContext.AdminUsers.Update(au);
                    _dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }

            return RedirectToAction("Index", "AdminUserSearch");
        }

        /// <summary>
        ///     Generates a repeatable fake guid based on the username.
        ///     Used for dev environments.
        /// </summary>
        private static Guid GetFakeUserGuid(string username)
        {
            byte[] ba = Encoding.Default.GetBytes(username.ToUpper());
            var hexString = BitConverter.ToString(ba).Replace("-", "");

            var guidString = (hexString + "00000000000000000000000000000000").ToLower();

            var g1 = guidString.Substring(0, 8);
            var g2 = guidString.Substring(8, 4);
            var g3 = guidString.Substring(12, 4);
            var g4 = guidString.Substring(16, 4);
            var g5 = guidString.Substring(20, 12);

            string guid = $"{g1}-{g2}-{g3}-{g4}-{g5}";
            return Guid.Parse(guid);
        }

        /// <summary>
        ///     Parse a Surname from an IDIR DisplayName
        ///     e.g.
        ///     XT:Kachman, Dave AEST:IN
        ///     Breheret, Julie AEST:EX
        /// </summary>
        private string ParseSurname(string displayName)
        {
            string[] displayNameParts = displayName.Split(',');

            if (displayNameParts.Length < 2)
            {
                return displayName;
            }

            string[] surnameParts = displayNameParts[0].Trim().Split(':');
            string surname = surnameParts.Length > 1
                ? surnameParts[1].Trim()
                : surnameParts[0].Trim();

            // max 40 characters
            return surname.Truncate(40);
        }

        /// <summary>
        ///     Parses a GivenName from an IDIR DisplayName
        ///     e.g.
        ///     XT:Kachman, Dave AEST:IN
        ///     Breheret, Julie AEST:EX
        /// </summary>
        private string ParseGivenName(string displayName)
        {
            string[] displayNameParts = displayName.Split(',');

            if (displayNameParts.Length < 2)
            {
                return displayName;
            }

            string[] givenNameParts = displayNameParts[1].Trim().Split(' ');
            string givenName = givenNameParts[0].Trim();

            // max 40 characters
            return givenName.Truncate(40);
        }
    }
}