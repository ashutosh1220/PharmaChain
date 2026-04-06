namespace PharmaChain.Application.DTOs
{
    public class MedicineBatchRequest
    {
        public string? BatchId { get; set; }
        public string? MedicineId { get; set; }
        public string? BatchNumber { get; set; }
        public int TotalStockReceived { get; set; }
        public decimal UnitPurchasePrice { get; set; }
        public decimal UnitSellingPrice { get; set; }
        public DateOnly? MfgDate { get; set; }
        public DateOnly? ExpDate { get; set; }
        public string? SupplierId { get; set; }
        public string? BranchId { get; set; }
        public string? GrnNumber { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class MedicineBatchResponse
    {
        public bool Success { get; set; }
        public string? Messege { get; set; }
    }

    public class MedicineBatchListResponse
    {
        public int TotalBatches { get; set; }
        public bool Success { get; set; }
        public string Messege { get; set; }
        public List<MedicineBatchRequest> Records { get; set; }
    }

    public class SingleMedicineBatchResponse
    {
        public bool Success { get; set; }
        public string Messege { get; set; }
        public MedicineBatchRequest Records { get; set; }
    }
}