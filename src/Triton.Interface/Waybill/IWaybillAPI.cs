using System.Threading.Tasks;
using Triton.Model.CRM.Custom;
using Triton.Model.FWWebservice.Custom;
using Triton.Model.TritonGroup.Tables;
using Triton.Model.TritonStaging.StoredProcs;

namespace Triton.Interface.Waybill
{
    public interface IWaybillAPI
    {
        //Task<DocumentRepositories> GetEWaybillasPDFUAT(string waybillNo);
        Task<FWResponsePacket> GetFWWSWaybillUAT(string waybillNo);
        Task<FWResponsePacket> GetFWWSWaybill(string waybillNo);
        Task<bool> PutWaybillDeliveredPODUAT(string waybillNo);
        Task<bool> PutWaybillDeliveredPODLive(string waybillNo);
        Task<TritonWaybillSubmitModels> GetTritonSubmitModelByWaybillID(long waybillId, string dbName = "CRM");
        Task<TritonWaybillSubmitModels> GetTritonSubmitModelByWaybillNo(string waybillNo, string dbName = "CRM");
        Task<DocumentRepositories> GetEWaybillasPDFUAT(string waybillNo);
        Task<DocumentRepositories> GetEWaybillasPDF(string waybillNo);
        Task<DocumentRepositories> GetStickersForWaybillasPDF(string waybillNo,bool test=false);
        Task<DocumentRepositories> GetPODStickersForWaybillasPDF(string waybillNo,int pages=1);
        Task<byte[]> GetWaybillPODImage(string waybillNo,string nodeName);
        Task<proc_FMOEndorsements_Signature_Select> GetWaybillSignature(string waybillNo);
        Task<FWResponsePacket> PostInternalWaybill(TritonWaybillSubmitModels model,string dbName="CRM");
        //Task<FWResponsePacket> PutInternalWaybill(InternalWaybills waybill,string dbName="CRM");
        Task<FWResponsePacket> PostInternalWaybillUAT(TritonWaybillSubmitModels model,string dbName="CRM");
        //Task<FWResponsePacket> PutInternalWaybillUAT(InternalWaybills waybill,string dbName="CRM");
    }
}
