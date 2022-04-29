using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Triton.Core;
using Triton.Interface.TritonGroup;
using Triton.Model.Applications.Tables;
using Triton.Model.LeaveManagement.Tables;
using Triton.Model.TritonGroup.Custom;
using Triton.Model.TritonGroup.StoredProcs;
using Triton.Model.TritonGroup.Tables;
using Roles = Triton.Model.TritonGroup.Tables.Roles;

namespace Triton.Repository.TritonGroup
{
    public class UsersRepository : IUser
    {
        private readonly IConfiguration _config;
        public UsersRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<Users> FindByNameAsync(string username)
        {
            const string sql = "SELECT * FROM Users WHERE UserName = @Username";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.QueryFirstOrDefault<Users>(sql, new { username });
        }

        public async Task<Users> CreateAsync(Users user)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            var userId = connection.Insert(user);
            user.UserID = (int)userId;
            return user;
        }

        public async Task<long> Post(Users user)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Insert(user);
        }


        public async Task<bool> PutUpdateAsync(Users user)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Update(user);
        }

        public async Task<Users> FindByIdAsync(int userId)
        {
            const string sql = "SELECT * FROM Users WHERE UserID = @UserID";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.QueryFirstOrDefault<Users>(sql, new { userId });
        }


        public async Task<List<Users>> GetAllUsersInclLockedOut()
        {
            const string sql = "SELECT UserID, Username, FirstName, LastName, LockoutEnabled FROM Users ORDER BY FirstName, LastName";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Query<Users>(sql).ToList();
        }

        public async Task<UserWithRoles> GetUserWithRoles(int userId, string roleIds)
        {
            var rolesSql = "SELECT * FROM Roles";
            if (roleIds != null)
                rolesSql = $"SELECT * FROM Roles WHERE RoleID IN ({roleIds})";

            var sql = $"SELECT * FROM Users WHERE UserID = {userId} " +
                            $"SELECT * FROM UserRoles WHERE UserID = {userId} " +
                            $"{rolesSql} ";

            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            var results = connection.QueryMultiple(sql);

            var model = new UserWithRoles
            {
                Users = results.ReadFirst<Users>(),
                UserRoles = results.ReadFirst<UserRoles>(),
                Roles = results.Read<Roles>().ToList()
            };

            return model;
        }

        public async Task<UserInformation> FindBysAmAccountName(string sAmAccountName, string database)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Query<UserInformation>("proc_Users_Information_Select", new { sAmAccountName, database }, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

    }
}
