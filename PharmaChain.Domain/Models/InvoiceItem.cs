using System;
using System.Collections.Generic;

namespace PharmaChain.Infrastructure.Models;

public partial class InvoiceItem
{
    public long Id { get; set; }

    public string InvoiceId { get; set; } = null!;

    public string MedicineId { get; set; } = null!;

    public string BranchId { get; set; } = null!;

    public string BatchId { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal GstPercentage { get; set; }

    public decimal TotalPrice { get; set; }

    public virtual MedicineBatch Batch { get; set; } = null!;

    public virtual Branch Branch { get; set; } = null!;

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual Medicine Medicine { get; set; } = null!;
}
