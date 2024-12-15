namespace ServiceDiscovery.Client;

public interface IServiceDiscoveryClient
{
    
}

public class ServiceDiscoveryClient : IServiceDiscoveryClient
{
    private readonly string serviceDiscoveryHost;
    private readonly string serviceName;
    
    private static readonly HttpClient httpClient = new();

    public ServiceDiscoveryClient(string serviceName, string? serviceDiscoveryHost)
    {
        this.serviceName = serviceName;
        this.serviceDiscoveryHost = serviceDiscoveryHost ?? "127.0.0.1:8888";
    }

    public async Task<HttpResponseMessage> SendRequest(HttpRequestMessage requestMessage, CancellationToken? token)
    {
        return token != null
            ? await httpClient.SendAsync(requestMessage, token.Value)
            : await httpClient.SendAsync(requestMessage);
    }

    private async Task<string> ResolveServiceAddress()
    {
        var serviceDiscoveryResponse = await httpClient.GetAsync(serviceDiscoveryHost + $"/api/Routing/{serviceName}");
        var a = serviceDiscoveryResponse.Content;
        throw new NotImplementedException();
    }
}