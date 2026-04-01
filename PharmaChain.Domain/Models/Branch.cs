using System;
using System.Collections.Generic;

namespace PharmaChain.Infrastructure.Models;

public partial class Branch
{
    public string BranchId { get; set; } = null!;

    public string BranchName { get; set; } = null!;

    public string AddressLine1 { get; set; } = null!;

    public string? AddressLine2 { get; set; }

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string Pincode { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Gstin { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<CustomerReturn> CustomerReturns { get; set; } = new List<CustomerReturn>();

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<MedicineBatch> MedicineBatches { get; set; } = new List<MedicineBatch>();

    public virtual ICollection<PurchaseInvoice> PurchaseInvoices { get; set; } = new List<PurchaseInvoice>();

    public virtual ICollection<StockLedger> StockLedgers { get; set; } = new List<StockLedger>();

    public virtual ICollection<StockRequest> StockRequestFromBranches { get; set; } = new List<StockRequest>();

    public virtual ICollection<StockRequest> StockRequestToBranches { get; set; } = new List<StockRequest>();

    public virtual ICollection<StockTransfer> StockTransferFromBranches { get; set; } = new List<StockTransfer>();

    public virtual ICollection<StockTransfer> StockTransferToBranches { get; set; } = new List<StockTransfer>();

    public virtual ICollection<SupplierReturn> SupplierReturns { get; set; } = new List<SupplierReturn>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
