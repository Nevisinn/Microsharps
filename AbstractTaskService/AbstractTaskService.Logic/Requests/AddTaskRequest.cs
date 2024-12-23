using System.ComponentModel;
using AbstractTaskService.Logic.Models;

namespace AbstractTaskService.Logic.Requests;

public class AddTaskRequest
{
    public string Description { get; set; }
    public int TTLInMillisecond { get; set; }
}