using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WorkBC.Data.Model.JobBoard;
using WorkBC.ElasticSearch.Models.Filters;
using WorkBC.Web.Models;
using WorkBC.Web.Services;

namespace WorkBC.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/recommended-jobs")]
    public class RecommendedJobsController : JobsControllerBase
    {
        private readonly ILogger<RecommendedJobsController> _logger;
        private readonly IRecommendedJobsService _recommendedJobsService;
        private readonly IUserService _userService;

        public RecommendedJobsController(IRecommendedJobsService recommendedJobsService, IUserService userService,
            ILogger<RecommendedJobsController> logger, SystemSettingsService systemSettingsService) : base(
            systemSettingsService)
        {
            _recommendedJobsService = recommendedJobsService;
            _userService = userService;
            _logger = logger;
        }

        private string UserId => HttpContext.User.Identity.Name;

        private async Task<RecommendedJobsResultModel> GetData(RecommendedJobsFilters filters)
        {
            // todo: the result of these lines could be stored in the session to make it faster
            JobSeeker jobSeeker = await _userService.GetByIdAsync(UserId);
            if (jobSeeker == null)
            {
                throw new Exception("Invalid userId!!!");
            }

            AccountCriteria userCriteria = await _recommendedJobsService.GetUserAccountCriteria(jobSeeker);

            RecommendedJobsResultModel model = await _recommendedJobsService.GetRecommendedJobs(filters, userCriteria);

            // set the IsNew bit on new jobs
            await SetNewJobs(model.Result);

            model.City = jobSeeker.City;

            JobSeekerFlags flags = jobSeeker.JobSeekerFlags;
            model.JobSeekerFlags = new
            {
                jobSeeker.JobSeekerFlags.IsYouth,
                isIndigenous = flags.IsIndigenousPerson,
                isNewcomer = flags.IsNewImmigrant,
                flags.IsApprentice,
                flags.IsMatureWorker,
                hasDisability = flags.IsPersonWithDisability,
                flags.IsStudent,
                flags.IsVeteran,
                isMinority = flags.IsVisibleMinority
            };

            return model;
        }

        /// <summary>
        ///     Gets the list of recommended jobs for a user.
        /// </summary>
        /// <param name="filters">
        ///     A RecommendedJobsFilters object.  This object implements the interface IPageableFilters
        ///     so typescript implementation should be very similar to the main job search.
        /// </param>
        [EnableCors("_API")]
        [HttpPost]
        public async Task<IActionResult> GetRecommendedJobsAsync([FromBody] RecommendedJobsFilters filters)
        {
            try
            {
                RecommendedJobsResultModel result = await GetData(filters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        ///     Gets the count of recommended jobs for a user
        /// </summary>
        [EnableCors("_API")]
        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            // todo: the result of these 2 lines can be stored in the session to make it faster
            JobSeeker jobSeeker = _userService.GetById(UserId);
            AccountCriteria userCriteria = await _recommendedJobsService.GetUserAccountCriteria(jobSeeker);

            long count = await _recommendedJobsService.GetRecommendedJobCount(userCriteria);

            return Ok(count);
        }
    }
}