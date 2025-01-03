using Infrastructure.API.Configuration.Builder;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.API.Configuration.Authentication;

public static class AuthorizationConfiguration
{
    public static MicrosharpsWebAppBuilder UseAuthorization(this MicrosharpsWebAppBuilder builder)
    {
        var scheme = AuthConfiguration.Scheme;
        builder.nativeBuilder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AuthConfiguration.Scheme;
                options.DefaultChallengeScheme = AuthConfiguration.Scheme;
                options.DefaultScheme = AuthConfiguration.Scheme;
            })
            .AddCookie(opt =>
            {
                opt.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = (context) =>
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = (context) =>
                    {
                        context.Response.StatusCode = 403;
                        return Task.CompletedTask;
                    },
                };
                opt.LoginPath = "/api/Login";
            });

        return builder;
    }
}