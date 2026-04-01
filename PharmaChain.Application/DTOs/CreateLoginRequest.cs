using System.ComponentModel.DataAnnotations;

namespace PharmaChain.Application.DTOs
{
    public class CreateLoginRequest
    {
        public string UserId { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsLocked { get; set; } = false;
    }
}