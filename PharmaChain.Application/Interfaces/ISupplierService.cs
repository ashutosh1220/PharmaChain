using static PharmaChain.Application.DTOs.SupplierRequest;

namespace PharmaChain.Application.Interfaces
{
    public interface ISupplierService
    {
        Task<CommonResponse> CreateSupplierAsync(CreateSupplierRequest request);
        Task<CommonResponse> UpdateSupplierAsync(CreateSupplierRequest request);
        Task<SupplierListResponse> GetSuppliersAsync(int page, int size);
    }
}
