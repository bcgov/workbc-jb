using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkBC.Data;
using WorkBC.ElasticSearch.Models.JobAttributes;

namespace WorkBC.Web.Services
{
    public class ViewCountService
    {
        private const string CacheKeyPrefix = "JV_";

        //Default time limit for the cache
        private readonly int _cacheMinutes = 15;

        //Reference to the cache service
        private readonly CacheService _cacheService;

        //Reference to the database
        private readonly JobBoardContext _dbContext;

        public ViewCountService(CacheService cacheService, JobBoardContext dbContext)
        {
            _cacheService = cacheService;
            _dbContext = dbContext;
        }

        /// <summary>
        ///     Gets the total views for a specific job
        /// </summary>
        /// <param name="jobId">Job ID</param>
        /// <returns>Number of views for this job</returns>
        private async Task<int> GetViewsForJob(long jobId)
        {
            string cacheKey = $"{CacheKeyPrefix}{jobId}";
            long? views = await _cacheService.GetLongAsync(cacheKey);

            if (views != null)
            {
                return (int) views.Value;
            }

            views = (await _dbContext.JobViews.FirstOrDefaultAsync(j => j.JobId == jobId))?.Views ?? 0;

            await _cacheService.SaveLongAsync(cacheKey, views.Value, _cacheMinutes * 60);

            return (int) views.Value;
        }

        /// <summary>
        ///     This will be used on the search result page to loop through each job and find the number of views for each job
        /// </summary>
        /// <param name="jobs">Array of jobs</param>
        /// <returns>Array of jobs with their number of views</returns>
        public async Task<Source[]> GetJobViews(Source[] jobs)
        {
            await CacheMultiple(jobs);

            foreach (Source job in jobs)
            {
                job.Views = await GetViewsForJob(job.JobId);
            }

            return jobs;
        }

        private async Task CacheMultiple(Source[] jobs)
        {
            var cacheMissJobIds = new List<long>();
            foreach (Source job in jobs)
            {
                if (!await _cacheService.ExistsAsync(CacheKeyPrefix + job.JobId))
                {
                    cacheMissJobIds.Add(job.JobId);
                }
            }

            if (cacheMissJobIds.Count <= 5)
            {
                return;
            }

            var dbViewCounts =
                await _dbContext.JobViews.Where(j => cacheMissJobIds.Contains(j.JobId))
                    .Select(j => new {j.JobId, j.Views})
                    .ToListAsync();

            foreach (dynamic job in dbViewCounts)
            {
                await _cacheService.SaveLongAsync(CacheKeyPrefix + job.JobId, job.Views, _cacheMinutes * 60);
            }

            foreach (long jobId in cacheMissJobIds)
            {
                // cache zero for any job that wasn't found in the db
                if (dbViewCounts.All(j => j.JobId != jobId))
                {
                    string cacheKey = $"{CacheKeyPrefix}{jobId}";
                    await _cacheService.SaveLongAsync(cacheKey, 0, _cacheMinutes * 60);
                }
            }
        }

        /// <summary>
        ///     Clears a specific job from the job view cache
        /// </summary>
        /// <param name="jobId">Job ID to update</param>
        public async Task RemoveJob(long jobId)
        {
            string cacheKey = $"{CacheKeyPrefix}{jobId}";
            // cacheService already checks to see if the item exists
            await _cacheService.RemoveAsync(cacheKey);
        }
    }
}