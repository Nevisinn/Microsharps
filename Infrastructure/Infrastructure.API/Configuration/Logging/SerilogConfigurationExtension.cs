using Serilog;
using Serilog.Events;

namespace Infrastructure.API.Configuration.Logging;

public static class SerilogConfigurationExtension
{
    private const string LogDefaultTemplate = "{Timestamp:HH:mm:ss} [{Level}] [{Service}]: {Message}{NewLine}{Exception}";

    
    public static LoggerConfiguration ConfigureConsole(this LoggerConfiguration configuration)
    {
        configuration.WriteTo.Console(
            outputTemplate: LogDefaultTemplate
        );
        return configuration;
    }
    
    public static LoggerConfiguration ConfigureFile(this LoggerConfiguration configuration, string serviceName)
    {
        configuration.WriteTo.File(
            path: $"logs/{serviceName}Logs-.txt",
            rollingInterval: RollingInterval.Day,
            restrictedToMinimumLevel: LogEventLevel.Information,
            outputTemplate: LogDefaultTemplate
        );
        return configuration;
    }
    
    public static LoggerConfiguration ConfigureElastic(this LoggerConfiguration configuration, string serviceName, string elasticsearchHost)
    {
        configuration.WriteTo.Elasticsearch(
            nodeUris: elasticsearchHost,
            autoRegisterTemplate: true,
            indexFormat: serviceName + "Logs-{0:yyyy.MM.dd}",
            inlineFields: true,
            numberOfReplicas: 2,
            numberOfShards: 2
        );
        return configuration;
    }
}