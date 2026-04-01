using PharmaChain.Application.DTOs;

namespace PharmaChain.Application.Interfaces
{
    public interface IUserService
    {
        Task<UsersListResponse> GetUsersAsync(int page, int size);
        Task<bool> ToggleUserActiveAsync(string userId);
        Task<bool> DeleteUserAsync(string userId);
        Task<UpdateUserResponse> UpdateUserInfoAsync(string id, string columnName, string updatedValue);
    }
}