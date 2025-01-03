namespace Infrastructure;

public static class EnvironmentVars
{
    public static string? SdHost => Environment.GetEnvironmentVariable("SD_HOST");
    public static string? OwhHost => Environment.GetEnvironmentVariable("HOST");
    public static string? SwaggerRequestsPrefix => Environment.GetEnvironmentVariable("SWAGGER_REQUESTS_PREFIX");
    public static string? ConnectionString => Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
}