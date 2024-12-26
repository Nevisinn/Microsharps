using Infrastructure;
using Infrastructure.API;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Modules.LogsModule;

[Route("api/[controller]")]
[ApiController]
public class LogsController: ControllerBase
{
    private readonly ILogsService logsService;
    private readonly ILogger<TestController> logger;
    
    public LogsController(ILogsService logsService, ILogger<TestController> logger)
    {
        this.logsService = logsService;
        this.logger = logger;
    }
    /// <summary>
    /// Current day logs 
    /// </summary>
    /// <param name="date">Date in yyyy-MM-dd format</param>
    [HttpGet]
    public async Task<ActionResult<string>> GetTodayLogs() => await GetLogs(DateOnly.FromDateTime(DateTime.UtcNow).ToString());

    /// <summary>
    /// Logs for the specified date
    /// </summary>
    /// <param name="date">Date in yyyy.MM.dd format</param>
    [HttpGet(@"{date:regex([[\d*]])}")]
    public async Task<ActionResult<string>> GetLogs([FromRoute] string date)
    {
        if (!DateOnly.TryParse(date, out var dateOnly))
            return BadRequest("Incorrect date format. Should be yyyy.MM.dd");
        
        return (await logsService.ReadLogAsync(dateOnly)).ActionResult();
    }
    
    [HttpPost("CreateTestLog")]
    public void CreateTestLog()
    {
        logger.LogWarning("Test Warning");
    }
}