using PharmaChain.Application.DTOs;

namespace PharmaChain.Application.Interfaces
{
    public interface IPurchaseStockService
    {
        Task<PurchaseResponseDto> CreatePurchaseInvoice(PurchaseEntryDto request);
    }
}
