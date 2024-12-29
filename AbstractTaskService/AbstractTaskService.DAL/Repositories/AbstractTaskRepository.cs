using AbstractTaskService.DAL.Context;
using AbstractTaskService.Logic.Models;

namespace AbstractTaskService.DAL.Repositories;

public class AbstractTaskRepository : IAbstractTaskRepository
{
    private readonly AbstractTaskDbContext context;

    internal AbstractTaskRepository(AbstractTaskDbContext context)
    {
        this.context = context;
    }

    public async Task<AbstractTask?> GetTask(Guid id)
        => await context.AbstractTasks.FindAsync(id); 

}