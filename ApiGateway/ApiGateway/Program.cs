using AbstractTaskService.Client;
using ApiGateway.Logic;
using Infrastructure.API.Configuration.Builder;

const string serviceName = "api-gateway";

var builder = MicrosharpsWebAppBuilder.Create(serviceName, false, args)
    .BaseConfiguration(
        isPrivateHosted: false
    )
    .UseLogging(true)
    .ConfigureDi(ConfigureDi);

builder.BuildAndRun();

void ConfigureDi(IServiceCollection services)
{
    services.AddSingleton<ITestService, TestService>();
    services.AddSingleton<IAbstractTaskServiceClient, AbstractTaskServiceClient>(); // TODO:  add DI to Logic and move into it  
}