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

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetTaskResponseModel>> GetTask(Guid id)
    {
        var response = await abstractTaskService.GetTask(ApiMapper.Map(id));
        return response.ActionResult(ApiMapper.Map);
    }
    
    [HttpPost("AddTask")]
    public async Task<ActionResult<AddTaskResponseModel>> AddTask(AddTaskRequestModel request)
    {
        var response = await abstractTaskService.AddTask(ApiMapper.Map(request));
        return response.ActionResult(ApiMapper.Map);
    }
    
    [HttpPost("RetryTask")]
    public async Task<ActionResult<RetryTaskResponseModel>> RetryTask(RetryTaskModel request)
    {
        var response = await abstractTaskService.RetryTask(ApiMapper.Map(request));
        return response.ActionResult(ApiMapper.Map);
    }
    
}