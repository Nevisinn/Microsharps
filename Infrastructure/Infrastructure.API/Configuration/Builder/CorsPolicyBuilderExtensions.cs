using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Infrastructure.API.Configuration.Builder;

internal static class CorsPolicyBuilderExtensions
{
    internal static CorsPolicyBuilder AllowAll(this CorsPolicyBuilder opt)
    {
        return opt
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    }
}