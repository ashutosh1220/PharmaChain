using Microsoft.EntityFrameworkCore;
using PharmaChain.Application.DTOs;
using PharmaChain.Application.Interfaces;

namespace PharmaChain.Infrastructure.BackendServices
{
    public class AuthService : IAuthService
    {
        private readonly IPharmaChainDbContext _context;

        public AuthService(IPharmaChainDbContext context)
        {
            _context = context;
        }

        public async Task<LoginResponse?> ValidateUserAsync(LoginsRequest request)
        {
            try
            {
                var user = await (
                    from l in _context.Logins
                    join u in _context.Users on l.UserId equals u.UserId
                    join r in _context.Roles on u.RoleId equals r.RoleId
                    where l.Username == request.Username
                        && l.PasswordHash == request.Password
                        && !l.IsLocked
                        && !u.IsDeleted
                    select new LoginResponse
                    {
                        UserId = l.UserId,
                        UserName = l.Username,
                        FullName = u.FullName,
                        Branch = u.BranchId,
                        Role = r.RoleName
                    }
                ).FirstOrDefaultAsync();

                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}