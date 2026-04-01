using PharmaChain.Application.DTOs;
using PharmaChain.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PharmaChain.Infrastructure.BackendServices
{
    public class PermissionService : IPermissionService
    {
        private readonly IPharmaChainDbContext _context;
        public PermissionService(IPharmaChainDbContext context)
        {
            _context = context;
        }

        public async Task<List<PermissionResponse>> GetAllPermissionsForRolesAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Role name is required.");
            }

            var rolePermissions = await _context.Roles
    .AsNoTracking()
    .Where(r => r.RoleName == roleName)
    .SelectMany(r => r.RolePermission)
    .Select(rp => new PermissionResponse
    {
        PermissionId = rp.Permission.PermissionId,
        PermissionName = rp.Permission.PermissionName,
        Module = rp.Permission.Module,
        IsActive = rp.IsActive
    })
    .ToListAsync();

            return rolePermissions;
        }


        public async Task<string> UpdateRolePermissionsAsync(UpdateRolePermissionRequest request)
        {
            var role = await _context.Roles
        .FirstOrDefaultAsync(r => r.RoleName == request.RoleName && r.IsActive);

            if (role == null)
                throw new Exception("Role not found or inactive.");

            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == role.RoleId)
                .ToListAsync();

            foreach (var rp in rolePermissions)
            {
                if (request.PermissionIds.Contains(rp.PermissionId))
                {
                    rp.IsActive = true;
                }
                else
                {
                    rp.IsActive = false;
                }
            }

            var existingPermissionIds = rolePermissions.Select(rp => rp.PermissionId).ToHashSet();

            var newPermissions = request.PermissionIds
                .Where(pid => !existingPermissionIds.Contains(pid))
                .Select(pid => new RolePermissions
                {
                    RoleId = role.RoleId,
                    PermissionId = pid,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });

            await _context.RolePermissions.AddRangeAsync(newPermissions);

            await _context.SaveChangesAsync(CancellationToken.None);

            return "Permissions updated successfully.";
        }
    }
}

