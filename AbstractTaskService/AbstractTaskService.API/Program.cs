using AbstractTaskService.Logic;
using Infrastructure.API.Configuration.Builder;

const string serviceName = "abstract-task-service";

var builder = MicrosharpsWebAppBuilder.Create(false, args)
    .BaseConfiguration(isPrivateHosted: true)
    .ConfigureDi(ConfigureDi)
    .RegisterInServiceDiscovery(serviceName);

builder.BuildAndRun();


void ConfigureDi(IServiceCollection services)
{
    services.AddSingleton<IAbstractTaskService, AbstractTaskService.Logic.AbstractTaskService>();
}