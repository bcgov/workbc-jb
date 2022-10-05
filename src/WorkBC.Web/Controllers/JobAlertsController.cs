using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WorkBC.Web.Helpers;
using WorkBC.Web.Models;
using WorkBC.Web.Services;

namespace WorkBC.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class JobAlertsController : AuthenticatedControllerBase
    {
        private readonly IJobAlertsService _jobAlertsService;
        public JobAlertsController(IJobAlertsService jobAlertsService)
        {
            _jobAlertsService = jobAlertsService;
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetJobAlertsTotalAsync()
        {
            var jobAlerts = await _jobAlertsService.GetJobAlertsTotalAsync(UserId);
            return Ok(jobAlerts);
        }

        [HttpGet]
        public IActionResult GetJobAlerts()
        {
            var jobAlerts = _jobAlertsService.GetJobAlertsAsync(UserId);
            return Ok(jobAlerts);
        }

        [HttpGet("{id}")]
        public IActionResult GetJobAlert(int id)
        {
            var jobAlert = _jobAlertsService.GetJobAlert(UserId, id);
            return Ok(jobAlert);
        }

        [HttpPost("save-job-alert")]
        public async Task<IActionResult> SaveJobAlertAsync([FromBody] JobAlertModel jobAlertModel)
        {
            try
            {
                if (jobAlertModel == null || string.IsNullOrWhiteSpace(UserId))
                {
                    return BadRequest(new { message = "User id and jobAlertModel are required" });
                }

                jobAlertModel.AspNetUserId = UserId;
                await _jobAlertsService.SaveJobAlertAsync(jobAlertModel, AdminUserId);

                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete-job-alert/{id}")]
        public async Task<IActionResult> DeleteJobAlertAsync(int id)
        {
            try
            {
                await _jobAlertsService.DeleteJobAlertAsync(UserId, AdminUserId, id);

                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("total-matched-jobs")]
        public async Task<IActionResult> GetTotalMatchedJobsAsync([FromBody]string filtersStr)
        {
            var result = await _jobAlertsService.GetTotalMatchedJobsAsync(filtersStr);
            return Ok(result);
        }

        [HttpGet("get-url-params")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUrlParameters([FromQuery(Name = "nid")] int jobAlertId,
            [FromQuery(Name = "jsid")] string aspNetUserId)
        {
            return new JsonResult(await _jobAlertsService.GetUrlParameters(jobAlertId, aspNetUserId) ?? "");
        }

    }
}