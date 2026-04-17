using PharmaChain.Infrastructure.Models;

public partial class StockTransferItem
{
    public long TransferItemId { get; set; }

    public string TransferId { get; set; } = null!;

    public string AssignedStockId { get; set; } = null!;

    public string MedicineId { get; set; } = null!;

    public string BatchId { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    //public virtual MedicineBatch Batch { get; set; } = null!;

    //public virtual Medicine Medicine { get; set; } = null!;

    //public virtual StockTransfer Transfer { get; set; } = null!;

    //public virtual StockRequest AssignedStock { get; set; } = null!;
}