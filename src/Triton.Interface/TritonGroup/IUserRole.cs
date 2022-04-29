using System.Threading.Tasks;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Interface.TritonGroup
{
    public interface IUserRole
    {
        Task<UserRoles> GetUserRole(int userRoleId);
        Task<long> Post(UserRoles userRoles, string dbName);
        Task<bool> Put(UserRoles userRoles, string dbName);
    }
}