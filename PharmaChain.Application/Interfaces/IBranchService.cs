using PharmaChain.Application.DTOs;

namespace PharmaChain.Application.Interfaces
{
    public interface IBranchService
    {
        Task<List<BranchResponse>> GetAllBranchesAsync();
        //Task<BranchDto> GetBranchByIdAsync(int branchId);
        //Task<int> CreateBranchAsync(BranchDto branchDto);
        //Task<bool> UpdateBranchAsync(BranchDto branchDto);
        //Task<bool> DeleteBranchAsync(int branchId);
    }
}
