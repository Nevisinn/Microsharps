using AbstractTaskService.Logic.Models;

namespace AbstractTaskService.Logic.Repositories;

public class AbstractTaskRepository : IAbstractTaskRepository
{
    private readonly AbstractTaskDbContext context;

    public AbstractTaskRepository(AbstractTaskDbContext context)
    {
        this.context = context;
    }

    public async Task AddAsync(AbstractTask task)
    {
        await context.AbstractTasks.AddAsync(task);
        await context.SaveChangesAsync();
    }

    public async Task<AbstractTask?> GetTask(Guid id) //TODO: переделать в polling
        => await context.AbstractTasks.FindAsync(id); 

}