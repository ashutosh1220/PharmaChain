namespace PharmaChain.Application.DTOs
{
    public class LoginResponse
    {
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Branch { get; set; } = null!;
    }
}
