using static PharmaChain.Application.DTOs.StockLevelDto;

namespace PharmaChain.Application.Interfaces
{
    public interface IStockTrackingService
    {
        Task<List<LowStockResponse>> GetLowStockMedicines();
        Task<RequestStatsResponse> GetRequestStats(string? branchId, int rangeInDays);
        Task<string> AddStockTransferAsync(StockTransferRequest request);
        Task<string> CreateStockTransferAsync(StockTransferRequest request);
        Task<List<object>> GetStockRequestActivitiesAsync();
        Task<bool> ApproveStockAsync(ApprovalRequestDto req, string? userId);
    }
}
