using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Infrastructure.API.Configuration.Logging;

public static class LoggingConfiguration
{
    public static string? LogsPath { get; private set; }

    /// <summary>
    /// Configures logger for service using appsettings.json file
    /// </summary>
    /// <param name="configuration">Logger configuration from appsettings.json file</param>
    /// <returns></returns>
    public static void ConfigureLogging(IConfiguration configuration)
    {
        if (configuration.GetSection("Serilog").Exists())
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration) // Чтение конфигурации из appsettings.json
                .CreateLogger();
            LogsPath = GetLogPathFromConfiguration(configuration);
        }
    }

    /// <summary>
    /// Configures logger for service using default configuration
    /// </summary>
    /// <param name="serviceName">Service name to configure logger for</param>
    /// <returns></returns>
    public static void ConfigureLogging(string serviceName)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(
                    outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Service}: {Message}{NewLine}{Exception}")
                .WriteTo.File(
                    path: $"logs/{serviceName}Logs-.txt",
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Service}: {Message}{NewLine}{Exception}"
                )
            .WriteTo.Elasticsearch(
                    nodeUris: "http://localhost:9200",
                    autoRegisterTemplate: true,
                    indexFormat: serviceName + "Logs-{0:yyyy.MM.dd}",
                    inlineFields: true,
                    numberOfReplicas: 2,
                    numberOfShards: 2
                )
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Service", serviceName)
                .CreateLogger();
        LogsPath = $"logs/{serviceName}Logs-";
    }

    private static string? GetLogPathFromConfiguration(IConfiguration configuration)
    {
        var logConfig  = configuration.GetSection("Serilog:WriteTo").GetChildren();
        foreach (var sink in logConfig )
        {
            if (sink["Name"] != "File") continue;
            var logFilePath = sink["Args:path"];
            return logFilePath?.Split('.')[0];
        }
        return null;
    }
}