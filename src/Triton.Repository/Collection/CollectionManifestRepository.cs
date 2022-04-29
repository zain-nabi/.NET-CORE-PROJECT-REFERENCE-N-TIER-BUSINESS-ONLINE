using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Triton.Core;
using Triton.Interface.Collection;
using Triton.Model.CRM.Tables;
using Triton.Model.TritonSecurity.Custom;
using Triton.Model.TritonSecurity.Tables;
using Triton.Model.TritonGroup.Tables;
using LookUpCodes = Triton.Model.TritonGroup.Tables.LookUpCodes;

namespace Triton.Repository.Collection
{
    public class CollectionManifestRepository : ICollectionManifests
    {
        private readonly IConfiguration _config;

        public CollectionManifestRepository(IConfiguration configuration)
        {
            _config = configuration;
        }
        public async Task<CollectionManifests> GetCollectionManifest(int CollectionManifestId)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                return connection.QueryFirstAsync<CollectionManifests>($"  SELECT * FROM CollectionManifests WHERE CollectionManifestID = {CollectionManifestId}").Result;
            }
        }

      
        public async Task<CollectionManifestsModel> GetComplex(int CollectionManifesId, string dbname="CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                var sql = $@"SELECT * FROM CRM..collectionManifests CM
                                INNER JOIN TritonSecurity..Branches B ON B.BranchID = CM.BranchID
                                LEFT OUTER JOIN CRM..SubContractors SC ON SC.SubContractorID= CM.SubContractorID
                                LEFT OUTER join TritonGroup..lookupcodes LUC ON LUC.AdditionalField1Value= CM.CollectionManifestStatusID AND LUC.LookUPCodeCategoryID = 54
                                WHERE CollectionManifestId = {CollectionManifesId}";
                var data = connection.Query<CollectionManifests, Branches, SubContractors, LookUpCodes,CollectionManifestsModel>(sql,(collectionManifestS, branches, SubContractorS, lookupcodeS) =>
                        {
                            var model = new CollectionManifestsModel
                            {
                                CollectionManifests = collectionManifestS,
                                Branches = branches,
                                SubContractors = SubContractorS,
                                LookUpCodes = lookupcodeS,
                            };
                            return model;

                        }, new {CollectionManifesId},
                        splitOn:"CollectionManifestID, BranchID, SubContractorID, LookUpCodeID").FirstOrDefault();
                return data;
            }
        }

        public async Task<long> Post(CollectionManifests collectionManifests, string dbname="CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                return connection.Insert(collectionManifests);
            }
        }

        public async Task<bool> Put(CollectionManifests collectionManifests, string dbname="CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                return connection.Update(collectionManifests);
            }
        }
    }
}