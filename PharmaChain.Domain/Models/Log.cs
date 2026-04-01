using System;
using System.Collections.Generic;

namespace PharmaChain.Infrastructure.Models;

public partial class Log
{
    public long LogId { get; set; }

    public string? UserId { get; set; }

    public string? BranchId { get; set; }

    public string Action { get; set; } = null!;

    public short ActionType { get; set; }

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

    public short? Severity { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public virtual Branch? Branch { get; set; }

    public virtual User? User { get; set; }
}
