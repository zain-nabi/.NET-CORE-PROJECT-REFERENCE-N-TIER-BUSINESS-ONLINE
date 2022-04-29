using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triton.Core;
using Triton.Interface.TritonGroup;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Repository.TritonGroup
{
    public class ExternalUserRoleRepository : IExternalUserRole
    {
        private readonly IConfiguration _config;
        public ExternalUserRoleRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<List<Roles>> GetRolesByUserId(int userId, string dbName)
        {
            const string sql = @"SELECT R.* FROM ExternalUser U
                                    INNER JOIN ExternalUserRole UR ON UR.ExternalUserID = U.ExternalUserID
                                    INNER JOIN Roles R ON R.RoleID = UR.RoleID
                                    WHERE U.ExternalUserID = @UserID AND DeletedOn IS NULL ORDER BY RoleName";

            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            return connection.Query<Roles>(sql, new { userId }).ToList();
        }

        public async Task<List<Roles>> GetRolesByIds(string roleIDs, string dbName)
        {
            var sql = $"SELECT * FROM Roles WHERE RoleID IN ({roleIDs})";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            return connection.Query<Roles>(sql).ToList();
        }

        public async Task<ExternalUserRole> GetUserRole(int userRoleId)
        {
            const string sql = "SELECT * FROM ExternalUserRole WHERE ExternalUserRoleID = @userRoleId";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.QueryFirstOrDefault<ExternalUserRole>(sql, new { userRoleId });
        }

        public async Task<long> Post(ExternalUserRole userRoles, string dbName)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            return connection.Insert(userRoles);
        }

        public async Task<bool> Put(ExternalUserRole userRoles, string dbName)
        {
                await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
                return connection.Update(userRoles);
            
        }

        public async Task<List<Roles>> GetActiveUserRoles()
        {
            const string sql = "SELECT * FROM Roles WHERE RoleID IN (1,2)";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Query<Roles>(sql).ToList();
        }

        public async Task<ExternalUserRole> GetUserRoleByID(int externalUserID)
        {
            const string sql = "SELECT * FROM ExternalUserRole WHERE ExternalUserID = @externalUserID";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.QueryFirstOrDefault<ExternalUserRole>(sql, new { externalUserID });
        }
    }
}
