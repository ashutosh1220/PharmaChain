using System;
using System.Collections.Generic;

namespace PharmaChain.Infrastructure.Models;

public partial class Login
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string Username { get; set; }

    public string PasswordHash { get; set; } = null!;

    public DateTime? LastLoginAt { get; set; }

    public int FailedLoginAttempts { get; set; }

    public int FailedAttemptsCount { get; set; }

    public bool IsLocked { get; set; }

    public DateTime? LockoutEndTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
