using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Infrastructure.API.Configuration.Builder;

public static class SwaggerConfiguration
{
    public static MicrosharpsWebAppBuilder UseSwagger(
        this MicrosharpsWebAppBuilder builder,
        string? swaggerRequestsPrefix = null)
    {
        builder.nativeBuilder.Services.AddSwaggerGen(opt => 
        {
            opt.AddXmlDocumentation();
            if (swaggerRequestsPrefix != null)
                opt.AddSwaggerRequestsPrefix(swaggerRequestsPrefix);
        });
        
        builder.appConfig.Add(app =>
        {
            // if (app.Environment.IsDevelopment()) TODO
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            } 
        });

        return builder;
    }

    private static void AddXmlDocumentation(this SwaggerGenOptions opt)
    {
        var xmlFilename = $"{Assembly.GetEntryAssembly()!.GetName().Name}.xml";
        opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    }

    private static void AddSwaggerRequestsPrefix(this SwaggerGenOptions opt, string prefix) 
        => opt.DocumentFilter<PathPrefixInsertDocumentFilter>(prefix);
}

public class PathPrefixInsertDocumentFilter : IDocumentFilter
{
    private readonly string pathPrefix;

    public PathPrefixInsertDocumentFilter(string pathPrefix)
    {
        this.pathPrefix = pathPrefix;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = swaggerDoc.Paths.Keys.ToList();
        foreach (var path in paths)
        {
            var pathToChange = swaggerDoc.Paths[path];
            swaggerDoc.Paths.Remove(path);
            swaggerDoc.Paths.Add("/" + pathPrefix + path, pathToChange);
        }
    }
}

