using System;
using System.Collections.Generic;

namespace PharmaChain.Infrastructure.Models;

public partial class SupplierPayment
{
    public long PaymentId { get; set; }

    public string PurchaseInvoiceId { get; set; } = null!;

    public string SupplierId { get; set; } = null!;

    public string PaymentMode { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateOnly PaymentDate { get; set; }

    public string? ReferenceNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual PurchaseInvoice PurchaseInvoice { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;
}
