using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.Tables;

namespace Triton.Interface.Collection
{
    public interface ICollectionRequestTrackAndTraces
    {
        Task<CollectionRequestTrackAndTraces> GetCollectionRequestTrackAndTraces(int collectionRequestTrackAndTraceId, string dbname = "CRM");
        Task<CollectionRequestTrackandTracesModel> GetComplex(int collectionRequestTrackAndTraceId, string dbname = "CRM");
        Task<List<CollectionRequestTrackandTracesModel>> FindCollectionRequest(int collectionRequestId, string dbName="CRM");
        Task<long> Post(CollectionRequestTrackAndTraces collectionRequestTrackAndTraces, string dbname = "CRM");
        Task<bool> Put(CollectionRequestTrackAndTraces collectionRequestTrackAndTraces, string dbname = "CRM");
        
    }
}
