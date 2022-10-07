using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Configuration;
using Serilog;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;
using WorkBC.Shared.Constants;
using WorkBC.Shared.Extensions;
using JobSource = WorkBC.Shared.Constants.JobSource;

namespace WorkBC.ElasticSearch.Indexing.Services
{
    public abstract class JobsTableSyncServiceBase
    {
        protected readonly JobBoardContext DbContext;
        protected readonly Dictionary<string, int> LocationIdLookup;
        protected readonly ILogger Logger;
        private const int MultipleLocationsId = -5;
        private const int VirtualJobsLocationId = -4;
        private readonly int _wantedJobExpiryDays;

        protected JobsTableSyncServiceBase(IConfiguration configuration, ILogger logger)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            DbContext = new JobBoardContext(connectionString);
            LocationIdLookup = DbContext.Locations
                .Where(d => !d.IsHidden && (!d.IsDuplicate || d.FederalCityId != null))
                .ToDictionary(d => d.Label, d => d.LocationId);
            Logger = logger;

            try
            {
                _wantedJobExpiryDays = configuration.GetSection("WantedSettings").Exists()
                    ? int.Parse(configuration["WantedSettings:JobExpiryDays"])
                    : General.DefaultWantedJobExpiryDays;
            } 
            catch
            {
                _wantedJobExpiryDays = General.DefaultWantedJobExpiryDays;
            }
        }

        public async Task DeactivateJobs(int jobSourceId)
        {
            List<Job> jobsToDeactivate;

            if (jobSourceId == JobSource.Wanted)
            {
                jobsToDeactivate = (from j in DbContext.Jobs
                    where j.JobSourceId == JobSource.Wanted && j.IsActive &&
                          !(from ij in DbContext.ImportedJobsWanted select ij.JobId).Contains(j.JobId)
                    select j).ToList();
            }
            else // jobSourceId == JobSource.Federal
            { 
                jobsToDeactivate = (from j in DbContext.Jobs
                    where j.JobSourceId == JobSource.Federal && j.IsActive &&
                          !(from ij in DbContext.ImportedJobsFederal select ij.JobId).Contains(j.JobId)
                    select j).ToList();
            }

            Logger.Information($"{jobsToDeactivate.Count()} jobs found to deactivate");

            //loop and deactivate
            foreach (Job jobToDeactivate in jobsToDeactivate)
            {
                DateTime now = DateTime.Now;

                // get the current job versions
                JobVersion oldVersion = DbContext.JobVersions
                    .FirstOrDefault(j => j.JobId == jobToDeactivate.JobId && j.IsCurrentVersion);

                using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (oldVersion != null)
                    {
                        oldVersion.IsCurrentVersion = false;
                        oldVersion.DateVersionEnd = now;

                        var newVersion = new JobVersion
                        {
                            JobId = jobToDeactivate.JobId,
                            DateVersionStart = now,
                            DatePosted = oldVersion.DatePosted,
                            JobSourceId = oldVersion.JobSourceId,
                            IndustryId = oldVersion.IndustryId,
                            NocCodeId = oldVersion.NocCodeId,
                            IsActive = false,
                            PositionsAvailable = oldVersion.PositionsAvailable,
                            LocationId = oldVersion.LocationId,
                            IsCurrentVersion = true,
                            ActualDatePosted = oldVersion.ActualDatePosted,
                            DateFirstImported = oldVersion.DateFirstImported,
                            VersionNumber = (short) (oldVersion.VersionNumber + 1)
                        };

                        DbContext.JobVersions.Update(oldVersion);
                        DbContext.JobVersions.Add(newVersion);
                    }

                    //deactivate job
                    jobToDeactivate.IsActive = false;
                    DbContext.Jobs.Update(jobToDeactivate);

                    Console.Write("D");

                    await DbContext.SaveChangesAsync();

                    trans.Complete();
                }
            }

            if (jobsToDeactivate.Any())
            {
                Console.WriteLine();
            }
        }

        protected bool CopyElasticJob(ElasticSearchJob elasticJob, Job job)
        {
            int locationId;
            if (elasticJob.WorkplaceType?.Id == (int)WorkplaceTypeId.Virtual)
            {
                locationId = VirtualJobsLocationId;
            }
            else
            {
                if (elasticJob.City == null || elasticJob.City.Length == 0)
                {
                    locationId = 0;
                }
                else
                {
                    locationId = elasticJob.City.Length > 1
                        ? MultipleLocationsId
                        : GetBestAvailableLocationId(elasticJob.City[0]);
                }
            }

            var nocId = Convert.ToInt16(elasticJob.Noc);

            bool newVersion = job.NocCodeId != nocId
                              || job.IndustryId != (short?)(elasticJob.NaicsId == 0 ? (int?)null : elasticJob.NaicsId)
                              || job.LocationId != locationId
                              || job.PositionsAvailable != (short)elasticJob.PositionsAvailable
                              || job.DatePosted != elasticJob.DatePosted
                              || job.ActualDatePosted != elasticJob.ActualDatePosted
                              || !job.IsActive;

            job.EmployerName = elasticJob.EmployerName.Truncate(100);
            job.City = string.Join(", ", elasticJob.City ?? new string[] { }).Truncate(120);
            job.Title = elasticJob.Title.Truncate(300);
            job.NocCodeId = nocId == 0 ? null : nocId;
            job.DatePosted = elasticJob.DatePosted;
            job.ActualDatePosted = elasticJob.ActualDatePosted;
            job.ExpireDate = elasticJob.ExpireDate ?? DateTime.Now.AddDays(_wantedJobExpiryDays);
            job.PositionsAvailable = (short)elasticJob.PositionsAvailable;
            job.Salary = elasticJob.Salary;
            job.SalarySummary = elasticJob.SalarySummary;
            job.IndustryId = (short?)(elasticJob.NaicsId == 0 ? (int?)null : elasticJob.NaicsId);
            job.LocationId = locationId;
            job.IsActive = true;

            return newVersion;
        }

        protected void IncrementJobVersion(Job job)
        {
            DateTime now = DateTime.Now;

            JobVersion oldVersion = DbContext.JobVersions
                .FirstOrDefault(j => j.JobId == job.JobId && j.IsCurrentVersion);

            if (oldVersion == null)
            {
                oldVersion = DbContext.JobVersions
                    .Where(j => j.JobId == job.JobId)
                    .OrderByDescending(j => j.VersionNumber)
                    .FirstOrDefault();
            }

            JobVersion newVersion = null;

            if (oldVersion != null)
            {
                oldVersion.IsCurrentVersion = false;
                oldVersion.DateVersionEnd = now;

                DbContext.JobVersions.Update(oldVersion);

                newVersion = new JobVersion
                {
                    JobId = job.JobId,
                    DateVersionStart = now,
                    DatePosted = job.DatePosted,
                    ActualDatePosted = job.ActualDatePosted,
                    DateFirstImported = oldVersion.DateFirstImported,
                    JobSourceId = oldVersion.JobSourceId,
                    IndustryId = job.IndustryId,
                    NocCodeId = job.NocCodeId,
                    IsActive = true,
                    PositionsAvailable = job.PositionsAvailable,
                    LocationId = job.LocationId,
                    IsCurrentVersion = true,
                    VersionNumber = (short) (oldVersion.VersionNumber + 1)
                };
            }
            else
            {
                newVersion = new JobVersion
                {
                    JobId = job.JobId,
                    DateVersionStart = now,
                    DatePosted = job.DatePosted,
                    ActualDatePosted = job.ActualDatePosted,
                    DateFirstImported = now,
                    JobSourceId = job.JobSourceId,
                    IndustryId = job.IndustryId,
                    NocCodeId = job.NocCodeId,
                    IsActive = true,
                    PositionsAvailable = job.PositionsAvailable,
                    LocationId = job.LocationId,
                    IsCurrentVersion = true,
                    VersionNumber = 1
                };
            }

            DbContext.JobVersions.Add(newVersion);
        }

        /// <summary>
        ///     Returns the minimum of DateImported and DatePosted.  However in order to prevent
        ///     dates in the distant past there is a check to make sure the date returned isn't more than
        ///     24 hours before the date imported.  Setting the timestamp to old values would make our
        ///     immutable query results mutable.
        /// </summary>
        /// <remarks>
        ///     The reports select jobs from the JobVersions table based on DatePosted, DateVersionStart
        ///     and DateVersionEnd.  This method alters the DateVersionStart to make up for the delay between
        ///     jobs being available on the Federal and Wanted API's and the time that the scheduled tasks
        ///     actually run (the tasks run 4 times per day)
        /// </remarks>
        protected DateTime GetVersion1StartDate(in DateTime datePosted, in DateTime dateImported)
        {
            if (dateImported <= datePosted)
            {
                return dateImported;
            }

            return datePosted < dateImported.AddHours(-24) 
                ? dateImported.AddHours(-24) 
                : datePosted;
        }

        /// <summary>
        ///     Gets the best available location id for a city name
        /// </summary>
        private int GetBestAvailableLocationId(string city)
        {
            city = (city ?? "").Trim().ToLower();

            // look for exact matches first
            int locationId = LocationIdLookup.FirstOrDefault(
                l => city.Equals(l.Key.ToLower())
            ).Value;

            if (locationId != 0)
            {
                // return the match if we found one
                return locationId;
            }

            // don't use fuzzy matching for really short strings.  results will be unpredictable
            if (city.Length < 5)
            {
                return 0;
            }

            // next look for matches where the city name starts with a value in our lookup table
            locationId = LocationIdLookup.FirstOrDefault(
                l => city.StartsWith(l.Key.ToLower())
            ).Value;

            if (locationId != 0)
            {
                // return the match if we found one
                return locationId;
            }

            // next look for matches where a value in our lookup table starts with the city name
            locationId = LocationIdLookup.FirstOrDefault(
                l =>
                    l.Key.ToLower().StartsWith(city)
            ).Value;

            if (locationId != 0)
            {
                // return the match if we found one
                return locationId;
            }

            // next look for matches where the city name contains a value in our lookup table
            locationId = LocationIdLookup.FirstOrDefault(
                l => city.StartsWith(l.Key.ToLower())
            ).Value;

            if (locationId != 0)
            {
                // return the match if we found one
                return locationId;
            }

            // finally look for matches where a value in our lookup table contains the city name
            locationId = LocationIdLookup.FirstOrDefault(
                l =>
                    l.Key.ToLower().StartsWith(city)
            ).Value;

            // return the match if we found one (or return 0 if we didn't find one)
            return locationId;
        }
    }
}