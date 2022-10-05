using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WorkBC.ElasticSearch.Models.JobAttributes;
using WorkBC.Shared.SystemSettings;
using WorkBC.Web.Models;
using WorkBC.Web.Services;

namespace WorkBC.Web.Controllers
{
    public abstract class JobsControllerBase : ControllerBase
    {
        private readonly SystemSettingsService _systemSettingsService;

        protected JobsControllerBase(SystemSettingsService systemSettingsService)
        {
            _systemSettingsService = systemSettingsService;
        }

        /// <summary>
        ///     Gets earliest the date when a job is considered "New"
        /// </summary>
        private async Task<DateTime> GetNewJobDate()
        {
            JbSearchSettings searchSettings = await _systemSettingsService.JbSearchSettingsAsync();
            int newJobPeriodDays = searchSettings.Settings.NewJobPeriodDays;
            return DateTime.Now.AddDays(-1 * newJobPeriodDays).Date;
        }

        /// <summary>
        ///     Sets the "new" bit on a list of jobs
        /// </summary>
        protected async Task SetNewJobs(SavedJobsModel[] jobs)
        {
            // set the IsNew bit on new jobs
            DateTime newJobDate = await GetNewJobDate();

            foreach (SavedJobsModel job in jobs)
            {
                job.IsNew = job.DatePosted >= newJobDate;
            }
        }

        /// <summary>
        ///     Sets the "new" bit on a list of jobs
        /// </summary>
        protected async Task SetNewJobs(Source[] jobs)
        {
            // set the IsNew bit on new jobs
            DateTime newJobDate = await GetNewJobDate();

            foreach (Source job in jobs)
            {
                job.IsNew = job.DatePosted >= newJobDate;
            }
        }
    }
}