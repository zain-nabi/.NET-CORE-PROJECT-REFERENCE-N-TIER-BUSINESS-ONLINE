using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triton.Core;
using Triton.Interface.CRM;
using Triton.Model.CRM.Tables;

namespace Triton.Repository.CRM
{
    public class LookupServiceRepository : ITransportTypes
    {
        private static IConfiguration _config;
        public LookupServiceRepository(IConfiguration config) { _config = config; }
        public async Task<List<TransportTypes>> GetAllTransportTypes()
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.Query<TransportTypes>(@"SELECT * FROM TransportTypes WHERE TransportTypeID <> 5 ORDER BY [Description]").ToList();
        }

        public async Task<List<TransportTypes>> GetTransportTypes()
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection
            .Query<TransportTypes>(
                @"SELECT *, [Description] + ' - '  + DeliveryType AS DropDownList From TransportTypes WHERE DeliveryType IS NOT NULL ORDER BY [Description]")
            .ToList();
        }
    }
}
