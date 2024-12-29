using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Infrastructure.API.Configuration.Logging;

public static class LoggingEndpoints
{
    public static void MapLoggingEndpoints(this WebApplication app)
    {
        app.MapGet("api/logs/today", GetTodayLogs)
            .WithSummary("Current day logs")
            .Produces((int)HttpStatusCode.OK, contentType: "text/plain");
        
        app.MapGet(@"api/logs/{date}", GetLogs) // TODO: add :regex([[\d*]])
            .WithSummary("Logs for the specified date")
            .WithDescription("Date in yyyy.MM.dd format")
            .Produces((int)HttpStatusCode.OK, contentType: "text/plain");

        app.MapPost(@"api/logs/test", TestLog)
            .WithSummary("Log all possible levels")
            .Produces((int)HttpStatusCode.NoContent);
    }


    private static async Task<IResult> GetTodayLogs([FromServices] ILogsService logsService)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var response = await logsService.ReadLogAsync(currentDate);
        return response.MinimalApiResult();
    }
    
    private static async Task<IResult> GetLogs([FromRoute] string date, [FromServices] ILogsService logsService)
    {
        if (!DateOnly.TryParse(date, out var dateOnly))
            Results.BadRequest("Incorrect date format. Should be yyyy.MM.dd");

        var response = await logsService.ReadLogAsync(dateOnly);
        return response.MinimalApiResult();
    }
    
    private static readonly LogLevel[] LogLevels = (LogLevel[])Enum.GetValues(typeof(LogLevel));
    private static IResult TestLog([FromServices] ILogger<LogsService> logger)
    {
        foreach (var level in LogLevels)
            logger.Log(level, $"It is testing log of level: {level.ToString()}");
        return Results.NoContent();
    }
}