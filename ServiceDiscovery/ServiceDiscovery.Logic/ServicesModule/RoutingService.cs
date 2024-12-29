using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Infrastructure;
using Microsoft.Extensions.Logging;
using ServiceDiscovery.Logic.ServicesModule.DTO;
using ServiceDiscovery.Logic.ServicesModule.Models;

namespace ServiceDiscovery.Logic.ServicesModule;

public interface IRoutingService
{
    Task<Result<RegisterServiceResponse>> RegisterHosts(RegisterServiceRequest request);
    Result<GetServiceHostResponse> GetServiceHost(string serviceName);
    Result<ServiceRoutingInfo[]> GetAllServicesWithHosts();
    EmptyResult RemoveHosts(RemoveServiceRequest request);
}

public class RoutingService : IRoutingService
{
    private readonly ILogger<RoutingService> logger;
    private readonly ConcurrentDictionary<string, List<string>> hostsByService; // TODO: Add ThreadSafeList
    private readonly ConcurrentDictionary<string, List<string>> blackListedHostsByService;
    private readonly Random random;

    private static readonly HttpClient HttpClient = new();
    private const string PingEndpoint = "api/health/ping";

    public RoutingService(ILogger<RoutingService> logger, TimeSpan healthCheckingDelay) // TODO: logger
    {
        this.logger = logger;
        hostsByService = new();
        blackListedHostsByService = new();
        random = new Random();
        StartHealthPing(healthCheckingDelay, new CancellationToken());
    }

    public async Task<Result<RegisterServiceResponse>> RegisterHosts(RegisterServiceRequest request)
    {
        var existedHosts = GetHosts(request.ServiceName);
        foreach (var newHost in request.Hosts)
        {
            if (!existedHosts.Contains(newHost))
            {
                var isHostAvailable = await TryPing(newHost);
                if (!isHostAvailable.IsSuccess)
                {
                    logger.LogWarning($"Can not ping new host: {newHost} of service: {request.ServiceName}");
                    continue;
                }
                
                lock (existedHosts)
                {
                    existedHosts.Add(newHost);
                    logger.LogInformation($"Registered service: {request.ServiceName} with host: {newHost}");
                }
            }
            else
            {
                return Result.BadRequest<RegisterServiceResponse>($"Same host already used for this service. host: {newHost}");
            }
        }
        
        return Result.Ok(new RegisterServiceResponse
        {
            Hosts = existedHosts.Select(e => e.ToString()).ToArray(),
        });
    }

    public Result<GetServiceHostResponse> GetServiceHost(string serviceName)
    {
        var hosts = GetHosts(serviceName);
        if (hosts.Count == 0)
            return Result.NotFound<GetServiceHostResponse>("Service has not registered any instances");
        
        var randomHost = hosts[random.Next(hosts.Count)];
        return Result.Ok(new GetServiceHostResponse()
        {
            Host = randomHost
        });
    }

    public Result<ServiceRoutingInfo[]> GetAllServicesWithHosts()
    {
        return Result.Ok(hostsByService.Select(e => new ServiceRoutingInfo
        {
            ServiceName = e.Key,
            Hosts = e.Value.Select(ip => ip.ToString()).ToArray(),
        }).ToArray());
    }

    public EmptyResult RemoveHosts(RemoveServiceRequest request)
    {
        var hosts = GetHosts(request.ServiceName);
        foreach (var hostToDel in request.Hosts)
        {
            if (!hosts.Contains(hostToDel))
                continue;
            
            lock (hosts)
            {
                hosts.Remove(hostToDel);
            }
        }
        
        return EmptyResult.Success();
    }
    
    private List<string> GetHosts(string serviceName) 
        => hostsByService.GetOrAdd(serviceName, _ => new());

    private async Task StartHealthPing(TimeSpan healthCheckingDelay, CancellationToken token)
    {
        async Task Ping()
        {
            foreach (var serviceWithHosts in hostsByService)
            {
                var serviceName = serviceWithHosts.Key;
                var hosts = serviceWithHosts.Value;
                foreach (var host in hosts)
                {
                    var connectionResult = await TryPing(host);
                    if (connectionResult.IsSuccess) continue;

                    logger.LogWarning($"Can not connect to host: {host}. It will be removed.{Environment.NewLine}Error: {connectionResult.Error}");
                    blackListedHostsByService.AddOrUpdate(
                        serviceName,
                        _ => new() { host }, 
                        (_, list) =>
                        {
                            list.Add(host);
                            return list;
                        });
                }
            }
            if (blackListedHostsByService.Count == 0)
                return;
            
            foreach (var toRemove in blackListedHostsByService)
            {
                var serviceName = toRemove.Key;
                var hostsToRemove = toRemove.Value;
                lock (hostsByService[serviceName])
                {
                    hostsByService[serviceName].RemoveAll(host => hostsToRemove.Contains(host));
                }
            }
            blackListedHostsByService.Clear();
        }

        while (!token.IsCancellationRequested)
        {
            await Task.Delay(healthCheckingDelay); // TODO: пинги падают после 1 неудачного
            await Ping();
        }
    }

    private async Task<EmptyResult> TryPing(string host)
    {
        try
        {
            var response = await HttpClient.GetAsync($"{host}/{PingEndpoint}");
            if (!response.IsSuccessStatusCode)
                return EmptyResult.BadRequest($"Can not establish connection with host. Error code: {response.StatusCode}");
            
            return EmptyResult.Success();
        }
        catch (Exception e)
        {
            return EmptyResult.BadRequest(e.Message + Environment.NewLine + e.StackTrace);
        }
    }
}