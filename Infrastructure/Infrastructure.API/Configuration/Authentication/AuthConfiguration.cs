using Microsoft.AspNetCore.Authentication.Cookies;

namespace Infrastructure.API.Configuration.Authentication;

public class AuthConfiguration
{
    public const string Scheme = CookieAuthenticationDefaults.AuthenticationScheme;
}