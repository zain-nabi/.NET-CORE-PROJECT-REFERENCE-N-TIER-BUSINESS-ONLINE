using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triton.Core;
using Triton.Interface.TritonGroup;
using Triton.Model.TritonGroup.Custom;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Repository.TritonGroup
{
    public class ExternalUserMapRepository : IExternalUserMap
    {
        private readonly IConfiguration _config;
        public ExternalUserMapRepository(IConfiguration configuration)
        {
            _config = configuration;
        }
        public async Task<List<ExternalUser>> GetUserMapByUserType(int userTypeLcid)
        {
            const string sql = @"SELECT DISTINCT 
                                    U.ExternalUserID, UserName, FirstName, LastName, 
                                    PasswordHash = NULL, SecurityStamp = NULL, 
                                    PhoneNumber = NULL, PhoneNumberConfirmed, Email, 
                                    EmailConfirmed, LockoutEndDateUtc, LockoutEnabled, 
                                    AccessFailedCount 
                                FROM ExternalUser U 
				                INNER JOIN UserMap UM ON UM.UserID = U.ExternalUserID WHERE UserTypeLCID = @userTypeLcid";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Query<ExternalUser>(sql, new { userTypeLcid }).ToList();
        }

        public async Task<ExternalUserMap> GetUserMap(int userMapId)
        {
            const string sql = @"SELECT * FROM ExternalUserMap WHERE ExternalUserMapID = @userMapId";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.QueryFirstOrDefault<ExternalUserMap>(sql, new { userMapId });
        }

        public async Task<ExternalUserMapCustomerModels> GetUserCustomerMapModel(int userId)
        {
            const string sql = @"SELECT * FROM ExternalUserMap UM INNER JOIN CRM.dbo.Customers C ON C.CustomerID = UM.CustomerID WHERE ExternalUserID = @userId";

            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            var modelList = new ExternalUserMapCustomerModels
            {
                Customers = new List<Model.CRM.Tables.Customers>(),
                UserMap = new List<ExternalUserMap>()
            };
            var data = connection.Query<ExternalUserMap, Model.CRM.Tables.Customers, ExternalUserMapCustomerModels>(sql, (userMap, customers) =>
            {
                modelList.Customers.Add(customers);
                modelList.UserMap.Add(userMap);
                return modelList;
            },
                new { userId },
                splitOn: "UserMapID, CustomerID").FirstOrDefault();
            return data;
        }


        public async Task<string> PostUserMapObject(ExternalUserMap userMap)
        {
                await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
                return connection.Query<string>("proc_ExternalUserMap_Insert",
                    new
                    {
                        userMap.ExternalUserID,
                        userMap.CustomerID,
                        userMap.UserTypeLCID,
                        userMap.CreatedByUserID
                    }, commandType: CommandType.StoredProcedure).FirstOrDefault();

        }

        public async Task<bool> PutUserMap(ExternalUserMap userMap)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Update(userMap);
        }

        public async Task<List<ExternalUserMapModel>> GetUserMapCustomers(int externalUserID)
        {
            const string sql = @"SELECT	
	                                C.CustomerID,
	                                C.Name CustomerName
                                FROM ExternalUserMap EUM
                                INNER JOIN CRM.dbo.Customers C ON C.CustomerID = EUM.CustomerID
                                WHERE ExternalUserID = @externalUserID";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
            return connection.Query<ExternalUserMapModel>(sql, new { externalUserID }).ToList();
        }

        public async Task<bool> UpdateExternalUserMap(ExternalUserMapModel obj)
        {
            try
            {
                var ExternalUserMap = Triton.Core.Extensions.ToDataTableFromList(obj.ExternalUserMapList, true);
                await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
                var data = connection.Query<bool>("proc_ExternalUserMap_Delete", new
                {
                    ExternalUserMap = ExternalUserMap.AsTableValuedParameter("ExternalUserMapTVP")
                }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> InsertExternalUserMap(ExternalUserMapModel obj)
        {
            if(obj.ExternalUserMapList != null)
            {
                var ExternalUserMap = Triton.Core.Extensions.ToDataTableFromList(obj.ExternalUserMapList, true);
                await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
                var data = connection.Query<string>("proc_ExternalUser_ExternalUserMap_Insert", new
                {
                    ExternalUserMap = ExternalUserMap.AsTableValuedParameter("ExternalUserMapTVP"),
                    obj.ExternalUser.UserName,
                    obj.ExternalUser.FirstName,
                    obj.ExternalUser.LastName,
                    obj.ExternalUser.Email,
                    obj.ExternalUser.EmailConfirmed,
                    obj.ExternalUser.PhoneNumber,
                    obj.ExternalUser.PhoneNumberConfirmed,
                    obj.ExternalUser.SecurityStamp,
                    obj.ExternalUser.AccessFailedCount,
                    obj.ExternalUser.LockoutEnabled,
                    obj.ExternalUser.LockoutEndDateUtc,
                    obj.ExternalUser.PasswordHash

                }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return data;
            }
            else
            {
                await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonGroup));
                var data = connection.Query<string>("proc_ExternalUser_ExternalUserMap_Insert", new
                {
                    obj.ExternalUser.UserName,
                    obj.ExternalUser.FirstName,
                    obj.ExternalUser.LastName,
                    obj.ExternalUser.Email,
                    obj.ExternalUser.EmailConfirmed,
                    obj.ExternalUser.PhoneNumber,
                    obj.ExternalUser.PhoneNumberConfirmed,
                    obj.ExternalUser.SecurityStamp,
                    obj.ExternalUser.AccessFailedCount,
                    obj.ExternalUser.LockoutEnabled,
                    obj.ExternalUser.LockoutEndDateUtc,
                    obj.ExternalUser.PasswordHash

                }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                return data;
            }
        }
    }
}
