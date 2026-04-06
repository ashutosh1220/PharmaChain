namespace PharmaChain.Application.DTOs
{
    public class PurchaseResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class PurchaseEntryDto
    {
        public string? PurchaseInvoiceId { get; set; } = null!;

        public string SupplierId { get; set; } = null!;

        public string BranchId { get; set; } = null!;

        public string SupplierInvoiceNumber { get; set; } = null!;

        public DateOnly InvoiceDate { get; set; }

        public decimal SubTotal { get; set; }

        public decimal TotalTax { get; set; }

        public decimal InwardCharges { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal GrandTotal { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; } = null!;

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }

        public List<PurchaseItemDto> Items { get; set; } = new();
    }

    public class PurchaseItemDto
    {
        public long Id { get; set; }

        public string? PurchaseInvoiceId { get; set; }

        public string MedicineId { get; set; } = null!;

        public string BatchNumber { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal UnitPurchasePrice { get; set; }

        public decimal UnitSellingPrice { get; set; }

        public decimal GstPercentage { get; set; }

        public DateOnly? MfgDate { get; set; }

        public DateOnly? ExpDate { get; set; }
    }
}