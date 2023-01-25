using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WorkBC.Admin.Areas.JobSeekers.Models;
using WorkBC.Admin.Areas.JobSeekers.Services;
using WorkBC.Admin.Controllers;
using WorkBC.Admin.Helpers;
using WorkBC.Admin.Services;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Admin.Areas.JobSeekers.Controllers
{
    [Authorize(Roles = MinAccessLevel.Admin)]
    [Area("JobSeekers")]
    [Route("jobseekers/[controller]/[action]")]
    public class UserManagementController : AdminControllerBase
    {
        private const int CanadaCountryId = 37;
        private readonly IConfiguration _configuration;
        private readonly JobBoardContext _jobBoardContext;
        private readonly IUserService _service;
        private readonly IMapper _mapper;
        private readonly int _adminLockHours;

        public UserManagementController(IConfiguration configuration, JobBoardContext jobBoardContext, UserManager<JobSeeker> userManager, IUserService service, IMapper mapper)
        {
            _configuration = configuration;
            _jobBoardContext = jobBoardContext;
            _service = service;
            _mapper = mapper;
            _adminLockHours = int.Parse(configuration["AppSettings:AdminLockHours"]);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UnlockUser(string currentUserId)
        {
            await _service.UnlockUser(currentUserId);

            //refresh list of admin users
            return RedirectToAction("Index", "UserSearch");
        }

        public async Task<ActionResult> BackToList(string id)
        {
            //remove lock from job-seeker
            var jobSeeker = await _service.FindByIdAsync(id);
                

            //only unlock user if its the same admin 
            if (jobSeeker?.LockedByAdminUserId != null && jobSeeker.LockedByAdminUserId.Value == CurrentAdminUserId)
            {
                await _service.UnlockUser(id);
            }

            return RedirectToAction("Index", "UserSearch");
        }

        public async Task<IActionResult> EditUser(string userId)
        {
            //find job-seeker  with ID
            JobSeeker js = await _service.FindByIdAsync(userId);

            js.JobSeekerFlags = await _jobBoardContext.JobSeekerFlags.FirstOrDefaultAsync(j => j.AspNetUserId == userId);

            var duplicateLocations = await _jobBoardContext.Locations.Where(x => x.IsDuplicate).ToListAsync();

            //Set ViewModel
            var model = _mapper.Map<UserViewModel>(js);

            if (duplicateLocations.Any(l => l.LocationId == model.LocationId))
            {
                model.IsDuplicateLocation = true;
                model.RegionId = duplicateLocations
                    .FirstOrDefault(l => l.LocationId == model.LocationId)?
                    .RegionId;
            }

            model.ReadOnlyMode = false;
            model.UserId = userId;

            //If the logged-in user is different from the locked user, the user should be in read-only mode
            if (js.LockedByAdminUserId.HasValue && js.LockedByAdminUserId.Value != CurrentAdminUserId && js.DateLocked > DateTime.Now.AddHours(-_adminLockHours))
            {
                model.ReadOnlyMode = true;
            }
            else if (js.LockedByAdminUserId != CurrentAdminUserId)
            {
                //lock current user
                await _service.LockUser(userId, base.CurrentAdminUserId);
            }

            return View("EditUser", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(UserViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError("error", "Please enter a valid email address");
            }

            if (model.IsDuplicateLocation ?? false)
            {
                if (model.RegionId == null)
                {
                    ModelState.AddModelError("RegionId", "Please select a region");
                }
                else
                {
                    // get the right location based on the region id
                    var correctLocationId = await GetCorrectLocationIdForRegion(model.LocationId, model.RegionId);

                    if (correctLocationId != null)
                    {
                        model.LocationId = correctLocationId;
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return View("EditUser", model);
            }

            try
            {
                //update user information
                await _service.UpdateUser(model, base.CurrentAdminUserId, model.RegionId);
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("error", ex.InnerException?.Message ?? ex.Message);
                return View("EditUser", model);
            }

            // unlock the user
            await _service.UnlockUser(model.UserId);

            //go back to listing page
            return RedirectToAction("Index", "UserSearch");
        }

        public async Task<IActionResult> ViewProfileHistory(string userId)
        {
            var history = await _service.GetJobSeekerHistory(userId);
            return View("ViewProfileHistory", history);
        }

        public async Task<IActionResult> ViewProfileComments(string userId)
        {
            var jobSeeker = await _service.FindUser(userId);

            if (jobSeeker == null)
                return RedirectToAction("EditUser", "UserManagement", userId);

            JobSeekerViewCommentsViewModel model = new JobSeekerViewCommentsViewModel()
            {
                JobSeekerId = userId,
                FirstName = jobSeeker.FirstName,
                LastName = jobSeeker.LastName,
                Status = jobSeeker.AccountStatus.GetLabel(),
                Comments = _service.GetComments(userId)
            };

            return View("ViewProfileComments", model);
        }

        public async Task<IActionResult> AddProfileComment(string userId)
        {
            var jobSeeker = await _service.FindUser(userId);

            if (jobSeeker == null)
                return RedirectToAction("ViewProfileComments", "UserManagement");

            JobSeekerAddCommentViewModel model = new JobSeekerAddCommentViewModel()
            {
                JobSeekerId = userId,
                Comment = string.Empty,
                FirstName = jobSeeker.FirstName,
                LastName = jobSeeker.LastName,
                Status = jobSeeker.AccountStatus.GetLabel()
            };

            return View("AddProfileComment", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProfileComment(JobSeekerAddCommentViewModel model)
        {
            //add comment
            await _service.AddComment(base.CurrentAdminUserId, model);

            //go back to listing page
            return RedirectToAction("ViewProfileComments", "UserManagement", new { UserId = model.JobSeekerId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> ToggleComment(int commentId, string jobSeekerId)
        {
            var result = await _service.ToggleComment(commentId);

            return result;
        }

        public IActionResult CreateUser()
        {
            UserViewModel uvm = new UserViewModel()
            {
                CountryId = 37,
                ProvinceId = 2,
                Cities = new List<SelectListItem>()
            };

            return View("CreateUser", uvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(UserViewModel model)
        {
            bool isModelValid = ModelState.IsValid;

            try
            {
                if (!String.IsNullOrWhiteSpace(model.Password) && !(model.Password.Length > 3))
                {
                    ModelState.AddModelError("error", "Password is required");
                    isModelValid = false;
                }

                //British Columbia with no city selected
                if (model.ProvinceId == 2 && !(model.LocationId != null && model.LocationId > 0))
                {
                    ModelState.AddModelError("error", "Please select a city");
                    isModelValid = false;
                }

                //set value so that we can pre-populate it again
                if (model.ProvinceId == 2 && model.LocationId > 0 && !isModelValid)
                {
                    model.IsDuplicateLocation = _jobBoardContext.Locations.FirstOrDefault(x => x.LocationId == model.LocationId)?.IsDuplicate;
                }

                //ensure we have an email address
                if (string.IsNullOrWhiteSpace(model.Email))
                {
                    ModelState.AddModelError("error", "Please enter a valid email address");
                    return View("EditUser", model);
                }

                if (model.IsDuplicateLocation ?? false)
                {
                    if (model.RegionId == null)
                    {
                        ModelState.AddModelError("RegionId", "Please select a region");
                    }
                    else
                    {
                        // get the right location based on the region id
                        var correctLocationId = await GetCorrectLocationIdForRegion(model.LocationId, model.RegionId);

                        if (correctLocationId != null)
                        {
                            model.LocationId = correctLocationId;
                        }
                    }
                }

                // trim and de-nullify string inputs
                model.Email = model.Email.Trim();
                model.FirstName = (model.FirstName ?? "").Trim();
                model.LastName= (model.LastName ?? "").Trim();
                model.SecurityAnswer = (model.SecurityAnswer ?? "").Trim();
                model.City = (model.City ?? "").Trim();

                if (isModelValid)
                {
                    //ensure the username/email is unique
                    if (_jobBoardContext.Users.Any(x => (x.Email == model.Email)))

                    {
                        ModelState.AddModelError("error", "Email [" + model.Email + "] is already taken");
                    }

                    if (_jobBoardContext.Users.Any(x => (x.UserName == model.Email)))
                    {
                        ModelState.AddModelError("error", "Username[" + model.Email + "] is already taken");
                    }

                    if (ModelState.IsValid)
                    {
                        IdentityResult result = await _service.CreateUser(model, CurrentAdminUserId);

                        if (result != null && result.Succeeded && ModelState.ErrorCount == 0)
                        {
                            //go back to listing page
                            return RedirectToAction("Index", "UserSearch");
                        }

                        ModelState.AddModelError("error",
                            "Error creating job seeker. Reason: " + result?.Errors.ToList()[0].Description ?? "");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", "Unexpected error: " + ex.Message);
            }

            return View("CreateUser", model);
        }

        private async Task<int?> GetCorrectLocationIdForRegion(int? locationId, int? regionId)
        {
            int? correctLocationId;
            var locationName = (await _jobBoardContext.Locations
                .FirstOrDefaultAsync(l => l.LocationId == locationId))?.City.ToLower();
            correctLocationId = (
                await _jobBoardContext.Locations
                    .FirstOrDefaultAsync(l =>
                        l.City.ToLower() == locationName && l.RegionId == regionId)
            )?.LocationId;
            return correctLocationId;
        }

        [HttpGet]
        public List<Province> GetProvinces(int countryId)
        {
            return countryId == CanadaCountryId
                ? _jobBoardContext.Provinces.AsNoTracking()
                    .OrderByDescending(p => p.ProvinceId == 2)
                    .ThenBy(p => p.Name).ToList()
                : new List<Province>();
        }

        [HttpGet]
        public List<CitySelect> GetCities()
        {
            var cities = (_jobBoardContext.Locations
                    .Where(x => !x.IsHidden &&
                                x.LocationId > 0)
                    .Select(x => new CitySelect { City = x.City.Trim(), IsDuplicate = x.IsDuplicate, Id = x.LocationId })
                    .Distinct()
                    .ToList())
                .OrderBy(x => x.City);

            return cities.ToList();
        }

        [HttpGet]
        public List<Region> GetRegionsByLocationName(string cityName)
        {
            var regions = from region in _jobBoardContext.Regions
                          join city in _jobBoardContext.Locations on region.Id equals city.RegionId
                          where city.City.Equals(cityName)
                          select region;

            return regions.Distinct().OrderBy(x => x.Name).ToList();
        }

        [HttpGet]
        public List<Region> GetRegionsByLocationId(int locationId)
        {
            var location = from loc in _jobBoardContext.Locations
                           where loc.LocationId.Equals(locationId)
                           select loc;

            return GetRegionsByLocationName(location.FirstOrDefault()?.City ?? "");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _service.DeleteUser(userId, base.CurrentAdminUserId);

            //Go back to listing page (refresh page)
            return RedirectToAction("Index", "UserSearch");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateUser(string userId)
        {
            await _service.ActivateUser(userId, base.CurrentAdminUserId);

            //(refresh page)
            return RedirectToAction("EditUser", await _service.FindUser(userId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateUser(string userId)
        {
            await _service.DeactivateUser(userId, base.CurrentAdminUserId);

            //(refresh page)
            return RedirectToAction("EditUser", await _service.FindUser(userId));
        }

        [HttpGet]
        public async Task<IActionResult> ImpersonateUser(string userId)
        {
            string token = await _service.ImpersonateUser(userId, base.CurrentAdminUserId);

            string jbAccountUrl = _configuration["AppSettings:JbAccountUrl"];

            var request = this.HttpContext.Request;

            var url = new UriBuilder(
                request.Scheme,
                request.Host.Host,
                request.Host.Port ?? -1,
                Url.Action("EditUser") + "?userid=").ToString();

            var returnUrl = UrlEncoder.Default.Encode(url);

            return Redirect($"{jbAccountUrl}#/impersonate/{token}/{returnUrl}");
        }
    }

    public class CitySelect
    {
        public int Id { get; set; }
        public string City { get; set; }
        public bool IsDuplicate { get; set; }
    }
}
