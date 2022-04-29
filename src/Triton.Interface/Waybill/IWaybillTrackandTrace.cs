using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.Tables;

namespace Triton.Interface.Waybill
{
    public interface IWaybillTrackandTrace
    {
        Task<List<WaybillTrackandTraces>> GetWaybillTrackandTraceby_WaybillID(Int64 waybillId);

        Task<List<CustomerWaybillTrackandTrace>> GetCustomerWaybillTrackandTraceby_WaybillID(long waybillId);
        Task<List<CustomerWaybillTrackandTrace>> GetCustomerWaybillTrackandTraceby_WaybillNo(string waybillNo);

        Task<List<CustomerWaybillTrackandTrace>> GetCustomerWaybillTrackandTraceby_Reference(string reference);
        //Task<bool> UpdateCrossRef1(long waybillTrackandTraceId, string reference);
    }
}
