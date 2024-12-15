using System.Net;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.API;

[PublicAPI]
public static class ResultExtensions
{
    public static ActionResult ActionResult<T>(this Result<T> result)
    {
        var (error, statusCode, value) = result;
        return statusCode switch
        {
            HttpStatusCode.OK => new OkObjectResult(value),
            HttpStatusCode.NoContent => new NoContentResult(),
            HttpStatusCode.BadRequest => new BadRequestObjectResult(error),
            HttpStatusCode.NotFound => new NotFoundObjectResult(error),
            _ => throw new ArgumentException($"Result does not support {statusCode}")
        };
    }
}