using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triton.Core;
using Triton.Interface.CRM;
using Triton.Model.CRM.Tables;

namespace Triton.Repository.CRM
{
    public class FinanceRepository : IFinance
    {
        public async Task<long> Post(CRNoteTemp crNoteTemp)
        {
            await using var connection = Connection.GetOpenConnection(StringHelpers.Database.Crm);
            return connection.Insert(crNoteTemp);
        }
    }
}
