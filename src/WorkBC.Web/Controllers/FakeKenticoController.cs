using Microsoft.AspNetCore.Mvc;
﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using WorkBC.Web.Settings;

namespace WorkBC.Web.Controllers
{
    /// <summary>
    ///     This controller is used to host pages with Kentico headers and footers (for testing purposes).
    ///     It may be removed once we have integration with the real Kentico server.
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class FakeKenticoController : Controller
    {
        private const string WorkbcJobBoardUrl = "https://www.workbc.ca/Jobs-Careers/Find-Jobs/Jobs.aspx";
        private readonly AppSettings _appSettings;

        public FakeKenticoController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return !_appSettings.IsProduction
                ? RedirectToAction(_appSettings.UseJbAccountApp ? "JbAccount" : "JbSearch")
                : Redirect(WorkbcJobBoardUrl);
        }

        [Route("/Test/JbSearch")]
        [HttpGet]
        public IActionResult JbSearch()
        {
            return !_appSettings.IsProduction
                ? View(_appSettings)
                : NotFound();
        }

        [Route("/Test/JbAccount")]
        [HttpGet]
        public IActionResult JbAccount()
        {
            return !_appSettings.IsProduction
                ? View()
                : NotFound();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("/health")]
        public IActionResult Health()
        {
            Response.StatusCode = StatusCodes.Status200OK;
            return Content("It's Alive!");
        }
    }
}