using AbstractTaskService.Logic.Models;

namespace AbstractTaskService.Logic.Repositories;

public class AbstractTaskRepository : IAbstractTaskRepository
{
    private readonly AbstractTaskDbContext context;

    public AbstractTaskRepository(AbstractTaskDbContext context)
    {
        this.context = context;
    }

    public async Task<AbstractTask?> GetTask(Guid id) 
        => await context.AbstractTasks.FindAsync(id); 

}