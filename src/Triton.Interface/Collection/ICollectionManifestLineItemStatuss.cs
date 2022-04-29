using System.Threading.Tasks;
using Triton.Model.CRM.Tables;

namespace Triton.Interface.Collection
{
    public interface ICollectionManifestLineItemStatuss
    {
        Task<CollectionManifestLineItemStatuss> GetCollectionManifestLineItemStatuss();
        Task<CollectionManifestLineItemStatuss> GetCollectionManifestLineItemStatusById(int CollectionManifestLineItemStatusId);
        Task<long> PostcollectionManifestLineItemStatus(CollectionManifestLineItemStatuss collectionManifestLineItemStatuss);
        Task<bool> PutcollectionManifestLineItemStatus(CollectionManifestLineItemStatuss collectionManifestLineItemStatuss);
    }
}