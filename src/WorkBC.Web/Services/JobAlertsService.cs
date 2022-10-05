using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using WorkBC.Data;
using WorkBC.Data.Enums;
using WorkBC.Data.Model.JobBoard;
using WorkBC.ElasticSearch.Models.Filters;
using WorkBC.ElasticSearch.Search.Queries;
using WorkBC.Shared.Extensions;
using WorkBC.Shared.Services;
using WorkBC.Web.Helpers;
using WorkBC.Web.Models;
using ILogger = Serilog.ILogger;

namespace WorkBC.Web.Services
{
    public interface IJobAlertsService
    {
        Task SaveJobAlertAsync(JobAlertModel jobAlertModel, int? adminUserId);
        Task<int> GetJobAlertsTotalAsync(string userId);
        IOrderedQueryable<JobAlert> GetJobAlertsAsync(string userId);
        JobAlert GetJobAlert(string userId, int id);
        Task DeleteJobAlertAsync(string userId, int? adminUserId, int id);
        Task<long> GetTotalMatchedJobsAsync(string filtersStr);
        Task<string> GetUrlParameters(int jobAlertId, string aspNetUserId);
    }

    public class JobAlertsService : IJobAlertsService
    {
        private readonly JobBoardContext _context;
        private readonly IUserService _userService;

        private readonly IConfiguration _configuration;
        private readonly IGeocodingService _geocodingService;

        public JobAlertsService(JobBoardContext context, IUserService userService,
            IConfiguration configuration, IGeocodingService geocodingService)
        {
            _context = context;
            _userService = userService;

            _configuration = configuration;
            _geocodingService = geocodingService;
        }

        public async Task DeleteJobAlertAsync(string userId, int? adminUserId, int id)
        {
            var jobAlert = await _context.JobAlerts.FirstOrDefaultAsync(x =>
                x.Id == id && x.AspNetUserId == userId && !x.IsDeleted);
            if (jobAlert != null)
            {
                jobAlert.IsDeleted = true;
                jobAlert.DateDeleted = DateTime.Now;
                await _context.SaveChangesAsync();

                var logEntry = new JobSeekerChangeEvent
                {
                    AspNetUserId = userId,
                    ModifiedByAdminUserId = adminUserId,
                    DateUpdated = DateTime.Now,
                    Field = $"Job alert '{jobAlert.Title}' deleted",
                    OldValue = "-",
                    NewValue = "-"
                };

                await _context.JobSeekerChangeLog.AddAsync(logEntry);
                await _context.SaveChangesAsync();
            }
        }

        public JobAlert GetJobAlert(string userId, int id)
        {
            var result = _context.JobAlerts.FirstOrDefault(x => x.AspNetUserId == userId && x.Id == id && !x.IsDeleted);
            return result;
        }

        public IOrderedQueryable<JobAlert> GetJobAlertsAsync(string userId)
        {
            var result = _context.JobAlerts.Where(x => x.AspNetUserId == userId && !x.IsDeleted).OrderByDescending(x => x.Id);
            return result;
        }

        public async Task<int> GetJobAlertsTotalAsync(string userId)
        {
            var result = await _context.JobAlerts.Where(x => x.AspNetUserId == userId && !x.IsDeleted).CountAsync();
            return result;
        }

        public async Task<long> GetTotalMatchedJobsAsync(string filtersStr)
        {
            var filters = JsonConvert.DeserializeObject<JobSearchFilters>(filtersStr);
            filters.PageSize = 0;

            //Search object that we will use to search Elastic Search
            var esq = new JobSearchQuery(_geocodingService, _configuration, filters);

            //Get search results from Elastic search
            var results = await esq.GetSearchResults();

            var result = results?.Hits?.Total?.Value ?? 0;
            return result;
        }

        public async Task<string> GetUrlParameters(int jobAlertId, string aspNetUserId)
        {
            return (await _context.JobAlerts
                    .FirstOrDefaultAsync(ja =>
                        ja.AspNetUserId == aspNetUserId
                        && ja.Id == jobAlertId
                        && ja.JobSeeker.AccountStatus == AccountStatus.Active
                        && !ja.IsDeleted)
                )?.UrlParameters;
        }

        private async Task UpdateJobAlertAsync(JobAlert jobAlert, JobAlertModel jobAlertModel, int? adminUserId)
        {
            jobAlert.Title = jobAlertModel.Title;
            jobAlert.AlertFrequency = (JobAlertFrequency)jobAlertModel.AlertFrequency;
            jobAlert.UrlParameters = jobAlertModel.UrlParameters;
            jobAlert.JobSearchFilters = jobAlertModel.JobSearchFilters;
            jobAlert.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            var logEntry = new JobSeekerChangeEvent
            {
                AspNetUserId = jobAlert.AspNetUserId,
                ModifiedByAdminUserId = adminUserId,
                DateUpdated = DateTime.Now,
                Field = $"Job alert '{jobAlert.Title}' updated",
                OldValue = "-",
                NewValue = "-"
            };

            await _context.JobSeekerChangeLog.AddAsync(logEntry);
            await _context.SaveChangesAsync();
        }

        public async Task SaveJobAlertAsync(JobAlertModel jobAlertModel, int? adminUserId)
        {
            // Generate a new Bookmarkable Url from the model to ensure
            // the state of the URL matches the state of the model. 
            var filters = JsonConvert.DeserializeObject<JobSearchFilters>(jobAlertModel.JobSearchFilters);

            // reset pagination and sort info
            filters.SortOrder = 1;
            filters.PageSize = 20;
            filters.Page = 1;

            // sanitize keywords
            filters.Keyword = (filters.Keyword ?? "")
                .Replace(";", " ")
                .Replace("%", " ")
                .Replace("  ", " ").Trim();

            // re-serialized the object
            jobAlertModel.JobSearchFilters = JsonConvert.SerializeObject(filters);

            jobAlertModel.UrlParameters = filters.BookmarkableUrl();

            if (jobAlertModel.Id.HasValue)
            {
                var jobAlert = await _context.JobAlerts.FirstOrDefaultAsync(
                    x => x.Title.Trim().ToLower() == jobAlertModel.Title.Trim().ToLower() &&
                    x.Id != jobAlertModel.Id &&
                    x.AspNetUserId == jobAlertModel.AspNetUserId && !x.IsDeleted);
                if (jobAlert != null)
                {
                    throw new AppException($"The title already exists.");
                }

                jobAlert = await _context.JobAlerts.FirstOrDefaultAsync(
                    x => x.Id == jobAlertModel.Id && 
                    x.AspNetUserId == jobAlertModel.AspNetUserId && !x.IsDeleted);
                if (jobAlert == null)
                {
                    throw new AppException($"The job alert [{jobAlertModel.Id}] does not exist or has been deleted.");
                }

                await UpdateJobAlertAsync(jobAlert, jobAlertModel, adminUserId);
            }
            else
            {
                var jobAlert = await _context.JobAlerts.FirstOrDefaultAsync(
                    x => x.Title.Trim().ToLower() == jobAlertModel.Title.Trim().ToLower() &&
                    x.AspNetUserId == jobAlertModel.AspNetUserId && !x.IsDeleted);
                if (jobAlert != null)
                {
                    if (!jobAlertModel.OverwriteExisting)
                    {
                        throw new AppException("The title already exists.");
                    }

                    await UpdateJobAlertAsync(jobAlert, jobAlertModel, adminUserId);
                    return;
                }

                jobAlert = new JobAlert
                {
                    Title = jobAlertModel.Title,
                    AlertFrequency = (JobAlertFrequency) jobAlertModel.AlertFrequency,
                    UrlParameters = jobAlertModel.UrlParameters,
                    JobSearchFilters = jobAlertModel.JobSearchFilters,
                    AspNetUserId = jobAlertModel.AspNetUserId
                };

                await _context.JobAlerts.AddAsync(jobAlert);
                await _context.SaveChangesAsync();

                var logEntry = new JobSeekerChangeEvent
                {
                    AspNetUserId = jobAlert.AspNetUserId,
                    ModifiedByAdminUserId = adminUserId,
                    DateUpdated = DateTime.Now,
                    Field = $"Job alert '{jobAlert.Title}' created",
                    OldValue = "-",
                    NewValue = "-"
                };

                await _context.JobSeekerChangeLog.AddAsync(logEntry);
                await _context.SaveChangesAsync();
            }
        }
    }
}
