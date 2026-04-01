using System;
using System.Collections.Generic;

namespace PharmaChain.Infrastructure.Models;

public partial class PurchaseInvoice
{
    public string PurchaseInvoiceId { get; set; } = null!;

    public string SupplierId { get; set; } = null!;

    public string BranchId { get; set; } = null!;

    public string SupplierInvoiceNumber { get; set; } = null!;

    public DateOnly InvoiceDate { get; set; }

    public decimal SubTotal { get; set; }

    public decimal TotalTax { get; set; }

    public decimal InwardCharges { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal GrandTotal { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual ICollection<SupplierPayment> SupplierPayments { get; set; } = new List<SupplierPayment>();

    public virtual User? UpdatedByNavigation { get; set; }
}
