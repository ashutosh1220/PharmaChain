public class UpdateRolePermissionRequest
{
    public required string RoleName { get; set; }
    public required List<int> PermissionIds { get; set; }
}