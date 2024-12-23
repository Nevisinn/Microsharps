using System.Reflection;
using AbstractTaskService.Logic;
using AbstractTaskService.Logic.Repositories;
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

builder.Services.RegisterServiceDiscovery(applicationName);
builder.Services.AddDbContext<AbstractTaskDbContext>(options
    => options.UseNpgsql(builder.Configuration.GetConnectionString("postgres")));
builder.Services.AddScoped<IAbstractTaskService, AbstractTaskService.Logic.AbstractTaskService>();
builder.Services.AddScoped<IAbstractTaskRepository, AbstractTaskRepository>();

var app = builder.Build();

app.BaseConfiguration(useHttps:false, true);
app.RegisterInServiceDiscovery();

app.Run();

