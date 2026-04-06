using PharmaChain.Application.DTOs;

namespace PharmaChain.Application.Interfaces
{
    public interface IBatchService
    {
        Task<MedicineBatchResponse> CreateBatch(MedicineBatchRequest request);
        Task<MedicineBatchResponse> UpdateBatch(MedicineBatchRequest request);
        Task<MedicineBatchResponse> DeleteBatch(string id);
        Task<MedicineBatchListResponse> GetBatchesAsync(int page, int size);
        Task<SingleMedicineBatchResponse> GetBatchByIdAsync(string id);
        //Task<MedicineBatchListResponse> GetBatchesByMedicineIdAsync(string medicineId);
        //Task<MedicineBatchListResponse> GetExpiringBatchesAsync(DateTime beforeDate);
    }
}