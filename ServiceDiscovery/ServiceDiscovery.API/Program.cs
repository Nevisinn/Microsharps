using Infrastructure.API.Configuration.Builder;
using ServiceDiscovery.Logic.ServicesModule;

const string serviceName = "service-discovery";

var builder = MicrosharpsWebAppBuilder.Create(serviceName, false, args)
    .BaseConfiguration(
        isPrivateHosted: true
    )
    .UseLogging(true)
    .ConfigureDi(ConfigureDi);

builder.BuildAndRun();


void ConfigureDi(IServiceCollection services)
{
    services.AddSingleton<IRoutingService, RoutingService>(s =>
    {
        var logger = s.GetRequiredService<ILogger<RoutingService>>();
        return new RoutingService(logger, TimeSpan.FromSeconds(10));
    });
}