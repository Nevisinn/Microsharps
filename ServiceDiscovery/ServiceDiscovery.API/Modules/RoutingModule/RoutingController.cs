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
        var origin = Request.Headers["Sd-host"].FirstOrDefault(); // TODO: headers-name to const/etc
        if (string.IsNullOrEmpty(origin))
            return BadRequest("Can not determine origin of request");

        var response = routingService.RegisterService(RoutingApiMapper.Map(request, origin));
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
}