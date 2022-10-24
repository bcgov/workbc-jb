using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBC.Web.Helpers;
using WorkBC.Web.Models;
using WorkBC.Web.Services;

namespace WorkBC.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
    public class SavedJobsController : JobsControllerBase
    {
        private readonly ISavedJobsService _savedJobsService;

        public SavedJobsController(ISavedJobsService savedJobsService, SystemSettingsService systemSettingsService) : base(systemSettingsService)
        {
            _savedJobsService = savedJobsService;
        }

        private string UserId => HttpContext.User.Identity.Name;

        [HttpDelete("delete-saved-job/{jobId}")]
        public async Task<IActionResult> DeleteSavedJobAsync(string jobId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jobId))
                {
                    return BadRequest(new {message = "Job id is required"});
                }

                await _savedJobsService.DeleteSavedJobAsync(UserId, jobId);

                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpPut("save-job-note")]
        public async Task<IActionResult> SaveJobNoteAsync([FromBody] JobNoteModel jobNoteModel)
        {
            try
            {
                if (jobNoteModel == null || string.IsNullOrWhiteSpace(jobNoteModel.JobId))
                {
                    return BadRequest(new {message = "Job id is required"});
                }

                jobNoteModel.UserId = UserId;
                await _savedJobsService.SaveJobNoteAsync(jobNoteModel);

                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpPost("save-jobs/{jobIds}")]
        public async Task<IActionResult> SaveJobsAsync(string jobIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jobIds))
                {
                    return BadRequest(new {message = "Job ids are required"});
                }

                long[] jobList = jobIds.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();

                await _savedJobsService.SaveJobsAsync(UserId, jobList);

                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpGet("saved-job-ids")]
        public async Task<IActionResult> GetSavedJobIdsAsync()
        {
            IList<long> savedJobs = await _savedJobsService.GetSavedJobIdsAsync(UserId);
            return Ok(savedJobs);
        }

        [HttpGet("saved-jobs")]
        public async Task<IActionResult> GetSavedJobsAsync()
        {
            IList<SavedJobsModel> savedJobs = await _savedJobsService.GetSavedJobsAsync(UserId);
            return Ok(savedJobs);
        }
    }
}