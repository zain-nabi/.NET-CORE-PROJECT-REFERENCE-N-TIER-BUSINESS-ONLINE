using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.StoredProcs;
using Triton.Model.CRM.Tables;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Interface.CRM
{
    public interface IInvoices
    {
        Task<Invoices>GetInvoicesById(int InvoiceId);
        Task<InvoiceSearchModel> GetInvoiceNo(string InvoiceNo, int CustomerID, DateTime? StartDate, DateTime? EndDate);
        Task<DocumentRepositories> GetExcelInvoice(string invoiceNumber,int customerId,DateTime? startDate=null,DateTime? endDate=null);
    }
}
