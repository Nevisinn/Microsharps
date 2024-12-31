using System.Reflection;
using Infrastructure.API.Configuration.Logging;
using Infrastructure.API.Configuration.ServiceDiscovery;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Infrastructure.API.Configuration.Builder;

/// <summary>
/// Билдер веб-аппа для стандартизации, облегчения заведения сервисов и централизованого изменения их базовой конфигурации
/// </summary>
public class MicrosharpsWebAppBuilder
{
    internal readonly string serviceName;
    internal readonly WebApplicationBuilder nativeBuilder;
    internal readonly WebApplicationConfig appConfig;

    /// <summary>
    /// Создаёт инстанс билдера
    /// </summary>
    /// <param name="serviceName">Название сервиса</param>
    /// <param name="isStrictBuild">Нужно ли проверять корректность конфигурации</param>
    /// <param name="args"></param>
    public static MicrosharpsWebAppBuilder Create(string serviceName, bool isStrictBuild, string[] args)
        => new(serviceName, isStrictBuild, args);
    
    private MicrosharpsWebAppBuilder(string serviceName, bool isStrictBuild, string[] args)
    {
        this.serviceName = serviceName;
        nativeBuilder = WebApplication.CreateBuilder(args);
        appConfig = new();
    }

    public MicrosharpsWebAppBuilder BaseConfiguration(
        bool isPrivateHosted, 
        string? swaggerRequestsPrefix = null)
    {
        nativeBuilder.Services.AddEndpointsApiExplorer();
        UseControllers();
        this.UseSwagger(swaggerRequestsPrefix ?? EnvironmentVars.SwaggerRequestsPrefix);
        UseLogging(true, LogSinks.OnlyLocal);
        appConfig.Add(app =>
        {
            app.UseAuthentication();
            app.UseAuthorization();
        });
        
        if (isPrivateHosted)
            appConfig.Add(app => app.UseCors(opt => opt.AllowAll()));
        else
            appConfig.Add(app => app.UseHttpsRedirection());
        

        return this;
    }

    public MicrosharpsWebAppBuilder UseControllers()
    {
        nativeBuilder.Services.AddControllers();
        appConfig.Add(app => app.MapControllers());

        return this;
    }
    


    

    public MicrosharpsWebAppBuilder UseLogging(
        bool withEndpoints, 
        LogSink[]? customLogSinks = null)
    {
        nativeBuilder.ConfigureSerilog(serviceName, customLogSinks ?? LogSinks.All);
        appConfig.Add(app => app.UseSerilogRequestLogging());
        if (withEndpoints)
        {
            nativeBuilder.Services.AddSingleton<ILogsService, LogsService>();
            appConfig.Add(app => app.MapLoggingEndpoints());
        }
        return this;
    }

    public MicrosharpsWebAppBuilder RegisterInServiceDiscovery(
        string? ownHost = null,
        string? serviceDiscoveryHost = null)
    {
        ownHost ??= EnvironmentVars.OwhHost;
        serviceDiscoveryHost ??= EnvironmentVars.SdHost;
        nativeBuilder.Services.RegisterServiceDiscoveryConfigurationClient(serviceName, ownHost, serviceDiscoveryHost);
        appConfig.Add(app => app.MapServiceDiscoveryEndpoints());
        appConfig.Add(app => app.ConfigureLifetimeInServiceDiscovery());
        
        return this;
    }

    public MicrosharpsWebAppBuilder ConfigureDi(Action<IServiceCollection> configuration)
    {
        configuration(nativeBuilder.Services);
        return this;
    }


    public void BuildAndRun()
    {
        var app = nativeBuilder.Build();
        appConfig.Apply(app);
        
        app.Run();
    }
}