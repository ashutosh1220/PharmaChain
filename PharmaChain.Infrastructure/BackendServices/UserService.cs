using Microsoft.EntityFrameworkCore;
using PharmaChain.Application.Common.Enums;
using PharmaChain.Application.DTOs;
using PharmaChain.Application.Interfaces;
using PharmaChain.Infrastructure.Models;
using System.Text.Json;

namespace PharmaChain.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly PharmaChainDbContext _context;
        private readonly ILogService _logService;

        public UserService(PharmaChainDbContext context, ILogService logService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logService = logService;
        }

        public async Task<UsersListResponse> GetUsersAsync(int page, int size)
        {
            var query = _context.Users.AsQueryable();

            var totalUsers = await query.CountAsync();
            var activeUsers = await query.CountAsync(x => x.IsActive);
            var inactiveUsers = await query.CountAsync(x => !x.IsActive);
            var suspendedUsers = await query.CountAsync(x => x.IsDeleted);

            if(size > 50)
            {
                size = 10;
            }

            var users = await query
                .OrderBy(x => x.UserId)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(x => new UserRequest
                {
                    UserId = x.UserId,
                    FullName = x.FullName,
                    Phone = x.Phone,
                    Email = x.Email,
                    Role = x.Role.RoleName,
                    Branch = x.BranchId,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return new UsersListResponse
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                InactiveUsers = inactiveUsers,
                SuspendedUsers = suspendedUsers,
                Users = users,
                TotalPages = (int)Math.Ceiling(totalUsers / (double)size),
                CurrentPage = page
            };
        }

        public async Task<bool> ToggleUserActiveAsync(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                    throw new KeyNotFoundException("User not found");

                var oldValue = new { user.IsActive };

                user.IsActive = !user.IsActive;

                var delta = new
                {
                    IsActive = new
                    {
                        Old = oldValue.IsActive,
                        New = user.IsActive
                    }
                };

                var result = await _context.SaveChangesAsync(CancellationToken.None);

                if (result > 0)
                {
                    await _logService.AddLogAsync(new LogRequest
                    {
                        Action = "Toggle User Active",
                        ActionType = (short)LogActionType.Update,
                        ModuleName = "User Management",
                        TableName = "Users",
                        RecordId = userId,
                        OldValue = oldValue,
                        NewValue = new { user.IsActive },
                        ChangedFields = "IsActive",
                        Delta = JsonSerializer.Serialize(delta),
                        Notes = $"User active changed from {oldValue.IsActive} to {user.IsActive}"
                    });

                    return true;
                }

                return false;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while toggling user active status", ex);
            }
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                    throw new KeyNotFoundException("User not found");

                var oldValue = new { user.IsDeleted };
                user.IsDeleted = true;

                var delta = new
                {
                    IsDeleted = new
                    {
                        Old = oldValue.IsDeleted,
                        New = user.IsDeleted
                    }
                };

                var result = await _context.SaveChangesAsync(CancellationToken.None);

                if (result > 0)
                {
                    await _logService.AddLogAsync(new LogRequest
                    {
                        Action = "Soft Delete User",
                        ActionType = (short)LogActionType.Delete,
                        ModuleName = "User Management",
                        TableName = "Users",
                        RecordId = userId,
                        OldValue = oldValue,
                        NewValue = new { user.IsDeleted },
                        ChangedFields = "IsDeleted",
                        Delta = JsonSerializer.Serialize(delta),
                        Notes = "User marked as deleted"
                    });

                    return true;
                }

                return false;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while deleting user", ex);
            }
        }

        public async Task<UpdateUserResponse> UpdateUserInfoAsync(string id, string columnName, string updatedValue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(columnName) || string.IsNullOrWhiteSpace(updatedValue))
                {
                    return new UpdateUserResponse
                    {
                        Success = false,
                        Message = "All fields are required"
                    };
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return new UpdateUserResponse
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                var property = typeof(User).GetProperty(columnName);
                if (property == null)
                {
                    return new UpdateUserResponse
                    {
                        Success = false,
                        Message = $"Column '{columnName}' does not exist."
                    };
                }

                var oldValue = property.GetValue(user);

                var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                var safeValue = Convert.ChangeType(updatedValue, targetType);
                property.SetValue(user, safeValue);

                // Save changes
                var result = await _context.SaveChangesAsync(CancellationToken.None);

                if (result > 0)
                {
                    // Prepare delta and log
                    var delta = new
                    {
                        UpdatedField = columnName,
                        Old = oldValue,
                        New = safeValue
                    };

                    await _logService.AddLogAsync(new LogRequest
                    {
                        Action = "Update User Info",
                        ActionType = (short)LogActionType.Update,
                        ModuleName = "User Management",
                        TableName = "Users",
                        RecordId = id,
                        OldValue = oldValue,
                        NewValue = safeValue,
                        ChangedFields = columnName,
                        Delta = JsonSerializer.Serialize(delta),
                        Notes = $"Updated {columnName} for user {id}"
                    });
                }

                return new UpdateUserResponse
                {
                    Success = true,
                    Message = "User info updated successfully.",
                    UpdatedValue = updatedValue
                };
            }
            catch (Exception ex)
            {
                return new UpdateUserResponse
                {
                    Success = false,
                    Message = $"An error occurred while updating user info: {ex.Message}"
                };
            }
        }
    }
}