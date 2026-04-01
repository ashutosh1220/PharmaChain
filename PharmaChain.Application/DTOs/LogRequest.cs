public class LogRequest
{
    public required string Action { get; set; }
    public short ActionType { get; set; }

    public string? ModuleName { get; set; }
    public string? TableName { get; set; }

    public string? RecordId { get; set; }

    public object? OldValue { get; set; }
    public object? NewValue { get; set; }

    public string? ChangedFields { get; set; }
    public string? Delta { get; set; }  
    public string? Notes { get; set; }
}
public class LogsInfo
{
    public long LogId { get; set; }

    public string? UserId { get; set; }
    public string? BranchId { get; set; }

    public string? Action { get; set; }
    public short? ActionType { get; set; }
    public string? ModuleName { get; set; }

    public string? TableName { get; set; }
    public string? RecordId { get; set; }
    public string? RelatedRecordId { get; set; }

    public string? OldValue { get; set; }
    public string? NewValue { get; set; }

    public string? ChangedFields { get; set; }
    public string? Delta { get; set; }

    public string? IpAddress { get; set; }
    public string? DeviceInfo { get; set; }
    public string? SessionId { get; set; }

    public short? Severity { get; set; }   // FIXED
    public string? Status { get; set; }     // FIXED

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

public class LogsResponse
{
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int TotalLogs { get; set; }
    public List<LogsInfo> Logs { get; set; } = new();
}