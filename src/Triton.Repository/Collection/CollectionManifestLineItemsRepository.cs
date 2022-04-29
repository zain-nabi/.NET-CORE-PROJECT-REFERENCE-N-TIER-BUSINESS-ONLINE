using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Triton.Core;
using Triton.Interface.Collection;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.Tables;

namespace Triton.Repository.Collection
{
    public class CollectionManifestLineItemsRepository : ICollectionManifestLineItems
    {
        private readonly IConfiguration _config;

        public CollectionManifestLineItemsRepository(IConfiguration configuration)
        {
            _config = configuration;
        }
        public async Task<CollectionManifestLineItems> GetCollectionManifestLineItems(int collectionManifestLineItemId, string dbname = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                return connection.QueryFirstAsync<CollectionManifestLineItems>($"SELECT * FROM CollectionManifestLineItems WHERE CollectionManifestLineItemID={collectionManifestLineItemId} ").Result;
            }
        }

        public async Task<CollectionManifestLineItemsModel> GetComplex(int collectionManifestLineItemId, string dbname = "CRM")
        {
            await using var connection =
                Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            var sql = $@"SELECT * FROM CRM..CollectionManifestLineItems CMLI
                                 INNER JOIN CRM..CollectionManifests CM ON CMLI.CollectionManifestID = CM.CollectionManifestID
                                 INNER JOIN CRM..CollectionManifestStatuss CMSS ON CMSS.CollectionManifestStatusID = CMLI.CollectionManifestLineItemStatusID
                                 LEFT OUTER JOIN CRM..CollectionRequests CR ON CR.CollectionManifestID = Cm.CollectionManifestID
                                 WHERE CollectionManifestLineItemID = {collectionManifestLineItemId}";
            var data = connection.Query<CollectionManifestLineItems, CollectionManifests, CollectionManifestStatuss, CollectionRequests,CollectionManifestLineItemsModel>(sql,(collectionmanifestlineitemS, collectionManfestS, CollectionManifestStatusS, collectionrequestS) =>
                    {
                        var model = new CollectionManifestLineItemsModel
                        {
                            CollectionManifestLineItems = collectionmanifestlineitemS,
                            CollectionManifestStatuss = CollectionManifestStatusS,
                            CollectionManifests = collectionManfestS,
                            CollectionRequests = collectionrequestS,
                        };
                        return model;
                    }, new {collectionManifestLineItemId}, splitOn:"CollectionManifestLineItemID, CollectionManifestID, CollectionManifestStatusID, CollectionRequestID").FirstOrDefault();
            return data;
        }

        public async  Task<long> Post(CollectionManifestLineItems collectionManifestLineItems, string dbname = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                return connection.Insert(collectionManifestLineItems);
            }
        }

        public async Task<bool> Put(CollectionManifestLineItems collectionManifestLineItems, string dbname = "CRM")
        {
            await using var connection = Connection.GetOpenConnection((_config.GetConnectionString(StringHelpers.Database.Crm)));
            {
                return connection.Update(collectionManifestLineItems);
            }
        }
    }
}