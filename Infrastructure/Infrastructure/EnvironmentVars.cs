namespace Infrastructure;

public static class EnvironmentVars
{
    public static string? SdHost => Environment.GetEnvironmentVariable("SD_HOST");
    public static string? OwhHost => Environment.GetEnvironmentVariable("HOST");
}