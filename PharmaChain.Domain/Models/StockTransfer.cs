using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaChain.Infrastructure.Models;

public partial class StockTransfer
{
    public string TransferId { get; set; } = null!;
    public string? AssignedStockId { get; set; }

    public string TransferStatus { get; set; } = null!;

    public string RequestedBy { get; set; } = null!;

    public string? ApprovedBy { get; set; }

    public DateTime? TransferDate { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
    public virtual User? ApprovedByNavigation { get; set; }
    public virtual StockRequest? AssignedStock { get; set; }

    public virtual User RequestedByNavigation { get; set; } = null!;

    //public virtual ICollection<StockTransferItem> StockTransferItems { get; set; } = new List<StockTransferItem>();
}