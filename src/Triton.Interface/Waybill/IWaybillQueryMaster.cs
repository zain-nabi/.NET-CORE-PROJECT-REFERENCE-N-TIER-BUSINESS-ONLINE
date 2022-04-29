using System.Collections.Generic;
using System.Threading.Tasks;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.Tables;

namespace Triton.Interface.Waybill
{
    public interface IWaybillQueryMaster
    {
        Task<WaybillQueryMaster> Get(int waybillQueryMasterId);
        Task<List<WaybillQueryMaster_Waybills_Model>> GetWaybillQueryMaster(int userId, string queryStatusLcid, int? systemId = 0);
        Task<List<WaybillQuery>> GetWaybillQueryList(long waybillId);
        Task<bool> PutWaybillQueryMaster(WaybillQueryMaster waybillQueryMaster);
        Task<bool> PostWaybillQueryMaster(WaybillQueryMasterInsertModel model);
        Task<bool> Delete(long waybillId, int userId);
    }
}
