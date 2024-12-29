using System.Reflection;
using AbstractTaskService.DAL.Repositories;
using AbstractTaskService.Logic;
using AbstractTaskService.Logic.Services;
using Infrastructure.API.Configuration;
using Infrastructure.API.Configuration.ServiceDiscovery;
using Microsoft.EntityFrameworkCore;

const string applicationName = "abstract-task-service";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(opt => 
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = "localhost";
    options.InstanceName = "redis";
});
builder.Services.RegisterServiceDiscovery(applicationName);
/*builder.Services.AddDbContext<AbstractTaskDbContext>(options
    => options.UseNpgsql(builder.Configuration.GetConnectionString("postgres")));*/
builder.Services.AddScoped<IAbstractTaskService, AbstractTaskService.Logic.Services.AbstractTaskService>();
builder.Services.AddScoped<IAbstractTaskRepository, AbstractTaskRepository>();

var app = builder.Build();

app.BaseConfiguration(useHttps:false, true);
app.RegisterInServiceDiscovery();

app.Run();

