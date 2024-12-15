using System.Collections.Concurrent;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Infrastructure;
using ServiceDiscovery.API.Logic.ServicesModule.DTO;

namespace ServiceDiscovery.API.Logic.ServicesModule;

public interface IRoutingService
{
    Result<RegisterServiceResponse> RegisterService(RegisterServiceRequest request);
    Result<string> GetService(string serviceName);
    Result<bool> RemoveService(RemoveServiceRequest request);
}

public class RoutingService : IRoutingService
{
    private readonly ConcurrentDictionary<string, List<string>> hostsByService; // TODO: Add ThreadSafeList
    private readonly Random random;

    public RoutingService(TimeSpan healthCheckingDelay) // TODO: logger
    {
        hostsByService = new ConcurrentDictionary<string, List<string>>();
        random = new Random();
        StartHealthPing(healthCheckingDelay, new CancellationToken());
    }

    public Result<RegisterServiceResponse> RegisterService(RegisterServiceRequest request)
    {
        var newHost = request.Host;
        var hosts = GetHosts(request.ServiceName);
        if (!hosts.Contains(newHost))
        {
            lock (hosts)
            {
                hosts.Add(newHost);
            }
        }
        else
        {
            return Result.BadRequest<RegisterServiceResponse>("Same host already used for this service");
        }

        return Result.Ok(new RegisterServiceResponse
        {
            Host = newHost,
        });
    }

    public Result<string> GetService(string serviceName)
    {
        var hosts = GetHosts(serviceName);
        if (hosts.Count == 0)
            return Result.NotFound<string>("Service has not registered any instances");
        
        var randomHost = hosts[random.Next(hosts.Count)];
        return Result.Ok(randomHost);
    }

    public Result<bool> RemoveService(RemoveServiceRequest request)
    {
        var hosts = GetHosts(request.ServiceName);
        var hostToDel = request.Host;
        if (hosts.Count == 0 || !hosts.Contains(hostToDel))
            return Result.NoContent<bool>();

        lock (hosts)
        {
            hosts.Remove(hostToDel);
        }

        return Result.NoContent<bool>();
    }
    
    private List<string> GetHosts(string serviceName) 
        => hostsByService.GetOrAdd(serviceName, _ => new List<string>());

    private async Task StartHealthPing(TimeSpan healthCheckingDelay, CancellationToken token)
    {
        async Task Ping()
        {
            foreach (var serviceWithHosts in hostsByService)
            {
                var hosts = serviceWithHosts.Value;
                foreach (var host in hosts)
                {
                    try
                    {
                        var endPoint = IPEndPoint.Parse(host);
                        using var tcpClient = new TcpClient();
                        await tcpClient.ConnectAsync(endPoint);
                        if (!tcpClient.Connected)
                            throw new SocketException();
                        Console.WriteLine($"Success connection with host: {host}.");
                    }
                    catch (SocketException socketEx)
                    {
                        Console.WriteLine($"Can not connect to host: {host}. It will be removed");
                        lock (hosts)
                        {
                            hosts.Remove(host);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
                    }
                }
            }
            
        }

        while (!token.IsCancellationRequested)
        {
            await Task.Delay(healthCheckingDelay);
            await Ping();
        }
    }
}