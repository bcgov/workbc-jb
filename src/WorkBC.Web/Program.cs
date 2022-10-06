using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WorkBC.Data;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Hosting;

namespace WorkBC.Web
{
    public class Program
    {
        private static JobBoardContext _dbContext;

        public static void Main(string[] args)
        {
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
                logCofig.WriteTo.File($"{logPath}\\WorkBC.Web\\log.txt", rollingInterval: RollingInterval.Day);
            }

            Log.Logger = logCofig.CreateLogger();

            //set connection string
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _dbContext = new JobBoardContext(connectionString);

            //Start website
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
