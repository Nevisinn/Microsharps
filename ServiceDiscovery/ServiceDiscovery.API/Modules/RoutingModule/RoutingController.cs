using Infrastructure.API;
using Microsoft.AspNetCore.Mvc;
using ServiceDiscovery.API.Modules.RoutingModule.DTO;
using ServiceDiscovery.API.Logic.ServicesModule;
using RegisterServiceResponse = ServiceDiscovery.API.Logic.ServicesModule.DTO.RegisterServiceResponse;

namespace ServiceDiscovery.API.Modules.RoutingModule;

[Route("api/[controller]")]
[ApiController]
public class RoutingController : ControllerBase
{
    private readonly IRoutingService routingService;

    public RoutingController(IRoutingService routingService)
    {
        this.routingService = routingService;
    }
    
    /// <summary>
    /// Регистрирует хост для сервиса
    /// </summary>
    [HttpPost]
    public ActionResult<RegisterServiceResponse> RegisterService([FromBody] RegisterServiceRequest request)
    {
        var host = BuildHost(Request);
        if (string.IsNullOrEmpty(host))
            return BadRequest("Can not determine host of request");

        var response = routingService.RegisterService(RoutingApiMapper.Map(request, host));
        return response.ActionResult();
    }

    /// <summary>
    /// Достаёт рандомный хост у сервиса
    /// </summary>
    [HttpGet("{serviceName}")]
    public ActionResult GetService([FromRoute] string serviceName)
    {
        var response = routingService.GetService(serviceName);
        return response.ActionResult();
    }

    /// <summary>
    /// Чистит хост сервиса
    /// </summary>
    [HttpDelete]
    public ActionResult DeleteService([FromBody] RemoveServiceRequest request)
    {
        var response = routingService.RemoveService(RoutingApiMapper.Map(request));
        return response.ActionResult();
    }

    private string? BuildHost(HttpRequest request)
    {
        var fullHost = request.Host;
        var host = fullHost.Host;
        var port = fullHost.Port;
        if (string.IsNullOrEmpty(host))
            return null;

        host = host == "localhost" 
            ? "127.0.0.1"
            : request.Host.Value;
        var portStr = port != null 
            ? $":{port}" 
            : "";
        return host + portStr;
    }
}