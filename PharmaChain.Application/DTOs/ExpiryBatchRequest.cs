namespace PharmaChain.Application.DTOs
{
    public class ExpiryBatchRequest
    {
        public string BatchId { get; set; }
        public string BatchNumber { get; set; }
        public string GrnNumber { get; set; }

        public string MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string GenericName { get; set; }
        public string Category { get; set; }
        public string Strength { get; set; }
        public string Manufacturer { get; set; }

        public string BranchId { get; set; }
        public string BranchName { get; set; }

        public string SupplierId { get; set; }
        public string SupplierName { get; set; }

        public DateOnly MfgDate { get; set; }
        public DateOnly ExpDate { get; set; }

        public int TotalStockReceived { get; set; }

        public decimal UnitPurchasePrice { get; set; }
        public decimal UnitSellingPrice { get; set; }

        public bool IsPrescriptionRequired { get; set; }

        public string HsnCode { get; set; }
        public decimal GstPercentage { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
