public interface ILogService
{
    Task AddLogAsync(LogRequest request);
    Task<LogsResponse> GetLogsAsync(int page, int size);
    Task<LogsResponse> GetLogByLogIdAsync(long LogId);
}