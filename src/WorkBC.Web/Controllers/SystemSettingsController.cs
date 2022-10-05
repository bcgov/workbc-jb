using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WorkBC.Web.Services;

namespace WorkBC.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class SystemSettingsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly SystemSettingsService _settingsService;

        public SystemSettingsController(SystemSettingsService settingsService, IConfiguration configuration)
        {
            _settingsService = settingsService;
            _configuration = configuration;
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
            return _configuration["Version:ReleaseName"];
        }
    }
}