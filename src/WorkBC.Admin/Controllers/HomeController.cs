using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkBC.Admin.Models;
using WorkBC.Admin.Services;

namespace WorkBC.Admin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ClaimsPrincipal user = HttpContext.User;

            return user.IsInRole(Roles.Admin) || user.IsInRole(Roles.SuperAdmin)
                ? RedirectToAction("Index", "UserSearch", new {area = "JobSeekers"}) 
                : RedirectToAction("Index", "Home", new { area = "Reports" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult NotAuthorized()
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult InternetExplorer()
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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