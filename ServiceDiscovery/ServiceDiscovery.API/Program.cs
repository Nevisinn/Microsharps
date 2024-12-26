using Infrastructure.API.Configuration.Builder;
using ServiceDiscovery.Logic.ServicesModule;

var builder = MicrosharpsWebAppBuilder.Create(false, args)
    .BaseConfiguration(
        isPrivateHosted: true
    )
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