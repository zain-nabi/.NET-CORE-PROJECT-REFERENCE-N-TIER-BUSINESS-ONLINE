using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Triton.Core;
using Triton.Interface.Collection;
using Triton.Model.CRM.Tables;

namespace Triton.Repository.Collection
{
    public class CollectionManifestLineItemStatusRepository : ICollectionManifestLineItemStatuss
    {
        private readonly IConfiguration _config;

        public CollectionManifestLineItemStatusRepository(IConfiguration configuration)
        {
            _config = configuration;
        }
        public async Task<CollectionManifestLineItemStatuss> GetCollectionManifestLineItemStatusById(int CollectionManifestLineItemStatusId)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                return connection.QueryFirstAsync<CollectionManifestLineItemStatuss>($"SELECT * FROM CRM..CollectionManifestLineItemStatuss WHERE CollectionManifestLineItemStatusID = {CollectionManifestLineItemStatusId}").Result;
            }
        }

        public async Task<CollectionManifestLineItemStatuss> GetCollectionManifestLineItemStatuss()
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                return connection.QueryFirstAsync<CollectionManifestLineItemStatuss>("SELECT * FROM CRM..CollectionManifestLineItemStatuss").Result;
            }
        }

        public async Task<long> PostcollectionManifestLineItemStatus(CollectionManifestLineItemStatuss collectionManifestLineItemStatuss)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                return connection.Insert(collectionManifestLineItemStatuss);
            }
        }

        public async Task<bool> PutcollectionManifestLineItemStatus(CollectionManifestLineItemStatuss collectionManifestLineItemStatuss)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                return connection.Update(collectionManifestLineItemStatuss);
            }
        }
    }
}