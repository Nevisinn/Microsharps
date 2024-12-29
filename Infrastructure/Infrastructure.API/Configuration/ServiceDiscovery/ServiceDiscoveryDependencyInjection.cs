using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.API.Configuration.ServiceDiscovery;

public static class ServiceDiscoveryDependencyInjection
{
    public static void RegisterServiceDiscoveryConfigurationClient(
        this IServiceCollection services, 
        string serviceName,
        string? ownHost,
        string? serviceDiscoveryHost)
    {
        services.AddSingleton<IServiceDiscoveryConfigurationClient, ServiceDiscoveryConfigurationClient>(
            s =>
            {
                var host = ownHost
                           ?? s.GetService<IServer>()!.Features.Get<IServerAddressesFeature>()!.Addresses.First();
                
                return new ServiceDiscoveryConfigurationClient(
                    new[] {host},
                    serviceName,
                    serviceDiscoveryHost);
            });
    }
}