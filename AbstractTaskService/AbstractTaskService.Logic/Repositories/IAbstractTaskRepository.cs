using AbstractTaskService.Logic.Models;

namespace AbstractTaskService.Logic.Repositories;

public interface IAbstractTaskRepository
{
    Task AddAsync(AbstractTask task);
    Task<AbstractTask?> GetTask(Guid id);
    
}