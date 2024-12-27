using AbstractTaskService.Logic;
using Infrastructure.API.Configuration.Builder;

const string serviceName = "abstract-task-service";

var builder = MicrosharpsWebAppBuilder.Create(serviceName, false, args)
    .BaseConfiguration(isPrivateHosted: true)
    .UseLogging(true)
    .ConfigureDi(ConfigureDi)
    .RegisterInServiceDiscovery();

builder.BuildAndRun();


void ConfigureDi(IServiceCollection services)
{
    services.AddSingleton<IAbstractTaskService, AbstractTaskService.Logic.AbstractTaskService>();
}