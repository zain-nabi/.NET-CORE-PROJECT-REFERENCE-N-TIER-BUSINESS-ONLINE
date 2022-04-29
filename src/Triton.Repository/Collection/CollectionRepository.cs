using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Triton.Core;
using Triton.Interface.CRM;
using Triton.Model.CRM.Tables;

namespace Triton.Repository.Collection
{
    public class CollectionRepository :ICollection
    { private readonly IConfiguration _config;

        public CollectionRepository(IConfiguration configuration)
        {
            _config = configuration;
        }
        #region ================Collections========================
        public async Task<long> Post(Collections collections, string dbName="CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.Insert(collections);
        }
        public async Task<bool> Put(Collections collections, string dbName="CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.Update(collections);
        }
        public async Task<Collections> GetCollection(int collectionId, string dbName="CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.QueryFirstAsync<Collections>($"SELECT * FROM Collections WHERE CollectionID = {collectionId}").Result;
        }

        public async Task<Collections> GetCollectionByNo(string CollectionNo, string dbName="CRM")
        {
            var sql = $"SELECT * FROM Collections WHERE CollectionNo LIKE '%{CollectionNo}%'";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.Query<Collections>(sql, new {CollectionNo}).FirstOrDefault();
        }

        #endregion
    }
}
