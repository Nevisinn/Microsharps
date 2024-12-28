namespace AbstractTaskService.Logic.Requests;

public class RetryTaskRequest
{
    public Guid Id { get; set; }
    public string Status { get; set; }
}