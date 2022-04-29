using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Triton.Core;
using Triton.Interface.CRM;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.StoredProcs;
using Triton.Model.CRM.Tables;

namespace Triton.Repository.CRM
{
    public class CollectionRequestRepository : ICollectionRequest
    {
        private readonly IConfiguration _config;

        public CollectionRequestRepository(IConfiguration configuration)
        {
            _config = configuration;
        }
        //public async Task<bool> Delete(CollectionRequests collectionRequests, string dbName = "CRM")
        //{
        //    await using var connection = Connection.GetOpenConnection(dbName);
        //    return connection.Update(collectionRequests);
        //}
        public async Task<CollectionRequests> GetCollectionRequest(int collectionRequestId, string dbName = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(dbName);
            return connection.QueryFirstAsync<CollectionRequests>($"SELECT * FROM CollectionRequests WHERE CollectionRequestID = {collectionRequestId}").Result;
        }
        public async Task<CollectionRequestsModel> FindCollectionRequest(string customerXRef, DateTime? startDate, DateTime? endDate, string CollectionRequestNumber, int customerId)
        {
            var model = new CollectionRequestsModel();
            const string sql = "proc_CollectionRequests";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            model.proc_CollectionRequests =  connection.Query<proc_CollectionRequests>(sql, new { startDate, endDate, customerXRef, CollectionRequestNumber, customerId }, commandType: CommandType.StoredProcedure).ToList();
            return model;
            //return connection.QueryFirst<CollectionRequestsModel>(sql, new { startDate, endDate, customerXRef, CollectionRequestNumber, customerId }, commandType: CommandType.StoredProcedure);            
            //var CR = new CollectionRequestsModel();
            //await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            //{
            //    const string sql = @"proc_CollectionRequests @StartDate, @endDate, @CustomerXRef, @CollectionRequestNumber, @CustomerID";
            //    using var multi = connection.Query<proc_CollectionRequests>(sql,new {startDate, endDate, customerXRef, CollectionRequestNumber, customerId}).ToList();
            //CR.proc_CollectionRequests = multi.Read<proc_CollectionRequests>().ToList();
            //}
            //return CR;
        }

        public async Task<CollectionRequestsModel> GetComplex(int collectionRequestId, string dbName = "CRM")
        {
            await using var connection =
                Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            var sql = $@"SELECT * FROM CollectionRequests CR
                                INNER JOIN Customers C ON CR.CustomerID = c.CustomerID
                                LEFT OUTER JOIN CollectionManifests CM ON CR.CollectionManifestID = CM.CollectionManifestID
                                INNER JOIN TritonSecurity..Branches B ON CR.BranchID = B.BranchID
                                WHERE CollectionRequestID  = {collectionRequestId}";
            var data = connection.Query<CollectionRequests, Customers, CollectionManifests, Model.TritonSecurity.Tables.Branches, CollectionRequestsModel>(sql, (collectionRequest, customerS, collectionManifestS, brancheS) =>
                    {
                        var model = new CollectionRequestsModel
                        {
                            CollectionRequests = collectionRequest,
                            Customers = customerS,
                            CollectionManifests = collectionManifestS,
                            Branches = brancheS,
                        };
                        return model;
                    },
                    new {collectionRequestId}, splitOn: "CollectionRequestID, CustomerID, CollectionManifestID, BranchID").FirstOrDefault();
            return data;
        }
        public async Task<long> Post(CollectionRequests collectionRequests, string dbName = "CRM")
        {
            try
            {
                // Set default dates
                collectionRequests.Captured = DateTime.Now;
                collectionRequests.LastUpdated = DateTime.Now;

                await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
                return connection.Insert(collectionRequests);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<long> PostProduction(CollectionRequests collectionRequests, string dbName = "CRM")
        {
            try
            {
                // Set default dates
                collectionRequests.Captured = DateTime.Now;
                collectionRequests.LastUpdated = DateTime.Now;

                await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
                return connection.Insert(collectionRequests);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<bool> Put(CollectionRequests collectionRequests, string dbName = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(dbName);
            return connection.Update(collectionRequests);
        }
    }
}