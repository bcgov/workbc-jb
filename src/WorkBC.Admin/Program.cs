using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using WorkBC.Data;

namespace WorkBC.Admin
{
    public class Program
    {
        private static JobBoardContext _dbContext;

        public static void Main(string[] args)
        {
            // Set Npgsql to avoid complaining about Datetimes.
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            // configuration
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables();
            IConfiguration configuration = builder.Build();

            var logPath = configuration.GetValue<string>("Serilog:WorkBCLogPath");
            var logToFile = configuration.GetValue<bool>("Serilog:FileLoggingEnabled");

            //Serilog settings
            LoggerConfiguration logCofig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console();

            if (logToFile)
            {
                logCofig.WriteTo.File($"{logPath}\\WorkBC.Admin\\log.txt", rollingInterval: RollingInterval.Day);
            }

            Log.Logger = logCofig.CreateLogger();

            //set connection string
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _dbContext = new JobBoardContext(connectionString);

            //check if we should apply any migrations
            bool applyDatabaseMigration = bool.Parse(configuration.GetSection("AppSettings").GetValue<string>("ApplyMigrations"));
            if (applyDatabaseMigration)
            {
                Log.Logger.Information("Checking for migrations");
                //Do DB Migrations if any
                if (_dbContext.Database.GetPendingMigrations().Any())
                {
                    Log.Logger.Information("Applying migrations at startup of website");

                    try
                    {
                        _dbContext.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Error(ex.ToString());
                        throw;
                    }
                }
                else
                {
                    Log.Logger.Information("No pending migrations");
                }
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();
    }
}
