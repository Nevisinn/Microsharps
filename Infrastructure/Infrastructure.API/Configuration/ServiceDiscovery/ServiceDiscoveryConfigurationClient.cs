using System.Net.Http.Json;
using Infrastructure.API.Configuration.ServiceDiscovery.Requests;

namespace Infrastructure.API.Configuration.ServiceDiscovery;

public interface IServiceDiscoveryConfigurationClient
{
    Task<Result<RegisterServiceResponseModel>> Register();
    Task<EmptyResult> Remove();
}

public class ServiceDiscoveryConfigurationClient : IServiceDiscoveryConfigurationClient
{
    private readonly string[] hosts;
    private readonly string serviceDiscoveryHost;
    private readonly string serviceName;
    private string BasePath => $"{serviceDiscoveryHost}/api/Routing";
    
    private static readonly HttpClient HttpClient = new();

    public ServiceDiscoveryConfigurationClient(
        IEnumerable<string> hosts,
        string serviceName, 
        string? serviceDiscoveryHost)
    {
        this.hosts = hosts.ToArray();
        this.serviceName = serviceName;
        this.serviceDiscoveryHost = serviceDiscoveryHost ?? "http://localhost:8888";
    }
    
    public async Task<Result<RegisterServiceResponseModel>> Register()
    {
        var content = JsonContent.Create(new RegisterServiceRequestModel()
        {
            Hosts = hosts,
            ServiceName = serviceName,
        });
        var response = await HttpClient.PostAsync(BasePath, content);
        if (!response.IsSuccessStatusCode)
            return Result.BadRequest<RegisterServiceResponseModel>(await response.Content.ReadAsStringAsync());

        var deserializedContent = await response.Content.ReadFromJsonAsync<RegisterServiceResponseModel>();
        if (deserializedContent == null)
            return Result.BadRequest<RegisterServiceResponseModel>("Success Register host, but empty response");
        
        return Result.Ok(deserializedContent);
    }

    public async Task<EmptyResult> Remove()
    {
        var content = JsonContent.Create(new RemoveServiceRequestModel()
        {
            Hosts = hosts,
            ServiceName = serviceName,
        });
        var httpRequestMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Delete,
            Content = content,
            RequestUri = new Uri(BasePath),
        };
        var response = await HttpClient.SendAsync(httpRequestMessage);
        if (!response.IsSuccessStatusCode)
            return EmptyResult.BadRequest(await response.Content.ReadAsStringAsync());

        return EmptyResult.Success();
    }

    private string ToIpAddress(string host)
    {
        return host
            .Replace("http://", "")
            .Replace("localhost", "127.0.0.1");
    }
}