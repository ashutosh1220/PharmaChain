using System.ComponentModel.DataAnnotations;

namespace PharmaChain.Application.DTOs
{
    public class StockLevelDto
    {
        public class StockTransferRequest
        {
            public string? TransferId { get; set; } = string.Empty;

            public string? AssignedStockId { get; set; } = string.Empty;

            [Required]
            public List<MedicineItemDto> Medicines { get; set; } = new(); 
        }

        public class MedicineItemDto
        {
            [Required]
            public string MedicineId { get; set; } = string.Empty;
            public string BatchId { get; set; } = string.Empty;

            [Range(1, int.MaxValue)]
            public int Quantity { get; set; }
        }

        public class LowStockResponse
        {
            public string MedicineId { get; set; } = string.Empty;
            public string MedicineName { get; set; } = string.Empty;
            public int CurrentStock { get; set; }
            public int MinimumStock { get; set; }
        }

        public class RequestStatsResponse
        {
            public int TotalRequests { get; set; }
            public int ApprovedRequests { get; set; }
            public int FulfilledRequests { get; set; }
            public int RejectedRequests { get; set; }
        }

        public class PendingRequestsResponse
        {
            public string RequestId { get; set; } = string.Empty;
            public string MedicineId { get; set; } = string.Empty;
            public string BranchId { get; set; } = string.Empty;
        }
    }
}