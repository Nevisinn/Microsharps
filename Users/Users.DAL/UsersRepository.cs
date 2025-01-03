using Microsoft.EntityFrameworkCore;

namespace Users.DAL;

public interface IUsersRepository
{
    public Task<UserEntity> RegisterUser(UserEntity entity);
    public Task<UserEntity?> GetUser(string login);
}

public class UsersRepository : IUsersRepository
{
    private readonly UsersContext context;

    public UsersRepository(UsersContext context)
    {
        this.context = context;
    }

    public async Task<UserEntity> RegisterUser(UserEntity entity)
    {
        await context.Users.AddAsync(entity);
        await context.SaveChangesAsync();

        return entity;
    }

    public async Task<UserEntity?> GetUser(string login)
    {
        return await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Login == login);
    }
}