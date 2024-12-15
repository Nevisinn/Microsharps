namespace ServiceDiscovery.API.Modules.RoutingModule.DTO;

public class RemoveServiceRequest
{
    public required string ServiceName { get; set; }
    public required string Host { get; set; }
}