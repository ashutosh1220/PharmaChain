namespace PharmaChain.Application.Interfaces
{
    public interface IRoleService
    {
        Task<int> AddRoleAsync(string roleName);
        Task<RoleResponse> GetRolesAsync(int? page, int? size);
        Task<int> UpdateRoleStateAsync(int roleId, string roleName);
        Task<List<RoleWithPermissionsDto>> GetRolesWithPermissionsAsync();
        Task<bool> ToggleRoleActiveStateAsync(int roleId, string roleName);
    }
}