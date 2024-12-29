namespace Infrastructure.API.Configuration.Logging;

public enum LogSink
{
    Console,
    File,
    Elastic
}

public static class LogSinks
{
    public static LogSink[] OnlyLocal => new[] { LogSink.Console, LogSink.File };
    public static LogSink[] All => OnlyLocal.Concat(new[] { LogSink.Elastic }).ToArray();
}