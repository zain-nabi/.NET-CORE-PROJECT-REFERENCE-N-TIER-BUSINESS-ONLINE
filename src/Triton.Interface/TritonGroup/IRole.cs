using System.Collections.Generic;
using System.Threading.Tasks;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Interface.TritonGroup
{
    public interface IRole
    {
        Task<List<Roles>> GetRolesByUserId(int userId, string dbName);
        Task<List<Roles>> GetRolesByIds(string roleIDs, string dbName);
    }
}
