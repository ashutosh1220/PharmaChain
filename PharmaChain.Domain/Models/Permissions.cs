public class Permissions
{
    public int PermissionId { get; set; }

    public string PermissionName { get; set; }

    public string? Module { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<RolePermissions> RolePermission { get; set; }
}