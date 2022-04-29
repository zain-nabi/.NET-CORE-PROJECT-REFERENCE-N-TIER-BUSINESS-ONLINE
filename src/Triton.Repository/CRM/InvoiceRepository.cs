using Dapper;
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
using Triton.Model.CRM.StoredProcs;
using Triton.Model.CRM.Tables;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Repository.CRM
{
    public class InvoiceRepository : IInvoices
    {
         private static IConfiguration _config;
        public InvoiceRepository(IConfiguration config) { _config = config; }

        [Obsolete("This method is specific to the api for the interface and should not be used.", true)]
        public Task<DocumentRepositories> GetExcelInvoice(string invoiceNumber, int customerId, DateTime? startDate = null, DateTime? endDate = null)
        {
            throw new NotImplementedException();
        }

        public async Task<InvoiceSearchModel> GetInvoiceNo(string InvoiceNo, int CustomerID, DateTime? StartDate, DateTime? EndDate)
        {
            var I = new InvoiceSearchModel();
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                const string sql = "proc_GetinvoiceNo @InvoiceNo, @CustomerID, @StartDate, @EndDate";
                using var multi = connection.QueryMultiple(sql,new {InvoiceNo, CustomerID, StartDate, EndDate});
                I.proc_InvoiceNo= multi.Read<proc_GetinvoiceNo>().ToList();
            }
            return I;
        }

        public async Task<Invoices> GetInvoicesById(int InvoiceId)
        {
             await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.Query<Invoices>($@"SELECT * FROM Invoices WHERE InvoiceID ={InvoiceId}", new { InvoiceId}).FirstOrDefault();
        }
    }
}

