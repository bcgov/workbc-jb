using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace WorkBC.Shared.Services
{
    public class LoggingService
    {
        public LoggingService(IConfiguration configuration, string projectName)
        {
            var logPath = configuration["Serilog:WorkBCLogPath"];
            var logToFile = configuration.GetValue<bool>("Serilog:FileLoggingEnabled");

            //Serilog settings
            LoggerConfiguration logCofig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console();

            if (logToFile)
            {
                logCofig.WriteTo.File($"{logPath}\\{projectName}\\log.txt", rollingInterval: RollingInterval.Day, shared: true);
            }

            Logger = logCofig.CreateLogger();
        }

        public Logger Logger { get; }
    }
}