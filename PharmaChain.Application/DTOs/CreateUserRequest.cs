using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PharmaChain.Application.DTOs
{
    public class CreateUserRequest
    {
        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        public string Phone { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        public string? Gender { get; set; }

        public string AddressLine1 { get; set; } = null!;
        public string? AddressLine2 { get; set; }
        public string Country { get; set; } = null!;
        public string State { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Pincode { get; set; } = null!;

        public string BranchId { get; set; } = null!;
        public int RoleId { get; set; }
        [Required]
        public string Password { get; set; } = null!;

        [Compare("Password")]
        public string ConfirmPassword { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string IdProofType { get; set; } = null!;
        public string IdProofNumber { get; set; } = null!;

        public IFormFile? IdProofDoc { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }
}