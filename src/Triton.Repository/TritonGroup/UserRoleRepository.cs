using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Triton.Core;
using Triton.Interface.TritonGroup;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Repository.TritonGroup
{
    public class UserRoleRepository : IUserRole
    {
        private readonly IConfiguration _config;
        public UserRoleRepository(IConfiguration configuration) { _config = configuration; }

        public async Task<UserRoles> GetUserRole(int userRoleId)
        {
            const string sql = "SELECT * FROM UserRoles WHERE UserRoleID = @userRoleId";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.QueryFirstOrDefault<UserRoles>(sql, new { userRoleId });
        }

        public async Task<long> Post(UserRoles userRoles, string dbName)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            return connection.Insert(userRoles);
        }

        public async Task<bool> Put(UserRoles userRoles, string dbName)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            return connection.Update(userRoles);
        }
    }
}
