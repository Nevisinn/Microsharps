namespace AbstractTaskService.Logic.Models;

public class AbstractTask
{   
    public Guid Id { get; set; }
    public string Description { get; set; }
    public int TTLInMillisecond { get; set; }
}