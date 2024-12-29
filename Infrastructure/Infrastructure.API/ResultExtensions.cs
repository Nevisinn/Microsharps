using System.Net;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
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

    public static ActionResult<T> ActionResult<T>(this Result<T> result)
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

    public static IResult MinimalApiResult<T>(this Result<T> result)
    {
        var (error, statusCode, value) = result;

        return statusCode switch
        {
            HttpStatusCode.OK => typeof(T) == typeof(string) 
                ? Results.Text(value!.ToString())
                : Results.Ok(value),
            HttpStatusCode.NoContent => Results.NoContent(),
            HttpStatusCode.BadRequest => Results.BadRequest(error),
            HttpStatusCode.NotFound => Results.NotFound(error),
            _ => throw new ArgumentException($"Result does not support {statusCode}")
        };
    }
}