public class UsersListResponse
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int SuspendedUsers { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public List<UserRequest> Users { get; set; } = new();
}