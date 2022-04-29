using System.Threading.Tasks;
using Triton.Model.CRM.Tables;
using Triton.Model.FWWebservice.Custom;

namespace Triton.Interface.Collection
{
    public interface ICollectionRequestAPI
    {
        Task<FWResponsePacket> PostCollectionRequestUAT(CollectionRequests collectionRequests);
        Task<FWResponsePacket> PostCollectionRequestProduction(CollectionRequests collectionRequests);
    }
}
