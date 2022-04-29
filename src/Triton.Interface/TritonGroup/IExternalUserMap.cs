using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triton.Model.TritonGroup.Custom;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Interface.TritonGroup
{
    public interface IExternalUserMap
    {
        Task<List<ExternalUser>> GetUserMapByUserType(int userTypeLcid);
        Task<ExternalUserMap> GetUserMap(int userMapId);
        Task<ExternalUserMapCustomerModels> GetUserCustomerMapModel(int userId);
        Task<string> PostUserMapObject(ExternalUserMap userMap);
        Task<bool> PutUserMap(ExternalUserMap userMap);
        Task<List<ExternalUserMapModel>> GetUserMapCustomers(int externalUserID);
        Task<bool> UpdateExternalUserMap(ExternalUserMapModel list);
        Task<string> InsertExternalUserMap(ExternalUserMapModel list);
    }
}
