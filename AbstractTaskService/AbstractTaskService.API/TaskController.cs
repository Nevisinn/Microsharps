using AbstractTaskService.Logic;
using AbstractTaskService.Logic.Requests;
using AbstractTaskService.Logic.Response;
using AbstractTaskService.Models;
using AbstractTaskService.Models.Request;
using AbstractTaskService.Models.Response;
using Infrastructure.API;
using Microsoft.AspNetCore.Mvc;

namespace AbstractTaskService.API;

[Route("api/[controller]")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly IAbstractTaskService abstractTaskService;

    public TaskController(IAbstractTaskService abstractTaskService)
    {
        this.abstractTaskService = abstractTaskService;
    }

    /*[HttpGet("Test")]
    public async Task<ActionResult<string>> Test()
    {
        var response = await abstractTaskService.TestGet();
        return response.ActionResult<string, string>();
    }

    [HttpPost("TestPost")]
    public async Task<ActionResult<TestPostResponseModel>> TestPost(TestPostRequestModel request)
    {
        var response = await abstractTaskService.TestPost(ApiMapper.Map(request));
        return response.ActionResult(ApiMapper.Map);
    }*/

    [HttpGet("{id}")]
    public async Task<ActionResult<GetTaskResponseModel>> GetTask(GetTaskRequestModel request)
    {
        var response = await abstractTaskService.GetTask(ApiMapper.Map(request));
        return response.ActionResult(ApiMapper.Map);
    }
    
    [HttpPost("AddTask")]
    public async Task<ActionResult<AddTaskResponseModel>> AddTask(AddTaskRequestModel request)
    {
        var response = await abstractTaskService.AddTask(ApiMapper.Map(request));
        return response.ActionResult(ApiMapper.Map);
    }
    
}