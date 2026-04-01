using Microsoft.AspNetCore.Http;
using PharmaChain.Application.Interfaces;
using PharmaChain.Infrastructure.Models;
using PharmaChain.Application.Common.Enums;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace PharmaChain.Infrastructure.BackendServices
{
    public class LogService : ILogService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IPharmaChainDbContext _context;
        public LogService(IPharmaChainDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }
        public async Task AddLogAsync(LogRequest request)
        {
            try
            {
                var http = _httpContext?.HttpContext;

                var userId = http?.User?.FindFirst("UserId")?.Value;

                var sessionId = http?.Session?.IsAvailable == true ? http.Session.Id : null;

                var log = new Log
                {
                    UserId = userId,
                    Action = request.Action,
                    ActionType = request.ActionType,
                    ModuleName = request.ModuleName,
                    TableName = request.TableName,
                    RecordId = request.RecordId,
                    OldValue = request.OldValue != null ? JsonSerializer.Serialize(request.OldValue) : null,
                    NewValue = request.NewValue != null ? JsonSerializer.Serialize(request.NewValue) : null,
                    ChangedFields = request.ChangedFields,
                    IpAddress = http?.Connection?.RemoteIpAddress?.ToString(),
                    DeviceInfo = http?.Request?.Headers["User-Agent"].ToString(),
                    SessionId = sessionId,
                    Severity = 1,
                    Status = "Success",
                    Notes = request.Notes,
                    Delta = request.Delta,
                    CreatedAt = DateTime.UtcNow,
                    ProcessedAt = DateTime.UtcNow
                };

                _context.Logs.Add(log);
                await _context.SaveChangesAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Log error: {ex.Message}");
            }
        }

        public async Task<LogsResponse> GetLogsAsync(int page, int size)
        {
            try
            {
                if (page <= 0)
                    throw new ArgumentException("Page must be greater than 0.");

                if (size <= 0)
                    throw new ArgumentException("Size must be greater than 0.");

                if (size > 50)
                    size = 10;

                var totalLogs = await _context.Logs.CountAsync();

                var logsData = await _context.Logs
                    .AsNoTracking()
                    .OrderByDescending(l => l.CreatedAt)
                    .Skip((page - 1) * size)
                    .Take(size)
                    .Select(l => new
                    {
                        l.LogId,
                        l.UserId,
                        l.BranchId,
                        l.Action,
                        l.ActionType,
                        l.ModuleName,
                        l.TableName,
                        l.RecordId,
                        l.RelatedRecordId,
                        l.OldValue,
                        l.NewValue,
                        l.ChangedFields,
                        l.Delta,
                        l.IpAddress,
                        l.DeviceInfo,
                        l.SessionId,
                        l.Severity,
                        l.Status,
                        l.Notes,
                        l.CreatedAt,
                        l.ProcessedAt
                    })
                    .ToListAsync();

                var logs = logsData.Select(l => new LogsInfo
                {
                    LogId = l.LogId,
                    UserId = l.UserId,
                    BranchId = l.BranchId,
                    Action = l.Action,
                    ActionType = l.ActionType,
                    ModuleName = l.ModuleName,
                    TableName = l.TableName,
                    RecordId = l.RecordId,
                    RelatedRecordId = l.RelatedRecordId,
                    OldValue = l.OldValue,
                    NewValue = l.NewValue,
                    ChangedFields = l.ChangedFields,
                    Delta = l.Delta,
                    IpAddress = l.IpAddress,
                    DeviceInfo = l.DeviceInfo,
                    SessionId = l.SessionId,
                    Severity = l.Severity,
                    Status = l.Status,
                    Notes = l.Notes,
                    CreatedAt = l.CreatedAt,
                    ProcessedAt = l.ProcessedAt
                }).ToList();

                return new LogsResponse
                {
                    CurrentPage = page,
                    TotalLogs = totalLogs,
                    TotalPages = (int)Math.Ceiling((double)totalLogs / size),
                    Logs = logs
                };
            }
            catch (ArgumentException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching logs. Please try again.");
            }
        }


        public async Task<LogsResponse> GetLogByLogIdAsync(long LogId)
        {
            try
            {
                var logsData = await _context.Logs
                    .AsNoTracking().Where(l => l.LogId == LogId)
                    .Select(l => new
                    {
                        l.LogId,
                        l.UserId,
                        l.BranchId,
                        l.Action,
                        l.ActionType,
                        l.ModuleName,
                        l.TableName,
                        l.RecordId,
                        l.RelatedRecordId,
                        l.OldValue,
                        l.NewValue,
                        l.ChangedFields,
                        l.Delta,
                        l.IpAddress,
                        l.DeviceInfo,
                        l.SessionId,
                        l.Severity,
                        l.Status,
                        l.Notes,
                        l.CreatedAt,
                        l.ProcessedAt
                    })
                    .ToListAsync();

                var logs = logsData.Select(l => new LogsInfo
                {
                    LogId = l.LogId,
                    UserId = l.UserId,
                    BranchId = l.BranchId,
                    Action = l.Action,
                    ActionType = l.ActionType,
                    ModuleName = l.ModuleName,
                    TableName = l.TableName,
                    RecordId = l.RecordId,
                    RelatedRecordId = l.RelatedRecordId,
                    OldValue = l.OldValue,
                    NewValue = l.NewValue,
                    ChangedFields = l.ChangedFields,
                    Delta = l.Delta,
                    IpAddress = l.IpAddress,
                    DeviceInfo = l.DeviceInfo,
                    SessionId = l.SessionId,
                    Severity = l.Severity,
                    Status = l.Status,
                    Notes = l.Notes,
                    CreatedAt = l.CreatedAt,
                    ProcessedAt = l.ProcessedAt
                }).ToList();

                return new LogsResponse
                {
                    Logs = logs
                };
            }
            catch (ArgumentException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching logs. Please try again.");
            }
        }
    }
}