using System.Reflection;
using AbstractTaskService.DAL.Repositories;
using AbstractTaskService.Logic;
using AbstractTaskService.Logic.Services;
using Infrastructure.API.Configuration;
using Infrastructure.API.Configuration.Builder;
using Infrastructure.API.Configuration.ServiceDiscovery;
using Microsoft.EntityFrameworkCore;

const string serviceName = "abstract-task-service";

var builder = MicrosharpsWebAppBuilder.Create(serviceName, false, args)
    .BaseConfiguration(isPrivateHosted: true)
    .UseLogging(true)
    .ConfigureDi(ConfigureDi)
    .RegisterInServiceDiscovery();

builder.BuildAndRun();


void ConfigureDi(IServiceCollection services)
{
    services.AddStackExchangeRedisCache(options => {
        options.Configuration = "localhost";
        options.InstanceName = "redis";
        });
    /*builder.Services.AddDbContext<AbstractTaskDbContext>(options
    => options.UseNpgsql(builder.Configuration.GetConnectionString("postgres")));*/
    services.AddScoped<IAbstractTaskService, AbstractTaskService.Logic.Services.AbstractTaskService>();
    services.AddScoped<IAbstractTaskRepository, AbstractTaskRepository>();
};
