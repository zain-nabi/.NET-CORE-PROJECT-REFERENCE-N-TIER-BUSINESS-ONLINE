using System.Collections.Generic;
using System.Threading.Tasks;
using Triton.Model.TritonGroup.Custom;
using Triton.Model.TritonGroup.StoredProcs;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Interface.TritonGroup
{
    public interface IUser
    {
        Task<Users> FindByNameAsync(string username);
        Task<Users> CreateAsync(Users user);
        Task<long> Post(Users user);
        Task<bool> PutUpdateAsync(Users user);
        Task<Users> FindByIdAsync(int userId);
        Task<List<Users>> GetAllUsersInclLockedOut();
        Task<UserWithRoles> GetUserWithRoles(int userId, string roleIds);
        Task<UserInformation> FindBysAmAccountName(string sAmAccountName, string database);
    }
}
