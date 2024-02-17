using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System;
using System.IO;
using System.Reflection;

namespace DotNetLib.Serilog
{
    public static class SerilogExtensions
    {
        public static ILogger CreateLoggerConfiguration(bool writeToConsole = true, bool writeToSqlServer = false)
        {
            var configuration = ReadConfiguration();

            return CreateLogger(configuration, writeToConsole, writeToSqlServer);
        }

        private static IConfigurationRoot ReadConfiguration()
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            if (!string.IsNullOrWhiteSpace(envName))
                builder.AddJsonFile($"appsettings.{envName}.json", optional: true);

            var configuration = builder.Build();
            return configuration;
        }

        private static ILogger CreateLogger(IConfigurationRoot configuration, bool writeToConsole = true, bool writeToSqlServer = false)
        {
            var lc = new LoggerConfiguration();

            lc
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProcessName()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("ApplicationName", AppName())
            .Filter.ByExcluding("RequestPath like '%health%'")
            .Filter.ByExcluding("RequestPath like '%/hangfire/stats%'")
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .WriteTo.Console();

            if (writeToConsole)
                WriteToFile(lc);

            if (writeToSqlServer)
                WriteToMSSqlServer(lc, configuration);

            return lc.CreateLogger();
        }

        private static void WriteToFile(LoggerConfiguration lc)
        {
            var filePath = LogFilePath();
            lc.WriteTo.File(filePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7);
        }

        private static void WriteToMSSqlServer(LoggerConfiguration loggerConfiguration, IConfiguration configuration)
        {
            var defaultConnection = configuration.GetConnectionString("DefaultConnection");
            if (defaultConnection != null)
            {
                loggerConfiguration.WriteTo.MSSqlServer(
                    connectionString: defaultConnection,
                    sinkOptions: new MSSqlServerSinkOptions { TableName = "AppLogs", AutoCreateSqlTable = true });
            }
        }

        private static string LogFilePath()
        {
            var assemblyName = AppName();
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), assemblyName, "log.txt");
            return filePath;
        }

        private static string AppName()
        {
            var aapName = Assembly.GetEntryAssembly()?.GetName().Name;
            return aapName;
        }
        public static void UseSerilogHttpRequestLogging(this IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging((o) =>
            {
                o.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("Identity", httpContext.Request.HttpContext.User.Identity.Name);
                    diagnosticContext.Set("RemoteIpAddress", httpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString());
                };
            });
        }
    }
}
