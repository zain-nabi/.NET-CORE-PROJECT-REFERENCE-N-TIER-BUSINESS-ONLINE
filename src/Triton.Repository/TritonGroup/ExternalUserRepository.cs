using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triton.Core;
using Triton.Interface.TritonGroup;
using Triton.Model.TritonGroup.Custom;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Repository.TritonGroup
{
    public class ExternalUserRepository : IExternalUser
    {
        private readonly IConfiguration _config;
        public ExternalUserRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<ExternalUser> FindByNameAsync(string username)
        {
            const string sql = "SELECT * FROM ExternalUser WHERE UserName = @Username";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.QueryFirstOrDefault<ExternalUser>(sql, new { username });
        }

        public async Task<ExternalUser> CreateAsync(ExternalUser user)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            var userId = connection.Insert(user);
            user.ExternalUserID = (int)userId;
            return user;
        }

        public async Task<long> Post(ExternalUser user)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Insert(user);
        }


        public async Task<bool> PutUpdateAsync(ExternalUser user)
        {
                await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
                return await connection.UpdateAsync(user);
        }

        public async Task<ExternalUser> FindByIdAsync(int userId)
        {
            const string sql = "SELECT * FROM ExternalUser WHERE ExternalUserID = @UserID";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.QueryFirstOrDefault<ExternalUser>(sql, new { userId });
        }


        public async Task<List<ExternalUser>> GetAllUsersInclLockedOut()
        {
            const string sql = "SELECT ExternalUserID, Username, FirstName, LastName, LockoutEnabled FROM ExternalUser ORDER BY FirstName, LastName";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Query<ExternalUser>(sql).ToList();
        }

        public async Task<List<ExternalUserModel>> GetUserWithRoles()
        {
            const string sql = "EXEC proc_ExternalUsers_StringAgg";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Query<ExternalUserModel>(sql).ToList();
        }
        public async Task<ExternalUser> CheckIfEmailExist(string email)
        {

            const string sql = "EXEC proc_ExternalUsers_CheckIfEmailExists @email";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Query<ExternalUser>(sql, new { email = email }).FirstOrDefault();
        }

        public async Task<ExternalUserModel> FindByExternalUserID(int externalUserID)
        {
            string sql =
                        @"SELECT 
                            EU.FirstName,
                            EU.LastName,
	                        EU.Email,
	                        EU.UserName,
	                        EU.PhoneNumber,
                            R.RoleID,
	                        R.RoleName AS UserRole,
	                        EU.LockoutEnabled,
                            EU.PasswordHash,
                            EU.ExternalUserID,
                            EU.SecurityStamp,
                            EU.PhoneNumberConfirmed,
                            EU.EmailConfirmed,
                            EU.AccessFailedCount,
                            EU.LockoutEndDateUtc
                        FROM ExternalUser EU
                        INNER JOIN ExternalUserRole EUR ON EUR.ExternalUserID = EU.ExternalUserID
                        INNER JOIN Roles R ON R.RoleID = EUR.RoleID
                        WHERE EU.ExternalUserID = @externalUserID";

            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Query<ExternalUserModel>(sql , new { externalUserID = externalUserID }).FirstOrDefault();
        }
    }
}
