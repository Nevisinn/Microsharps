using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AbstractTaskService.Logic.Models;
using AbstractTaskService.Logic.Repositories;
using AbstractTaskService.Logic.Requests;
using AbstractTaskService.Logic.Response;
using Infrastructure;
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
    private IAbstractTaskRepository repository;
    private MessageSender sender;

    public AbstractTaskService(IAbstractTaskRepository repository)
    {
        this.repository = repository;
        sender = new MessageSender();

    }
    public async Task<Result<GetTaskResponse>> GetTask(GetTaskRequest request)
    {
        var task = await repository.GetTask(request.Id);

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
        await repository.AddAsync(task);
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