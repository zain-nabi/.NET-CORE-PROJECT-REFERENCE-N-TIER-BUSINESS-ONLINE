using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Triton.Core;
using Triton.Interface.CRM;
using Triton.Model.CRM.StoredProcs;
using Triton.Model.TritonGroup.Custom;
using Triton.Model.TritonGroup.Tables;
using Customers = Triton.Model.CRM.Tables.Customers;

namespace Triton.Repository.CRM
{
    public class CustomerRepository : ICustomers
    {
        private static IConfiguration _config;
        public CustomerRepository(IConfiguration config) { _config = config; }

        public async Task<List<Customers>> GetAllActiveCustomers()
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return  connection.Query<Customers>("SELECT * , AccountCode + ' - ' + Name AS DropDownList FROM Customers WHERE CustomerStatusID <> 7 ORDER BY AccountCode").ToList();
        }

        public async Task<Customers> GetCrmCustomerById(int id)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.QueryFirstAsync<Customers>($"SELECT * FROM Customers WHERE CustomerID = {id}").Result;
        }

        public async Task<Customers> GetCRMCustomerByAccountCode(string accountCode,string dbName="CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            return connection.Query<Customers>($"SELECT * FROM Customers WHERE AccountCode = '{accountCode}'").FirstOrDefault();
        }

        public async Task<List<Customers>> FindCrmCustomer(string search)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            var sql = $"SELECT * FROM Customers WHERE CustomerStatusID <> 7 AND AccountCode like '%{search}%' OR Name like '%{search}%' ORDER BY AccountCode, Name ASC";
            return connection.QueryAsync<Customers>(sql).Result.ToList();
        }

        public async Task<bool> PutCustomer(Customers customer)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.Update(customer);
        }

        public async Task<UserMapCustomerModels> GetUserMap_With_Customers(int userid)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            const string sql = "SELECT * FROM UserMap UM INNER JOIN CRM.dbo.Customers C ON C.CustomerID = UM.CustomerID WHERE UserID = 165";
            var modelList = new UserMapCustomerModels
            {
                Customers = new List<Customers>(),
                UserMap = new List<UserMap>(),
            };
            var data = connection.Query<UserMap, Customers, UserMapCustomerModels>(sql, (userMap, customers) =>
                {
                    modelList.Customers.Add(customers);
                    modelList.UserMap.Add(userMap);
                    return modelList;
                },
                new { userid },
                splitOn: "UserMapID, CustomerID").FirstOrDefault();
            return data;
        }

        public async Task<List<Customers>> GetCrmCustomersByRepUserId(int userId)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.Query<Customers>($"SELECT Distinct C.* FROM Customers C " +
                                                    $"inner join TritonOps..vw_CSAReps CSA on CSA.CustomerID=C.CustomerID" +
                                                    $" WHERE CSA.UserID = '{userId}'").ToList();
        }

        public async Task<List<Customers>> GetCrmInternalAccounts()
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                return connection.Query<Customers>($"SELECT Distinct C.* FROM Customers C where InternalAccount=1 and CustomerStatusID<>7").ToList();
            }
        }

        public async Task<CustomerAssessment> GetCustomerAssessment(int customerId, string periodFrom, string periodTo, int? branch = null, int? rep = null)
        {
            const string sql = "CustomerAssesment";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.QueryFirstOrDefault<CustomerAssessment>(sql, new { customerId, periodFrom, periodTo, branch, rep }, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<Customers>> FindCrmCustomerByIds(string customerIds)
        {
            var sql = $"SELECT * FROM Customers WHERE CustomerID IN ({customerIds}) AND CustomerStatusID = 3 ORDER BY Name";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.Query<Customers>(sql).ToList();
        }
    }
}
