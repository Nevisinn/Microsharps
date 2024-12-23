using AbstractTaskService.Logic.Models;
using Microsoft.EntityFrameworkCore;

namespace AbstractTaskService.Logic;

public sealed class AbstractTaskDbContext : DbContext
{
    public DbSet<AbstractTask?> AbstractTasks { get; set; }

    public AbstractTaskDbContext(DbContextOptions<AbstractTaskDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
    
}