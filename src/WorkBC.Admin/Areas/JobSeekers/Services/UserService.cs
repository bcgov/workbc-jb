using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WorkBC.Admin.Areas.JobSeekers.Extentions;
using WorkBC.Admin.Areas.JobSeekers.Models;
using WorkBC.Admin.Helpers;
using WorkBC.Admin.Models;
using WorkBC.Data;
using WorkBC.Data.Enums;
using WorkBC.Data.Model.JobBoard;
using WorkBC.Shared.Repositories;

namespace WorkBC.Admin.Areas.JobSeekers.Services
{
    public interface IUserService
    {
        Task<(IList<JobSeekerSearchViewModel> result,
            int filteredResultCount,
            int totalResultsCount,
            int totalActive,
            int totalDeleted,
            int totalDeactivated,
            int totalPending)> Search(DataTablesModel model, int currentAdminUserId);
        Task<IdentityResult> DeleteUser(string userId, int adminUserId);
        Task<IdentityResult> ActivateUser(string userId, int adminUserId);
        Task<IdentityResult> DeactivateUser(string userId, int adminUserId);
        Task<IdentityResult> CreateUser(UserViewModel model, int adminUserId);
        Task<UserViewModel> FindUser(string userId);
        Task<IdentityResult> UpdateUser(UserViewModel model, int adminUserId, int? regionId = null);
        Task<bool> AddComment(int adminUserId, JobSeekerAddCommentViewModel model);
        List<JobSeekerJobComment> GetComments(string userId);
        Task<bool> ToggleComment(int commentId);
        Task<JobSeekerViewProfileHistoryViewModel> GetJobSeekerHistory(string userId);
        Task<string> ImpersonateUser(string userId, int adminUserId);
        Task<IdentityResult> UnlockUser(string userId);
        Task<JobSeeker> FindByIdAsync(string id);
        Task<IdentityResult> LockUser(string userId, int adminUserId);
    }

    public class UserService : IUserService
    {
        private readonly JobBoardContext _jobBoardContext;
        private readonly JobSeekerRepository _jobSeekerRepo;
        private readonly IMapper _mapper;
        private readonly Dictionary<int, AdminUser> _cachedAdminUsers = new Dictionary<int, AdminUser>();
        private readonly int _adminLockHours;

        public UserService(JobBoardContext jobBoardContext, UserManager<JobSeeker> userManager, IMapper mapper, IConfiguration configuration)
        {
            _jobBoardContext = jobBoardContext;
            _jobSeekerRepo = new JobSeekerRepository(jobBoardContext, userManager);
            _mapper = mapper;
            _adminLockHours = int.Parse(configuration["AppSettings:AdminLockHours"]);
        }

        public async Task<IdentityResult> DeleteUser(string userId, int adminUserId)
        {
            return await _jobSeekerRepo.DeleteJobSeekerAsync(userId, adminUserId);
        }

        public async Task<IdentityResult> ActivateUser(string userId, int adminUserId)
        {
            var jobSeeker = await GetByIdAsync(userId);
            if (jobSeeker == null)
                return IdentityResult.Failed();

            var userParam = _mapper.Map<JobSeeker>(jobSeeker);

            // confirm the email
            jobSeeker.EmailConfirmed = true;

            // set the status
            userParam.AccountStatus = AccountStatus.Active;

            return await _jobSeekerRepo.UpdateJobSeekerAsync(jobSeeker, userParam, adminUserId: adminUserId);
        }

        public async Task<IdentityResult> DeactivateUser(string userId, int adminUserId)
        {
            var jobSeeker = await GetByIdAsync(userId);
            if (jobSeeker == null)
                return IdentityResult.Failed();

            var userParam = _mapper.Map<JobSeeker>(jobSeeker);

            // set the status
            userParam.AccountStatus = AccountStatus.Deactivated;

            return await _jobSeekerRepo.UpdateJobSeekerAsync(jobSeeker, userParam, adminUserId: adminUserId);
        }

        public async Task<IdentityResult> CreateUser(UserViewModel model, int adminUserId)
        {
            IdentityResult result = null;
            var now = DateTime.Now;

            //create new job seeker account
            JobSeeker jobSeeker = new JobSeeker
            {
                DateRegistered = now,
                LastModified = now,
                AccountStatus = AccountStatus.Active,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                CountryId = model.CountryId,
                ProvinceId = model.ProvinceId == -1 ? null : model.ProvinceId,
                LocationId = model.LocationId,
                UserName = model.Email,
                City = _jobBoardContext.Locations.FirstOrDefault(x => x.LocationId == model.LocationId)?.City,
                SecurityQuestionId = model.SecurityQuestionId,
                SecurityAnswer = model.SecurityAnswer,
                JobSeekerFlags = new JobSeekerFlags
                {
                    IsApprentice = model.JobSeekerFlags.IsApprentice,
                    IsIndigenousPerson = model.JobSeekerFlags.IsIndigenousPerson,
                    IsMatureWorker = model.JobSeekerFlags.IsMatureWorker,
                    IsNewImmigrant = model.JobSeekerFlags.IsNewImmigrant,
                    IsPersonWithDisability = model.JobSeekerFlags.IsPersonWithDisability,
                    IsStudent = model.JobSeekerFlags.IsStudent,
                    IsVeteran = model.JobSeekerFlags.IsVeteran,
                    IsVisibleMinority = model.JobSeekerFlags.IsVisibleMinority,
                    IsYouth = model.JobSeekerFlags.IsYouth
                },
                EmailConfirmed = true
            };

            //create new identity user
            (_, result) = await _jobSeekerRepo.CreateUserAsync(jobSeeker, model.Password, adminUserId);

            return result;
        }

        public async Task<UserViewModel> FindUser(string userId)
        {
            //load job seeker
            var jobSeeker = await GetByIdAsync(userId);

            //if JobSeeker is NULL return empty object
            if (jobSeeker == null)
            {
                return new UserViewModel();
            }

            //convert to ViewModel
            UserViewModel uvm = new UserViewModel()
            {
                FirstName = jobSeeker.FirstName,
                LastName = jobSeeker.LastName,
                Email = jobSeeker.Email,
                CountryId = jobSeeker.CountryId,
                LocationId = jobSeeker.LocationId,
                ProvinceId = jobSeeker.ProvinceId,
                DateRegistered = jobSeeker.DateRegistered,
                LastModified = jobSeeker.LastModified,
                City = _jobBoardContext.Locations.FirstOrDefault(x => x.LocationId == jobSeeker.LocationId)?.City,
                UserId = jobSeeker.Id,
                AccountStatus = jobSeeker.AccountStatus,
                RegionId = _jobBoardContext.Locations.FirstOrDefault(x => x.LocationId == jobSeeker.LocationId)?.RegionId,
                IsDuplicateLocation = _jobBoardContext.Locations.FirstOrDefault(x => x.LocationId == jobSeeker.LocationId)?.IsDuplicate,
                UserName = jobSeeker.UserName,
                SecurityQuestionId = jobSeeker.SecurityQuestionId,
                SecurityAnswer = jobSeeker.SecurityAnswer
            };

            //setting jobseeker flags
            jobSeeker.JobSeekerFlags = _jobBoardContext.JobSeekerFlags.FirstOrDefault(s => s.AspNetUserId == userId);
            uvm.JobSeekerFlags = new JobSeekerFlags()
            {
                IsApprentice = jobSeeker.JobSeekerFlags?.IsApprentice ?? false,
                IsIndigenousPerson = jobSeeker.JobSeekerFlags?.IsIndigenousPerson ?? false,
                IsMatureWorker = jobSeeker.JobSeekerFlags?.IsMatureWorker ?? false,
                IsNewImmigrant = jobSeeker.JobSeekerFlags?.IsNewImmigrant ?? false,
                IsPersonWithDisability = jobSeeker.JobSeekerFlags?.IsPersonWithDisability ?? false,
                IsStudent = jobSeeker.JobSeekerFlags?.IsStudent ?? false,
                IsVeteran = jobSeeker.JobSeekerFlags?.IsVeteran ?? false,
                IsVisibleMinority = jobSeeker.JobSeekerFlags?.IsVisibleMinority ?? false,
                IsYouth = jobSeeker.JobSeekerFlags?.IsYouth ?? false
            };

            return uvm;
        }

        public async Task<IdentityResult> UpdateUser(UserViewModel model, int adminUserId, int? regionId = null)
        {
            var jobSeeker = await GetByIdAsync(model.UserId);
            var userParam = _mapper.Map<JobSeeker>(model);

            // fix the user id
            userParam.Id = jobSeeker.Id;

            return await _jobSeekerRepo.UpdateJobSeekerAsync(jobSeeker, userParam, model.Password, adminUserId, regionId);
        }

        public async Task<JobSeeker> GetByIdAsync(string userId)
        {
            var jobSeeker = await _jobSeekerRepo.FindByIdAsync(userId);
            if (jobSeeker != null)
            {
                var jobSeekerFlags = _jobBoardContext.JobSeekerFlags.AsNoTracking().FirstOrDefault(x => x.AspNetUserId == userId);
                jobSeeker.JobSeekerFlags = jobSeekerFlags;
            }
            return jobSeeker;
        }

        public async Task<(IList<JobSeekerSearchViewModel> result,
            int filteredResultCount,
            int totalResultsCount,
            int totalActive,
            int totalDeleted,
            int totalDeactivated,
            int totalPending)> Search(DataTablesModel model, int currentAdminUserId)
        {
            string searchBy = model.Search?.Value;
            int take = model.Length;
            int skip = model.Start;
            string filter = model.Filter;

            var sortBy = "";
            var sortDir = true;

            if (model.Order != null && model.Order.Count != 0)
            {
                // in this example we just default sort on the 1st column
                sortBy = model.Columns[model.Order[0].Column].Data;
                sortDir = model.Order[0].Dir.ToLower() == "asc";
            }

            if (filter == null)
            {
                filter = string.Empty;
            }

            // search the database taking into consideration table sorting and paging
            (List<JobSeekerSearchViewModel> result, int filteredResultsCount,
                int totalResultsCount,
                int totalActive,
                int totalDeleted,
                int totalDeactivated,
                int totalPending
                ) = await QueryDatabase(searchBy, take, skip, sortBy, sortDir, filter, currentAdminUserId);

            if (result == null)
            {
                // empty collection...
                return (new List<JobSeekerSearchViewModel>(), 0, 0, 0, 0, 0, 0);
            }

            return (result, filteredResultsCount, totalResultsCount, totalActive, totalDeleted,
                totalDeactivated, totalPending);
        }

        private async Task<(List<JobSeekerSearchViewModel>, 
            int filteredResultsCount,
            int totalResultsCount, 
            int totalActive, 
            int totalDeleted, 
            int totalDeactivated,
            int totalPending)> 
            QueryDatabase(string searchBy, int take, int skip, string sortBy, bool sortDir, string filter, int currentAdminUserId)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                // if we have an empty search then just order the results by FirstName ascending
                sortBy = "FirstName";
                sortDir = true;
            }

            // apply the filter to get a queryable object
            IQueryable<JobSeeker> queryable = FilterUsers(searchBy, filter)
                .AsExpandable();

            // apply the filter to get a queryable object with NO filter for counters
            IQueryable<JobSeeker> queryableNoFilter = FilterUsers(searchBy, "")
                .AsExpandable();

            // convert to viewmodel and apply sorting and pagination
            List<JobSeekerSearchViewModel> result = await queryable
                .OrderBy(sortBy, sortDir)
                .Skip(skip)
                .Take(take)
                .Select(m => new JobSeekerSearchViewModel
                {
                    Id = m.Id,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    AccountStatus = m.AccountStatus.ToString(),
                    Email = GetEmail(m),
                    City = m.Location.City ?? string.Empty,
                    Province = m.Province.ShortName,
                    Country = m.Country.Name,
                    LastModified = m.LastModified.ToString("yyyy-MM-dd"),
                    DateRegistered = m.DateRegistered.ToString("yyyy-MM-dd"),
                    LockedByAdminUserId = m.DateLocked > DateTime.Now.AddHours(-_adminLockHours) 
                        ? m.LockedByAdminUserId 
                        : null,
                    DateLocked = m.LockedByAdminUserId > 0 && m.DateLocked > DateTime.Now.AddHours(-_adminLockHours) 
                        ? m.DateLocked.Value.ToString("MMM. dd, yyyy") 
                        : null,
                    TimeLocked = m.LockedByAdminUserId > 0 && m.DateLocked > DateTime.Now.AddHours(-_adminLockHours) 
                        ? m.DateLocked.Value.ToString("HH:mm") 
                        : null
                })
                .ToListAsync();

            foreach (JobSeekerSearchViewModel row in result)
            {
                if (row.LockedByAdminUserId > 0)
                {
                    row.AdminDisplayName =
                        GetDisplayNameClean(GetAdminUser(row.LockedByAdminUserId.Value)?.DisplayName ?? "");
                    row.LockedByCurrentAdmin = row.LockedByAdminUserId == currentAdminUserId;
                }
            }

            Dictionary<AccountStatus, int> counts = await (
                from q in queryableNoFilter
                group q by q.AccountStatus
                into g
                select new {Status = g.Key, Count = g.Count()}
            ).ToDictionaryAsync(
                g => g.Status,
                g => g.Count
            );

            int totalActive = counts.ContainsKey(AccountStatus.Active) ? counts[AccountStatus.Active] : 0;
            int totalDeleted = counts.ContainsKey(AccountStatus.Deleted) ? counts[AccountStatus.Deleted] : 0;
            int totalDeactivated = counts.ContainsKey(AccountStatus.Deactivated) ? counts[AccountStatus.Deactivated] : 0;
            int totalPending = counts.ContainsKey(AccountStatus.Pending) ? counts[AccountStatus.Pending] : 0;

            // get the total results for the filter
            int filteredResultsCount = await queryable.CountAsync();

            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            var totalResultsCount = 0; //_jobBoardContext.Users.Count();

            return (result, filteredResultsCount, totalResultsCount, totalActive, totalDeleted, totalDeactivated, totalPending);
        }

        private AdminUser GetAdminUser(int id)
        {
            if (_cachedAdminUsers.ContainsKey(id))
            {
                return _cachedAdminUsers[id];
            }

            AdminUser adminUser = _jobBoardContext.AdminUsers
                .FirstOrDefault(a => a.Id == id);

            _cachedAdminUsers.Add(id, adminUser);

            return adminUser;
        }

        private IQueryable<JobSeeker> FilterUsers(string searchBy, string filter)
        {
            searchBy = (searchBy ?? "").Trim();
            filter = (filter ?? "").Trim();
            string s = $"{searchBy}%";

            if (searchBy.Contains(" "))
            {
                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                searchBy = regex.Replace(searchBy, " ");

                var split = searchBy.Split(" ");

                var s1 = $"{split[0]}%";
                var s2 = $"{split[1]}%";

                string s3 = "%";

                if (split.Length > 2)
                {
                    s3 = $"{split[2]}%";
                }

                if (filter != string.Empty)
                {
                    Enum.TryParse(filter, out AccountStatus filterEnum);

                    return _jobBoardContext.Users
                        .Where(user =>
                            ((
                                EF.Functions.Like(user.FirstName, s) ||
                                EF.Functions.Like(user.LastName, s) ||
                                EF.Functions.Like(user.Email, s)
                            ) ||
                            ((
                                EF.Functions.Like(user.FirstName, s1) ||
                                EF.Functions.Like(user.LastName, s1) ||
                                EF.Functions.Like(user.Email, s1)
                            ) &&
                            (
                                EF.Functions.Like(user.FirstName, s2) ||
                                EF.Functions.Like(user.LastName, s2) ||
                                EF.Functions.Like(user.Email, s2)
                            ) &&
                             (
                                 EF.Functions.Like(user.FirstName, s3) ||
                                 EF.Functions.Like(user.LastName, s3) ||
                                 EF.Functions.Like(user.Email, s3)
                             )
                             )) &&
                            user.AccountStatus == filterEnum);
                }

                return _jobBoardContext.Users
                    .Where(user =>
                        ((
                             EF.Functions.Like(user.FirstName, s) ||
                             EF.Functions.Like(user.LastName, s) ||
                             EF.Functions.Like(user.Email, s)
                         ) ||
                         ((
                              EF.Functions.Like(user.FirstName, s1) ||
                              EF.Functions.Like(user.LastName, s1) ||
                              EF.Functions.Like(user.Email, s1)
                          ) &&
                          (
                              EF.Functions.Like(user.FirstName, s2) ||
                              EF.Functions.Like(user.LastName, s2) ||
                              EF.Functions.Like(user.Email, s2)
                          ) &&
                          (
                              EF.Functions.Like(user.FirstName, s3) ||
                              EF.Functions.Like(user.LastName, s3) ||
                              EF.Functions.Like(user.Email, s3)
                          )
                         )));
            }

            if (searchBy != "%" && filter != string.Empty)
            {
                Enum.TryParse(filter, out AccountStatus filterEnum);

                return _jobBoardContext.Users
                    .Where(user => (EF.Functions.Like(user.FirstName, s) ||
                                    EF.Functions.Like(user.LastName, s) ||
                                    EF.Functions.Like(user.Email, s)) && user.AccountStatus == filterEnum);

            }

            if (searchBy != "%")
            {
                return _jobBoardContext.Users
                    .Where(user => EF.Functions.Like(user.FirstName, s) ||
                                    EF.Functions.Like(user.LastName, s) ||
                                    EF.Functions.Like(user.Email, s));
            }

            if (filter != string.Empty)
            {
                Enum.TryParse(filter, out AccountStatus filterEnum);
                return _jobBoardContext.Users.Where(user => user.AccountStatus == filterEnum);

            }

            return _jobBoardContext.Users;
        }

        public async Task<bool> AddComment(int adminUserId, JobSeekerAddCommentViewModel model)
        {
            //get job seeker
            JobSeeker jobSeeker = await GetByIdAsync(model.JobSeekerId);
            if (jobSeeker == null)
                return false;

            //get current logged-in admin user
            AdminUser adminUser = _jobBoardContext.AdminUsers.FirstOrDefault(x => x.Id == adminUserId);
            if (adminUser == null)
                return false;

            //set comment
            JobSeekerAdminComment adminComment = new JobSeekerAdminComment()
            {
                DateEntered = DateTime.Now,
                Comment = model.Comment,
                IsPinned = false,
                JobSeeker = jobSeeker,
                AspNetUserId = jobSeeker.Id,
                EnteredByAdminUser = adminUser,
                EnteredByAdminUserId = adminUser.Id
            };

            //save to database
            _jobBoardContext.JobSeekerAdminComments.Add(adminComment);
            await _jobBoardContext.SaveChangesAsync();

            return true;
        }

        public List<JobSeekerJobComment> GetComments(string userId)
        {
            //get all comments for job seeker ID
            var comments = _jobBoardContext.JobSeekerAdminComments.Where(x => x.JobSeeker.Id == userId).ToList();

            //select all comments into new object
            var jobSeekerComments = from comms in comments
                                    join admin in _jobBoardContext.AdminUsers on comms.EnteredByAdminUserId equals admin.Id
                                    select new JobSeekerJobComment { Id = comms.Id, Comment = comms.Comment, DateCreated = comms.DateEntered, CommentMadeBy = GetDisplayNameClean(admin.DisplayName), IsSticky = comms.IsPinned };

            //order comments
            //sticky notes at the top, then by date
            jobSeekerComments = jobSeekerComments.OrderByDescending(x => x.IsSticky).ThenByDescending(d => d.DateCreated);

            //return comments
            return jobSeekerComments.ToList();
        }

        public async Task<bool> ToggleComment(int commentId)
        {
            bool success = false;

            //load comment
            var comment = _jobBoardContext.JobSeekerAdminComments.FirstOrDefault(c => c.Id == commentId);

            if (comment != null)
            {
                //toggle the sticky status
                comment.IsPinned = !comment.IsPinned;

                //save to database
                _jobBoardContext.JobSeekerAdminComments.Update(comment);
                await _jobBoardContext.SaveChangesAsync();

                success = true;
            }

            return success;
        }

        /// <summary>
        ///     Removes the ".000" from the email addresses of deleted users
        /// </summary>
        private static string GetEmail(JobSeeker jobSeeker)
        {
            if (jobSeeker.AccountStatus != AccountStatus.Deleted)
            {
                return jobSeeker.Email;
            }

            string[] emailParts = jobSeeker.Email.Split(".");

            if (emailParts.Length > 2)
            {
                emailParts = emailParts.Take(emailParts.Length - 1).ToArray();
            }

            return string.Join(".", emailParts);
        }

        /// <summary>
        /// Extract name from this string in this format:  "XT:Doe, John AEST:IN"
        /// </summary>
        private static string GetDisplayNameClean(string displayName)
        {
            //split firstname and last name
            var split = displayName.Split(",");

            //ensure we have a comma
            if (split.Length != 2)
                return displayName;

            //split and strip unnecessary characters
            try
            {
                var firstname = split[1].Substring(0, split[1].IndexOf(":")).Trim();
                firstname = firstname.Substring(0, firstname.LastIndexOf(" ")).Trim();

                var lastname = split[0].Substring(split[0].IndexOf(":") + 1).Trim();

                //return
                return $"{firstname} {lastname}";
            }
            catch
            {
                return displayName;
            }
        }

        public async Task<JobSeekerViewProfileHistoryViewModel> GetJobSeekerHistory(string userId)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                return null;

            var history = new JobSeekerViewProfileHistoryViewModel()
            {
                History = new List<JobSeekerProfileHistory>(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                JobSeekerId = userId
            };

            List<JobSeekerChangeEvent> jobSeekerHistory =
                _jobBoardContext.JobSeekerChangeLog.Where(h => h.AspNetUserId == userId).ToList();

            IEnumerable<JobSeekerProfileHistory> logEntries =
                from log in jobSeekerHistory
                join admin in _jobBoardContext.AdminUsers on log.ModifiedByAdminUserId equals admin.Id into a
                from subAdmin in a.DefaultIfEmpty()
                select new JobSeekerProfileHistory
                {
                    Activity = log.Field,
                    DateCreated = log.DateUpdated,
                    Editor = subAdmin == null ? user.Email : GetDisplayNameClean(subAdmin.DisplayName),
                    FromValue = log.OldValue,
                    ToValue = log.NewValue,
                    Id = log.Id
                };

            var impersonations = _jobBoardContext.ImpersonationLog.Where(l => l.AspNetUserId == userId)
                .Select(l => new JobSeekerProfileHistory
                {
                    DateCreated = l.DateTokenCreated,
                    Activity = "Admin logged into job seeker account",
                    Editor = GetDisplayNameClean(_jobBoardContext.AdminUsers.FirstOrDefault(u => u.Id == l.AdminUserId).DisplayName),
                    FromValue = "-",
                    ToValue = "-",
                    Id = 0
                });

            history.History = logEntries.Union(impersonations).OrderByDescending(d => d.DateCreated).ToList();

            return history;
        }

        public async Task<string> ImpersonateUser(string userId, int adminUserId)
        {
            var token = TokenHelper.GenerateRandomStringToken();

            await _jobBoardContext.ImpersonationLog.AddAsync(new Impersonation
            {
                Token = token,
                AdminUserId =  adminUserId,
                AspNetUserId =  userId,
                DateTokenCreated = DateTime.Now
            });

            await _jobBoardContext.SaveChangesAsync();

            return token;
        }

        public async Task<IdentityResult> UnlockUser(string userId)
        {
            return await _jobSeekerRepo.UnlockUser(userId);
        }

        public async Task<JobSeeker> FindByIdAsync(string userId)
        {
            return await _jobSeekerRepo.FindByIdAsync(userId);
        }

        public async Task<IdentityResult> LockUser(string userId, int adminUserId)
        {
            return await _jobSeekerRepo.LockUser(userId, adminUserId);
        }
    }
}