using System.Security;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WorkBC.Data.Enums;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Admin.Controllers
{
    public abstract class AdminControllerBase : Controller
    {
        public AdminLevel AdminLevel;
        private const string InternetExplorerErrorPath = "/home/internetexplorer";

        public int CurrentAdminUserId
        {
            get
            {
                ClaimsPrincipal user = HttpContext.User;
                string userId = user.FindFirst(ClaimTypes.Sid)?.Value ?? "";

                if (userId == "" || userId == "0")
                {
                    throw new SecurityException("Invalid User Id");
                }

                try
                {
                    return int.Parse(userId);
                }
                catch
                {
                    throw new SecurityException($"Invalid User Id: {userId}");
                }
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string path = (context.HttpContext.Request.Path.Value ?? "").ToLower();

            if (!path.StartsWith(InternetExplorerErrorPath))
            {
                string userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString().ToLower();

                if (userAgent.Contains("msie") || userAgent.Contains("trident/7.0"))
                {
                    context.HttpContext.Response.Redirect(InternetExplorerErrorPath);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}