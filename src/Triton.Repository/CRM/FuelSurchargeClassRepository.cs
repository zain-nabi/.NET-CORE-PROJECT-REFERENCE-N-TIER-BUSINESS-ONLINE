using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Triton.Core;
using Triton.Interface.CRM;
using Triton.Model.CRM.Tables;

namespace Triton.Repository.CRM
{
    public class FuelSurchargeClassRepository : IFuelSurchargeClass
    {
        private readonly IConfiguration _config;

        public FuelSurchargeClassRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<List<FuelSurchargeClasss>> GetAsync()
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.GetAll<FuelSurchargeClasss>().ToList();
        }

        public async Task<bool> UpdateAsync(List<FuelSurchargeClasss> fuelSurchargeClass)
        {
            try
            {
                // Scope transaction
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                // Set the connection
                await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
                const string sql = "SELECT TOP 1 DATEADD(MONTH, 1, GETDATE()) AS MonthValid FROM FuelSurchargeClassAudits ORDER BY MonthValid DESC";
                var fuelAudit = await connection.QueryFirstAsync<FuelSurchargeClassAudits>(sql);

                foreach (var item in fuelSurchargeClass)
                {
                    // Update the record async
                    var fuelSurchargeClassId = await connection.UpdateAsync(fuelSurchargeClass).ConfigureAwait(false);
                    var fuelSurchargeClassAudit = new FuelSurchargeClassAudits
                    {
                        Code = item.Code,
                        Description = item.Description,
                        CurrentValue = item.CurrentValue,
                        MinumumValue = item.MinumumValue,
                        FuelSurchargeClassID = item.FuelSurchargeClassID,
                        MonthValid = fuelAudit.MonthValid
                    };
                    _ = await connection.InsertAsync(fuelSurchargeClassAudit);
                }

                // Save the record
                scope.Complete();

                // Return success
                return true;
            }
            catch //(Exception e)
            {
                // Log error
                return false;
            }
        }
    }
}
