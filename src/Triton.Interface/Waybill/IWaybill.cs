using System.Collections.Generic;
using System.Threading.Tasks;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.StoredProcs;
using Triton.Model.CRM.Tables;
using Triton.Model.CRM.Views;

namespace Triton.Interface.Waybill
{
    public interface IWaybill
    {
        Task<Waybills> GetWaybillById(long waybillId);
        Task<Waybills> GetWaybillByWaybillNo(string WaybillNo,string dbName="CRM");

         Task<bool> StoreWaybillLines(dynamic waybill,long waybillID,string dbName="CRM");
         Task<List<vwOpsWaybills>> GetWaybillViewList(string customerIds, string waybillNo, string customerXRef, int? waybillId);
         Task<List<vwOpsWaybills>> GetWaybillsByWaybillNo(string WaybillNo);
         Task<WayBillInfoModel> GetWaybillInfoById(long waybillId);
         Task<InternalWaybills> GetInternalWaybill(long internalWaybillID, string dbName = "CRM");
         Task<InternalWaybills> GetInternalWaybillByReference(string waybillNo, string dbName = "CRM");
         Task<long> PostInternalWaybill(InternalWaybills internalWaybill,string dbName="CRM");
         Task<bool> PutInternalWaybill(InternalWaybills internalWaybill,string dbName="CRM");
         Task<long> PostWaybillFromFWWS(WaybillCustomerModel Waybill,string dbName="CRM");

         Task<CreditNotePageModel> GetCreditNotePage(string waybillNo);
    }
}
