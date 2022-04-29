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
using Triton.Interface.CRM;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.Tables;

namespace Triton.Repository.CRM
{
 

    public class CustomerNotificationMapRespository : ICustomerNotificationMap
    {
        private static IConfiguration _config;
        public CustomerNotificationMapRespository(IConfiguration config) { _config = config; }

        public async Task<List<CustomerNotificationMaps>> GetAsync()
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                return connection.Query<CustomerNotificationMaps>(@"SELECT * FROM CustomerNotificationMaps WHERE CustomerActive = 1 ORDER BY Customeractive").ToList();
            }
        }

        public async Task<CustomerNotificationMapsModel> GetComplexCustomerNotificationMapsbyCustomerNotificationMapID(int CustomerNotificationMapID)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                const string sql = (@" SELECT CNM.*,C.*,FWE.*
                                        FROM  CustomerNotificationMaps CNM
                                        INNER JOIN Customers C ON CNM.CustomerID = C.CustomerID
										INNER JOIN FWEventCodes FWE ON CNM.FWEventCodeID = FWE.FWEventCodeID
										WHERE CustomerNotificationMapID = @CustomerNotificationMapID");
                var data = connection.Query<CustomerNotificationMaps, Customers, FWEventCodes, CustomerNotificationMapsModel>(sql, (CustomerNotificationMaps, Customers, FWEventCodes) =>
                {
                    var model = new CustomerNotificationMapsModel
                    {
                        CustomerNotificationMaps = CustomerNotificationMaps,
                        Customers = Customers,
                        FWEventCodes = FWEventCodes,
                    };
                    return model;
                },
                new { CustomerNotificationMapID }
                , splitOn: "CustomerNotificationMapID, CustomerID, FWEventCodeID").FirstOrDefault();
                return data;

            }
        }

      

        public async Task<CustomerNotificationMaps> GetCustomerNotificationMapsByCustomerNotificationMapID(int CustomerNotificationMapID)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                return  connection.Query<CustomerNotificationMaps>(@"SELECT * FROM CustomerNotificationMaps WHERE CustomerNotificationMapID = @CustomerNotificationMapID", new { CustomerNotificationMapID }).FirstOrDefault();
            }
        }

        public async Task<CustomerNotificationMapsEditModel> GetCustomerNotificationMapsCreateModel()
        {
            var CNM = new CustomerNotificationMapsEditModel();

            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                var sql = string.Format(@"SELECT * from FWEventCodes where CustomerActive = 1
                                          SELECT * from customers where CustomerStatusID<>7 ");
                using (var multi = connection.QueryMultiple(sql))
                {
                    CNM.CustomerNotificationMaps = new CustomerNotificationMaps();
                    CNM.FWEventCodes = multi.Read<FWEventCodes>().ToList();
                    CNM.Customers = multi.Read<Customers>().ToList();
                }
            }
            return CNM;
        }

        public async Task<CustomerNotificationMapsEditModel> GetCustomerNotificationMapsEditModel(int CustomerNotificationMapID)
        {
            var CNM = new CustomerNotificationMapsEditModel();

            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                var sql = string.Format(@"SELECT * FROM CustomerNotificationMaps where CUstomerNotificationMapID={0}
                                          SELECT * from FWEventCodes where CustomerActive = 1
                                          SELECT * from customers where CustomerStatusID<>7", CustomerNotificationMapID);
                using (var multi = connection.QueryMultiple(sql))
                {
                    CNM.CustomerNotificationMaps = multi.Read<CustomerNotificationMaps>().FirstOrDefault();
                    CNM.FWEventCodes = multi.Read<FWEventCodes>().ToList();
                    CNM.Customers = multi.Read<Customers>().ToList();
                }
            }
            return CNM;
        }

        public async Task<CustomerNotificationMapsModel> GetCustomerNotificationMapsSearch(string CustomerName, string AccountCode)
        {
            const string sql = "proc_CustomerNotificationMapSearch";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return await connection.QueryFirstAsync<CustomerNotificationMapsModel>(sql, new { CustomerName, AccountCode }, commandType: CommandType.StoredProcedure);
        }

        public async Task<long> PostCustomerNotificationMaps(CustomerNotificationMaps CustomerNotificationMap)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.Insert(CustomerNotificationMap);
        }

        public async Task<bool> PutCustomerNotificationMaps(CustomerNotificationMaps CustomerNotificationMap)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.Update(CustomerNotificationMap);
        }
    }
}
