using Microsoft.EntityFrameworkCore;

namespace Users.DAL;

public class UsersContext : DbContext
{
    public UsersContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<UserEntity>()
            .HasIndex(e => e.Login)
            .IsUnique();
    }

    public DbSet<UserEntity> Users => Set<UserEntity>();
}