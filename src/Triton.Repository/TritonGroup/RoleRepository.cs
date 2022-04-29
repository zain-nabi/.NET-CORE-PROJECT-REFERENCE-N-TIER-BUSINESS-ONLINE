using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Triton.Interface.TritonGroup;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Repository.TritonGroup
{
    public class RoleRepository : IRole
    {
        private readonly IConfiguration _config;
        public RoleRepository(IConfiguration configuration) { _config = configuration; }

        public async Task<List<Roles>> GetRolesByUserId(int userId, string dbName)
        {
            const string sql = @"SELECT R.* FROM Users U
            INNER JOIN UserRoles UR ON UR.UserID = U.UserID
            INNER JOIN Roles R ON R.RoleID = UR.RoleID
            WHERE U.UserID = @UserID AND DeletedOn IS NULL ORDER BY RoleName";

            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            return connection.Query<Roles>(sql, new { userId }).ToList();
        }

        public async Task<List<Roles>> GetRolesByIds(string roleIDs, string dbName)
        {
            var sql = $"SELECT * FROM Roles WHERE RoleID IN ({roleIDs})";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            return connection.Query<Roles>(sql).ToList();
        }
    }
}
