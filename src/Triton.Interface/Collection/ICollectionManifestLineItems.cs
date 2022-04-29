using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.Tables;

namespace Triton.Interface.Collection
{
    public interface ICollectionManifestLineItems
    {
        Task<CollectionManifestLineItems> GetCollectionManifestLineItems(int collectionManifestLineItemId, string dbName="CRM");
        Task<CollectionManifestLineItemsModel> GetComplex(int collectionManifestLineItemId, string dbName="CRM");
        Task<bool> Put(CollectionManifestLineItems collectionManifestLineItems, string dbName="CRM");
        Task<long> Post(CollectionManifestLineItems collectionManifestLineItems, string dbName="CRM");
    }
}
