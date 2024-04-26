using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;
using WorkBC.Web.Services;
using System.Linq;

namespace WorkBC.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class SystemSettingsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly SystemSettingsService _settingsService;
        private readonly JobBoardContext _context;

        public SystemSettingsController(SystemSettingsService settingsService, IConfiguration configuration,JobBoardContext context)
        {
            _settingsService = settingsService;
            _configuration = configuration;
            _context = context;
        }

        [EnableCors("_API")]
        [HttpGet]
        public async Task<IActionResult> ClientSettings()
        {
            Shared.SystemSettings.Settings settings = await _settingsService.GetAllAsync();
            return Ok(new
            {
                settings.JbSearch,
                settings.JbAccount,
                settings.Shared
            });
        }

        /// <summary>
        ///     Helper for debugging settings.  Contains all the information from the Settings()
        ///     action, plus emails, server-side setting, and cache timestamps.
        /// </summary>
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Test()
        {
            return Ok(await _settingsService.GetAllAsync());
        }

        [HttpGet]
        public string Version()
        {
            return _configuration["Version:RunNumber"];
        }

        [HttpGet]
        public IActionResult BuildInfo()
        {
            // the 3 values below are set at build time as environment variables
            // in the docker container
            return Ok(
                new
                {
                    SHA = _configuration["Version:SHA"],
                    RunNumber = _configuration["Version:RunNumber"],
                    BuildDate = _configuration["Version:BuildDate"]
                }
            );
        }

        [HttpGet]
        public IActionResult DebugInfo()
        {
            var query = from p in _context.NocCodes2021
                        select new NocCode2021
                        {
                            Id = p.Id,
                            Code = p.Code,
                            Title = p.Title,
                            FrenchTitle = p.FrenchTitle,
                        };

            return Ok( query.ToList());
        }
    }
}