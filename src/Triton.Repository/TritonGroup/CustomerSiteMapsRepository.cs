using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triton.Interface.TritonGroup;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Repository.TritonGroup
{

    public class CustomerSiteMapsRepository : ICustomerSiteMaps
    {
        private static IConfiguration _config;
        public CustomerSiteMapsRepository(IConfiguration config) { _config = config; }
        public async Task<bool> PostCustomerSiteMap(CustomerSiteMaps customerSiteMap, String dbName = "TritonGroup")
        {
           await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            {
                long _result = connection.Insert(customerSiteMap);
                if (_result != 0)
                    return true;
                else return false;
            }
        }

        public async Task<bool> PutCustomerSiteMap(Triton.Model.TritonGroup.Tables.CustomerSiteMaps customerSiteMap, String dbName = "TritonGroup")
        {
           await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            {
                bool _result = connection.Update(customerSiteMap);
                return _result;
            }
        }

        public async Task<CustomerSiteMaps> GetCustomerSiteMap(long customerSiteMapID, String dbName = "TritonGroup")
        {
           await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            {
                var sql = string.Format(@"select * from CustomerSiteMaps where CustomerSiteMapID=@CustomerSiteMapID");

                var data = connection.Query<CustomerSiteMaps>(sql
                , new { customerSiteMapID }).FirstOrDefault();

                return data;
            }
        }

        public async Task<CustomerSiteMaps> GetCustomerSiteMapsByCustomerID_And_SiteID(int customerID, int siteID, String dbName = "TritonGroup")
        {
           await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            {
                return connection.Query<CustomerSiteMaps>("SELECT * FROM CustomerSiteMaps WHERE CustomerID = @CustomerID AND SiteID = @SiteID", new { customerID, siteID }).FirstOrDefault();
            }
        }



    }
}
