using AbstractTaskService.Logic.Models;

namespace AbstractTaskService.Logic.Response;

public class GetTaskResponse
{
    public string Description { get; set; }
    public string Status { get; set; }
}