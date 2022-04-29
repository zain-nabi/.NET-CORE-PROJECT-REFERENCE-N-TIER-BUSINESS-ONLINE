using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  Triton.Model.CRM.Tables;
using  Triton.Model.TritonOps.StoredProcs;

namespace  Triton.Model.CRM.Custom
{
    public class CustomerWaybillModel
    {
        public proc_CSA_Waybills_Select ProcCsaWaybillsSelect { get; set; }
        public List<CustomerWaybillTrackandTrace> CustomerWaybillTrackandTrace { get; set; }
        public Waybills Waybills { get; set; }
        public WaybillTrackandTraces WaybillTrackandTraces { get; set; }
        public FWEventCodes FwEventCodes { get; set; }
        public TritonSecurity.Tables.Branches Branches { get; set; }
        public CustomerWaybillTrackAndTraceModel CustomerWaybillTrackAndTraceModel { get; set; }


    }
}
