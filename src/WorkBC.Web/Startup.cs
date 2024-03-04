using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WorkBC.Data;
using WorkBC.Data.Model.JobBoard;
using WorkBC.Shared.Services;
using WorkBC.Shared.Settings;
using WorkBC.Web.Helpers;
using WorkBC.Web.Services;
using WorkBC.Web.Settings;
using EmailSettings = WorkBC.Shared.Settings.EmailSettings;
using IEmailSender = WorkBC.Shared.Services.IEmailSender;

namespace WorkBC.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string ApiOrigins = "_API";
        private AppSettings _appSettings;
        private EmailSettings _emailSettings;
        private ProxySettings _proxySettings;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddAutoMapper(typeof(Startup));

            services.AddDataProtection()
                .PersistKeysToDbContext<JobBoardContext>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<JobBoardContext>(options =>
                options
                    .EnableSensitiveDataLogging()
                    .UseSqlServer(Configuration.GetConnectionString("DefaultConnection")), 
                ServiceLifetime.Transient);

            services.AddDbContext<EnterpriseContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("EnterpriseConnection")),
                ServiceLifetime.Transient);
            
            services.AddDbContext<SsotContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("SsotConnection")), ServiceLifetime.Transient);

            services.AddDefaultIdentity<JobSeeker>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.SignIn.RequireConfirmedEmail = true; 
                // lockout for 10 minutes after 50 failed login attempts
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                // don't restrict characters allowed in usernames (trust email address validation instead)
                options.User.AllowedUserNameCharacters = null; 
            })
                .AddEntityFrameworkStores<JobBoardContext>();

            // configure strongly typed token management object
            var tokenManagementSection = Configuration.GetSection("TokenManagement");
            services.Configure<TokenManagement>(tokenManagementSection);
            var tokenManagement = tokenManagementSection.Get<TokenManagement>();
            var key = Encoding.ASCII.GetBytes(tokenManagement.Secret);

            // configure strongly typed settings objects
            // AppSettings
            IConfigurationSection appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            _appSettings = appSettingsSection.Get<AppSettings>();
            // EmailSettings
            IConfigurationSection emailSettingsSection = Configuration.GetSection("EmailSettings");
            services.Configure<EmailSettings>(emailSettingsSection);
            _emailSettings = emailSettingsSection.Get<EmailSettings>();
            // ProxySettings
            IConfigurationSection proxySettingsSection = Configuration.GetSection("ProxySettings");
            services.Configure<ProxySettings>(proxySettingsSection);
            _proxySettings = proxySettingsSection.Get<ProxySettings>();

            //CORS
            services.AddCors(options =>
            {
                options.AddPolicy(ApiOrigins,
                    builder => builder
                        .WithOrigins(_appSettings.CorsDomains)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                );
            });

            // configure jwt authentication
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                            string userId = context.Principal.Identity.Name;
                            JobSeeker user = userService.GetById(userId);
                            if (user == null)
                            {
                                // return unauthorized if user no longer exists
                                context.Fail("Unauthorized");
                            }

                            return Task.CompletedTask;
                        }
                    };
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            // Replace the existing scoped IPasswordHasher<> implementation
            services.Replace(new ServiceDescriptor(
                typeof(IPasswordHasher<JobSeeker>),
                typeof(Md5PasswordHasher<JobSeeker>),
                ServiceLifetime.Scoped));

            services.AddMvc(options => options.EnableEndpointRouting = false);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "WorkBC Job Board API"
                });

                // Set the comments path for the Swagger JSON and UI.
                string xmlPath = Path.Combine(AppContext.BaseDirectory, "WorkBC.Web.xml");
                c.IncludeXmlComments(xmlPath);
                xmlPath = Path.Combine(AppContext.BaseDirectory, "WorkBC.ElasticSearch.Models.xml");
                c.IncludeXmlComments(xmlPath);
            });

            // configure DI for application services
            services.AddHttpContextAccessor();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ISecurityQuestionService, SecurityQuestionService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<ILogger<IGeocodingService>, Logger<IGeocodingService>>();
            services.AddScoped<IGeocodingService, GeocodingService>();
            services.AddScoped<INocSearchService, NocSearchService>();
            services.AddScoped<ISavedJobsService, SavedJobsService>();
            services.AddScoped<IRecommendedJobsService, RecommendedJobsService>();
            services.AddScoped<IJobAlertsService, JobAlertsService>();
            services.AddSingleton<SystemSettingsService>();
            services.AddSingleton(Serilog.Log.Logger);

            if (_emailSettings.UseSmtp)
            {
                services.AddTransient<IEmailSender, SmtpEmailSender>();
            }
            else if (_emailSettings.UseSes)
            {
                services.AddTransient<IEmailSender, AmazonSesEmailSender>();
            }
            else
            {
                services.AddTransient<IEmailSender, SendGridEmailSender>();
            }

            // The Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => {
                if (_appSettings.UseJbAccountApp)
                {
                    configuration.RootPath = "wwwroot/dist/jb-account";
                }
                else
                {
                    configuration.RootPath = "wwwroot/dist/jb-search";
                }
            });

            //Cache
            if (bool.Parse(_appSettings.UseRedisCache))
            {
                ConfigurationOptions redisOptions =
                    ConfigurationOptions.Parse(Configuration.GetConnectionString("Redis"));

                redisOptions.TieBreaker = "";
                redisOptions.AllowAdmin = true;
                redisOptions.AbortOnConnectFail = false;

                //var multiplexer = ConnectionMultiplexer.Connect(redisOptions);
                //services.AddSingleton<IConnectionMultiplexer>(multiplexer);

                services.AddDistributedRedisCache(options => options.ConfigurationOptions = redisOptions);
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            services.AddSingleton<CacheService>();

            // todo: temporarily  added newtonsoft json
            services.AddControllersWithViews().AddNewtonsoftJson(
                options => { options.SerializerSettings.Formatting = Formatting.Indented; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            if (!_appSettings.IsProduction)
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = context =>
                {
                    context.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                }
            });

            bool useSpa = env.IsDevelopment() && _appSettings.UseSpa;

            if (useSpa)
            {
                app.UseSpaStaticFiles();
            }

            app.UseCors(ApiOrigins);
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                // don't set a default MVC controller if Spa is being used
                // (Spa will be come the default)
                string routeTemplate = useSpa
                    ? "{controller}/{action=Index}/{id?}"
                    : "{controller=FakeKentico}/{action=Index}/{id?}";

                routes.MapRoute(
                    name: "default",
                    template: routeTemplate);
            });

            if (useSpa)
            {
                app.UseSpa(spa =>
                {
                    // To learn more about options for serving an Angular SPA from ASP.NET Core,
                    // see https://go.microsoft.com/fwlink/?linkid=864501

                    spa.Options.SourcePath = "ClientApp";
                    if (_appSettings.UseJbAccountApp)
                    {
                        spa.UseAngularCliServer(npmScript: "startJbAccount");
                    }
                    else
                    {
                        spa.UseAngularCliServer(npmScript: "startJbSearch");
                    }
                });
            }
        }
    }
}
