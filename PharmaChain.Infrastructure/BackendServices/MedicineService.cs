using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmaChain.Application.Common.Enums;
using PharmaChain.Application.DTOs;
using PharmaChain.Application.Interfaces;
using PharmaChain.Infrastructure.Models;
using System.Text.Json;

namespace PharmaChain.Infrastructure.BackendServices
{
    public class MedicineService : IMedicineService
    {
        private readonly IPharmaChainDbContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ILogService _logService;

        public MedicineService(
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
        public async Task<MedicineResponse> CreateMedicine(MedicineRequest request)
        {
            if (request == null)
                ArgumentNullException.ThrowIfNull(request);

            try
            {
                var userId = GetUserId();

                var exists = await _context.Medicines
                    .AnyAsync(x => x.MedicineName == request.MedicineName
                                && x.Strength == request.Strength
                                && !x.IsDeleted);

                if (exists)
                {
                    return new MedicineResponse
                    {
                        Success = false,
                        Messege = "Medicine already exists."
                    };
                }

                var lastMedicine = await _context.Medicines
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync();

                int nextNumber = 1;

                if (lastMedicine != null && !string.IsNullOrEmpty(lastMedicine.MedicineId))
                {
                    var lastNumberPart = lastMedicine.MedicineId.Replace("MED", "");

                    if (int.TryParse(lastNumberPart, out int lastNumber))
                    {
                        nextNumber = lastNumber + 1;
                    }
                }

                string newMedicineId = $"MED{nextNumber:D6}";

                var entity = new Medicine
                {
                    MedicineId = newMedicineId,
                    MedicineName = request.MedicineName,
                    GenericName = request.GenericName,
                    Category = request.Category,
                    Strength = request.Strength,
                    Manufacturer = request.Manufacturer,
                    IsPrescriptionRequired = request.IsPrescriptionRequired,
                    MinimumStockLevel = request.MinimumStockLevel,
                    HsnCode = request.HsnCode,
                    GstPercentage = request.GstPercentage,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Medicines.AddAsync(entity);
                var result = await _context.SaveChangesAsync(CancellationToken.None);

                if (result > 0)
                {
                    await _logService.AddLogAsync(new LogRequest
                    {
                        Action = "Create Medicine",
                        ActionType = (short)LogActionType.Create,
                        ModuleName = "Medicine Management",
                        TableName = "Medicines",
                        RecordId = entity.MedicineId,
                        OldValue = null,
                        NewValue = entity,
                        ChangedFields = "All",
                        Delta = null,
                        Notes = "New medicine created"
                    });
                }

                return new MedicineResponse
                {
                    Success = true,
                    Messege = "Medicine Added Successfully."
                };
            }
            catch (Exception ex)
            {
                return new MedicineResponse
                {
                    Success = false,
                    Messege = "Failed To Insert Record. " + ex.InnerException?.Message
                };
            }

        }
        public async Task<MedicineResponse> UpdateMedicine(MedicineRequest request)
        {
            if (request == null)
                ArgumentNullException.ThrowIfNull(request);

            try
            {
                var userId = GetUserId();

                var entity = await _context.Medicines
                    .FirstOrDefaultAsync(x => x.MedicineId == request.MedicineId && !x.IsDeleted);

                if (entity == null)
                {
                    return new MedicineResponse
                    {
                        Success = false,
                        Messege = "Medicine not found."
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

                TrackChange("MedicineName", entity.MedicineName, request.MedicineName,
                    () => entity.MedicineName = request.MedicineName);

                TrackChange("GenericName", entity.GenericName, request.GenericName,
                    () => entity.GenericName = request.GenericName);

                TrackChange("Category", entity.Category, request.Category,
                    () => entity.Category = request.Category);

                TrackChange("Strength", entity.Strength, request.Strength,
                    () => entity.Strength = request.Strength);

                TrackChange("Manufacturer", entity.Manufacturer, request.Manufacturer,
                    () => entity.Manufacturer = request.Manufacturer);

                TrackChange("IsPrescriptionRequired", entity.IsPrescriptionRequired, request.IsPrescriptionRequired,
                    () => entity.IsPrescriptionRequired = request.IsPrescriptionRequired);

                TrackChange("MinimumStockLevel", entity.MinimumStockLevel, request.MinimumStockLevel,
                    () => entity.MinimumStockLevel = request.MinimumStockLevel);

                TrackChange("HsnCode", entity.HsnCode, request.HsnCode,
                    () => entity.HsnCode = request.HsnCode);

                TrackChange("GstPercentage", entity.GstPercentage, request.GstPercentage,
                    () => entity.GstPercentage = request.GstPercentage);

                if (!delta.Any())
                {
                    return new MedicineResponse
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
                        Action = "Update Medicine",
                        ActionType = (short)LogActionType.Update,
                        ModuleName = "Medicine Management",
                        TableName = "Medicines",
                        RecordId = entity.MedicineId,
                        OldValue = oldValue,
                        NewValue = newValue,
                        ChangedFields = string.Join(",", delta.Keys),
                        Delta = System.Text.Json.JsonSerializer.Serialize(delta),
                        Notes = "Only changed fields updated"
                    });
                }

                return new MedicineResponse
                {
                    Success = true,
                    Messege = "Medicine Updated Successfully."
                };
            }
            catch
            {
                return new MedicineResponse
                {
                    Success = false,
                    Messege = "Failed To Update Record."
                };
            }
        }
        public async Task<MedicineResponse> DeleteMedicine(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new MedicineResponse
                {
                    Success = false,
                    Messege = "Medicine id is empty."
                };
            }

            try
            {
                var userId = GetUserId();

                var entity = await _context.Medicines
                    .FirstOrDefaultAsync(x => x.MedicineId == id && !x.IsDeleted);

                if (entity == null)
                {
                    return new MedicineResponse
                    {
                        Success = false,
                        Messege = "Medicine not found."
                    };
                }

                var oldValue = new { entity.IsDeleted };

                entity.IsDeleted = true;
                entity.DeletedAt = DateTime.UtcNow;
                entity.DeletedBy = userId;

                var result = await _context.SaveChangesAsync(CancellationToken.None);

                if (result > 0)
                {
                    var delta = new
                    {
                        IsDeleted = new { Old = oldValue.IsDeleted, New = entity.IsDeleted }
                    };

                    await _logService.AddLogAsync(new LogRequest
                    {
                        Action = "Soft Delete Medicine",
                        ActionType = (short)LogActionType.Delete,
                        ModuleName = "Medicine Management",
                        TableName = "Medicines",
                        RecordId = id,
                        OldValue = oldValue,
                        NewValue = new { entity.IsDeleted },
                        ChangedFields = "IsDeleted",
                        Delta = JsonSerializer.Serialize(delta),
                        Notes = "Medicine marked as deleted"
                    });
                }

                return new MedicineResponse
                {
                    Success = true,
                    Messege = "Medicine Deleted Successfully."
                };
            }
            catch
            {
                return new MedicineResponse
                {
                    Success = false,
                    Messege = "Failed To Delete Record."
                };
            }
        }
        public async Task<MedicineResponse> ToggleActiveMedicine(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new MedicineResponse
                {
                    Success = false,
                    Messege = "Medicine id is empty."
                };
            }

            try
            {
                var userId = GetUserId();

                var entity = await _context.Medicines
                    .FirstOrDefaultAsync(x => x.MedicineId == id && !x.IsDeleted);

                if (entity == null)
                {
                    return new MedicineResponse
                    {
                        Success = false,
                        Messege = "Medicine not found."
                    };
                }

                var oldValue = new { entity.IsActive };

                entity.IsActive = !entity.IsActive;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = userId;

                var result = await _context.SaveChangesAsync(CancellationToken.None);

                if (result > 0)
                {
                    var delta = new
                    {
                        IsActive = new { Old = oldValue.IsActive, New = entity.IsActive }
                    };

                    await _logService.AddLogAsync(new LogRequest
                    {
                        Action = "Toggle Medicine Status",
                        ActionType = (short)LogActionType.Update,
                        ModuleName = "Medicine Management",
                        TableName = "Medicines",
                        RecordId = id,
                        OldValue = oldValue,
                        NewValue = new { entity.IsActive },
                        ChangedFields = "IsActive",
                        Delta = JsonSerializer.Serialize(delta),
                        Notes = "Medicine active status toggled"
                    });
                }

                return new MedicineResponse
                {
                    Success = true,
                    Messege = "Medicine Status Updated Successfully."
                };
            }
            catch
            {
                return new MedicineResponse
                {
                    Success = false,
                    Messege = "Failed To Update Record."
                };
            }
        }


        public async Task<MedicineListResponse> GetMedicinesAsync(int page, int size)
        {
            try
            {
                var query = _context.Medicines.AsQueryable();

                var totalMedicines = await query.CountAsync();
                var activeMedicines = await query.CountAsync(x => x.IsActive && !x.IsDeleted);
                var inactiveMedicines = await query.CountAsync(x => !x.IsActive && !x.IsDeleted);
                var suspendedMedicines = await query.CountAsync(x => x.IsDeleted);

                if (size > 50)
                {
                    size = 10;
                }

                var medicines = await query
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.MedicineId)
                    .Skip((page - 1) * size)
                    .Take(size)
                    .Select(x => new MedicineRequest
                    {
                        MedicineId = x.MedicineId,
                        MedicineName = x.MedicineName,
                        GenericName = x.GenericName,
                        Category = x.Category,
                        Strength = x.Strength,
                        Manufacturer = x.Manufacturer,
                        IsPrescriptionRequired = x.IsPrescriptionRequired,
                        MinimumStockLevel = x.MinimumStockLevel,
                        HsnCode = x.HsnCode,
                        GstPercentage = x.GstPercentage,
                        IsActive = x.IsActive,
                        CreatedAt = x.CreatedAt
                    })
                    .ToListAsync();

                return new MedicineListResponse
                {
                    TotalMedicines = totalMedicines,
                    ActiveMedicines = activeMedicines,
                    InactiveMedicines = inactiveMedicines,
                    SuspendedMedicines = suspendedMedicines,
                    Records = medicines,
                    Success = true,
                    Messege = "Medicines fetched successfully."
                };
            }
            catch (Exception ex)
            {

                return new MedicineListResponse
                {
                    Success = false,
                    Messege = "Something went wrong while fetching medicines.",
                    Records = new List<MedicineRequest>()
                };
            }
        }

        public async Task<SingleMedicineListResponse> GetMedicineByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new SingleMedicineListResponse
                {
                    Success = false,
                    Messege = "Medicine id is empty."
                };
            }

            try
            {
                var entity = await _context.Medicines
    .FirstOrDefaultAsync(x => x.MedicineId == id);

                if (entity == null)
                {
                    return new SingleMedicineListResponse
                    {
                        Success = false,
                        Messege = "Medicine not found."
                    };
                }

                var dto = new MedicineRequest
                {
                    MedicineId = entity.MedicineId,
                    MedicineName = entity.MedicineName,
                    GenericName = entity.GenericName,
                    Category = entity.Category,
                    Strength = entity.Strength,
                    Manufacturer = entity.Manufacturer,
                    IsPrescriptionRequired = entity.IsPrescriptionRequired,
                    MinimumStockLevel = entity.MinimumStockLevel,
                    HsnCode = entity.HsnCode,
                    GstPercentage = entity.GstPercentage,
                    IsActive = entity.IsActive,
                    CreatedAt = entity.CreatedAt,
                    UpdatedAt = entity.UpdatedAt,
                    IsDeleted = entity.IsDeleted,
                    DeletedAt = entity.DeletedAt,
                    DeletedBy = entity.DeletedBy,
                    CreatedBy = entity.CreatedBy,
                    UpdatedBy = entity.UpdatedBy
                };

                return new SingleMedicineListResponse
                {
                    Success = true,
                    Messege = "Medicine fetched successfully.",
                    Records = dto
                };
            }
            catch (Exception ex)
            {
                return new SingleMedicineListResponse
                {
                    Success = false,
                    Messege = ex.Message 
                };
            }
        }
    }
}