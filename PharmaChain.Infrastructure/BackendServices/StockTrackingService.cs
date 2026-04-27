using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmaChain.Application.Common.Enums;
using PharmaChain.Application.Interfaces;
using PharmaChain.Infrastructure.Models;
using System.Text.Json;
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

        public async Task<string> CreateStockTransferAsync(StockTransferRequest request)
        {
            string transferId = "";

            try
            {
                transferId = await AddStockTransferAsync(request); // existing logic

                try
                {
                    await _logService.AddLogAsync(new LogRequest
                    {
                        Action = "Create Stock Transfer Request",
                        ActionType = (short)LogActionType.Create,
                        ModuleName = "Stock Management",
                        TableName = "StockTransfers",
                        RecordId = transferId,
                        OldValue = null,
                        NewValue = request,
                        ChangedFields = "All",
                        Notes = $"Stock request created with {request.Medicines.Count} medicines"
                    });
                }
                catch { }

                return transferId;
            }
            catch (Exception ex)
            {
                try
                {
                    await _logService.AddLogAsync(new LogRequest
                    {
                        Action = "Create Stock Transfer Request Failed",
                        ActionType = (short)LogActionType.Error,
                        ModuleName = "Stock Management",
                        TableName = "StockTransfers",
                        RecordId = string.IsNullOrEmpty(transferId) ? "N/A" : transferId,
                        OldValue = null,
                        NewValue = request,
                        ChangedFields = null,
                        Notes = ex.Message
                    });
                }
                catch {
                
                }

                throw;
            }
        }

        public async Task<List<object>> GetStockRequestActivitiesAsync()
        {
            var data = await (
                from r in _context.StockRequests
                where !string.IsNullOrEmpty(r.ApprovalStatus)

                join i in _context.StockTransferItems
                    on r.AssignedStockId equals i.AssignedStockId

                join m in _context.Medicines
                    on i.MedicineId equals m.MedicineId

                group new { i, m } by new
                {
                    r.AssignedStockId,
                    r.FulfillmentType,
                    r.RequestDate,
                    r.ToBranchId,
                    r.ApprovalStatus,
                    r.Remarks
                }
                into g

                select new
                {
                    id = g.Key.AssignedStockId,
                    branch = g.Key.ToBranchId,
                    type = g.Key.FulfillmentType,
                    date = g.Key.RequestDate,
                    status = g.Key.ApprovalStatus,
                    remark = g.Key.Remarks,

                    medicines = g.Select(x => new
                    {
                        medicineId = x.i.MedicineId,
                        medicineName = x.m.MedicineName,
                        batchId = x.i.BatchId,
                        qty = x.i.Quantity
                    }).ToList()
                }
            ).ToListAsync();

            return data.Cast<object>().ToList();
        }

        public async Task<bool> ApproveStockAsync(ApprovalRequestDto req, string? userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var now = DateTime.UtcNow;

                var transfer = await _context.StockTransfers
                    .FirstOrDefaultAsync(x => x.AssignedStockId == req.AssignedStockId);

                if (transfer == null)
                    throw new Exception("StockTransfer not found");

                var request = await _context.StockRequests
                    .FirstOrDefaultAsync(x => x.AssignedStockId == req.AssignedStockId);

                var items = await _context.StockTransferItems
                    .Where(x => x.AssignedStockId == req.AssignedStockId)
                    .ToListAsync();

                var oldValue = new
                {
                    transfer.ApprovedBy,
                    transfer.UpdatedAt,
                    Request = request == null ? null : new
                    {
                        request.QuantityApproved,
                        request.ApprovalStatus,
                        request.ApprovedDate
                    },
                    Items = items.Select(i => new
                    {
                        i.MedicineId,
                        i.QtyApproved
                    }).ToList()
                };

                transfer.ApprovedBy = userId;
                transfer.UpdatedAt = now;

                if (request != null)
                {
                    request.QuantityApproved = req.Medicines.Sum(x => x.QtyApproved);
                    request.ApprovalStatus = req.ApprovalStatus;
                    request.ApprovedDate = now;
                    request.FulfillmentType = req.FulfillmentType;
                    request.Remarks = req.Remark ?? "Approved";
                }

                foreach (var item in items)
                {
                    var match = req.Medicines
                        .FirstOrDefault(m => m.MedicineId == item.MedicineId);

                    if (match != null)
                        item.QtyApproved = match.QtyApproved;
                }

                var newValue = new
                {
                    transfer.ApprovedBy,
                    transfer.UpdatedAt,
                    Request = request == null ? null : new
                    {
                        request.QuantityApproved,
                        request.ApprovalStatus,
                        request.ApprovedDate
                    },
                    Items = items.Select(i => new
                    {
                        i.MedicineId,
                        i.QtyApproved
                    }).ToList()
                };

                var delta = new
                {
                    Status = "Approved",
                    MedicinesCount = req.Medicines.Count
                };

                await _context.SaveChangesAsync(CancellationToken.None);
                await transaction.CommitAsync(CancellationToken.None);

                try
                {
                    await _logService.AddLogAsync(new LogRequest
                    {
                        Action = "Approve Stock",
                        ActionType = (short)LogActionType.Update,
                        ModuleName = "Stock Management",
                        TableName = "StockTransfers",
                        RecordId = req.AssignedStockId,
                        OldValue = oldValue,
                        NewValue = newValue,
                        ChangedFields = "Approval + Items",
                        Delta = JsonSerializer.Serialize(delta),
                        Notes = $"Stock approved with {req.Medicines.Count} medicines"
                    });
                }
                catch { }

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(CancellationToken.None);

                try
                {
                    await _logService.AddLogAsync(new LogRequest
                    {
                        Action = "Approve Stock Failed",
                        ActionType = (short)LogActionType.Error,
                        ModuleName = "Stock Management",
                        TableName = "StockTransfers",
                        RecordId = req.AssignedStockId ?? "N/A",
                        OldValue = null,
                        NewValue = req,
                        ChangedFields = null,
                        Notes = ex.Message
                    });
                }
                catch { }

                throw;
            }
        }

        public async Task<bool> RejectStockAsync(RejectStockRequestDto req, string? userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (string.IsNullOrEmpty(req.AssignedStockId))
                    throw new Exception("AssignedStockId is required");

                var stockTransfers = await _context.StockTransfers
                    .FirstOrDefaultAsync(x => x.AssignedStockId == req.AssignedStockId);

                var stockRequest = await _context.StockRequests
                    .FirstOrDefaultAsync(x => x.AssignedStockId == req.AssignedStockId);

                if (stockTransfers == null || stockRequest == null)
                    throw new Exception("Stock request not found");

                var oldValue = new
                {
                    TransferStatus = stockTransfers.TransferStatus,
                    TransferRemarks = stockTransfers.Remarks,
                    RequestStatus = stockRequest.ApprovalStatus,
                    RequestRemarks = stockRequest.Remarks
                };

                var now = DateTime.UtcNow;

                stockTransfers.TransferStatus = "CANCELLED";
                stockTransfers.Remarks = req.Remark;
                stockTransfers.ApprovedBy = userId;
                stockTransfers.UpdatedAt = now;

                stockRequest.ApprovalStatus = "REJECTED";
                stockRequest.Remarks = req.Remark;
                stockRequest.ApprovedBy = userId;
                stockRequest.UpdatedAt = now;

                await _context.SaveChangesAsync(CancellationToken.None);
                await transaction.CommitAsync();

                var newValue = new
                {
                    TransferStatus = stockTransfers.TransferStatus,
                    TransferRemarks = stockTransfers.Remarks,
                    ApprovalStatus = stockRequest.ApprovalStatus,
                    RequestRemarks = stockRequest.Remarks
                };

                var delta = new
                {
                    Action = "Rejected",
                    Remark = req.Remark
                };

                try
                {
                    await _logService.AddLogAsync(new LogRequest
                    {
                        Action = "Reject Stock",
                        ActionType = (short)LogActionType.Update,
                        ModuleName = "Stock Management",
                        TableName = "StockTransfers",
                        RecordId = req.AssignedStockId,
                        OldValue = oldValue,
                        NewValue = newValue,
                        ChangedFields = "Status + Remarks",
                        Delta = JsonSerializer.Serialize(delta),
                        Notes = $"Stock rejected. Remark: {req.Remark}"
                    });
                }
                catch 
                {
                
                }

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                try
                {
                    await _logService.AddLogAsync(new LogRequest
                    {
                        Action = "Reject Stock Failed",
                        ActionType = (short)LogActionType.Error,
                        ModuleName = "Stock Management",
                        TableName = "StockTransfers",
                        RecordId = req.AssignedStockId ?? "N/A",
                        OldValue = null,
                        NewValue = req,
                        ChangedFields = null,
                        Notes = ex.Message
                    });
                }
                catch 
                { 
                
                }

                throw;
            }
        }
    }
}
