using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace WorkBC.Web.Controllers
{
    public abstract class AuthenticatedControllerBase : ControllerBase
    {
        protected int? AdminUserId
        {
            get
            {
                ClaimsPrincipal user = HttpContext.User;
                string role = user.FindFirst(ClaimTypes.Role)?.Value ?? "";
                string userId = user.FindFirst(ClaimTypes.UserData)?.Value ?? "";

                if (role == "JobBoardAdmin")
                {
                    try
                    {
                        return int.Parse(userId);
                    }
                    catch
                    {
                        //
                    }
                }

                return null;
            }
        }

        protected string UserId => HttpContext.User.Identity.Name;
    }
}