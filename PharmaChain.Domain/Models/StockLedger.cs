using System;
using System.Collections.Generic;

namespace PharmaChain.Infrastructure.Models;

public partial class StockLedger
{
    public long LedgerId { get; set; }

    public string MedicineId { get; set; } = null!;

    public string BatchId { get; set; } = null!;

    public string BranchId { get; set; } = null!;

    public string TransactionType { get; set; } = null!;

    public string? ReferenceType { get; set; }

    public string? ReferenceId { get; set; }

    public int QuantityIn { get; set; }

    public int QuantityOut { get; set; }

    public string? Remarks { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual MedicineBatch Batch { get; set; } = null!;

    public virtual Branch Branch { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Medicine Medicine { get; set; } = null!;
}
