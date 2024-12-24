using System.Reflection;
using AbstractTaskService.Client;
using ApiGateway.Logic;
using Infrastructure;
using Infrastructure.API.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

LoggingConfiguration.ConfigureLogging("ApiGateway");
builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(opt => 
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddScoped<ILogsService, LogsService>();
builder.Services.AddSingleton<ITestService, TestService>();
builder.Services.AddSingleton<IAbstractTaskServiceClient, AbstractTaskServiceClient>(); // TODO:  add DI to Logic and move into it  

var app = builder.Build();

app.BaseConfiguration(
    useHttps: false,
    isPrivateHosted: false
);

app.Run();