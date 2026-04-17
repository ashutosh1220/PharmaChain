namespace PharmaChain.Infrastructure.Models;

public partial class Medicine
{
    public string MedicineId { get; set; } = null!;

    public string MedicineName { get; set; } = null!;

    public string? GenericName { get; set; }

    public string? Category { get; set; }

    public string? Strength { get; set; }

    public string? Manufacturer { get; set; }

    public bool IsPrescriptionRequired { get; set; }

    public int MinimumStockLevel { get; set; }

    public string? HsnCode { get; set; }

    public decimal? GstPercentage { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<CustomerReturn> CustomerReturns { get; set; } = new List<CustomerReturn>();

    public virtual User? DeletedByNavigation { get; set; }

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    public virtual ICollection<MedicineBatch> MedicineBatches { get; set; } = new List<MedicineBatch>();

    public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();

    public virtual ICollection<StockLedger> StockLedgers { get; set; } = new List<StockLedger>();

    //public virtual ICollection<StockRequest> StockRequests { get; set; } = new List<StockRequest>();

    //public virtual ICollection<StockTransferItem> StockTransferItems { get; set; } = new List<StockTransferItem>();

    public virtual ICollection<SupplierReturn> SupplierReturns { get; set; } = new List<SupplierReturn>();

    public virtual User? UpdatedByNavigation { get; set; }
}
