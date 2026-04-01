using PharmaChain.Application.DTOs;

namespace PharmaChain.Application.Interfaces
{
    public interface IPermissionService
    {
        Task<List<PermissionResponse>> GetAllPermissionsForRolesAsync(string roleName);
        Task<string> UpdateRolePermissionsAsync(UpdateRolePermissionRequest request);
    }
}