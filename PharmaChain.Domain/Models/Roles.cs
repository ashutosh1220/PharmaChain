using PharmaChain.Infrastructure.Models;

public class Roles
{
    public int RoleId { get; set; }

    public string RoleName { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<RolePermissions> RolePermission { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
}