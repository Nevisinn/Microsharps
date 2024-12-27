using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Infrastructure.API.Configuration.Logging;

public static class LoggingConfiguration
{
    public static string? LogsPath { get; private set; }


    /// <summary>
    /// Configures logger for service using default configuration
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="serviceName">Service name to configure logger for</param>
    /// <param name="logSinks">Куда писать логи</param>
    /// <param name="elasticsearchHost"></param>
    /// <returns></returns>
    public static void ConfigureSerilog(
        this WebApplicationBuilder builder, 
        string serviceName,
        LogSink[] logSinks,
        string elasticsearchHost = "http://localhost:9200")
    {
        LogsPath = $"logs/{serviceName}Logs-";
        
        var configuration = new LoggerConfiguration();
        if (logSinks.Contains(LogSink.Console))
            configuration.ConfigureConsole();
        if (logSinks.Contains(LogSink.File))
            configuration.ConfigureFile(serviceName);
        if (logSinks.Contains(LogSink.Elastic))
            configuration.ConfigureElastic(serviceName, elasticsearchHost);

        Log.Logger = configuration
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Service", serviceName)
            .CreateLogger();
        builder.Services.AddSerilog();
    }

    /// <summary>
    /// Configures logger for service using appsettings.json file
    /// </summary>
    /// <param name="configuration">Logger configuration from appsettings.json file</param>
    /// <returns></returns>
    [Obsolete("Better use code configuration (with serviceName)")]
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