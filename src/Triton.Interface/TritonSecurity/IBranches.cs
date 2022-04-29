using System.Collections.Generic;
using System.Threading.Tasks;
using Triton.Model.TritonSecurity.Tables;

namespace Triton.Interface.TritonSecurity
{
    public interface IBranches
    {
        Task<List<Branches>> GetAllActiveBranches();

        Task<List<Branches>> GetBranchesByBranchNameorFWDepotCode(string BranchSearch);

        Task<Branches> GetUserBranch(int userId);

        Task<Branches> GetQuestionnnaireBranch(int userId);
    }
}
