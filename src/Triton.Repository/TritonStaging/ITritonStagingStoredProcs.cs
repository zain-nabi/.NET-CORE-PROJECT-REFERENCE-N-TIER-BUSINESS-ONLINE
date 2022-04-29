using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triton.Model.TritonStaging.StoredProcs;

namespace Triton.Repository.TritonStaging
{
    public interface ITritonStagingStoredProcs
    {
        Task<proc_FMOEndorsements_Signature_Select> GetPODSignature(long waybillId);
        //List<proc_GetWaybillsForRWSTracking> GetWaybillsForRWSTracking();
        //List<proc_GetWaybillsforRWSPodInfo> GetWaybillsforRWSPodInfo();
        //List<proc_GetRWSWaybillsForPODImagePopulation> GetRWSWaybillsForPODImagePopulation();
        //List<proc_GetWaybillsForRWSCreation> GetWaybillsForRWSCreation();
        //bool UpdateWaybillPODInfo(long WaybillID, long WaybillTrackandTraceID, string Signee, DateTime SignedAt);
        //bool SubmitRWSTracking(List<TVP_RWSStatusTrack> model);

        //bool SubmitPulsTrackTracking(List<TVP_PulstrackTracking> model);
    }
}
