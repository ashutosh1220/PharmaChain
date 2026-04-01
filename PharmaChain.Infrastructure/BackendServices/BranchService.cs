using PharmaChain.Application.Interfaces;
using PharmaChain.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace PharmaChain.Infrastructure.BackendServices
{
    public class BranchService : IBranchService
    {
        private readonly IPharmaChainDbContext _context;
        public BranchService(IPharmaChainDbContext context) 
        { 
             _context = context;
        }

        public async Task<List<BranchResponse>> GetAllBranchesAsync()
        {
            try
            {
                var records = await _context.Branches.ToListAsync();

                var result = records.Select(b => new BranchResponse
                {
                    BranchId = b.BranchId,
                    BranchName = b.BranchName,
                    AddressLine1 = b.AddressLine1,
                    City = b.City,
                    IsDeleted = b.IsDeleted,
                    IsActive = b.IsActive
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                return new List<BranchResponse>();
            }
        }
    }
}
