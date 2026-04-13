namespace PharmaChain.Application.DTOs
{
    public class ExpiryBatchRequest
    {
        public string BatchId { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public string GrnNumber { get; set; } = string.Empty;

        public string MedicineId { get; set; } = string.Empty;
        public string MedicineName { get; set; } = string.Empty;
        public string GenericName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;

        public string BranchId { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;

        public string SupplierId { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;

        public DateOnly MfgDate { get; set; }
        public DateOnly ExpDate { get; set; }

        public int TotalStockReceived { get; set; }

        public decimal UnitPurchasePrice { get; set; }
        public decimal UnitSellingPrice { get; set; }

        public bool IsPrescriptionRequired { get; set; }

        public string HsnCode { get; set; } = string.Empty;
        public decimal GstPercentage { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
