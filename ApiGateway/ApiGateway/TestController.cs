using ApiGateway.Logic;
using ApiGateway.Models.Requests;
using Infrastructure.API;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly ITestService testService;
    private readonly ILogger<TestController> _logger;

    public TestController(ITestService testService, ILogger<TestController> logger)
    {
        this.testService = testService;
        this._logger = logger;
    }

    [HttpGet("Test")]
    public async Task<ActionResult<string>> Test()
    {
        var response = await testService.Test();
        return response.ActionResult();
    }
    
    [HttpPost("TestPost")]
    public async Task<ActionResult<TestPostResponseModel>> TestPost(TestPostRequestModel request)
    {
        var response = await testService.TestPost(TestApiMapper.Map(request));
        return response.ActionResult(TestApiMapper.Map);
    }
}