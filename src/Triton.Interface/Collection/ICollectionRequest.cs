using System;
using System.Threading.Tasks;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.Tables;
using Triton.Model.FWWebservice.Custom;

namespace Triton.Interface.CRM
{
    public interface ICollectionRequest
    {
     
        Task<CollectionRequests> GetCollectionRequest(int collectionRequestId,string dbName = "CRM");
        Task<CollectionRequestsModel> GetComplex(int collectionRequestId, string dbName = "CRM");
        Task<CollectionRequestsModel> FindCollectionRequest(string customerXRef,  DateTime? StartDate, DateTime? EndDate, string CollectionRequestNumber, int customerId);
        Task<bool> Put(CollectionRequests collectionRequests, string dbName = "CRM");
        Task<long> Post(CollectionRequests collectionRequests, string dbName = "CRM");
        Task<long> PostProduction(CollectionRequests collectionRequests, string dbName = "CRM");
    }
}