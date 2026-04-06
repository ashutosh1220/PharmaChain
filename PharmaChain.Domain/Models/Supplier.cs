using System;
namespace PharmaChain.Infrastructure.Models;

public partial class Supplier
{
    public string SupplierId { get; set; } = null!;

    public string SupplierName { get; set; } = null!;

    public string? ContactPerson { get; set; }

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? Gstin { get; set; }

    public string? DrugLicenseNumber { get; set; }

    public string? BankAccountNumber { get; set; }

    public string? IfscCode { get; set; }

    public string? BankBranchName { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? DeletedByNavigation { get; set; }

    public virtual ICollection<MedicineBatch> MedicineBatches { get; set; } = new List<MedicineBatch>();

    public virtual ICollection<PurchaseInvoice> PurchaseInvoices { get; set; } = new List<PurchaseInvoice>();

    public virtual ICollection<StockRequest> StockRequests { get; set; } = new List<StockRequest>();

    public virtual ICollection<SupplierPayment> SupplierPayments { get; set; } = new List<SupplierPayment>();

    public virtual ICollection<SupplierReturn> SupplierReturns { get; set; } = new List<SupplierReturn>();

    public virtual User? UpdatedByNavigation { get; set; }
}
