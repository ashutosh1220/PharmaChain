public class RoleWithPermissionsDto
{
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public string Status => IsActive ? "Active" : "Inactive";
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<string> Permissions { get; set; }
}

public class BaseResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }

    public int TotalRoles { get; set; }
    public int ActiveRoles { get; set; }
    public int InactiveRoles { get; set; }

    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
}

public class RoleDto
{
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class RoleResponse : BaseResponse
{
    public List<RoleDto> Roles { get; set; }
}