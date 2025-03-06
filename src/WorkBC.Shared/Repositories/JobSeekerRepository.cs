using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkBC.Data;
using WorkBC.Data.Enums;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Shared.Repositories
{
    /// <summary>
    ///     Helper methods for creating and updating JobSeekers.  This is in the shared library
    ///     because Job Seekers can be edited from both the public project and the admin project.
    /// </summary>
    public class JobSeekerRepository
    {
        private readonly JobBoardContext _context;
        private readonly UserManager<JobSeeker> _userManager;
        private readonly JobSeekerVersionRepository _versionRepo;

        public JobSeekerRepository(JobBoardContext context, UserManager<JobSeeker> userManager)
        {
            _context = context;
            _userManager = userManager;
            _versionRepo = new JobSeekerVersionRepository(context);
        }

        public async Task<JobSeeker> FindByIdAsync(string userId)
        {
            return (await _userManager.FindByIdAsync(userId));
        }

        public async Task<(JobSeeker, IdentityResult)> CreateUserAsync(JobSeeker jobSeeker, string password, int? adminUserId = null)
        {
            // set date fields
            var now = DateTime.Now;
            jobSeeker.DateRegistered = now;
            jobSeeker.LastModified = now;

            // make sure there are no zeroes where there should be nulls
            RemoveInvalidForeignKeys(jobSeeker);

            // create the user
            IdentityResult result = await _userManager.CreateAsync(jobSeeker, password);

            // get the user back from the db
            jobSeeker = await _userManager.FindByEmailAsync(jobSeeker.Email);

            //check if the jobSeeker could be created 
            if (jobSeeker != null)
            {
                // log the event
                var eventActivateUser = new JobSeekerEvent
                {
                    AspNetUserId = jobSeeker.Id,
                    DateLogged = DateTime.Now,
                    EventTypeId = EventType.Register
                };
                await _context.JobSeekerEventLog.AddAsync(eventActivateUser);

                //job seeker admin event log
                var changeEventCreateUser = new JobSeekerChangeEvent()
                {
                    Field = "Job seeker created",
                    OldValue = "-",
                    NewValue = "-",
                    AspNetUserId = jobSeeker.Id,
                    DateUpdated = DateTime.Now,
                    ModifiedByAdminUserId = adminUserId
                };
                await _context.JobSeekerChangeLog.AddAsync(changeEventCreateUser);

                // create a new version record
                await _versionRepo.CreateNewVersionIfNeeded(jobSeeker, jobSeeker.JobSeekerFlags);
                await _context.SaveChangesAsync();

            }

            return (jobSeeker, result);
        }

        /// <summary>
        ///     Removes invalid zeros from foreign key columns
        /// </summary>
        private static void RemoveInvalidForeignKeys(JobSeeker jobSeeker)
        {
            if (jobSeeker.ProvinceId == 0)
            {
                jobSeeker.ProvinceId = null;
            }

            if (jobSeeker.LocationId == 0)
            {
                jobSeeker.LocationId = null;
                jobSeeker.City = string.Empty;
            }

            if (jobSeeker.CountryId == 0)
            {
                jobSeeker.CountryId = null;
            }
        }

        public async Task<IdentityResult> UpdateJobSeekerAsync(JobSeeker jobSeeker, JobSeeker userParam, string password = "", int? adminUserId = null, int? regionId = null)
        {
            // don't allow the jobseeker to be updated if they have been deleted in another browser window.  
            if (jobSeeker.AccountStatus == AccountStatus.Deleted)
            {
                throw new InvalidDataException(
                    $"Cannot complete the update operation.  The user has been deleted. [{jobSeeker.Email}/{jobSeeker.Id}]");
            }

            IdentityResult result = IdentityResult.Success;

            // make sure there are no zeroes where there should be nulls
            RemoveInvalidForeignKeys(userParam);

            // update user properties
            bool modified = await ApplyJobSeekerChanges(jobSeeker, userParam, adminUserId, regionId);

            modified = modified | await ApplyJobSeekerFlagsChangesAsync(jobSeeker,userParam, adminUserId);

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                await _userManager.AddPasswordAsync(jobSeeker, password);
                modified = true;
            }

            // set the LastModified date
            if (modified)
            {
                jobSeeker.LastModified = DateTime.Now;
            }

            await _versionRepo.CreateNewVersionIfNeeded(jobSeeker, userParam.JobSeekerFlags);

            // save the changes
            result = await _userManager.UpdateAsync(jobSeeker);
            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<IdentityResult> DeleteJobSeekerAsync(string userId, int? adminUserId = null)
        {
            JobSeeker jobSeeker = await _userManager.FindByIdAsync(userId);

            if (jobSeeker == null)
            {
                return IdentityResult.Failed();
            }

            // get the uniqueIndexer to add the the username and email
            var username = jobSeeker.Email.ToLower();
            var uniqueIndexer = 0;
            while (await _userManager.FindByEmailAsync($"{username}.{uniqueIndexer:0000}") != null)
            {
                uniqueIndexer++;
            }

            // add a digit to the email address and username so they can be used again
            jobSeeker.UserName = $"{jobSeeker.UserName.ToLower()}.{uniqueIndexer:0000}";
            jobSeeker.Email = $"{jobSeeker.Email}.{uniqueIndexer:0000}";

            // clear the security question & answer (no need to store this anymore)
            jobSeeker.SecurityAnswer = null;
            jobSeeker.SecurityQuestionId = null;

            // deactivate the account
            jobSeeker.AccountStatus = AccountStatus.Deleted;
            jobSeeker.LockoutEnabled = true;
            jobSeeker.LockoutEnd = DateTimeOffset.MaxValue;
            jobSeeker.EmailConfirmed = false;

            //update user
            //job seeker admin event log
            var changeEventDeleteUser = new JobSeekerChangeEvent
            {
                Field = "Job seeker deleted",
                OldValue = "-",
                NewValue = "-",
                AspNetUserId = jobSeeker.Id,
                DateUpdated = DateTime.Now,
                ModifiedByAdminUserId = adminUserId
            };
            await _context.JobSeekerChangeLog.AddAsync(changeEventDeleteUser);

            var jsEventDeleteUser = new JobSeekerEvent
            {
                AspNetUserId = jobSeeker.Id,
                DateLogged = DateTime.Now,
                EventTypeId = EventType.DeleteAccount,
            };
            await _context.JobSeekerEventLog.AddAsync(jsEventDeleteUser);

            jobSeeker.LastModified = DateTime.Now;
            await _versionRepo.CreateNewVersionIfNeeded(jobSeeker, jobSeeker.JobSeekerFlags);
            IdentityResult result = await _userManager.UpdateAsync(jobSeeker);
            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<IdentityResult> ConfirmEmailAsync(JobSeeker jobSeeker, string token)
        {
            // update these two fields
            jobSeeker.LastModified = DateTime.Now;
            jobSeeker.AccountStatus = AccountStatus.Active;

            await LogJobSeekerEvent(jobSeeker.Id, EventType.ConfirmEmail, false);

            // confirm the email
            IdentityResult result = await _userManager.ConfirmEmailAsync(jobSeeker, token);

            // get a new copy of the jobseeker object
            jobSeeker = await _userManager.FindByIdAsync(jobSeeker.Id);
            await _versionRepo.CreateNewVersionIfNeeded(jobSeeker, jobSeeker.JobSeekerFlags);
            await _context.SaveChangesAsync();
            return result;
        }

        public async Task LogJobSeekerEvent(string aspNetUserId, EventType eventType, bool saveChanges = true)
        {
            // log the event for statistical reporting purposes
            var eventActivateUser = new JobSeekerEvent
            {
                AspNetUserId = aspNetUserId,
                DateLogged = DateTime.Now,
                EventTypeId = eventType
            };
            await _context.JobSeekerEventLog.AddAsync(eventActivateUser);

            if (eventType == EventType.ConfirmEmail)
            {
                // log the event again for the profile history (for certain envent types)
                var changeEventActivateAccount = new JobSeekerChangeEvent
                {
                    Field = "Account status modified",
                    OldValue = AccountStatus.Pending.ToString(),
                    NewValue = AccountStatus.Active.ToString(),
                    AspNetUserId = aspNetUserId,
                    DateUpdated = DateTime.Now,
                    ModifiedByAdminUser = null
                };
                await _context.JobSeekerChangeLog.AddAsync(changeEventActivateAccount);
            }

            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
        }

        private async Task<bool> ApplyJobSeekerChanges(JobSeeker jobSeeker, JobSeeker userParam, int? adminUserId, int? regionId)
        {
            var modified = false;

            // clear the city if the province isn't BC
            if (userParam.ProvinceId != 2)
            {
                userParam.LocationId = null;
                userParam.City = string.Empty;
            }

            if (jobSeeker.FirstName != userParam.FirstName)
            {
                var changeEventFirstname = new JobSeekerChangeEvent
                {
                    Field = "First name edited",
                    OldValue = jobSeeker.FirstName,
                    NewValue = userParam.FirstName,
                    AspNetUserId = jobSeeker.Id,
                    DateUpdated = DateTime.Now,
                    ModifiedByAdminUserId = adminUserId
                };
                await _context.JobSeekerChangeLog.AddAsync(changeEventFirstname);

                jobSeeker.FirstName = userParam.FirstName;
                modified = true;
            }

            if (jobSeeker.LastName != userParam.LastName)
            {
                var changeEventLastName = new JobSeekerChangeEvent()
                {
                    Field = "Last name edited",
                    OldValue = jobSeeker.LastName,
                    NewValue = userParam.LastName,
                    AspNetUserId = jobSeeker.Id,
                    DateUpdated = DateTime.Now,
                    ModifiedByAdminUserId = adminUserId
                };
                await _context.JobSeekerChangeLog.AddAsync(changeEventLastName);

                jobSeeker.LastName = userParam.LastName;
                modified = true;
            }

            if (jobSeeker.Email != userParam.Email)
            {
                var changeEventEmail = new JobSeekerChangeEvent
                {
                    Field = "Email edited",
                    OldValue = jobSeeker.Email,
                    NewValue = userParam.Email,
                    AspNetUserId = jobSeeker.Id,
                    DateUpdated = DateTime.Now,
                    ModifiedByAdminUserId = adminUserId
                };
                await _context.JobSeekerChangeLog.AddAsync(changeEventEmail);

                jobSeeker.Email = userParam.Email;
                modified = true;
            }

            if (jobSeeker.UserName != userParam.UserName)
            {
                jobSeeker.UserName = userParam.UserName;
                modified = true;
            }

            if (jobSeeker.CountryId != userParam.CountryId)
            {
                jobSeeker.Country = _context.Countries.FirstOrDefault(c => c.Id == jobSeeker.CountryId);
                userParam.Country = _context.Countries.FirstOrDefault(c => c.Id == userParam.CountryId);

                var changeEventCountryId = new JobSeekerChangeEvent
                {
                    Field = "Country edited",
                    OldValue = jobSeeker.Country?.Name ?? "-",
                    NewValue = userParam.Country?.Name ?? "-",
                    AspNetUserId = jobSeeker.Id,
                    DateUpdated = DateTime.Now,
                    ModifiedByAdminUserId = adminUserId
                };
                await _context.JobSeekerChangeLog.AddAsync(changeEventCountryId);

                jobSeeker.CountryId = userParam.CountryId.HasValue && userParam.CountryId.Value != 0
                    ? userParam.CountryId
                    : null;
                jobSeeker.Country = null;

                modified = true;
            }

            if (jobSeeker.ProvinceId != userParam.ProvinceId)
            {
                jobSeeker.Province = _context.Provinces.FirstOrDefault(p => p.ProvinceId == jobSeeker.ProvinceId);
                userParam.Province = _context.Provinces.FirstOrDefault(p => p.ProvinceId == userParam.ProvinceId);

                if (jobSeeker.Province != null || userParam.Province != null)
                {
                    var changeEventProvinceId = new JobSeekerChangeEvent
                    {
                        Field = "Province edited",
                        OldValue = jobSeeker.Province?.Name ?? "-",
                        NewValue = userParam.Province?.Name ?? "-",
                        AspNetUserId = jobSeeker.Id,
                        DateUpdated = DateTime.Now,
                        ModifiedByAdminUserId = adminUserId
                    };
                    await _context.JobSeekerChangeLog.AddAsync(changeEventProvinceId);
                }

                jobSeeker.ProvinceId = userParam.ProvinceId.HasValue && userParam.ProvinceId.Value != -1
                    ? userParam.ProvinceId
                    : null;
                jobSeeker.Province = null;

                modified = true;
            }

            if (jobSeeker.LocationId != userParam.LocationId)
            {
                userParam.Location = await _context.Locations.FirstOrDefaultAsync(c => c.LocationId == userParam.LocationId);
                jobSeeker.Location = await _context.Locations.FirstOrDefaultAsync(c => c.LocationId == jobSeeker.LocationId);

                var changeEventCity = new JobSeekerChangeEvent
                {
                    Field = "City edited",
                    OldValue = jobSeeker.Location?.Label ?? "-",
                    NewValue = userParam.Location?.Label ?? "-",
                    AspNetUserId = jobSeeker.Id,
                    DateUpdated = DateTime.Now,
                    ModifiedByAdminUserId = adminUserId
                };
                await _context.JobSeekerChangeLog.AddAsync(changeEventCity);

                jobSeeker.LocationId = userParam.LocationId;
                jobSeeker.City = userParam.Location?.City;
                jobSeeker.Location = null;
                modified = true;
            }

            if (regionId != null && regionId > 0)
            {
                //set location based on the region selected

                //find city name
                var location = from loc in _context.Locations
                               where loc.LocationId == userParam.LocationId
                               select loc;
                var city = location.FirstOrDefault()?.City;

                if (city != null)
                {
                    //find new location based on city name and region ID
                    var newLocation = from loc in _context.Locations
                                      where loc.City == city && loc.RegionId == regionId
                                      select loc;

                    jobSeeker.LocationId = newLocation.FirstOrDefault()?.LocationId;

                    modified = true;
                }
            }
            else if (jobSeeker.LocationId != userParam.LocationId)
            {
                //not creating an admin event log here
                //the location translate to the City field, and we do log the city field.

                jobSeeker.LocationId = userParam.LocationId.HasValue && userParam.LocationId.Value != 0
                    ? userParam.LocationId
                    : null;

                modified = true;
            }

            if (jobSeeker.SecurityQuestionId != userParam.SecurityQuestionId)
            {

                jobSeeker.SecurityQuestion = _context.SecurityQuestions.FirstOrDefault(p => p.Id == jobSeeker.SecurityQuestionId);
                userParam.SecurityQuestion = _context.SecurityQuestions.FirstOrDefault(p => p.Id == userParam.SecurityQuestionId);

                if (jobSeeker.SecurityQuestion != null || userParam.SecurityQuestion != null)
                {
                    var changeEventSecurityQuestionId = new JobSeekerChangeEvent
                    {
                        Field = "Security question edited",
                        OldValue = jobSeeker.SecurityQuestion?.QuestionText ?? "-",
                        NewValue = userParam.SecurityQuestion?.QuestionText ?? "-",
                        AspNetUserId = jobSeeker.Id,
                        DateUpdated = DateTime.Now,
                        ModifiedByAdminUserId = adminUserId
                    };
                    await _context.JobSeekerChangeLog.AddAsync(changeEventSecurityQuestionId);
                }

                jobSeeker.SecurityQuestionId = userParam.SecurityQuestionId;
                jobSeeker.SecurityQuestion = null;
                modified = true;
            }

            if (jobSeeker.SecurityAnswer != userParam.SecurityAnswer)
            {
                var changeEventSecurityAnswer = new JobSeekerChangeEvent()
                {
                    Field = "Security answer edited",
                    OldValue = new string('*', jobSeeker.SecurityAnswer?.Length ?? 0),
                    NewValue = new string('*', userParam.SecurityAnswer?.Length ?? 0),
                    AspNetUserId = jobSeeker.Id,
                    DateUpdated = DateTime.Now,
                    ModifiedByAdminUserId = adminUserId
                };
                await _context.JobSeekerChangeLog.AddAsync(changeEventSecurityAnswer);

                jobSeeker.SecurityAnswer = userParam.SecurityAnswer;
                modified = true;
            }

            if (jobSeeker.AccountStatus != userParam.AccountStatus && userParam.AccountStatus != AccountStatus.InvalidStatusZero)
            {
                // log deactivation to the JobSeekerEventLog
                if (jobSeeker.AccountStatus == AccountStatus.Active &&
                    userParam.AccountStatus == AccountStatus.Deactivated)
                {
                    var eventDeactivateUser = new JobSeekerEvent
                    {
                        AspNetUserId = jobSeeker.Id,
                        DateLogged = DateTime.Now,
                        EventTypeId = EventType.AdminDeactivateAccount
                    };
                    await _context.JobSeekerEventLog.AddAsync(eventDeactivateUser);
                }

                // log re-activation to the JobSeekerEventLog
                if ((jobSeeker.AccountStatus == AccountStatus.Deactivated ||
                     jobSeeker.AccountStatus == AccountStatus.InvalidStatusZero) &&
                    userParam.AccountStatus == AccountStatus.Active)
                {
                    var eventActivateUser = new JobSeekerEvent
                    {
                        AspNetUserId = jobSeeker.Id,
                        DateLogged = DateTime.Now,
                        EventTypeId = EventType.AdminActivateAccount
                    };
                    await _context.JobSeekerEventLog.AddAsync(eventActivateUser);
                }

                // log re-activation to the JobSeekerEventLog
                if (jobSeeker.AccountStatus == AccountStatus.Pending &&
                    userParam.AccountStatus == AccountStatus.Active)
                {
                    var eventActivateUser = new JobSeekerEvent
                    {
                        AspNetUserId = jobSeeker.Id,
                        DateLogged = DateTime.Now,
                        EventTypeId = EventType.ConfirmEmail
                    };
                    await _context.JobSeekerEventLog.AddAsync(eventActivateUser);
                }

                // log any admin changes to JobSeekerAdminLog
                var changeEventCountryId = new JobSeekerChangeEvent()
                {
                    Field = "Account status modified",
                    OldValue = jobSeeker.AccountStatus.ToString(),
                    NewValue = userParam.AccountStatus.ToString(),
                    AspNetUserId = jobSeeker.Id,
                    DateUpdated = DateTime.Now,
                    ModifiedByAdminUserId = adminUserId
                };
                await _context.JobSeekerChangeLog.AddAsync(changeEventCountryId);

                jobSeeker.AccountStatus = userParam.AccountStatus;
                modified = true;
            }

            return modified;
        }

        private async Task<bool> ApplyJobSeekerFlagsChangesAsync(JobSeeker jobSeeker, JobSeeker userParam, int? adminUserId)
        {
            var modified = false;
            if (userParam.JobSeekerFlags != null)
            {
                JobSeekerFlags flags = await _context.JobSeekerFlags.FirstOrDefaultAsync(x => x.AspNetUserId == jobSeeker.Id);

                // if flags is not found then create 
                if (flags == null)
                {
                    flags = new JobSeekerFlags
                    {
                        AspNetUserId = jobSeeker.Id
                    };
                    await _context.JobSeekerFlags.AddAsync(flags);
                }

                if (flags.IsApprentice != jobSeeker.JobSeekerFlags.IsApprentice)
                {
                    var changeEventIsApprentice = new JobSeekerChangeEvent
                    {
                        Field = "Apprentice edited",
                        OldValue = flags.IsApprentice ? "Yes" : "No",
                        NewValue = jobSeeker.JobSeekerFlags.IsApprentice ? "Yes" : "No",
                        AspNetUserId = jobSeeker.Id,
                        DateUpdated = DateTime.Now,
                        ModifiedByAdminUserId = adminUserId
                    };
                    await _context.JobSeekerChangeLog.AddAsync(changeEventIsApprentice);

                    flags.IsApprentice = jobSeeker.JobSeekerFlags.IsApprentice;
                    modified = true;
                }

                if (flags.IsIndigenousPerson != jobSeeker.JobSeekerFlags.IsIndigenousPerson)
                {
                    var changeEventIsIndigenousPerson = new JobSeekerChangeEvent
                    {
                        Field = "Indigenous person edited",
                        OldValue = flags.IsIndigenousPerson ? "Yes" : "No",
                        NewValue = jobSeeker.JobSeekerFlags.IsIndigenousPerson ? "Yes" : "No",
                        AspNetUserId = jobSeeker.Id,
                        DateUpdated = DateTime.Now,
                        ModifiedByAdminUserId = adminUserId
                    };
                    await _context.JobSeekerChangeLog.AddAsync(changeEventIsIndigenousPerson);

                    flags.IsIndigenousPerson = jobSeeker.JobSeekerFlags.IsIndigenousPerson;
                    modified = true;
                }

                if (flags.IsMatureWorker != jobSeeker.JobSeekerFlags.IsMatureWorker)
                {
                    var changeEventIsMature = new JobSeekerChangeEvent()
                    {
                        Field = "Mature edited",
                        OldValue = flags.IsMatureWorker ? "Yes" : "No",
                        NewValue = jobSeeker.JobSeekerFlags.IsMatureWorker ? "Yes" : "No",
                        AspNetUserId = jobSeeker.Id,
                        DateUpdated = DateTime.Now,
                        ModifiedByAdminUserId = adminUserId
                    };
                    await _context.JobSeekerChangeLog.AddAsync(changeEventIsMature);

                    flags.IsMatureWorker = jobSeeker.JobSeekerFlags.IsMatureWorker;
                    modified = true;
                }

                if (flags.IsNewImmigrant != jobSeeker.JobSeekerFlags.IsNewImmigrant)
                {
                    var changeEventIsNewImmigrant = new JobSeekerChangeEvent
                    {
                        Field = "Newcomer to B.C. edited",
                        OldValue = flags.IsNewImmigrant ? "Yes" : "No",
                        NewValue = jobSeeker.JobSeekerFlags.IsNewImmigrant ? "Yes" : "No",
                        AspNetUserId = jobSeeker.Id,
                        DateUpdated = DateTime.Now,
                        ModifiedByAdminUserId = adminUserId
                    };
                    await _context.JobSeekerChangeLog.AddAsync(changeEventIsNewImmigrant);

                    flags.IsNewImmigrant = jobSeeker.JobSeekerFlags.IsNewImmigrant;
                    modified = true;
                }

                if (flags.IsPersonWithDisability != jobSeeker.JobSeekerFlags.IsPersonWithDisability)
                {
                    var changeEventIsPersonWithDisability = new JobSeekerChangeEvent
                    {
                        Field = "Person with a disability edited",
                        OldValue = flags.IsPersonWithDisability ? "Yes" : "No",
                        NewValue = jobSeeker.JobSeekerFlags.IsPersonWithDisability ? "Yes" : "No",
                        AspNetUserId = jobSeeker.Id,
                        DateUpdated = DateTime.Now,
                        ModifiedByAdminUserId = adminUserId
                    };
                    await _context.JobSeekerChangeLog.AddAsync(changeEventIsPersonWithDisability);

                    flags.IsPersonWithDisability = jobSeeker.JobSeekerFlags.IsPersonWithDisability;
                    modified = true;
                }

                if (flags.IsStudent != jobSeeker.JobSeekerFlags.IsStudent)
                {
                    var changeEventIsStudent = new JobSeekerChangeEvent()
                    {
                        Field = "Student edited",
                        OldValue = flags.IsStudent ? "Yes" : "No",
                        NewValue = jobSeeker.JobSeekerFlags.IsStudent ? "Yes" : "No",
                        AspNetUserId = jobSeeker.Id,
                        DateUpdated = DateTime.Now,
                        ModifiedByAdminUserId = adminUserId
                    };
                    await _context.JobSeekerChangeLog.AddAsync(changeEventIsStudent);

                    flags.IsStudent = jobSeeker.JobSeekerFlags.IsStudent;
                    modified = true;
                }

                if (flags.IsVeteran != jobSeeker.JobSeekerFlags.IsVeteran)
                {
                    var changeEventIsVeteran = new JobSeekerChangeEvent()
                    {
                        Field = "Veteran of the Canadian Armed Forces edited",
                        OldValue = flags.IsVeteran ? "Yes" : "No",
                        NewValue = jobSeeker.JobSeekerFlags.IsVeteran ? "Yes" : "No",
                        AspNetUserId = jobSeeker.Id,
                        DateUpdated = DateTime.Now,
                        ModifiedByAdminUserId = adminUserId
                    };
                    await _context.JobSeekerChangeLog.AddAsync(changeEventIsVeteran);

                    flags.IsVeteran = jobSeeker.JobSeekerFlags.IsVeteran;
                    modified = true;
                }

                if (flags.IsVisibleMinority != jobSeeker.JobSeekerFlags.IsVisibleMinority)
                {
                    var changeEventIsVisibleMinority = new JobSeekerChangeEvent()
                    {
                        Field = "Visible minority edited",
                        OldValue = flags.IsVisibleMinority ? "Yes" : "No",
                        NewValue = jobSeeker.JobSeekerFlags.IsVisibleMinority ? "Yes" : "No",
                        AspNetUserId = jobSeeker.Id,
                        DateUpdated = DateTime.Now,
                        ModifiedByAdminUserId = adminUserId
                    };
                    await _context.JobSeekerChangeLog.AddAsync(changeEventIsVisibleMinority);

                    flags.IsVisibleMinority = jobSeeker.JobSeekerFlags.IsVisibleMinority;
                    modified = true;
                }

                if (flags.IsYouth != jobSeeker.JobSeekerFlags.IsYouth)
                {
                    var changeEventIsYouth = new JobSeekerChangeEvent()
                    {
                        Field = "Youth edited",
                        OldValue = flags.IsYouth ? "Yes" : "No",
                        NewValue = jobSeeker.JobSeekerFlags.IsYouth ? "Yes" : "No",
                        AspNetUserId = jobSeeker.Id,
                        DateUpdated = DateTime.Now,
                        ModifiedByAdminUserId = adminUserId
                    };
                    await _context.JobSeekerChangeLog.AddAsync(changeEventIsYouth);

                    flags.IsYouth = jobSeeker.JobSeekerFlags.IsYouth;
                    modified = true;
                }
                jobSeeker.JobSeekerFlags = flags;
            }
            return modified;
        }

        public async Task<IdentityResult> UnlockUser(string userId)
        {
            JobSeeker js = await _userManager.FindByIdAsync(userId);

            if (js?.LockedByAdminUserId != null)
            {
                js.LockedByAdminUserId = null;
                js.LockedByAdminUser = null;
                js.DateLocked = null;

                return await _userManager.UpdateAsync(js);
            }

            return IdentityResult.Failed();
        }

        public async Task<IdentityResult> LockUser(string userId, int adminUserId)
        {
            JobSeeker js = await _userManager.FindByIdAsync(userId);

            if (js != null)
            {
                js.LockedByAdminUserId = adminUserId;
                js.DateLocked = DateTime.Now;

                return await _userManager.UpdateAsync(js);
            }

            return IdentityResult.Failed();
        }
    }
}
