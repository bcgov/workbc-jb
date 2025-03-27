using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Serilog.Extensions.Logging;
using StackExchange.Redis;
using WorkBC.Admin.Areas.Jobs.Services;
using WorkBC.Admin.Areas.JobSeekers.Services;
using WorkBC.Admin.Areas.Reports.Data;
using WorkBC.Admin.Helpers;
using WorkBC.Admin.Middleware;
using WorkBC.Admin.Services;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Admin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var oidcRealmUri = $"{Configuration["Keycloak:Domain"]}/auth/realms/{Configuration["Keycloak:Realm"]}";
            string oidcClientId = Configuration["Keycloak:ClientId"];
            string oidcClientSecret = Configuration["Keycloak:ClientSecret"];

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddAutoMapper(typeof(Startup));

            services.AddDataProtection()
                .PersistKeysToDbContext<JobBoardContext>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.Secure = CookieSecurePolicy.Always;
            });

            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<JobBoardContext>(options =>
                options.UseSqlServer(connectionString));

            // We need to hook up IdentityService in the admin site so we can get an instance of the UserManager to
            // add and edit users.  It's not actually used for logins on the admin site (Keycloak is used instead).
            services.AddDefaultIdentity<JobSeeker>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.SignIn.RequireConfirmedEmail = true;
                // don't restrict characters allowed in usernames (trust email address validation instead)
                options.User.AllowedUserNameCharacters = null;
            })
                .AddEntityFrameworkStores<JobBoardContext>();

            // Custom Dapper DB context
            services.AddScoped(db => new DapperContext(connectionString));

            //Cache
            //Temp logs
            var logger1 = new SerilogLoggerFactory().CreateLogger<Startup>();
            logger1.LogWarning("WorkBC Admin logs- Value of UseRedisCache setting is :" + Configuration["AppSettings:UseRedisCache"]);

            if (Configuration["AppSettings:UseRedisCache"] == "true")
            {
                ConfigurationOptions redisOptions =
                    ConfigurationOptions.Parse(Configuration.GetConnectionString("Redis"));

                logger1.LogWarning("WorkBC Admin logs- Value of redisOptions.SslHost setting is :" + redisOptions.SslHost);

                redisOptions.TieBreaker = "";
                redisOptions.AllowAdmin = true;
                redisOptions.AbortOnConnectFail = false;

                // var multiplexer = ConnectionMultiplexer.Connect(redisOptions);
                // services.AddSingleton<IConnectionMultiplexer>(multiplexer);

                services.AddDistributedRedisCache(options => options.ConfigurationOptions = redisOptions);
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            services.AddTransient<SelectListService>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(options =>
                {
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.LoginPath = "/home/NotAuthorized";
                    options.AccessDeniedPath = "/home/NotAuthorized";
                    options.Events = new CookieAuthenticationEvents
                    {
                        // check if the access token needs to be refreshed, and refresh it if needed
                        OnValidatePrincipal = async ctx =>
                        {
                            await JobBoardAdminOpenIdConnectHelper.HandleOidcRefreshToken(
                                ctx,
                                oidcRealmUri,
                                oidcClientId,
                                oidcClientSecret
                            );
                        }
                    };
                })
                .AddOpenIdConnect(options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = oidcRealmUri;
                    options.RequireHttpsMetadata = true;
                    options.ClientId = oidcClientId;
                    options.ClientSecret = oidcClientSecret;
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.SaveTokens = true;
                    if (bool.Parse(Configuration["ProxySettings:UseProxy"]))
                    {
                        options.BackchannelHttpHandler = new HttpClientHandler
                        {
                            UseProxy = true,
                            Proxy = new WebProxy(
                                Configuration["ProxySettings:ProxyHost"],
                                int.Parse(Configuration["ProxySettings:ProxyPort"])),
                            ServerCertificateCustomValidationCallback = delegate { return true; }
                        };
                    }

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        ValidateIssuer = true
                    };
                    options.Events = new OpenIdConnectEvents
                    {
                        OnTokenValidated = ctx =>
                        {
                            // add extra job board admin claims
                            JobBoardAdminOpenIdConnectHelper.HandleAdminUserLogin(ctx);
                            return Task.CompletedTask;
                        },
                        OnRedirectToIdentityProvider = ctx =>
                        {
                            // add a parameter to the keycloak redirect querystring
                            ctx.ProtocolMessage.SetParameter("kc_idp_hint", "idir");
                            // change the redirect uri to the reverse proxy
                            if (ctx.Request.Headers.Keys.Contains("X-Forwarded-Host"))
                            {
                                var host = ctx.Request.Headers["X-Forwarded-Host"][0];
                                ctx.ProtocolMessage.SetParameter("redirect_uri", $"https://{host}/signin-oidc");
                            }
                            return Task.FromResult(0);
                        },
                        OnRedirectToIdentityProviderForSignOut = ctx =>
                        {
                            // change the post-logout redirect uri to the reverse proxy
                            if (ctx.Request.Headers.Keys.Contains("X-Forwarded-Host"))
                            {
                                var host = ctx.Request.Headers["X-Forwarded-Host"][0];
                                ctx.ProtocolMessage.SetParameter("post_logout_redirect_uri", $"https://{host}/");
                            }
                            return Task.FromResult(0);
                        },
                        OnRemoteFailure = ctx =>
                        {
                            var logger = new SerilogLoggerFactory().CreateLogger<Startup>();
                            logger.LogWarning("WorkBC KC OnRemoteFailure redirect to '/'");
                            logger.LogWarning(ctx.Failure?.ToString() ?? "ctx.Failure is null");
                            ctx.Response.Redirect("/");
                            ctx.HandleResponse();
                            return Task.FromResult(0);
                        }
                    };
                });

            services.AddAuthorization();

            services.AddTransient<JobBoardAdminAccountMiddleware>();

            services.AddMvc(options => options.EnableEndpointRouting = false);

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJobService, JobService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                // for debugging Keycloak
                IdentityModelEventSource.ShowPII = true;
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = HttpOnlyPolicy.Always,
                MinimumSameSitePolicy = SameSiteMode.None,
                Secure = CookieSecurePolicy.Always
            });

            app.UseAuthentication();
            app.UseMiddleware<JobBoardAdminAccountMiddleware>();
            app.UseAuthorization();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "areaRoute",
                    "{area}/{controller=Default}/{action=Index}/{id?}");

                routes.MapRoute(
                    "defaultRoute",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}