using PharmaChain.Application.DTOs;
using PharmaChain.Infrastructure.Models;

namespace PharmaChain.Application.Interfaces
{
    public interface IJwtService
    {
        public string GenerateToken(LoginResponse user);
    }
}
