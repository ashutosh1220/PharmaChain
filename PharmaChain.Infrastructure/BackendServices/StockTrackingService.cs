using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmaChain.Application.DTOs;
using PharmaChain.Application.Interfaces;
using PharmaChain.Infrastructure.Models;
using static PharmaChain.Application.DTOs.StockLevelDto;

namespace PharmaChain.Infrastructure.BackendServices
{
    public class StockTrackingService : IStockTrackingService
    {

        private readonly IPharmaChainDbContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ILogService _logService;

        public StockTrackingService(
            IPharmaChainDbContext context,
            IHttpContextAccessor httpContext,
            ILogService logService
            )
        {
            _context = context;
            _httpContext = httpContext;
            _logService = logService;
        }

        private string? GetUserId()
        {
            return _httpContext?.HttpContext?.User?.FindFirst("UserId")?.Value;
        }
        public async Task<List<LowStockResponse>> GetLowStockMedicines()
        {

            try
            {
                var userId = GetUserId();

                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("Invalid user context.");
                }

                var branchId = await _context.Users
                    .Where(u => u.UserId == userId)
                    .Select(u => u.BranchId)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(branchId))
                {
                    throw new Exception("User is not mapped to any branch.");
                }

                var result = await _context.Medicines
    .Select(m => new
    {
        m.MedicineId,
        m.MedicineName,
        m.MinimumStockLevel,

        CurrentStock = _context.StockLedgers
            .Where(l => l.BranchId == branchId
                        && l.MedicineId == m.MedicineId
                        && l.TransactionType != "ADJUSTMENT_LOSS")
            .Sum(l => (int?)(l.QuantityIn - l.QuantityOut)) ?? 0
    })
    .Where(x => x.CurrentStock > 0 && x.CurrentStock < x.MinimumStockLevel)
    .Select(x => new LowStockResponse
    {
        MedicineId = x.MedicineId,
        MedicineName = x.MedicineName,
        CurrentStock = x.CurrentStock,
        MinimumStock = x.MinimumStockLevel
    })
    .ToListAsync();

                return result;
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception("Unauthorized: " + ex.Message);
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException != null
                    ? ex.InnerException.Message
                    : ex.Message;

                throw new Exception("Database error: " + message);
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong: " + ex.Message);
            }
        }

        public async Task<RequestStatsResponse> GetRequestStats(string? branchId, int rangeInDays)
        {
            try
            {
                var userId = GetUserId();

                if (string.IsNullOrEmpty(branchId))
                {
                    if (string.IsNullOrEmpty(userId))
                    {
                        throw new UnauthorizedAccessException("Invalid user context.");
                    }

                    branchId = await _context.Users
                        .Where(u => u.UserId == userId)
                        .Select(u => u.BranchId)
                        .FirstOrDefaultAsync();
                }

                if (string.IsNullOrEmpty(branchId))
                {
                    throw new Exception("Branch not found.");
                }

                var fromDate = DateTime.UtcNow.AddDays(-rangeInDays);

                var stats = await _context.StockRequests
                    .Where(x => x.FromBranchId == branchId
                                && x.CreatedAt >= fromDate)
                    .GroupBy(x => 1)
                    .Select(g => new RequestStatsResponse
                    {
                        TotalRequests = g.Count(),
                        ApprovedRequests = g.Count(x => x.ApprovalStatus == "APPROVED"),
                        FulfilledRequests = g.Count(x => x.ApprovalStatus == "FULFILLED"),
                        RejectedRequests = g.Count(x => x.ApprovalStatus == "REJECTED")
                    })
                    .FirstOrDefaultAsync();

                return stats ?? new RequestStatsResponse();
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception("Unauthorized: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to fetch request stats: " + ex.Message);
            }
        }


        public async Task<string> AddStockTransferAsync(StockTransferRequest request)
        {
            var a = (_context as DbContext)?.Model.ToDebugString();
            Console.WriteLine();
            if (request.Medicines == null || !request.Medicines.Any())
                throw new ArgumentException("No medicines provided");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var lastTransferId = await _context.StockTransfers
                    .Where(x => x.TransferId.StartsWith("TRFR"))
                    .OrderByDescending(x => x.TransferId)
                    .Select(x => x.TransferId)
                    .FirstOrDefaultAsync();

                int transferCounter = 0;

                if (!string.IsNullOrEmpty(lastTransferId))
                {
                    var numericPart = lastTransferId.Replace("TRFR", "");
                    if (!int.TryParse(numericPart, out transferCounter))
                        transferCounter = 0;
                }

                transferCounter++;
                string newTransferId = $"TRFR{transferCounter:D4}";

                var lastAssignedId = await _context.StockRequests
                    .Where(x => x.AssignedStockId.StartsWith("SRQ"))
                    .OrderByDescending(x => x.AssignedStockId)
                    .Select(x => x.AssignedStockId)
                    .FirstOrDefaultAsync();

                int stockCounter = 0;

                if (!string.IsNullOrEmpty(lastAssignedId))
                {
                    var numericPart = lastAssignedId.Replace("SRQ", "");
                    if (!int.TryParse(numericPart, out stockCounter))
                        stockCounter = 0;
                }

                stockCounter++;
                string newStockId = $"SRQ{stockCounter:D4}";

                var stockRequest = new StockRequest
                {
                    AssignedStockId = newStockId,
                    FromBranchId = "BR001",   
                    ToBranchId = "BR002",
                    QuantityRequested = request.Medicines.Sum(x => x.Quantity),
                    RequestDate = DateTime.UtcNow,
                    ApprovalStatus = "PENDING",
                    CreatedBy = GetUserId() ?? "USR001",
                    CreatedAt = DateTime.UtcNow
                };

                _context.StockRequests.Add(stockRequest);
                await _context.SaveChangesAsync(CancellationToken.None);

                var transfer = new StockTransfer
                {
                    TransferId = newTransferId,
                    AssignedStockId = newStockId,
                    CreatedAt = DateTime.UtcNow,
                    TransferStatus = "PENDING",
                    RequestedBy = GetUserId() ?? "USR001"
                };

                _context.StockTransfers.Add(transfer);
                await _context.SaveChangesAsync(CancellationToken.None);

                var transferItems = new List<StockTransferItem>();

                foreach (var item in request.Medicines)
                {
                    transferItems.Add(new StockTransferItem
                    {
                        TransferId = newTransferId,
                        AssignedStockId = newStockId,
                        MedicineId = item.MedicineId,
                        BatchId = item.BatchId,
                        Quantity = item.Quantity,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _context.StockTransferItems.AddRangeAsync(transferItems);
                await _context.SaveChangesAsync(CancellationToken.None);

                var generatedIds = transferItems
                    .Select(x => x.TransferItemId)
                    .ToList();

                string itemsIdString = "{" + string.Join(",", generatedIds) + "}";

                stockRequest.ItemsId = itemsIdString;

                _context.StockRequests.Update(stockRequest);
                await _context.SaveChangesAsync(CancellationToken.None);

                await transaction.CommitAsync(CancellationToken.None);

                return newTransferId;
            }
            catch
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        }
    }
}
