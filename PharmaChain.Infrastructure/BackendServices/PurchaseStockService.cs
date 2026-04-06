using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmaChain.Application.Common.Enums;
using PharmaChain.Application.DTOs;
using PharmaChain.Application.Interfaces;
using PharmaChain.Infrastructure.Models;

namespace PharmaChain.Infrastructure.BackendServices
{
    public class PurchaseStockService : IPurchaseStockService
    {
        private readonly IPharmaChainDbContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ILogService _logService;
        public PurchaseStockService(
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

        public async Task<PurchaseResponseDto> CreatePurchaseInvoice(PurchaseEntryDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            try
            {
                var userId = GetUserId() ?? "System";

                if (request.Items == null || !request.Items.Any())
                {
                    return new PurchaseResponseDto
                    {
                        Success = false,
                        Message = "At least one item is required."
                    };
                }

                Console.WriteLine($"Items received: {request.Items.Count}");
                foreach (var i in request.Items)
                {
                    Console.WriteLine($"Medicine: {i?.MedicineId}, Batch: {i?.BatchNumber}");
                }

                var lastInvoice = await _context.PurchaseInvoices
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync();

                int nextNumber = 1;

                if (lastInvoice != null && !string.IsNullOrEmpty(lastInvoice.PurchaseInvoiceId))
                {
                    var lastNumberPart = lastInvoice.PurchaseInvoiceId.Replace("PIN", "");

                    if (int.TryParse(lastNumberPart, out int lastNumber))
                        nextNumber = lastNumber + 1;
                }

                string newInvoiceId = $"PIN{nextNumber:D6}";

                var invoice = new PurchaseInvoice
                {
                    PurchaseInvoiceId = newInvoiceId,
                    SupplierId = request.SupplierId,
                    BranchId = request.BranchId,
                    SupplierInvoiceNumber = request.SupplierInvoiceNumber,
                    InvoiceDate = request.InvoiceDate,

                    SubTotal = request.SubTotal,
                    TotalTax = request.TotalTax,
                    InwardCharges = request.InwardCharges,
                    DiscountAmount = request.DiscountAmount,
                    GrandTotal = request.GrandTotal,

                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };

                await _context.PurchaseInvoices.AddAsync(invoice);

                var validItems = request.Items
                    .Where(x => x != null && !string.IsNullOrWhiteSpace(x.MedicineId))
                    .ToList();

                if (!validItems.Any())
                {
                    return new PurchaseResponseDto
                    {
                        Success = false,
                        Message = "No valid items found."
                    };
                }

                var items = validItems.Select(x => new PurchaseItem
                {
                    PurchaseInvoiceId = invoice.PurchaseInvoiceId,
                    MedicineId = x.MedicineId,
                    BatchNumber = x.BatchNumber,
                    Quantity = x.Quantity,
                    UnitPurchasePrice = x.UnitPurchasePrice,
                    UnitSellingPrice = x.UnitSellingPrice,
                    GstPercentage = x.GstPercentage,
                    MfgDate = x.MfgDate,
                    ExpDate = x.ExpDate
                }).ToList();

                await _context.PurchaseItems.AddRangeAsync(items);

                // 🔹 Save
                var result = await _context.SaveChangesAsync(CancellationToken.None);

                Console.WriteLine($"Rows affected: {result}");

                if (result <= 0)
                {
                    return new PurchaseResponseDto
                    {
                        Success = false,
                        Message = "No records were saved."
                    };
                }
                try
                {
                    await _logService.AddLogAsync(new LogRequest
                    {
                        Action = "Create Purchase Invoice",
                        ActionType = (short)LogActionType.Create,
                        ModuleName = "Purchase Management",
                        TableName = "PurchaseInvoices",
                        RecordId = invoice.PurchaseInvoiceId,
                        OldValue = null,
                        NewValue = invoice,
                        ChangedFields = "All",
                        Notes = "New purchase invoice created"
                    });
                }
                catch (Exception logEx)
                {
                    Console.WriteLine("Log error: " + logEx.Message);
                }

                return new PurchaseResponseDto
                {
                    Success = true,
                    Message = "Purchase invoice created successfully."
                };
            }
            catch (Exception ex)
            {
                return new PurchaseResponseDto
                {
                    Success = false,
                    Message = ex.ToString()
                };
            }
        }
    }
}
