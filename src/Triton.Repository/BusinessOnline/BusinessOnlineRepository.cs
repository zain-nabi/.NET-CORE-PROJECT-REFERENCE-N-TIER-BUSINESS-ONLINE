using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Triton.Core;
using Triton.Interface.BusinessOnline;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.StoredProcs;
using Triton.Model.TritonOps.StoredProcs;

namespace Triton.Repository.BusinessOnline
{
    public class BusinessOnlineRepository : IBusinessOnline
    {
        private readonly IConfiguration _config;
        public BusinessOnlineRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<List<proc_Waybills_CSA_Dashboard_Select>> GetCustomerDashboard(int userId, bool isTritonGroupUserId, DateTime date)
        {
            const string sql = "proc_Waybills_CSA_Dashboard_Select";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.Query<proc_Waybills_CSA_Dashboard_Select>(sql, new { userId, TritonGroupUserID = isTritonGroupUserId, date }, commandType: CommandType.StoredProcedure).ToList();
        }

        public async Task<List<proc_CSA_WaybillList_Select>> GetWaybillsByType(string customerId, string type, bool mobile, DateTime? date)
        {
            const string sql = "proc_CSA_WaybillList_Select";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.Query<proc_CSA_WaybillList_Select>(sql, new { customerId, type, mobile, date }, commandType: CommandType.StoredProcedure).ToList();
        }

        public async Task<proc_CSA_Customer_Select> GetDeliveryStatusCount(string customerIds)
        {
            const string sql = "proc_CSA_Customer_Select";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonOps));
            return connection.QueryFirst<proc_CSA_Customer_Select>(sql, new { CustomerID = customerIds }, commandType: CommandType.StoredProcedure);
        }

        public async Task<CSADashboardModel> GetDashboardForCustomerMultiQuery(string customerIds, int userId, bool? isTritonGroupUserId, DateTime? date, string tableName)
        {
            const string sql = "EXEC TritonOps.dbo.proc_CSA_Customer_Select @CustomerID " +
                               "EXEC proc_Waybills_CSA_Dashboard_Select @UserID, @TritonGroupUserID, @Date, @TableName";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            var results =  connection.QueryMultiple(sql, new { CustomerID = customerIds, userId, TritonGroupUserID = isTritonGroupUserId, date, tableName });
            try
            {
                var model = new CSADashboardModel
                {
                    DeliveryStatusCount = results.Read<proc_CSA_Customer_Select>().FirstOrDefault(),
                    CustomerDeliveryList = results.Read<proc_Waybills_CSA_Dashboard_Select>().ToList()
                };

                return model;
            }
            catch
            {

                throw;
            }
        }

        public async Task<List<proc_Customer_By_CustomerID_Tabs_Select>> GetCustomerDeliveriesByStatus(string customerIds, string type)
        {
            const string sql = "proc_CSA_Customer_By_CustomerID_Tabs_Select";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonOps));
            return connection.Query<proc_Customer_By_CustomerID_Tabs_Select>(sql, new { CustomerID = customerIds, type }, commandType: CommandType.StoredProcedure, commandTimeout: 50000).ToList();
        }
    }
}
