using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Infrastructure.API.Validations;

namespace ServiceDiscovery.API.Modules.RoutingModule.DTO;

public class RegisterServiceRequest
{
    public required string ServiceName { get; set; }
    //[RegularExpression(Regexps.IPv4)] TODO
    [DefaultValue("127.0.0.1:8888")]
    public required string[] Hosts { get; set; }
}