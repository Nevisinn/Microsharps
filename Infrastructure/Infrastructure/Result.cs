using System.Net;
using JetBrains.Annotations;

namespace Infrastructure;

[PublicAPI]
public struct Result<T>
{
    public Result(string? error, HttpStatusCode statusCode, T value = default(T))
    {
        Error = error;
        StatusCode = statusCode;
        Value = value;
    }

    public string? Error { get; }
    public HttpStatusCode StatusCode { get; }
    public T Value { get; }
    public bool IsSuccess => Error == null;
    
    public void Deconstruct(
        out string? error,
        out HttpStatusCode statusCode,
        out T value)
    {
        error = Error;
        statusCode = StatusCode;
        value = Value;
    }
}

public static class Result
{
    public static Result<T> Ok<T>(T value)
    {
        return new Result<T>(null, HttpStatusCode.OK, value);
    }

    public static Result<T> NoContent<T>()
    {
        return new Result<T>(null, HttpStatusCode.NoContent);
    }

    public static Result<T> BadRequest<T>(string e)
    {
        return new Result<T>(e, HttpStatusCode.BadRequest);
    }

    public static Result<T> NotFound<T>(string e)
    {
        return new Result<T>(e, HttpStatusCode.NotFound);
    }
}