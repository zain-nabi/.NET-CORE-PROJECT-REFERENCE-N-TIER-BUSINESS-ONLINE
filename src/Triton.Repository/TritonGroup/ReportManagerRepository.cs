using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Triton.Core;
using Triton.Interface.TritonGroup;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Repository.TritonGroup
{
    /// <inheritdoc/>
    public class ReportManagerRepository : IReportManager
    {
        private readonly IConfiguration _config;
        public ReportManagerRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<List<ReportManager>> Get(string roleIds)
        {
            var sql = $"SELECT * FROM ReportManager WHERE RoleID IN ({roleIds})";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Query<ReportManager>(sql).ToList();
        }

        public async Task<List<ReportManager>> Get(int systemId, string categoryLciDs, string roleIds)
        {
            var sql = $"SELECT * FROM ReportManager WHERE SystemID = {systemId} AND CategoryLCID = {categoryLciDs} AND RoleID IN ({roleIds})";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Query<ReportManager>(sql).ToList();
        }

        public async Task<ReportManager> GetReport(int reportManagerId)
        {
            var sql = $"SELECT * FROM ReportManager WHERE ReportManagerID ={reportManagerId}";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Query<ReportManager>(sql).FirstOrDefault();
        }
    }
}
