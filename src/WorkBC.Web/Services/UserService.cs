using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WorkBC.Data;
using WorkBC.Data.Enums;
using WorkBC.Data.Model.JobBoard;
using WorkBC.Shared.Repositories;
using WorkBC.Shared.Services;
using WorkBC.Shared.Settings;
using WorkBC.Web.Helpers;
using WorkBC.Web.Models;
using EmailSettings = WorkBC.Shared.SystemSettings.EmailSettings;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace WorkBC.Web.Services
{
    public interface IUserService
    {
        Task<(JobSeeker, SignInResult)> Authenticate(string email, string password);
        Task<JobSeeker> GetByIdAsync(string userId);
        JobSeeker GetById(string id);
        Task<JobSeeker> CreateAsync(JobSeeker jobSeeker, string password);
        Task<dynamic> ConfirmEmailAsync(ConfirmEmailModel confirmEmailModel);
        Task SendActivationEmailAsync(string email);
        Task SendPwdResetEmailAsync(string email);
        Task<dynamic> VerifyRecaptchaAsync(string token);
        Task<bool> VerifyUserTokenAsync(ForgotPasswordModel forgotPasswordModel);
        Task<IdentityResult> ResetPasswordAsync(ForgotPasswordModel forgotPasswordModel);
        Task UpdateAsync(JobSeeker user, string password = null, int? adminUserId = null);
        Task<IdentityResult> DeleteAsync(string userId, int? adminUserId);
        Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task RecordLogon(JobSeeker jobSeeker);
        Task<(JobSeeker jobSeeker, int? adminUserId)> GetUserByImpersonationTokenAsync(string impersonateModelToken);
    }

    public class UserService : IUserService
    {
        private readonly IConfiguration _config;
        private readonly JobBoardContext _context;
        private readonly IEmailSender _emailSender;
        private readonly EmailSettings _emailSettings;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<UserService> _logger;
        private readonly JobSeekerRepository _repository;
        private readonly SignInManager<JobSeeker> _signInManager;
        private readonly UserManager<JobSeeker> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(JobBoardContext context,
            UserManager<JobSeeker> userManager,
            SignInManager<JobSeeker> signInManager,
            IEmailSender emailSender,
            IConfiguration config,
            IWebHostEnvironment env,
            SystemSettingsService systemSetting,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UserService> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _config = config;
            _env = env;
            _repository = new JobSeekerRepository(context, userManager);
            _emailSettings = systemSetting.EmailSettingsAsync().Result;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<(JobSeeker, SignInResult)> Authenticate(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return (null, null);
            }

            JobSeeker jobSeeker = await _userManager.FindByEmailAsync(email);

            // check if username exists
            if (jobSeeker == null)
            {
                throw new AppException(
                    "This email address does not exist in our system.",
                    AppException.Fields.Email
                );
            }

            SignInResult result = await _signInManager.PasswordSignInAsync(jobSeeker, password, false, true);

            if (result.Succeeded)
            {
                if (jobSeeker.AccountStatus == AccountStatus.Deactivated)
                {
                    result = SignInResult.NotAllowed;
                }
            }
            else
            {
                // if the login failed, get a new fresh copy of the jobseeker
                jobSeeker = await _userManager.FindByEmailAsync(email);
            }

            return (jobSeeker, result);
        }

        public JobSeeker GetById(string userId)
        {
            return GetByIdAsync(userId).Result;
        }

        public async Task<JobSeeker> GetByIdAsync(string userId)
        {
            JobSeeker jobSeeker = await _userManager.FindByIdAsync(userId);
            if (jobSeeker != null)
            {
                JobSeekerFlags jobSeekerFlags =
                    _context.JobSeekerFlags.AsNoTracking().FirstOrDefault(x => x.AspNetUserId == userId);
                jobSeeker.JobSeekerFlags = jobSeekerFlags;
            }

            return jobSeeker;
        }

        public async Task<JobSeeker> CreateAsync(JobSeeker jobSeeker, string password)
        {
            if (jobSeeker == null || string.IsNullOrWhiteSpace(jobSeeker.Email) &&
                string.IsNullOrWhiteSpace(jobSeeker.UserName))
            {
                throw new AppException(
                    "Email is required",
                    AppException.Fields.Email
                );
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new AppException(
                    "Password is required",
                    AppException.Fields.Password);
            }

            if (!new EmailAddressAttribute().IsValid(jobSeeker.Email))
            {
                throw new AppException(
                    "Email address is invalid",
                    AppException.Fields.Email
                );
            }

            if (_context.Users.Any(x => x.Email == jobSeeker.Email))
            {
                throw new AppException(
                    $"Email [{jobSeeker.Email}] is already taken",
                    AppException.Fields.Email);
            }

            // set account status to waiting
            jobSeeker.AccountStatus = AccountStatus.Pending;

            (JobSeeker newJobSeeker, IdentityResult result) = await _repository.CreateUserAsync(jobSeeker, password);

            if (!result.Succeeded)
            {
                throw new AppException(
                    result.ToString(), 
                    AppException.Fields.Password);
            }

            await SendActivationEmailAsync(newJobSeeker);

            return newJobSeeker;
        }

        public async Task UpdateAsync(JobSeeker userParam, string password = null, int? adminUserId = null)
        {
            // get the JobSeeker
            JobSeeker jobSeeker = await GetByIdAsync(userParam.Id);

            if (userParam.Email != jobSeeker.Email)
            {
                // Email has changed 
                if (!new EmailAddressAttribute().IsValid(jobSeeker.Email))
                {
                    throw new AppException(
                        "Email address is invalid",
                        AppException.Fields.Email
                    );
                }

                //  check if the new email is already taken
                if (_context.Users.Any(x => x.Email == userParam.Email))
                {
                    throw new AppException(
                        $"Email {userParam.Email} is already taken",
                        AppException.Fields.Email);
                }
            }

            if (userParam.UserName != jobSeeker.UserName)
            {
                // username has changed
                if (!new EmailAddressAttribute().IsValid(jobSeeker.UserName))
                {
                    throw new AppException(
                        "Username is invalid",
                        AppException.Fields.Email);
                }

                // check if the new username is already taken
                if (_context.Users.Any(x => x.UserName == userParam.UserName))
                {
                    throw new AppException(
                        $"Username {userParam.UserName} is already taken",
                        AppException.Fields.Email
                    );
                }
            }

            await _repository.UpdateJobSeekerAsync(jobSeeker, userParam, password, adminUserId);
        }

        public async Task<IdentityResult> DeleteAsync(string userId, int? adminUserId)
        {
            return await _repository.DeleteJobSeekerAsync(userId, adminUserId);
        }

        public async Task<dynamic> ConfirmEmailAsync(ConfirmEmailModel confirmEmailModel)
        {
            JobSeeker user = await GetByIdAsync(confirmEmailModel.UserId);

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                return new {isEmailAlreadyConfirmed = true};
            }

            string token = !confirmEmailModel.IsEncodedCode.HasValue || !confirmEmailModel.IsEncodedCode.Value
                ? confirmEmailModel.Code
                : HttpUtility.UrlDecode(confirmEmailModel.Code);

            IdentityResult result = await _repository.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                _logger.LogInformation("!result.Succeeded");
                throw new InvalidOperationException(
                    $"Error confirming email for user with ID '{confirmEmailModel.UserId}'.");
            }

            _logger.LogInformation("exiting UserService.ConfirmEmailAsync");
            return new {isEmailAlreadyConfirmed = false};
        }

        public async Task SendActivationEmailAsync(string email)
        {
            JobSeeker jobSeeker = await GetByEmailAsync(email);

            await SendActivationEmailAsync(jobSeeker);
        }

        public async Task<dynamic> VerifyRecaptchaAsync(string token)
        {
            string secret = _config["RecaptchaSettings:SecretKey"];
            string url = $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={token}";
            var result = await GetCaptchaResultAsync<dynamic>(url, true);
            return result;
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            JobSeeker jobSeeker = await GetByIdAsync(userId);

            jobSeeker.JobSeekerFlags = null;
            return await _userManager.ChangePasswordAsync(jobSeeker, currentPassword, newPassword);
        }

        public async Task RecordLogon(JobSeeker jobSeeker)
        {
            jobSeeker.LastLogon = DateTime.Now;

            await _userManager.UpdateAsync(jobSeeker);

            var jsLog = new JobSeekerEvent
            {
                AspNetUserId = jobSeeker.Id,
                DateLogged = DateTime.Now,
                EventTypeId = EventType.Login
            };

            await _context.JobSeekerEventLog.AddAsync(jsLog);
            await _context.SaveChangesAsync();
        }

        public async Task<(JobSeeker jobSeeker,int? adminUserId)> GetUserByImpersonationTokenAsync(string token)
        {
            Impersonation impersonationRecord =
                await _context.ImpersonationLog.FirstOrDefaultAsync(i => i.Token == token);

            if (impersonationRecord == null)
            {
                return (null, null);
            }

            if (impersonationRecord.DateTokenCreated.AddSeconds(60) < DateTime.Now)
            {
                return (null, null);
            }

            return (await GetByIdAsync(impersonationRecord.AspNetUserId), impersonationRecord.AdminUserId);
        }

        public async Task SendPwdResetEmailAsync(string email)
        {
            JobSeeker jobSeeker = await GetByEmailAsync(email);
            await SendPwdResetEmailAsync(jobSeeker);
        }

        public async Task<bool> VerifyUserTokenAsync(ForgotPasswordModel forgotPasswordModel)
        {
            JobSeeker jobSeeker = await GetByEmailAsync(forgotPasswordModel.Email);
            jobSeeker.LastModified = DateTime.Now;

            bool result = await _userManager.VerifyUserTokenAsync(
                jobSeeker,
                _userManager.Options.Tokens.PasswordResetTokenProvider,
                UserManager<JobSeeker>.ResetPasswordTokenPurpose,
                forgotPasswordModel.Token);
            return result;
        }

        public async Task<IdentityResult> ResetPasswordAsync(ForgotPasswordModel forgotPasswordModel)
        {
            JobSeeker jobSeeker = await GetByEmailAsync(forgotPasswordModel.Email);

            if (jobSeeker.AccountStatus == AccountStatus.Deleted)
            {
                return  IdentityResult.Failed(new IdentityError { Code = "-1", Description = "Account is deleted."});
            }

            if (jobSeeker.AccountStatus == AccountStatus.Deactivated)
            {
                return IdentityResult.Failed(new IdentityError { Code = "-1", Description = "Account has been deactivated by an administrator." });
            }

            // if the email isn't confirmed then mark it as confirmed (the act of resetting the password confirms that the email is valid)
            if (!jobSeeker.EmailConfirmed)
            {
                // manually confirm the email address because we don't have the token right now
                var userParam = await GetByEmailAsync(forgotPasswordModel.Email);
                userParam.EmailConfirmed = true;
                userParam.AccountStatus = AccountStatus.Active;

                // log the event so it appears in the user's history
                await _repository.LogJobSeekerEvent(jobSeeker.Id, EventType.ConfirmEmail);

                // save the updated jobSeeker to the DB
                await _repository.UpdateJobSeekerAsync(jobSeeker, userParam);
            }

            jobSeeker.LastModified = DateTime.Now;

            IdentityResult result = await _userManager.ResetPasswordAsync(
                jobSeeker,
                forgotPasswordModel.Token,
                forgotPasswordModel.NewPassword);
            return result;
        }

        private async Task SendActivationEmailAsync(JobSeeker jobSeeker)
        {
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(jobSeeker);

            string code = HttpUtility.UrlEncode(token);
            string hashPath = $"#/confirm-email/{jobSeeker.Id}/{code}";
            string linkUrl = GetEmailLinkUrl(hashPath);
            string subject = _emailSettings.Registration.Subject;

            string htmlMessage = string.Format(_emailSettings.Registration.BodyHtml, jobSeeker.FirstName,
                jobSeeker.LastName, jobSeeker.Email, linkUrl, subject);

            string textMessage = string.Format(_emailSettings.Registration.BodyText, jobSeeker.FirstName,
                jobSeeker.LastName, jobSeeker.Email, linkUrl);

            await _emailSender.SendEmailAsync(jobSeeker.Email, subject, htmlMessage, textMessage);
        }

        private async Task<T> GetCaptchaResultAsync<T>(string apiUrl, bool isPostRequest = false,
            HttpContent content = null)
        {
            var result = default(T);

            if (!Uri.TryCreate(apiUrl, UriKind.Absolute, out Uri uri))
            {
                string path = _env.WebRootPath + (apiUrl.StartsWith("\\") ? apiUrl : "\\" + apiUrl);
                if (File.Exists(path))
                {
                    result = JsonConvert.DeserializeObject<T>(await File.ReadAllTextAsync(path));
                }
            }
            else
            {
                HttpClientHandler handler = GetHandler();
                using (var client = new HttpClient(handler))
                {
                    client.BaseAddress = uri; // new Uri(apiUrl);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = !isPostRequest
                        ? await client.GetAsync(apiUrl)
                        : await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        if (typeof(T) != typeof(string))
                        {
                            result = JsonConvert.DeserializeObject<T>(data);
                        }
                        else
                        {
                            result = (T) Convert.ChangeType(data, typeof(T));
                        }
                    }
                }
            }

            return result;
        }

        private HttpClientHandler GetHandler()
        {
            var proxySettings = new ProxySettings();
            _config.GetSection("ProxySettings").Bind(proxySettings);

            var handler = new HttpClientHandler();

            if (proxySettings.UseProxy)
            {
                handler.Proxy = new WebProxy(proxySettings.ProxyHost, proxySettings.ProxyPort)
                {
                    BypassProxyOnLocal = true
                };
            }

            if (proxySettings.IgnoreSslErrors)
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) => true;
            }

            return handler;
        }

        public async Task<JobSeeker> GetByEmailAsync(string email)
        {
            JobSeeker jobSeeker = await _userManager.FindByEmailAsync(email);
            if (jobSeeker == null)
            {
                throw new AppException($"User [{email}] not found", AppException.Fields.Email);
            }

            return jobSeeker;
        }

        public async Task LogJobSeekerEvent(string userId, EventType eventType)
        {
            await _repository.LogJobSeekerEvent(userId, eventType);
        }

        private string GetEmailLinkUrl(string hashPath)
        {
            if (_env.IsDevelopment() && _config["AppSettings:UseSpa"]?.ToLower() == "true")
            {
                var request = _httpContextAccessor.HttpContext.Request;
                return   new Uri(request.Scheme + "://" + request.Host.Value + hashPath).ToString();
            }

            return $"{_config["AppSettings:JbAccountUrl"]}{hashPath}";
        }

        private async Task SendPwdResetEmailAsync(JobSeeker jobSeeker)
        {
            jobSeeker.LastModified = DateTime.Now;

            string token = await _userManager.GeneratePasswordResetTokenAsync(jobSeeker);
            string code = HttpUtility.UrlEncode(token);
            string hashPath = $"#/password-reset/{HttpUtility.UrlEncode(jobSeeker.Email)}/{code}";
            string linkUrl = GetEmailLinkUrl(hashPath);
            string subject = _emailSettings.PasswordReset.Subject;

            string htmlMessage = string.Format(_emailSettings.PasswordReset.BodyHtml, jobSeeker.FirstName,
                jobSeeker.LastName, linkUrl, subject);

            string textMessage = string.Format(_emailSettings.PasswordReset.BodyText, jobSeeker.FirstName,
                jobSeeker.LastName, linkUrl);

            await _emailSender.SendEmailAsync(jobSeeker.Email, subject, htmlMessage, textMessage);
        }
    }
}