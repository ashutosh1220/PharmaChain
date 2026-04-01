using System;
using System.Collections.Generic;

namespace PharmaChain.Infrastructure.Models;

public partial class PurchaseItem
{
    public long Id { get; set; }

    public string PurchaseInvoiceId { get; set; } = null!;

    public string MedicineId { get; set; } = null!;

    public string BatchNumber { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitPurchasePrice { get; set; }

    public decimal UnitSellingPrice { get; set; }

    public decimal GstPercentage { get; set; }

    public DateOnly? MfgDate { get; set; }

    public DateOnly? ExpDate { get; set; }

    public virtual Medicine Medicine { get; set; } = null!;

    public virtual PurchaseInvoice PurchaseInvoice { get; set; } = null!;
}
