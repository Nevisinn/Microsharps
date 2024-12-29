using AbstractTaskService.Logic.Models;

namespace AbstractTaskService.DAL.Repositories;

public interface IAbstractTaskRepository
{
    Task<AbstractTask?> GetTask(Guid id);
}