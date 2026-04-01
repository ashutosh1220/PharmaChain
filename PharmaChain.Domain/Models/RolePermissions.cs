using System.Data;
using System.Security;

public class RolePermissions
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsActive { get; set; }
    public Roles Role { get; set; }
    public Permissions Permission { get; set; }
}