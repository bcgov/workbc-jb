using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using WorkBC.Admin.Services;
using WorkBC.Data;
using WorkBC.Data.Enums;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Admin.Helpers
{
    public class JobBoardAdminOpenIdConnectHelper
    {
        // These are the claim names we use on the old Keycloak silver cluster
        // todo: this code can be deleted after all environments have been migrated to gold.
        private const string UserGuidClaimOld = "idir_userid";
        private const string UsernameClaimOld = "preferred_username";

        // These are the claim names we use on the new Keycloak gold cluster
        private const string UserGuidClaimNew = "idir_user_guid";
        private const string UsernameClaimNew = "idir_username";

        public static void HandleAdminUserLogin(TokenValidatedContext tokenCtx)
        {
            string guid = tokenCtx.Principal.FindFirstValue(UserGuidClaimNew);
            if (guid == null)
            {
                // if the Gold claim isn't available, try to get the silver claim
                // todo: this code can be deleted after all environments have been migrated to gold.
                guid = tokenCtx.Principal.FindFirstValue(UserGuidClaimOld);
            }
            string tidyGuid = guid.Replace("-", "").ToUpper();

            //Get EF context
            var dbCtx = tokenCtx.HttpContext.RequestServices.GetRequiredService<JobBoardContext>();

            AdminUser user = dbCtx.AdminUsers.FirstOrDefault(u => u.Guid == tidyGuid && !u.Deleted);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Sid, user.Id.ToString())
                };

                switch (user.AdminLevel)
                {
                    case AdminLevel.SuperAdmin:
                        claims.Add(new Claim(ClaimTypes.Role, Roles.SuperAdmin));
                        break;
                    case AdminLevel.Admin:
                        claims.Add(new Claim(ClaimTypes.Role, Roles.Admin));
                        break;
                    case AdminLevel.Reporting:
                        claims.Add(new Claim(ClaimTypes.Role, Roles.Reporting));
                        break;
                }

                var appIdentity = new ClaimsIdentity(claims);

                if (tokenCtx.Principal != null)
                {
                    tokenCtx.Principal.AddIdentity(appIdentity);
                }

                UpdateAdminUser(dbCtx, tokenCtx, user);
            }
        }


        /// <summary>
        ///     Update the last login date and refresh the account info
        /// </summary>
        private static void UpdateAdminUser(JobBoardContext dbCtx, TokenValidatedContext tokenCtx, AdminUser user)
        {
            // Update the last login date and refresh the account info 
            if (!user.DateLastLogin.HasValue || user.DateLastLogin.Value < DateTime.Now.AddMinutes(-1))
            {
                user.DateLastLogin = DateTime.Now;

                string givenName = tokenCtx.Principal.FindFirstValue(ClaimTypes.GivenName);

                if (user.GivenName != givenName)
                {
                    user.GivenName = givenName;
                    user.DateUpdated = DateTime.Now;
                }

                string surname = tokenCtx.Principal.FindFirstValue(ClaimTypes.Surname);

                if (user.Surname != surname)
                {
                    user.Surname = surname;
                    user.DateUpdated = DateTime.Now;
                }

                string userSamAccountName = tokenCtx.Principal.FindFirstValue(UsernameClaimNew);

                if (userSamAccountName == null)
                {
                    // if the Gold claim isn't available, try to get the silver claim
                    // todo: this code can be deleted after all environments have been migrated to gold.
                    string username = tokenCtx.Principal.FindFirstValue(UsernameClaimOld);
                    // the preferred_username claim will be like "bob@idir".  We just want "BOB" (uppercase)
                    userSamAccountName = username.Split('@')[0].ToUpper();
                }

                if (user.SamAccountName != userSamAccountName)
                {
                    user.SamAccountName = userSamAccountName;
                    user.DateUpdated = DateTime.Now;
                }

                dbCtx.AdminUsers.Update(user);
                dbCtx.SaveChanges();
            }
        }

        /// <summary>
        ///     Checks if the access token needs to be refreshed, and uses the refresh token to update
        ///     the access token.
        ///     The decryption of the cookie has already happened so we have access to the user claims
        ///     and cookie properties - expiration, etc..
        /// </summary>
        /// <remarks>
        ///     Based on https://github.com/mderriey/aspnet-core-token-renewal
        /// </remarks>
        public static async Task HandleOidcRefreshToken(CookieValidatePrincipalContext context, string oidcRealmUri,
            string oidcClientId, string oidcClientSecret)
        {
            // Since our cookie lifetime is based on the access token one,
            // check if we're more than halfway of the cookie lifetime
            DateTimeOffset now = DateTimeOffset.UtcNow;
            TimeSpan timeElapsed = now.Subtract(context.Properties.IssuedUtc.Value);
            TimeSpan timeRemaining = context.Properties.ExpiresUtc.Value.Subtract(now);

            if (timeElapsed > timeRemaining)
            {
                var identity = (ClaimsIdentity)context.Principal.Identity;
                Claim accessTokenClaim = identity.FindFirst("access_token");
                Claim refreshTokenClaim = identity.FindFirst("refresh_token");

                // If we have to refresh, grab the refresh token from the claims, and request
                // new access token and refresh token
                string refreshToken = refreshTokenClaim.Value;
                TokenResponse response = await new HttpClient().RequestRefreshTokenAsync(
                    new RefreshTokenRequest
                    {
                        Address = $"{oidcRealmUri}/protocol/openid-connect/token",
                        ClientId = oidcClientId,
                        ClientSecret = oidcClientSecret,
                        RefreshToken = refreshToken
                    });

                if (!response.IsError)
                {
                    // Everything went right, remove old tokens and add new ones
                    identity.RemoveClaim(accessTokenClaim);
                    identity.RemoveClaim(refreshTokenClaim);

                    identity.AddClaims(new[]
                    {
                        new Claim("access_token", response.AccessToken),
                        new Claim("refresh_token", response.RefreshToken)
                    });

                    // Indicate to the cookie middleware to renew the session cookie.
                    // The new lifetime will be the same as the old one, so the alignment
                    // between cookie and access token is preserved
                    context.ShouldRenew = true;
                }
            }
        }
    }
}