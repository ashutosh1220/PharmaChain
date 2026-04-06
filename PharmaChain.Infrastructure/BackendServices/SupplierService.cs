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


        public async Task<CommonResponse> UpdateSupplierAsync(CreateSupplierRequest request)
        {
            if (request == null)
                ArgumentNullException.ThrowIfNull(request);

            try
            {
                var userId = GetUserId();

                var entity = await _context.Suppliers
                    .FirstOrDefaultAsync(x => x.SupplierId == request.SupplierId && !x.IsDeleted);

                if (entity == null)
                {
                    return new CommonResponse
                    {
                        Success = false,
                        Messege = "Supplier not found."
                    };
                }

                var oldValue = new Dictionary<string, object?>();
                var newValue = new Dictionary<string, object?>();
                var delta = new Dictionary<string, object>();

                void TrackChange<T>(string field, T oldVal, T newVal, Action updateAction)
                {
                    if (!EqualityComparer<T>.Default.Equals(oldVal, newVal))
                    {
                        oldValue[field] = oldVal;
                        newValue[field] = newVal;

                        delta[field] = new
                        {
                            Old = oldVal,
                            New = newVal
                        };

                        updateAction();
                    }
                }

                TrackChange("SupplierName", entity.SupplierName, request.SupplierName,
                    () => entity.SupplierName = request.SupplierName);

                TrackChange("ContactPerson", entity.ContactPerson, request.ContactPerson,
                    () => entity.ContactPerson = request.ContactPerson);

                TrackChange("Phone", entity.Phone, request.Phone,
                    () => entity.Phone = request.Phone);

                TrackChange("Email", entity.Email, request.Email,
                    () => entity.Email = request.Email);

                TrackChange("Address", entity.Address, request.Address,
                    () => entity.Address = request.Address);

                TrackChange("GSTIN", entity.Gstin, request.GSTIN,
                    () => entity.Gstin = request.GSTIN);

                TrackChange("DrugLicenseNumber", entity.DrugLicenseNumber, request.DrugLicenseNumber,
                    () => entity.DrugLicenseNumber = request.DrugLicenseNumber);

                TrackChange("BankAccountNumber", entity.BankAccountNumber, request.BankAccountNumber,
                    () => entity.BankAccountNumber = request.BankAccountNumber);

                TrackChange("IFSCCode", entity.IfscCode, request.IFSCCode,
                    () => entity.IfscCode = request.IFSCCode);

                TrackChange("BankBranchName", entity.BankBranchName, request.BankBranchName,
                    () => entity.BankBranchName = request.BankBranchName);

                TrackChange("IsActive", entity.IsActive, request.IsActive,
                    () => entity.IsActive = request.IsActive);

                // ✅ No changes check
                if (!delta.Any())
                {
                    return new CommonResponse
                    {
                        Success = true,
                        Messege = "No changes detected."
                    };
                }

                entity.UpdatedBy = userId;
                entity.UpdatedAt = DateTime.UtcNow;

                var result = await _context.SaveChangesAsync(CancellationToken.None);

                if (result > 0)
                {
                    await _logService.AddLogAsync(new LogRequest
                    {
                        Action = "Update Supplier",
                        ActionType = (short)LogActionType.Update,
                        ModuleName = "Supplier Management",
                        TableName = "Suppliers",
                        RecordId = entity.SupplierId,
                        OldValue = oldValue,
                        NewValue = newValue,
                        ChangedFields = string.Join(",", delta.Keys),
                        Delta = System.Text.Json.JsonSerializer.Serialize(delta),
                        Notes = "Only changed fields updated"
                    });
                }

                return new CommonResponse
                {
                    Success = true,
                    Messege = "Supplier Updated Successfully."
                };
            }
            catch (Exception ex)
            {
                return new CommonResponse
                {
                    Success = false,
                    Messege = "Failed To Update Record. " + ex.InnerException?.Message
                };
            }
        }


        public async Task<SupplierListResponse> GetSuppliersAsync(int page, int size)
        {
            try
            {
                var query = _context.Suppliers.AsQueryable();

                var totalSuppliers = await query.CountAsync();
                var activeSuppliers = await query.CountAsync(x => x.IsActive && !x.IsDeleted);
                var inactiveSuppliers = await query.CountAsync(x => !x.IsActive && !x.IsDeleted);
                var suspendedSuppliers = await query.CountAsync(x => x.IsDeleted);

                if (size > 50)
                {
                    size = 10;
                }

                var suppliers = await query
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.SupplierId)
                    .Skip((page - 1) * size)
                    .Take(size)
                    .Select(x => new CreateSupplierRequest   
                    {
                        SupplierId = x.SupplierId,
                        SupplierName = x.SupplierName,
                        ContactPerson = x.ContactPerson,
                        Phone = x.Phone,
                        Email = x.Email,
                        Address = x.Address,

                        GSTIN = x.Gstin,         
                        DrugLicenseNumber = x.DrugLicenseNumber,

                        BankAccountNumber = x.BankAccountNumber,
                        IFSCCode = x.IfscCode,            

                        BankBranchName = x.BankBranchName,
                        IsActive = x.IsActive,
                        CreatedBy = x.CreatedBy,
                        CreatedAt = x.CreatedAt
                    })
                    .ToListAsync();

                return new SupplierListResponse
                {
                    TotalSuppliers = totalSuppliers,
                    ActiveSuppliers = activeSuppliers,
                    InactiveSuppliers = inactiveSuppliers,
                    SuspendedSuppliers = suspendedSuppliers,
                    Records = suppliers,
                    Success = true,
                    Messege = "Suppliers fetched successfully."
                };
            }
            catch (Exception)
            {
                return new SupplierListResponse
                {
                    Success = false,
                    Messege = "Something went wrong while fetching suppliers.",
                    Records = new List<CreateSupplierRequest>()
                };
            }
        }
    }
}
