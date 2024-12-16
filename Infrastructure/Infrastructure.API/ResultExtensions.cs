using System.Net;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.API;

[PublicAPI]
public static class ResultExtensions
{
    public static ActionResult<TApi> ActionResult<TLogic, TApi>(
        this Result<TLogic> result, 
        Func<TLogic, TApi>? mapping = null)
    {
        var (error, statusCode, value) = result;
        
        return statusCode switch
        {
            HttpStatusCode.OK => new OkObjectResult(mapping == null 
                ? value
                : mapping(value)),
            HttpStatusCode.NoContent => new NoContentResult(),
            HttpStatusCode.BadRequest => new BadRequestObjectResult(error),
            HttpStatusCode.NotFound => new NotFoundObjectResult(error),
            _ => throw new ArgumentException($"Result does not support {statusCode}")
        };
    }
}