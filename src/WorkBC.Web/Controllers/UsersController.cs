using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WorkBC.Data.Enums;
using WorkBC.Data.Model.JobBoard;
using WorkBC.Web.Helpers;
using WorkBC.Web.Models;
using WorkBC.Web.Services;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace WorkBC.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : AuthenticatedControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UsersController> _logger;
        private readonly SystemSettingsService _settingsService;

        public UsersController(
            IUserService userService,
            ITokenService tokenService,
            IMapper mapper,
            ILogger<UsersController> logger,
            SystemSettingsService settingsService)
        {
            _userService = userService;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
            _settingsService = settingsService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginModel loginModel)
        {
            try
            {
                if (loginModel == null)
                {
                    return BadRequest(new
                    {
                        message = "Both email and password are required.",
                        field = (int) AppException.Fields.NoSpecificField
                    });
                }

                var usernameOrEmail = loginModel.Email;

                JobSeeker jobSeeker;
                SignInResult signInResult;

                (jobSeeker, signInResult) = await _userService.Authenticate(usernameOrEmail, loginModel.Password);

                if (signInResult != null && signInResult.IsLockedOut)
                {
                    var lockoutMinutes = 0;
                    if (jobSeeker.LockoutEnd != null)
                    {
                        lockoutMinutes = (int) jobSeeker.LockoutEnd.Value.Subtract(DateTime.Now).TotalMinutes + 1;
                    }

                    return BadRequest(new
                    {
                        message =
                            $"Your account has been locked.  Please try again after {(lockoutMinutes == 1 ? " 1 minute" : $" {lockoutMinutes} minutes")}.",
                        field = (int) AppException.Fields.NoSpecificField
                    });
                }

                if (signInResult != null && signInResult.IsNotAllowed)
                {
                    if (jobSeeker.AccountStatus == AccountStatus.Deactivated)
                    {
                        return BadRequest(new
                        {
                            message = "ERR_LOGIN_FAILED_DEACTIVATED",
                            field = (int)AppException.Fields.NoSpecificField
                        });
                    }

                    return BadRequest(new
                    {
                        message = "Your account is awaiting email activation.",
                        field = (int) AppException.Fields.NoSpecificField
                    });
                }

                if (signInResult == null || !signInResult.Succeeded)
                {
                    var settings = await _settingsService.JbAccountSettingsAsync();
                    var message = string.IsNullOrEmpty(settings.Errors.LoginFailed) 
                        ? "The password you entered is incorrect.  Please try again. You have {0} remaining before your account is locked."
                        : settings.Errors.LoginFailed;

                    int remainingTries = 5 - jobSeeker.AccessFailedCount;

                    string tries = remainingTries == 1 
                        ? "one try" 
                        : $"{remainingTries.GetWord()} tries";

                    message = string.Format(message, tries);

                    return BadRequest(new { message, field = (int) AppException.Fields.Password });
                }

                // return basic user info (without password) and token to store client side
                var user = _mapper.Map<IUserInfo>(jobSeeker);

                user.Token = _tokenService.GetJwtToken(jobSeeker);

                // log this
                await _userService.RecordLogon(jobSeeker);

                return Ok(user);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message, field = (int) ex.Field });
            }
        }

        [AllowAnonymous]
        [HttpPost("impersonate")]
        public async Task<IActionResult> Impersonate([FromBody] ImpersonateModel impersonateModel)
        {
            if (impersonateModel == null || string.IsNullOrEmpty(impersonateModel.Token))
            {
                return BadRequest(new {message = "Token is required."});
            }

            (JobSeeker jobSeeker, int? adminUserId) = await _userService.GetUserByImpersonationTokenAsync(impersonateModel.Token);

            if (jobSeeker == null)
            {
                return BadRequest(new { message = "Token is invalid or expired." });
            }

            if (jobSeeker.LockoutEnabled && jobSeeker.LockoutEnd > DateTime.Now)
            {
                return BadRequest(new {message = "This account is locked out."});
            }

            if (!jobSeeker.EmailConfirmed)
            {
                return BadRequest(new {message = "This account is awaiting email activation."});
            }

            // return basic user info (without password) and token to store client side
            var user = _mapper.Map<IUserInfo>(jobSeeker);
            user.Token = _tokenService.GetJwtToken(jobSeeker, adminUserId);

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            try
            {
                if (registerModel == null)
                {
                    return BadRequest(new { message = "User View Model is required" });
                }

                // map dto to entity
                var jobSeeker = _mapper.Map<JobSeeker>(registerModel);

                await _userService.CreateAsync(jobSeeker, registerModel.Password);

                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("send-activation-email")]
        public async Task<IActionResult> SendActivationEmailAsync([FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            await _userService.SendActivationEmailAsync(email);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("send-pwd-reset-email")]
        public async Task<IActionResult> SendPwdResetEmailAsync([FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            await _userService.SendPwdResetEmailAsync(email);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmEmailModel confirmEmailModel)
        {
            if (confirmEmailModel == null || string.IsNullOrWhiteSpace(confirmEmailModel.UserId) ||
                string.IsNullOrWhiteSpace(confirmEmailModel.Code))
            {
                _logger.LogInformation("400 BadRequest = userId or code is required");
                return BadRequest(new {message = "userId or code is required"});
            }

            _logger.LogInformation("Starting ConfirmEmailAsync");
            _logger.LogInformation($"UserId={confirmEmailModel.UserId}");
            _logger.LogInformation($"Code={confirmEmailModel.Code}");

            dynamic result = await _userService.ConfirmEmailAsync(confirmEmailModel);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("verify-recaptcha")]
        public async Task<IActionResult> VerifyRecaptchaAsync([FromBody] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new { message = "Token is required" });
            }

            var result = await _userService.VerifyRecaptchaAsync(token);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("verify-token")]
        public async Task<IActionResult> VerifyUserTokenAsync([FromBody] ForgotPasswordModel forgotPasswordModel)
        {
            if (forgotPasswordModel == null || string.IsNullOrWhiteSpace(forgotPasswordModel.Email) || string.IsNullOrWhiteSpace(forgotPasswordModel.Token))
            {
                return BadRequest(new { message = "Email and Token are required" });
            }

            var result = await _userService.VerifyUserTokenAsync(forgotPasswordModel);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("reset-pwd")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ForgotPasswordModel forgotPasswordModel)
        {
            if (forgotPasswordModel == null ||
                string.IsNullOrWhiteSpace(forgotPasswordModel.Email) ||
                string.IsNullOrWhiteSpace(forgotPasswordModel.NewPassword) ||
                string.IsNullOrWhiteSpace(forgotPasswordModel.Token))
            {
                return BadRequest(new { message = "ForgotPasswordModel is required" });
            }

            var result = await _userService.ResetPasswordAsync(forgotPasswordModel);

            return Ok(result);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetById()
        {
            var jobSeeker = await _userService.GetByIdAsync(UserId);
            var user = _mapper.Map<IRegisterModel>(jobSeeker);
            return Ok(user);
        }

        [HttpPut("update-personal-settings")]
        public async Task<IActionResult> UpdatePersonalSettings([FromBody] RegisterModel registerModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserId) || registerModel == null)
                {
                    return BadRequest(new { message = "Id and User View Model are required" });
                }

                // map dto to entity
                var jobSeeker = _mapper.Map<JobSeeker>(registerModel);
                jobSeeker.Id = UserId;

                await _userService.UpdateAsync(jobSeeker, registerModel.Password, AdminUserId);

                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("change-pwd")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] PasswordResetModel passwordResetModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserId) || passwordResetModel == null)
                {
                    return BadRequest(new { message = "User Id and related View Model are required" });
                }

                var result = await _userService.ChangePasswordAsync(UserId, passwordResetModel.CurrentPassword, passwordResetModel.NewPassword);

                return Ok(result);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            await _userService.DeleteAsync(UserId, AdminUserId);
            return Ok();
        }
    }
}