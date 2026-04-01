namespace PharmaChain.Application.DTOs
{
    public class MedicineRequest
    {
        public string MedicineId { get; set; } = string.Empty;
        public string? MedicineName { get; set; }
        public string? GenericName { get; set; }
        public string? Category { get; set; }
        public string? Strength { get; set; }
        public string? Manufacturer { get; set; }
        public bool IsPrescriptionRequired { get; set; }
        public int MinimumStockLevel { get; set; }
        public string? HsnCode { get; set; }
        public decimal GstPercentage { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class MedicineResponse
    {
        public bool Success { get; set; }
        public string? Messege { get; set; }
    }

    public class MedicineListResponse
    {
        public int TotalMedicines { get; set; }
        public int ActiveMedicines{ get; set; }
        public int InactiveMedicines { get; set; }
        public int SuspendedMedicines { get; set; }
        public bool Success { get; set; }
        public string Messege { get; set; }
        public List<MedicineRequest> Records { get; set; }
    }

    public class SingleMedicineListResponse
    {
        public bool Success { get; set; }
        public string Messege { get; set; }
        public MedicineRequest Records { get; set; }
    }
}
