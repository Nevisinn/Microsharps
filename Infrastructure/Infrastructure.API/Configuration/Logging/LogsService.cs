using System.Text;

namespace Infrastructure.API.Configuration.Logging;

public interface ILogsService
{
    public Task<Result<string>> ReadLogAsync(DateOnly date);
}

public class LogsService: ILogsService
{
    public async Task<Result<string>> ReadLogAsync(DateOnly date)
    {
        var formatDate = date.ToString("yyyyMMdd");
        var sb = new StringBuilder();
        var logFilePath = $"{LoggingConfiguration.LogsPath}{formatDate}.txt";

        if (!File.Exists(logFilePath)) return Result.NotFound<string>("No logs found");
        
        await using var stream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize: 4096, useAsync: true);
        using var reader = new StreamReader(stream);
        
        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            sb.AppendLine(line);
        }

        return Result.Ok(sb.ToString());
    }
}