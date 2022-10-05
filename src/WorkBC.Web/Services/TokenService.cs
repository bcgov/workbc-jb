using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WorkBC.Data.Model.JobBoard;
using WorkBC.Web.Helpers;

namespace WorkBC.Web.Services
{
    public interface ITokenService
    {
        string GetJwtToken(JobSeeker user, int? adminUserId = null);
    }

    public class TokenService : ITokenService
    {
        private readonly TokenManagement _tokenManagement;

        public TokenService(IOptions<TokenManagement> tokenManagement)
        {
            _tokenManagement = tokenManagement.Value;
        }

        public string GetJwtToken(JobSeeker user, int? adminUserId = null)
        {
            var result = string.Empty;
            if (user != null)
            {
                byte[] secret = Encoding.ASCII.GetBytes(_tokenManagement.Secret);
                var key = new SymmetricSecurityKey(secret);

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Id)
                };

                if (adminUserId != null)
                {
                    var claimsList = claims.ToList();
                    claimsList.Add(new Claim(ClaimTypes.Role, "JobBoardAdmin"));
                    claimsList.Add(new Claim(ClaimTypes.UserData, adminUserId.ToString()));
                    claims = claimsList.ToArray();
                }

                var securityToken = new JwtSecurityToken(
                    _tokenManagement.Issuer,
                    _tokenManagement.Audience,
                    claims,
                    expires: DateTime.Now.AddMinutes(_tokenManagement.AccessExpiration),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

                var tokenHandler = new JwtSecurityTokenHandler();

                result = tokenHandler.WriteToken(securityToken);
            }

            return result;
        }
    }
}