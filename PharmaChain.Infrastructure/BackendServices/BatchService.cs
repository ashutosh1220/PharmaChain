using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmaChain.Application.Common.Enums;
using PharmaChain.Application.DTOs;
using PharmaChain.Application.Interfaces;
using PharmaChain.Infrastructure.Models;
using System.Text.Json;

namespace PharmaChain.Infrastructure.BackendServices
{
    public class BatchService : IBatchService
    {
        private readonly IPharmaChainDbContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ILogService _logService;

        public BatchService(
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

        // ================= CREATE =================
        public async Task<MedicineBatchResponse> CreateBatch(MedicineBatchRequest request)
        {
            if (request == null)
                ArgumentNullException.ThrowIfNull(request);

            try
            {
                var userId = GetUserId();

                if (request.MfgDate.HasValue && request.ExpDate.HasValue)
                {
                    if (request.MfgDate > request.ExpDate)
                    {
                        return new MedicineBatchResponse
                        {
                            Success = false,
                            Messege = "Expiry date cannot be before manufacturing date."
                        };
                    }
                }

                if (request.UnitSellingPrice < request.UnitPurchasePrice)
                {
                    return new MedicineBatchResponse
                    {
                        Success = false,
                        Messege = "Selling price cannot be less than purchase price."
                    };
                }

                var lastBatch = await _context.MedicineBatches
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync();

                int nextNumber = 1;

                if (lastBatch != null && !string.IsNullOrEmpty(lastBatch.BatchId))
                {
                    var num = lastBatch.BatchId.Replace("BAT", "");
                    if (int.TryParse(num, out int last))
                        nextNumber = last + 1;
                }

                string newBatchId = $"BAT{nextNumber:D6}";

                var entity = new MedicineBatch
                {
                    BatchId = newBatchId,
                    MedicineId = request.MedicineId,
                    BatchNumber = request.BatchNumber,
                    TotalStockReceived = request.TotalStockReceived,
                    UnitPurchasePrice = request.UnitPurchasePrice,
                    UnitSellingPrice = request.UnitSellingPrice,

                    MfgDate = request.MfgDate?? default,
                    ExpDate = request.ExpDate?? default,

                    SupplierId = request.SupplierId,
                    BranchId = request.BranchId,
                    GrnNumber = request.GrnNumber,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.MedicineBatches.AddAsync(entity);
                var result = await _context.SaveChangesAsync(CancellationToken.None);

                if (result > 0)
                {
                    await _logService.AddLogAsync(new LogRequest
                    {
                        Action = "Create Batch",
                        ActionType = (short)LogActionType.Create,
                        ModuleName = "Batch Management",
                        TableName = "MedicineBatches",
                        RecordId = entity.BatchId,
                        NewValue = entity,
                        ChangedFields = "All",
                        Notes = "New batch created"
                    });
                }

                return new MedicineBatchResponse
                {
                    Success = true,
                    Messege = "Batch created successfully."
                };
            }
            catch (Exception ex)
            {
                return new MedicineBatchResponse
                {
                    Success = false,
                    Messege = ex.InnerException?.Message ?? ex.Message
                };
            }
        }

        // ================= UPDATE =================
        public async Task<MedicineBatchResponse> UpdateBatch(MedicineBatchRequest request)
        {
            if (request == null)
                ArgumentNullException.ThrowIfNull(request);

            try
            {
                var userId = GetUserId();

                var entity = await _context.MedicineBatches
                    .FirstOrDefaultAsync(x => x.BatchId == request.BatchId);

                if (entity == null)
                {
                    return new MedicineBatchResponse
                    {
                        Success = false,
                        Messege = "Batch not found."
                    };
                }

                var oldValue = new Dictionary<string, object?>();
                var newValue = new Dictionary<string, object?>();
                var delta = new Dictionary<string, object>();

                void TrackChange<T>(string field, T oldVal, T newVal, Action update)
                {
                    if (!EqualityComparer<T>.Default.Equals(oldVal, newVal))
                    {
                        oldValue[field] = oldVal;
                        newValue[field] = newVal;

                        delta[field] = new { Old = oldVal, New = newVal };

                        update();
                    }
                }

                TrackChange("BatchNumber", entity.BatchNumber, request.BatchNumber,
                    () => entity.BatchNumber = request.BatchNumber);

                TrackChange("TotalStockReceived", entity.TotalStockReceived, request.TotalStockReceived,
                    () => entity.TotalStockReceived = request.TotalStockReceived);

                TrackChange("UnitPurchasePrice", entity.UnitPurchasePrice, request.UnitPurchasePrice,
                    () => entity.UnitPurchasePrice = request.UnitPurchasePrice);

                TrackChange("UnitSellingPrice", entity.UnitSellingPrice, request.UnitSellingPrice,
                    () => entity.UnitSellingPrice = request.UnitSellingPrice);

                TrackChange("MfgDate", entity.MfgDate, request.MfgDate,
                    () => entity.MfgDate = request.MfgDate?? default);

                TrackChange("ExpDate", entity.ExpDate, request.ExpDate,
                    () => entity.ExpDate = request.ExpDate ?? default);

                TrackChange("SupplierId", entity.SupplierId, request.SupplierId,
                    () => entity.SupplierId = request.SupplierId);

                TrackChange("BranchId", entity.BranchId, request.BranchId,
                    () => entity.BranchId = request.BranchId);

                TrackChange("GrnNumber", entity.GrnNumber, request.GrnNumber,
                    () => entity.GrnNumber = request.GrnNumber);

                if (!delta.Any())
                {
                    return new MedicineBatchResponse
                    {
                        Success = true,
                        Messege = "No changes detected."
                    };
                }

                entity.CreatedBy = userId;
                entity.CreatedAt = DateTime.UtcNow;

                var result = await _context.SaveChangesAsync(CancellationToken.None);

                if (result > 0)
                {
                    await _logService.AddLogAsync(new LogRequest
                    {
                        Action = "Update Batch",
                        ActionType = (short)LogActionType.Update,
                        ModuleName = "Batch Management",
                        TableName = "MedicineBatches",
                        RecordId = entity.BatchId,
                        OldValue = oldValue,
                        NewValue = newValue,
                        ChangedFields = string.Join(",", delta.Keys),
                        Delta = JsonSerializer.Serialize(delta),
                        Notes = "Batch updated"
                    });
                }

                return new MedicineBatchResponse
                {
                    Success = true,
                    Messege = "Batch updated successfully."
                };
            }
            catch
            {
                return new MedicineBatchResponse
                {
                    Success = false,
                    Messege = "Failed to update batch."
                };
            }
        }

        // ================= DELETE =================
        public async Task<MedicineBatchResponse> DeleteBatch(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new MedicineBatchResponse
                {
                    Success = false,
                    Messege = "Batch id is empty."
                };
            }

            try
            {
                var entity = await _context.MedicineBatches
                    .FirstOrDefaultAsync(x => x.BatchId == id);

                if (entity == null)
                {
                    return new MedicineBatchResponse
                    {
                        Success = false,
                        Messege = "Batch not found."
                    };
                }

                _context.MedicineBatches.Remove(entity);
                var result = await _context.SaveChangesAsync(CancellationToken.None);

                if (result > 0)
                {
                    await _logService.AddLogAsync(new LogRequest
                    {
                        Action = "Delete Batch",
                        ActionType = (short)LogActionType.Delete,
                        ModuleName = "Batch Management",
                        TableName = "MedicineBatches",
                        RecordId = id,
                        Notes = "Batch deleted"
                    });
                }

                return new MedicineBatchResponse
                {
                    Success = true,
                    Messege = "Batch deleted successfully."
                };
            }
            catch
            {
                return new MedicineBatchResponse
                {
                    Success = false,
                    Messege = "Failed to delete batch."
                };
            }
        }

        // ================= GET LIST =================
        public async Task<MedicineBatchListResponse> GetBatchesAsync(int page, int size)
        {
            try
            {
                var query = _context.MedicineBatches.AsQueryable();

                var total = await query.CountAsync();

                var records = await query
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip((page - 1) * size)
                    .Take(size)
                    .Select(x => new MedicineBatchRequest
                    {
                        BatchId = x.BatchId,
                        MedicineId = x.MedicineId,
                        BatchNumber = x.BatchNumber,
                        TotalStockReceived = x.TotalStockReceived,
                        UnitPurchasePrice = x.UnitPurchasePrice,
                        UnitSellingPrice = x.UnitSellingPrice,
                        MfgDate = x.MfgDate,
                        ExpDate = x.ExpDate,
                        SupplierId = x.SupplierId,
                        BranchId = x.BranchId,
                        GrnNumber = x.GrnNumber,
                        CreatedAt = x.CreatedAt
                    })
                    .ToListAsync();

                return new MedicineBatchListResponse
                {
                    TotalBatches = total,
                    Records = records,
                    Success = true,
                    Messege = "Batches fetched successfully."
                };
            }
            catch
            {
                return new MedicineBatchListResponse
                {
                    Success = false,
                    Messege = "Failed to fetch batches.",
                    Records = new List<MedicineBatchRequest>()
                };
            }
        }
        // ================= GET BY ID =================
        public async Task<SingleMedicineBatchResponse> GetBatchByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new SingleMedicineBatchResponse
                {
                    Success = false,
                    Messege = "Batch id is empty."
                };
            }

            try
            {
                var entity = await _context.MedicineBatches
                    .FirstOrDefaultAsync(x => x.BatchId == id);

                if (entity == null)
                {
                    return new SingleMedicineBatchResponse
                    {
                        Success = false,
                        Messege = "Batch not found."
                    };
                }

                var dto = new MedicineBatchRequest
                {
                    BatchId = entity.BatchId,
                    MedicineId = entity.MedicineId,
                    BatchNumber = entity.BatchNumber,
                    TotalStockReceived = entity.TotalStockReceived,
                    UnitPurchasePrice = entity.UnitPurchasePrice,
                    UnitSellingPrice = entity.UnitSellingPrice,
                    MfgDate = entity.MfgDate,
                    ExpDate = entity.ExpDate,
                    SupplierId = entity.SupplierId,
                    BranchId = entity.BranchId,
                    GrnNumber = entity.GrnNumber,
                    CreatedAt = entity.CreatedAt,
                    CreatedBy = entity.CreatedBy
                };

                return new SingleMedicineBatchResponse
                {
                    Success = true,
                    Messege = "Batch fetched successfully.",
                    Records = dto
                };
            }
            catch (Exception ex)
            {
                return new SingleMedicineBatchResponse
                {
                    Success = false,
                    Messege = ex.Message
                };
            }
        }
    }
}