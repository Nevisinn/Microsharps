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
    /*Task<Result<string>> TestGet();
    Task<Result<TestPostResponse>> TestPost(TestPostRequest request);*/
    Task<Result<GetTaskResponse>> GetTask(GetTaskRequest request);
    Task<Result<AddTaskResponse>> AddTask(AddTaskRequest request);
}

public class AbstractTaskService : IAbstractTaskService
{
    private IAbstractTaskRepository repository;
    private ConnectionFactory factory;

    public AbstractTaskService(IAbstractTaskRepository repository)
    {
        this.repository = repository;
        factory = new ConnectionFactory { HostName = "localhost" };
    }
    
    /*public async Task<Result<string>> TestGet()
    {
        return Result.Ok("Success");
    }

    public async Task<Result<TestPostResponse>> TestPost(TestPostRequest request)
    {
        tasks.Add(new AbstractTask
        {
            Description = request.Description,
        });
        return Result.Ok(new TestPostResponse
        {
            Tasks = tasks.ToArray(),
        });
    }*/

    public async Task<Result<GetTaskResponse>> GetTask(GetTaskRequest request)
    { 
        return Result.Ok(new GetTaskResponse
            (
                ));
    }

    public async Task<Result<AddTaskResponse>> AddTask(AddTaskRequest request)
    {
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        
        await channel.QueueDeclareAsync(queue: "hello", durable: false, exclusive: false, autoDelete: false,
            arguments: null);
        var messageBody = await SerializeToJsonAsync(request);
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "hello", body: messageBody);
        
        
        return Result.Ok(new AddTaskResponse());
    }
    
    private async Task<byte[]> SerializeToJsonAsync<T>(T obj)
    {
        using var memoryStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(memoryStream, obj);
        return memoryStream.ToArray();
    }
}