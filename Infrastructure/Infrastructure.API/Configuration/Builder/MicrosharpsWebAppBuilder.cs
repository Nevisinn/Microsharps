using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.API.Configuration.Builder;

/// <summary>
/// Билдер веб-аппа для стандартизации, облегчения заведения сервисов и централизованого изменения их базовой конфигурации
/// </summary>
public class MicrosharpsWebAppBuilder
{
    private readonly WebApplicationBuilder builder;
    private readonly List<Action<WebApplication>> appConfigs;

    /// <summary>
    /// Создаёт инстанс билдера
    /// </summary>
    /// <param name="isStrictBuild">Нужно ли проверять корректность конфигурации</param>
    /// <param name="args"></param>
    public static MicrosharpsWebAppBuilder Create(bool isStrictBuild, string[] args)
        => new(isStrictBuild, args);
    
    private MicrosharpsWebAppBuilder(bool isStrictBuild, string[] args)
    {
        builder = WebApplication.CreateBuilder(args);
        appConfigs = new List<Action<WebApplication>>();
    }

    public MicrosharpsWebAppBuilder BaseConfiguration(bool isPrivateHosted)
    {
        builder.Services.AddEndpointsApiExplorer();
        UseControllers();
        UseSwagger();
        appConfigs.Add(app =>
        {
            app.UseAuthentication();
            app.UseAuthorization();
        });
        
        if (isPrivateHosted)
            appConfigs.Add(app => app.UseCors(opt => opt.AllowAll()));
        else
            appConfigs.Add(app => app.UseHttpsRedirection());
        

        return this;
    }

    public MicrosharpsWebAppBuilder UseControllers()
    {
        builder.Services.AddControllers();
        appConfigs.Add(app => app.MapControllers());

        return this;
    }

    public MicrosharpsWebAppBuilder UseSwagger()
    {
        builder.Services.AddSwaggerGen(opt => 
        {
            var xmlFilename = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
            opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        
        appConfigs.Add(app =>
        {
            // if (app.Environment.IsDevelopment()) TODO
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            } 
        });

        return this;
    }

    public MicrosharpsWebAppBuilder RegisterInServiceDiscovery(
        string serviceName, 
        string? serviceDiscoveryHost = null,
        string? scheme = null)
    {
        builder.Services.RegisterServiceDiscovery(serviceName);
        appConfigs.Add(app => app.RegisterInServiceDiscovery());
        
        return this;
    }

    public MicrosharpsWebAppBuilder ConfigureDi(Action<IServiceCollection> configuration)
    {
        configuration(builder.Services);
        return this;
    }


    public void BuildAndRun()
    {
        var app = builder.Build();
        foreach (var appConfig in appConfigs)
        {
            appConfig(app);
        }
        
        app.Run();
    }
}