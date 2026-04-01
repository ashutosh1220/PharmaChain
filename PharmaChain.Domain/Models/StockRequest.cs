using System;
using System.Collections.Generic;

namespace PharmaChain.Infrastructure.Models;

public partial class StockRequest
{
    public string AssignedStockId { get; set; } = null!;

    public string MedicineId { get; set; } = null!;

    public string? BatchId { get; set; }

    public string? FromBranchId { get; set; }

    public string ToBranchId { get; set; } = null!;

    public string? SupplierId { get; set; }

    public int QuantityRequested { get; set; }

    public int? QuantityApproved { get; set; }

    public int? QuantityTransferred { get; set; }

    public DateTime RequestDate { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public DateTime? TransferDate { get; set; }

    public string ApprovalStatus { get; set; } = null!;

    public string? FulfillmentType { get; set; }

    public string? ApprovedBy { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual MedicineBatch? Batch { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Branch? FromBranch { get; set; }

    public virtual Medicine Medicine { get; set; } = null!;

    public virtual ICollection<StockTransfer> StockTransfers { get; set; } = new List<StockTransfer>();

    public virtual Supplier? Supplier { get; set; }

    public virtual Branch ToBranch { get; set; } = null!;
}
