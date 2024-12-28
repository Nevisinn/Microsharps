using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AbstractTaskService.Logic.Models;
using AbstractTaskService.Logic.Repositories;
using AbstractTaskService.Logic.Requests;
using AbstractTaskService.Logic.Response;
using Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using RabbitMQ.Client;

namespace AbstractTaskService.Logic;

public interface IAbstractTaskService
{
    Task<Result<GetTaskResponse>> GetTask(GetTaskRequest request);
    Task<Result<AddTaskResponse>> AddTask(AddTaskRequest request);
    Task<Result<RetryTaskResponse>> RetryTask(RetryTaskRequest request);
}

public class AbstractTaskService : IAbstractTaskService
{
    private readonly IAbstractTaskRepository repository;
    private readonly MessageSender sender;
    private readonly IDistributedCache cache;

    public AbstractTaskService(IAbstractTaskRepository repository, IDistributedCache cache)
    {
        this.repository = repository;
        sender = new MessageSender();
        this.cache = cache;

    }
    public async Task<Result<GetTaskResponse>> GetTask(GetTaskRequest request)
    {   
        AbstractTask? task = null;
        var taskBody = await cache.GetAsync(request.Id.ToString());
        if (taskBody != null)
        {
            using var memoryStream = new MemoryStream(taskBody);
            task = await JsonSerializer.DeserializeAsync<AbstractTask>(memoryStream);
        }
        else
            task = await repository.GetTask(request.Id);
        
        if (task is null)
            return Result.NotFound<GetTaskResponse>($"Task with {request.Id} id not found");
        
        return Result.Ok(new GetTaskResponse
        {
            Description = task.Description,
            Status = task.Status
        });
    }

    public async Task<Result<AddTaskResponse>> AddTask(AddTaskRequest request)
    {
        var task = new AbstractTask
        {
            Id = Guid.NewGuid(),
            Description = request.Description,
            TTLInMillisecond = request.TTLInMillisecond,
            Status = "InProgress"
        };
        await sender.SendMessage(task);
        
        return Result.Ok(new AddTaskResponse { Id = task.Id} );
    }

    public async Task<Result<RetryTaskResponse>> RetryTask(RetryTaskRequest request)
    {
        var task = await repository.GetTask(request.Id);
        
        if (task is null)
            return Result.NotFound<RetryTaskResponse>($"Task with {request.Id} id not found");
        
        await sender.SendMessage(task);

        return Result.Ok(new RetryTaskResponse { Id = task.Id, Status = "InProgress" });
    }
}