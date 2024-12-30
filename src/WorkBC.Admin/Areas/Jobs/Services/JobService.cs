using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WorkBC.Admin.Areas.Jobs.Models;
using WorkBC.Admin.Areas.JobSeekers.Extentions;
using WorkBC.Admin.Areas.JobSeekers.Models;
using WorkBC.Admin.Models;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;
using WorkBC.Shared.Constants;
using WorkBC.Shared.Utilities;
using JobSource = WorkBC.Shared.Constants.JobSource;

namespace WorkBC.Admin.Areas.Jobs.Services
{
    public interface IJobService
    {
        Task<(IList<JobSearchViewModel> result, int filteredResultsCount)> Search(DataTablesModel model);

        Task DeleteJob(long jobId, int currentAdminUserId);
    }

    public class JobService : IJobService
    {
        private readonly IConfiguration _configuration;
        private readonly JobBoardContext _jobBoardContext;

        public JobService(JobBoardContext jobBoardContext, IConfiguration configuration)
        {
            _jobBoardContext = jobBoardContext;
            _configuration = configuration;
        }


        public async Task<(IList<JobSearchViewModel> result,
            int filteredResultsCount)> Search(DataTablesModel model)
        {
            string searchBy = model.Search.Value;
            int take = model.Length;
            int skip = model.Start;
            string filter = model.Filter;

            var sortBy = "";
            var sortDir = true;

            if (model.Order != null)
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
            (List<JobSearchViewModel> result,
                    int filteredResultsCount) = await GetDataFromDatabase(searchBy, take, skip, sortBy, sortDir, filter);

            if (result == null)
            {
                // empty collection...
                result = new List<JobSearchViewModel>();
            }

            return (result, filteredResultsCount);
        }

        public async Task DeleteJob(long jobId, int currentAdminUserId)
        {
          using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Suppress))
            {
                #region change job status to inactive

                Job job = _jobBoardContext.Jobs.FirstOrDefault(j => j.JobId == jobId);
                if (job != null)
                {
                    job.IsActive = false;
                    _jobBoardContext.Jobs.Update(job);
                }

                #endregion

                #region add job ID to DeletedJobs table

                var deletedJob = new DeletedJob
                {
                    DateDeleted = DateTime.Now,
                    DeletedByAdminUserId = currentAdminUserId,
                    JobId = jobId
                };
                _jobBoardContext.DeletedJobs.Add(deletedJob);

                #endregion

                #region delete job from Elastic search index (english only since exernal jobs aren't in the french index)

                string index = General.EnglishIndex;
                await DeleteFromElastic(jobId.ToString(), index);

                #endregion

                #region add new record in JobVersions to keep track of changes

                JobVersion oldVersion =
                    _jobBoardContext.JobVersions.FirstOrDefault(j => j.JobId == jobId && j.IsCurrentVersion);
                if (oldVersion != null)
                {
                    oldVersion.IsCurrentVersion = false;
                    oldVersion.DateVersionEnd = DateTime.Now;

                    var newVersion = new JobVersion
                    {
                        JobId = jobId,
                        DateVersionStart = DateTime.Now,
                        DatePosted = oldVersion.DatePosted,
                        JobSourceId = oldVersion.JobSourceId,
                        IndustryId = oldVersion.IndustryId,
                        NocCodeId2021 = oldVersion.NocCodeId2021,
                        IsActive = false,
                        PositionsAvailable = oldVersion.PositionsAvailable,
                        LocationId = oldVersion.LocationId,
                        IsCurrentVersion = true,
                        VersionNumber = (short) (oldVersion.VersionNumber + 1)
                    };

                    _jobBoardContext.JobVersions.Update(oldVersion);
                    _jobBoardContext.JobVersions.Add(newVersion);
                }

                #endregion

                #region update SavedJobs to deleted

                List<SavedJob> savedJobs = _jobBoardContext.SavedJobs.Where(s => s.JobId == jobId).ToList();
                foreach (SavedJob savedJob in savedJobs)
                {
                    savedJob.DateDeleted = DateTime.Now;
                    savedJob.IsDeleted = true;

                    //Not sure if we need to add a note?
                    savedJob.Note = "Job deleted";
                    savedJob.NoteUpdatedDate = DateTime.Now;

                    _jobBoardContext.SavedJobs.Update(savedJob);
                }

                #endregion

                #region Copy job from ImportedJobsWanted to ExpiredJobs

                ImportedJobWanted importedJob = _jobBoardContext.ImportedJobsWanted
                    .FirstOrDefault(j => j.JobId == jobId);

                ExpiredJob existingExpiredJob = _jobBoardContext.ExpiredJobs
                    .FirstOrDefault(e => e.JobId == jobId);

                if (importedJob != null)
                {
                    if (existingExpiredJob == null)
                    {
                        //add new expired job
                        var expiredJob = new ExpiredJob
                        {
                            DateRemoved = DateTime.Now,
                            JobId = jobId,
                            RemovedFromElasticsearch = true
                        };
                        await _jobBoardContext.ExpiredJobs.AddAsync(expiredJob);
                    }
                    else
                    {
                        //update the job in the expired jobs 
                        existingExpiredJob.DateRemoved = DateTime.Now;
                        existingExpiredJob.RemovedFromElasticsearch = true;

                        _jobBoardContext.ExpiredJobs.Update(existingExpiredJob);
                    }

                    //remove from ImportedJobsWanted
                    _jobBoardContext.ImportedJobsWanted.Remove(importedJob);
                }

                #endregion

                //save all changes to the database
                await _jobBoardContext.SaveChangesAsync();
                //complete transaction
                trans.Complete();
            }
        }

        private async Task<(List<JobSearchViewModel> result,
                int filteredResultsCount)>
            GetDataFromDatabase(string searchBy, int take, int skip, string sortBy, bool sortDir, string filter)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                // if we have an empty search then just order the results by FirstName ascending
                sortBy = "Title";
                sortDir = true;
            }

            // apply the filter to get a queryable object
            IQueryable<Job> queryable = FilterJobs(searchBy, filter).AsExpandable();

            // apply the filter to get a queryable object with NO filter for counters
            IQueryable<Job> queryableNoFilter = FilterJobs(searchBy, "").AsExpandable();

            string jbSearchUrl = _configuration["AppSettings:JbSearchUrl"];

            // convert to viewmodel and apply sorting and pagination
            List<JobSearchViewModel> result = await queryable
                .OrderBy(sortBy, sortDir)
                .Skip(skip)
                .Take(take)
                .Select(m => new JobSearchViewModel
                {
                    JobId = m.JobId,
                    Title = m.Title,
                    ExpireDate = m.ExpireDate.ToString("yyyy-MMM-dd HH:mm") + " PST",
                    LastUpdated = m.LastUpdated.ToString("yyyy-MMM-dd HH:mm") + " PST",
                    Location = m.Location.Label,
                    DatePosted = m.DatePosted.ToString("yyyy-MMM-dd HH:mm") + " PST",
                    JobSourceId = m.JobSourceId,
                    JobSource = (m.JobSourceId == JobSource.Federal ? "Federal Job Bank" : "External Job Posting API"),
                    Url = m.JobSourceId == JobSource.Federal
                        ? $"{jbSearchUrl}/#/job-details/{m.JobId}"
                        : m.ExternalSourceUrl,
                    OriginalSource = m.OriginalSource
                })
                .ToListAsync();

            //total results found by applying the filter
            int filteredResultsCount = await queryable.CountAsync();

            return (result, filteredResultsCount);
        }

        private IQueryable<Job> FilterJobs(string searchBy, string filter)
        {
            searchBy = (searchBy ?? "").Trim().ToLower();
            filter = (filter ?? "").Trim();
            string s = searchBy + "%";

            // search on workbc.ca and jobbank.gc.ca
            if (searchBy.Contains("workbc.ca") || searchBy.Contains("jobbank.gc.ca") || searchBy.Contains("localhost") || searchBy.Contains("idir."))
            {
                // get the biggest number from the string 
                string[] numbers = Regex.Split(searchBy, @"\D+");
                if (numbers.Any())
                {
                    long jobId = numbers
                        .Where(n => !string.IsNullOrEmpty(n))
                        .Select(long.Parse)
                        .Max();

                    if (jobId > 1000000)
                    {
                        s = jobId.ToString();
                    }
                }
            }

            //search both keyword + filter
            if (s != "%" && filter != string.Empty)
            {
                switch (filter.ToLower())
                {
                    case "federal":
                        return _jobBoardContext.Jobs.Where(job =>
                            (EF.Functions.Like(job.JobId.ToString(), s)
                             || job.ExternalSourceUrl.ToLower() == searchBy)
                            && job.JobSourceId == JobSource.Federal && job.IsActive && job.ExpireDate > DateTime.Now);
                    case "external":
                        return _jobBoardContext.Jobs.Where(job =>
                            (EF.Functions.Like(job.JobId.ToString(), s)
                             || job.ExternalSourceUrl.ToLower() == searchBy)
                            && job.JobSourceId == JobSource.Wanted && job.IsActive && job.ExpireDate > DateTime.Now);
                }

                return _jobBoardContext.Jobs.Where(job =>
                    (EF.Functions.Like(job.JobId.ToString(), s)
                     || job.ExternalSourceUrl.ToLower() == searchBy)
                    && job.IsActive && job.ExpireDate > DateTime.Now);
            }

            //search by keyword only
            if (s != "%")
            {
                return _jobBoardContext.Jobs
                    .Where(job =>
                        (EF.Functions.Like(job.JobId.ToString(), s)
                         || job.ExternalSourceUrl.ToLower() == searchBy)
                        && job.IsActive && job.ExpireDate > DateTime.Now);
            }

            //search by filter only
            if (filter != string.Empty)
            {
                switch (filter.ToLower())
                {
                    case "federal":
                        return _jobBoardContext.Jobs.Where(job => job.IsActive && job.ExpireDate > DateTime.Now && job.JobSourceId == JobSource.Federal);
                    case "external":
                        return _jobBoardContext.Jobs.Where(job => job.IsActive && job.ExpireDate > DateTime.Now && job.JobSourceId == JobSource.Wanted);
                }
            }

            return _jobBoardContext.Jobs.Where(job => job.IsActive && job.ExpireDate > DateTime.Now);
        }

        private async Task<string> DeleteFromElastic(string id, string index)
        {
            string server = _configuration["ConnectionStrings:ElasticSearchServer"];
            string docType = General.ElasticDocType;
            string url = $"{server}/{index}/{docType}/{id}";

            try
            {
                return await new ElasticHttpHelper(_configuration).PostToElasticSearch(string.Empty, url, "DELETE");
            }
            catch (WebException)
            {
                Thread.Sleep(15000); //wait 15 seconds and try again
                return await new ElasticHttpHelper(_configuration).PostToElasticSearch(string.Empty, url, "DELETE");
            }
        }
    }
}
