using System.Globalization;
using AbstractTaskService.Logic.Models;
using AbstractTaskService.Logic.Requests;
using AbstractTaskService.Logic.Response;
using AbstractTaskService.Models;
using AbstractTaskService.Models.Models;
using AbstractTaskService.Models.Request;
using AbstractTaskService.Models.Response;

namespace AbstractTaskService.API;

public static class ApiMapper
{
    /*public static TestPostRequest Map(TestPostRequestModel request)
        => new()
        {
            Name = request.Name,
            Description = request.Description,
        };
    
    public static TestPostResponseModel Map(TestPostResponse response)
        => new()
        {
            Tasks = response.Tasks.Select(Map).ToArray(),
        };*/
    
    private static AbstractTaskModel Map(AbstractTask task)
        => new()
        {
            Description = task.Description,
        };

    public static AddTaskRequest Map(AddTaskRequestModel request)
        => new()
        {
            Description = request.Description,
            TTLInMillisecond = request.TTLInMillisecond
        };
    
    public static AddTaskResponseModel Map(AddTaskResponse response)
        => new()
        {
            Id = response.Id
        };

    public static GetTaskRequest Map(GetTaskRequestModel request)
        => new()
        {
            Id = request.Id
        };
    
    public static GetTaskResponseModel Map(GetTaskResponse response)
        => new()
        {
            Description = response.Description,
            TTLInMillisecond = response.TTLInMillisecond
        };
    



}