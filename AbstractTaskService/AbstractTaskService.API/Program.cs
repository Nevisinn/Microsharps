using System.Reflection;
using AbstractTaskService.Logic;
using ServiceDiscovery.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(opt => 
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddSingleton<IAbstractTaskService, AbstractTaskService.Logic.AbstractTaskService>();
builder.Services.AddSingleton<IServiceDiscoveryClient, ServiceDiscoveryClient>(
    _ => new ServiceDiscoveryClient("abstract-task-service", null)); // TODO: in ideal world it is inside Logic

var app = builder.Build();

// if (app.Environment.IsDevelopment()) TODO
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); TODO

app.UseAuthentication();
app.UseAuthorization();

app.Lifetime.ApplicationStarted.Register(() =>
{
    // TODO: Довести до ума(конфиги и тд) + вынести в инфру/SD
    var http = app.Urls.First(e => e.StartsWith("http")); 
    var registerResponse = app.Services.GetService<IServiceDiscoveryClient>().Register(http).GetAwaiter().GetResult();
    if (!registerResponse.IsSuccess)
        throw new Exception("Can not register service in SD. Shut down...");
});

app.Lifetime.ApplicationStopping.Register(() =>
{
    // TODO: remove service from SD 
});

app.Run();

