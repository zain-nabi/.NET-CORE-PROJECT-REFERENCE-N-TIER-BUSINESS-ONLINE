using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vendor.Services.Freightware.PROD.GetStatement;

namespace Triton.Interface.CRM
{
    public interface IStatements
    {
        Task<GetStatementResponseStatementOutput> GetCustomerStatement(int customerId,DateTime period);
    }
}
