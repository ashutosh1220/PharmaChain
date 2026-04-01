using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmaChain.Application.Common.Enums;
using PharmaChain.Application.DTOs;
using PharmaChain.Application.Interfaces;
using PharmaChain.Infrastructure.Models;
using static PharmaChain.Application.DTOs.SupplierRequest;
namespace PharmaChain.Infrastructure.BackendServices
{
    public class SupplierService : ISupplierService
    {

        private readonly IPharmaChainDbContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ILogService _logService;

        public SupplierService(
            IPharmaChainDbContext context,
            IHttpContextAccessor httpContext,
            ILogService logService)
        {
            _context = context;
            _httpContext = httpContext;
            _logService = logService;
        }
        private string? GetUserId()
        {
            return _httpContext?.HttpContext?.User?.FindFirst("UserId")?.Value;
        }
        public async Task<CommonResponse> CreateSupplierAsync(CreateSupplierRequest request)
        {
            if (request == null)
                ArgumentNullException.ThrowIfNull(request);

            try
            {
                var userId = GetUserId();
                var exists = await _context.Suppliers
                    .AnyAsync(x => x.SupplierName == request.SupplierName
                                && x.Phone == request.Phone
                                && !x.IsDeleted);

                if (exists)
                {
                    return new CommonResponse
                    {
                        Success = false,
                        Messege = "Supplier already exists."
                    };
                }
                var lastSupplier = await _context.Suppliers
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync();

                int nextNumber = 1;

                if (lastSupplier != null && !string.IsNullOrEmpty(lastSupplier.SupplierId))
                {
                    var lastNumberPart = lastSupplier.SupplierId.Replace("SUP", "");

                    if (int.TryParse(lastNumberPart, out int lastNumber))
                    {
                        nextNumber = lastNumber + 1;
                    }
                }

                string newSupplierId = $"SUP{nextNumber:D6}";

                var entity = new Supplier
                {
                    SupplierId = newSupplierId,
                    SupplierName = request.SupplierName,
                    ContactPerson = request.ContactPerson,
                    Phone = request.Phone,
                    Email = request.Email,
                    Address = request.Address,
                    Gstin = request.GSTIN,
                    DrugLicenseNumber = request.DrugLicenseNumber,
                    BankAccountNumber = request.BankAccountNumber,
                    IfscCode = request.IFSCCode,
                    BankBranchName = request.BankBranchName,

                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Suppliers.AddAsync(entity);
                var result = await _context.SaveChangesAsync(CancellationToken.None);

                if (result > 0)
                {
                    await _logService.AddLogAsync(new LogRequest
                    {
                        Action = "Create Supplier",
                        ActionType = (short)LogActionType.Create,
                        ModuleName = "Supplier Management",
                        TableName = "Suppliers",
                        RecordId = entity.SupplierId,
                        OldValue = null,
                        NewValue = entity,
                        ChangedFields = "All",
                        Delta = null,
                        Notes = "New supplier created"
                    });
                }

                return new CommonResponse
                {
                    Success = true,
                    Messege = "Supplier Added Successfully."
                };
            }
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Success = false,
                    Messege = "Failed To Insert Record. " + ex.InnerException?.Message
                };
            }
        }
    }
}
