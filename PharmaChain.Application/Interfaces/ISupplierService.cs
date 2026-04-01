using static PharmaChain.Application.DTOs.SupplierRequest;

namespace PharmaChain.Application.Interfaces
{
    public interface ISupplierService
    {
        Task<CommonResponse> CreateSupplierAsync(CreateSupplierRequest request);
    }
}
