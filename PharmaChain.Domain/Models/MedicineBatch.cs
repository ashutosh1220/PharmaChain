using System;
using System.Collections.Generic;

namespace PharmaChain.Infrastructure.Models;

public partial class MedicineBatch
{
    public string BatchId { get; set; } = null!;

    public string MedicineId { get; set; } = null!;

    public string BatchNumber { get; set; } = null!;

    public int TotalStockReceived { get; set; }

    public decimal UnitPurchasePrice { get; set; }

    public decimal UnitSellingPrice { get; set; }

    public DateOnly MfgDate { get; set; }

    public DateOnly ExpDate { get; set; }

    public string SupplierId { get; set; } = null!;

    public string BranchId { get; set; } = null!;

    public string? GrnNumber { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<CustomerReturn> CustomerReturns { get; set; } = new List<CustomerReturn>();

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    public virtual Medicine Medicine { get; set; } = null!;

    public virtual ICollection<StockLedger> StockLedgers { get; set; } = new List<StockLedger>();

    public virtual ICollection<StockRequest> StockRequests { get; set; } = new List<StockRequest>();

    public virtual ICollection<StockTransferItem> StockTransferItems { get; set; } = new List<StockTransferItem>();

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual ICollection<SupplierReturn> SupplierReturns { get; set; } = new List<SupplierReturn>();
}
