using AbstractTaskService.Logic.Models;

namespace AbstractTaskService.Logic.Repositories;

public interface IAbstractTaskRepository
{

    Task<AbstractTask?> GetTask(Guid id);
    
}