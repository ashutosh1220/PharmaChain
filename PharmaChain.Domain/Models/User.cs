using System;
using System.Collections.Generic;
using System.Data;

namespace PharmaChain.Infrastructure.Models;

public partial class User
{
    public string UserId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int RoleId { get; set; }
    public Roles? Role { get; set; }
    public string BranchId { get; set; } = null!;
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }

    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }

    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }

    public string? IdProofType { get; set; }
    public string? IdProofNumber { get; set; }
    public string? IdProofDocumentPath { get; set; }
    public string? ProfilePhotoPath { get; set; }

    // Address fields
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? Pincode { get; set; }

    public virtual Branch Branch { get; set; } = null!;
    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<CustomerPayment> CustomerPayments { get; set; } = new List<CustomerPayment>();
    public virtual ICollection<CustomerReturn> CustomerReturns { get; set; } = new List<CustomerReturn>();
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual User? DeletedByNavigation { get; set; }

    public virtual ICollection<User> InverseCreatedByNavigation { get; set; } = new List<User>();
    public virtual ICollection<User> InverseDeletedByNavigation { get; set; } = new List<User>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public virtual ICollection<Login> Logins { get; set; } = new List<Login>();
    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<MedicineBatch> MedicineBatches { get; set; } = new List<MedicineBatch>();

    public virtual ICollection<Medicine> MedicineCreatedByNavigations { get; set; } = new List<Medicine>();
    public virtual ICollection<Medicine> MedicineDeletedByNavigations { get; set; } = new List<Medicine>();
    public virtual ICollection<Medicine> MedicineUpdatedByNavigations { get; set; } = new List<Medicine>();

    public virtual ICollection<PurchaseInvoice> PurchaseInvoiceCreatedByNavigations { get; set; } = new List<PurchaseInvoice>();
    public virtual ICollection<PurchaseInvoice> PurchaseInvoiceUpdatedByNavigations { get; set; } = new List<PurchaseInvoice>();

    public virtual ICollection<StockLedger> StockLedgers { get; set; } = new List<StockLedger>();

    public virtual ICollection<StockRequest> StockRequestApprovedByNavigations { get; set; } = new List<StockRequest>();
    public virtual ICollection<StockRequest> StockRequestCreatedByNavigations { get; set; } = new List<StockRequest>();

    public virtual ICollection<StockTransfer> StockTransferApprovedByNavigations { get; set; } = new List<StockTransfer>();
    public virtual ICollection<StockTransfer> StockTransferRequestedByNavigations { get; set; } = new List<StockTransfer>();

    public virtual ICollection<Supplier> SupplierCreatedByNavigations { get; set; } = new List<Supplier>();
    public virtual ICollection<Supplier> SupplierDeletedByNavigations { get; set; } = new List<Supplier>();
    public virtual ICollection<SupplierReturn> SupplierReturns { get; set; } = new List<SupplierReturn>();
    public virtual ICollection<Supplier> SupplierUpdatedByNavigations { get; set; } = new List<Supplier>();
}