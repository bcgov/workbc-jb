using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Shared.Repositories
{
    /// <summary>
    ///     Helper methods for generating JobSeekerVersions.  This is in the shared library
    ///     because Job Seekers can be edited from both the public project and the admin project.
    /// </summary>
    public class JobSeekerVersionRepository
    {
        private readonly JobBoardContext _jobBoardContext;

        public JobSeekerVersionRepository(JobBoardContext jobBoardContext)
        {
            _jobBoardContext = jobBoardContext;
        }

        /// <summary>
        ///     Checks if a new version if required and creates one.
        /// </summary>
        public async Task<bool> CreateNewVersionIfNeeded(JobSeeker jobSeeker, JobSeekerFlags jobSeekerFlags)
        {
            JobSeekerVersion oldVersion = await
                _jobBoardContext.JobSeekerVersions.FirstOrDefaultAsync(
                    jv => jv.AspNetUserId == jobSeeker.Id && (jv.IsCurrentVersion || jv.DateVersionEnd == null));

            // if there is no old version then create version 1
            if (oldVersion == null)
            {
                return await CreateVersion(null, jobSeeker, jobSeekerFlags);
            }

            // if anything has changed since the old version then create a new version
            if (oldVersion.Email != jobSeeker.Email ||
                oldVersion.CountryId != jobSeeker.CountryId ||
                oldVersion.ProvinceId != jobSeeker.ProvinceId ||
                oldVersion.LocationId != jobSeeker.LocationId ||
                oldVersion.DateRegistered != jobSeeker.DateRegistered ||
                oldVersion.AccountStatus != jobSeeker.AccountStatus ||
                oldVersion.EmailConfirmed != jobSeeker.EmailConfirmed ||
                (jobSeekerFlags != null && (
                oldVersion.IsApprentice != jobSeekerFlags.IsApprentice ||
                oldVersion.IsIndigenousPerson != jobSeekerFlags.IsIndigenousPerson ||
                oldVersion.IsMatureWorker != jobSeekerFlags.IsMatureWorker ||
                oldVersion.IsNewImmigrant != jobSeekerFlags.IsNewImmigrant ||
                oldVersion.IsPersonWithDisability != jobSeekerFlags.IsPersonWithDisability ||
                oldVersion.IsStudent != jobSeekerFlags.IsStudent ||
                oldVersion.IsVeteran != jobSeekerFlags.IsVeteran ||
                oldVersion.IsVisibleMinority != jobSeekerFlags.IsVisibleMinority ||
                oldVersion.IsYouth != jobSeekerFlags.IsYouth)))
            {
                return await CreateVersion(oldVersion, jobSeeker, jobSeekerFlags);
            }

            return false;
        }

        /// <summary>
        ///     Data manipulation routine for creating a new version and updating the old version
        /// </summary>
        private async Task<bool> CreateVersion(JobSeekerVersion oldVersion, JobSeeker jobSeeker,
            JobSeekerFlags jobSeekerFlags)
        {
            DateTime now = DateTime.Now;

            var newVersion = new JobSeekerVersion
            {
                AspNetUserId = jobSeeker.Id,
                Email = jobSeeker.Email,
                CountryId = jobSeeker.CountryId,
                ProvinceId = jobSeeker.ProvinceId,
                LocationId = jobSeeker.LocationId,
                DateRegistered = jobSeeker.DateRegistered,
                AccountStatus = jobSeeker.AccountStatus,
                EmailConfirmed = jobSeeker.EmailConfirmed,
                IsCurrentVersion = true,
                DateVersionStart = now
            };

            if (jobSeekerFlags != null)
            {
                newVersion.IsApprentice = jobSeekerFlags.IsApprentice;
                newVersion.IsIndigenousPerson = jobSeekerFlags.IsIndigenousPerson;
                newVersion.IsMatureWorker = jobSeekerFlags.IsMatureWorker;
                newVersion.IsNewImmigrant = jobSeekerFlags.IsNewImmigrant;
                newVersion.IsPersonWithDisability = jobSeekerFlags.IsPersonWithDisability;
                newVersion.IsStudent = jobSeekerFlags.IsStudent;
                newVersion.IsVeteran = jobSeekerFlags.IsVeteran;
                newVersion.IsVisibleMinority = jobSeekerFlags.IsVisibleMinority;
                newVersion.IsYouth = jobSeekerFlags.IsYouth;
            } 
            else if (oldVersion != null)
            {
                newVersion.IsApprentice = oldVersion.IsApprentice;
                newVersion.IsIndigenousPerson = oldVersion.IsIndigenousPerson;
                newVersion.IsMatureWorker = oldVersion.IsMatureWorker;
                newVersion.IsNewImmigrant = oldVersion.IsNewImmigrant;
                newVersion.IsPersonWithDisability = oldVersion.IsPersonWithDisability;
                newVersion.IsStudent = oldVersion.IsStudent;
                newVersion.IsVeteran = oldVersion.IsVeteran;
                newVersion.IsVisibleMinority = oldVersion.IsVisibleMinority;
                newVersion.IsYouth = oldVersion.IsYouth;
            }

            if (oldVersion != null)
            {
                oldVersion.IsCurrentVersion = false;
                oldVersion.DateVersionEnd = now;
                newVersion.VersionNumber = (short) (oldVersion.VersionNumber + 1);

                // save the old version
                _jobBoardContext.JobSeekerVersions.Update(oldVersion);
            }
            else
            {
                newVersion.VersionNumber = 1;
            }

            // save the new version
            await _jobBoardContext.AddAsync(newVersion);

            return true;
        }
    }
}