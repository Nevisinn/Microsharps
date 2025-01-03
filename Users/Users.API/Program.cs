using Infrastructure;
using Infrastructure.API.Configuration.Builder;
using Users.DAL;
using Users.Logic;

const string serviceName = "users";

var builder = MicrosharpsWebAppBuilder.Create(serviceName, false, args)
    .BaseConfiguration(isPrivateHosted: true)
    .UseLogging(true)
    .ConfigureDi(ConfigureDi)
    .AddDbContext(services => services.AddUsersDbContext(EnvironmentVars.ConnectionString
                                                         ?? "Server=localhost;Port=5433;Database=micro-users;User Id=postgres;Password=password"))
    .RegisterInServiceDiscovery();

builder.BuildAndRun();


void ConfigureDi(IServiceCollection services)
{
    services.AddScoped<IUsersService, UsersService>();
}