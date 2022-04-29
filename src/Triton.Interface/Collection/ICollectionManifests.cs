using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triton.Model.CRM.Tables;
using Triton.Model.TritonSecurity.Custom;

namespace Triton.Interface.Collection
{
    public interface ICollectionManifests

    {
        Task<CollectionManifests> GetCollectionManifest(int CollectionManifestId);
        Task<CollectionManifestsModel> GetComplex(int CollectionManifesId, string dbname="CRM");
        Task<long> Post(CollectionManifests collectionManifests, string dbname="CRM");
        Task<bool> Put(CollectionManifests collectionManifests, string dbname="CRM");
    }
}
