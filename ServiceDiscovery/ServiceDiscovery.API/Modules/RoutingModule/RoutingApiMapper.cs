using ServiceDiscovery.API.Logic.ServicesModule.DTO;

namespace ServiceDiscovery.API.Modules.RoutingModule;

public static class RoutingApiMapper
{
    public static RegisterServiceRequest Map(DTO.RegisterServiceRequest request, string host)
        => new()
        {
            ServiceName = request.ServiceName,
            Host = host,
        };

    public static RemoveServiceRequest Map(DTO.RemoveServiceRequest request)
        => new()
        {
            ServiceName = request.ServiceName,
            Host = request.Host,
        };

    public static DTO.RegisterServiceResponse Map(RegisterServiceResponse response)
        => new()
        {
            Host = response.Host,
        };
}