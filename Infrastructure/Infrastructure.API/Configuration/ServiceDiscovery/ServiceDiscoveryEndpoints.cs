using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.API.Configuration.ServiceDiscovery;

public static class ServiceDiscoveryEndpoints
{
    public static void MapServiceDiscoveryEndpoints(this WebApplication app)
    {
        app.MapGet("api/health/ping", Mock)
            .WithSummary("Ping endpoint for Service-discovery")
            .Produces((int)HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Мок-функция, чтобы в сваггере имя Группы методов было как у этого класса (ServiceDiscoveryEndpoints)
    /// </summary>
    /// <returns></returns>
    private static IResult Mock() => Results.NoContent();
}