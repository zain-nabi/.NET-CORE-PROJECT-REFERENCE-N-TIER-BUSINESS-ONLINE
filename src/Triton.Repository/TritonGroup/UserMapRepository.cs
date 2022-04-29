using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Triton.Core;
using Triton.Interface.TritonGroup;
using Triton.Model.TritonGroup.Custom;
using Triton.Model.TritonGroup.Tables;
using Customers = Triton.Model.CRM.Tables.Customers;

namespace Triton.Repository.TritonGroup
{
    public class UserMapRepository : IUserMap
    {
        private readonly IConfiguration _config;
        public UserMapRepository(IConfiguration configuration)
        {
            _config = configuration;
        }
        public async Task<List<Users>> GetUserMapByUserType(int userTypeLcid)
        {
            const string sql = @"SELECT DISTINCT U.UserID, UserName, FirstName, LastName, PasswordHash = NULL, SecurityStamp = NULL, PhoneNumber = NULL, PhoneNumberConfirmed, Email, EmailConfirmed, LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, U.EmployeeID, AppVersion, sAMAccountName, FWUsername, OldUserID 
                FROM Users U INNER JOIN UserMap UM ON UM.UserID = U.UserID WHERE UserTypeLCID = @userTypeLcid";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Query<Users>(sql, new { userTypeLcid }).ToList();
        }

        public async Task<UserMap> GetUserMap(int userMapId)
        {
            const string sql = @"SELECT * FROM UserMap WHERE UserMapID = @userMapId";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.QueryFirstOrDefault<UserMap>(sql, new { userMapId });
        }

        public async Task<UserMapCustomerModels> GetUserCustomerMapModel(int userId)
        {
            const string sql = @"SELECT * FROM ExternalUserMap UM 
                                INNER JOIN CRM.dbo.Customers C ON C.CustomerID = UM.CustomerID 
                                WHERE ExternalUserID = @userId";

            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            var modelList = new UserMapCustomerModels
            {
                Customers = new List<Model.CRM.Tables.Customers>(),
                UserMap = new List<UserMap>()
            };
            var data = connection.Query<UserMap, Customers, UserMapCustomerModels>(sql, (userMap, customers) =>
                {
                    modelList.Customers.Add(customers);
                    modelList.UserMap.Add(userMap);
                    return modelList;
                },
                new { userId },
                splitOn: "UserMapID, CustomerID").FirstOrDefault();
            return data;
        }


        public async Task<string> PostUserMapObject(UserMap userMap)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Query<string>("proc_UserMap_Insert",
                new
                {
                    userMap.UserID,
                    userMap.EmployeeID,
                    userMap.CustomerID,
                    userMap.SupplierID,
                    userMap.UserTypeLCID,
                    userMap.SystemID,
                    userMap.CreatedByUserID
                }, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public async Task<bool> PutUserMap(UserMap userMap)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Update(userMap);
        }
    }
}
