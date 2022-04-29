using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Triton.Core;
using Triton.Interface.CRM;
using Triton.Model.CRM.Tables;

namespace Triton.Repository.CRM
{
    public class InsuranceTypesRepository: IInsuranceTypes
    {
        private static IConfiguration _config;
        public InsuranceTypesRepository(IConfiguration config) { _config = config; }

        public async Task<List<InsuranceTypes>> GetInsuranceTypes()
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.Query<InsuranceTypes>("SELECT * FROM InsuranceTypes").ToList();
        }

        public async Task<InsuranceTypes> GetInsuranceTypesById(int InsuranceTypeId)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.Query<InsuranceTypes>($"SELECT * FROM InsuranceTypes WHERE InsuranceTypeID = {InsuranceTypeId}").FirstOrDefault();
        }

        public async Task<long> Post(InsuranceTypes insuranceTypes)
        {
            await using var connection = Connection.GetOpenConnection(StringHelpers.Database.Crm);
            return connection.Insert(insuranceTypes);
        }

        public async Task<bool> Put(InsuranceTypes insuranceTypes)
        {
            await using var connection = Connection.GetOpenConnection(StringHelpers.Database.Crm);
            return connection.Update(insuranceTypes);
        }
    }
}
