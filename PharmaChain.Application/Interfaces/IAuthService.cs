using PharmaChain.Application.DTOs;

public interface IAuthService
{
    Task<LoginResponse?> ValidateUserAsync(LoginsRequest request);
}