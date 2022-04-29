using System.Collections.Generic;
using System.Threading.Tasks;
using Triton.Model.TritonGroup.Custom;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Interface.TritonGroup
{
    public interface IUserCustomerMap
    {
        Task<List<UserCustomerMap>> GetUserCustomerMapByUserId(int userId);
    }
}
