using System;
using System.Collections.Generic;

namespace PharmaChain.Infrastructure.Models;

public partial class CustomerReturn
{
    public string ReturnId { get; set; } = null!;

    public string InvoiceId { get; set; } = null!;

    public string MedicineId { get; set; } = null!;

    public string BatchId { get; set; } = null!;

    public string BranchId { get; set; } = null!;

    public int Quantity { get; set; }

    public string? Reason { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual MedicineBatch Batch { get; set; } = null!;

    public virtual Branch Branch { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual Medicine Medicine { get; set; } = null!;
}
