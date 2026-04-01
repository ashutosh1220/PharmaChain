using Microsoft.EntityFrameworkCore;
using PharmaChain.Application.Interfaces;
using PharmaChain.Infrastructure.Models;
using PharmaChain.Application.DTOs;
using System.Data;

namespace PharmaChain.Infrastructure.BackendServices
{
    public class RoleService(PharmaChainDbContext context) : IRoleService
    {
        private readonly PharmaChainDbContext _context = context;

        public async Task<int> AddRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Role name cannot be empty");

            try
            {
                var role = new Roles
                {
                    RoleName = roleName.Trim(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Roles.Add(role);
                await _context.SaveChangesAsync();

                return role.RoleId;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Database error while saving role. Possible duplicate RoleId.", ex);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error occurred while adding role.", ex);
            }
        }

        public async Task<List<RoleWithPermissionsDto>> GetRolesWithPermissionsAsync()
        {
            try
            {
                var data = await _context.Roles
                    .Include(r => r.RolePermission)
                    .ThenInclude(rp => rp.Permission)
                    .Select(r => new RoleWithPermissionsDto
                    {
                        RoleId = r.RoleId,
                        RoleName = r.RoleName,
                        IsActive = r.IsActive,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt,

                        Permissions = r.RolePermission != null
                            ? r.RolePermission
                                .Where(rp => rp.Permission != null)
                                .Select(rp => rp.Permission.PermissionName)
                                .ToList()
                            : new List<string>()
                    })
                    .ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to fetch roles with permissions.", ex);
            }
        }

        public async Task<int> UpdateRoleStateAsync(int roleId, string roleName)
        {
            try
            {
                var role = await _context.Roles.FindAsync(roleId);

                if (role == null)
                {
                    throw new KeyNotFoundException($"Role with ID {roleId} not found.");
                }

                role.RoleName = roleName;
                role.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return roleId;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update role.", ex);
            }
        }


        public async Task<bool> ToggleRoleActiveStateAsync(int roleId, string roleName)
        {
            try
            {
                var role = await _context.Roles.FindAsync(roleId);

                if (role == null)
                    throw new KeyNotFoundException($"Role with ID {roleId} not found.");

                if (!string.Equals(role.RoleName, roleName, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Role name does not match.");

                role.IsActive = !role.IsActive;
                role.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return role.IsActive;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to toggle role state.", ex);
            }
        }

        public async Task<RoleResponse> GetRolesAsync(int? page, int? size)
        {
            try
            {
                
                int currentPage = page.HasValue && page.Value > 0 ? page.Value : 1;
                int pageSize = size.HasValue && size.Value > 0 ? size.Value : 5;

                if (pageSize > 50)
                    pageSize = 10;

                var query = _context.Roles.AsQueryable();

                var totalRoles = await query.CountAsync();
                var activeRoles = await query.CountAsync(x => x.IsActive);
                var inactiveRoles = totalRoles - activeRoles;

                var roles = await query
                    .OrderBy(x => x.RoleId)
                    .Skip((currentPage - 1) * pageSize)  
                    .Take(pageSize)                     
                    .Select(x => new RoleDto
                    {
                        RoleId = x.RoleId,
                        RoleName = x.RoleName,
                        IsActive = x.IsActive,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    })
                    .ToListAsync();

                return new RoleResponse
                {
                    Success = true,
                    Message = "Roles fetched successfully",
                    TotalRoles = totalRoles,
                    ActiveRoles = activeRoles,
                    InactiveRoles = inactiveRoles,
                    TotalPages = (int)Math.Ceiling(totalRoles / (double)pageSize),
                    CurrentPage = currentPage,
                    Roles = roles
                };
            }
            catch (Exception)
            {
                return new RoleResponse
                {
                    Success = false,
                    Message = "Failed to fetch roles"
                };
            }
        }
    }
}