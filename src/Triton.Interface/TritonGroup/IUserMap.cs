using System.Collections.Generic;
using System.Threading.Tasks;
using Triton.Model.TritonGroup.Custom;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Interface.TritonGroup
{
    public interface IUserMap
    {
        Task<List<Users>> GetUserMapByUserType(int userTypeLcid);
        Task<UserMap> GetUserMap(int userMapId);
        Task<UserMapCustomerModels> GetUserCustomerMapModel(int userId);
        Task<string> PostUserMapObject(UserMap userMap);
        Task<bool> PutUserMap(UserMap userMap);
    }
}
