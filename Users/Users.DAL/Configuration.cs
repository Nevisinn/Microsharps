using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Users.DAL;

public static class Configuration
{
    public static void AddUsersDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<UsersContext>(options =>
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            options.UseNpgsql(
                connectionString,
                builder => { builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null); });
        });

        services.AddScoped<IUsersRepository, UsersRepository>();
    } 
}