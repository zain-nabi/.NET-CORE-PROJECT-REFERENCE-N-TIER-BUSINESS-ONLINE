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
    public class UserCustomerMapRepository : IUserCustomerMap
    {
        private readonly IConfiguration _config;
        public UserCustomerMapRepository(IConfiguration configuration)
        {
            _config = configuration;
        }
        public async Task<List<UserCustomerMap>> GetUserCustomerMapByUserId(int userId)
        {
            const string sql = "SELECT * FROM UserMap WHERE UserID = @UserID AND CustomerID IS NOT NULL AND DeletedOn IS NULL";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Query<UserCustomerMap>(sql, new { userId }).ToList();
        }
    }
}
