using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using WorkBC.Admin.Services;

namespace WorkBC.Admin.Middleware
{
    /// <summary>
    ///     Custom authorization middleware for Job Board Admin logins
    /// </summary>
    public class JobBoardAdminAccountMiddleware : IMiddleware
    {
        private const string NotAuthorizedPath = "/home/notauthorized";
        private const string HealthPath = "/health";
        private const string FakeFirstName = "Scooby";
        private const string FakeUserName = "FAKEUSER";
        private const int FakeUserId = 1;
        private readonly string _environment;
        private readonly bool _devModeBypassEnabled;

        public JobBoardAdminAccountMiddleware(IConfiguration configuration)
        {
            _environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production";
            _devModeBypassEnabled = bool.Parse(configuration["Keycloak:DevModeBypassEnabled"] ?? "false");
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string path = (context.Request.Path.Value ?? "").ToLower();

            if (!path.StartsWith(NotAuthorizedPath) && !path.StartsWith(HealthPath))
            {
                if (IsDevelopment(context))
                {
                    var claimsIdentity = new ClaimsIdentity(FakeUserName);
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.GivenName, FakeFirstName));
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, Roles.SuperAdmin));
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Sid, FakeUserId.ToString()));

                    await context.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));
                }
                else
                {
                    if (context.User.Identity is not { IsAuthenticated: true })
                    {
                        // If the user isn't authenticated then the OpenIdConnect middleware hasn't run
                        // yet. So we exit and let the OpenIdConnect middleware do it's thing.
                        // This middleware will run again later after the redirect back from Keycloak.
                        await next(context);
                        return;
                    }

                    string role = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                    string sid = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;

                    if (string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(sid) || !sid.All(char.IsNumber))
                    {
                        context.Response.Redirect(NotAuthorizedPath);
                        return;
                    }

                    if (role != Roles.Reporting && role != Roles.SuperAdmin && role != Roles.Admin)
                    {
                        context.Response.Redirect(NotAuthorizedPath);
                        return;
                    }
                }
            }

            await next(context);
        }

        /// <summary>
        ///     Checks if the site is running on a local dev machine
        /// </summary>
        private bool IsDevelopment(HttpContext context)
        {
            if (!_devModeBypassEnabled)
            {
                // Keycloak:DevModeBypassEnabled is a safety switch in case Development mode
                // gets turned on for a public server (e.g. for debugging) and a hacker finds a
                // way to access it as "localhost".  This config setting automatically gets set
                // to "false" in TFS every time a release deployment runs on the server. 
                return false;
            }

            var displayUrl = context.Request.GetDisplayUrl();

            // This should work on non-standard ports only.  If the protocol/port is http/80
            // or https/443 then the browser will remove the colon.
            return _environment == "Development"
                   && (displayUrl.Contains("/127.0.0.1:") || displayUrl.ToLower().Contains("/localhost:"));
        }
    }
}