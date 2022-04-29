using System;
using System.Threading.Tasks;

namespace Triton.Interface.Documents
{
    public interface IDocuments
    {
        Task<byte[]> GeteDocsImageByCustomerID(string WaybillNo,string NodeName);
        Task<byte[]> GetCustomerStatementByAccountCodeandPeriod(string AccountCode,DateTime Period);
        Task<byte[]> GetCustomerInvoicebyInvoiceNumber(string InvoiceNumber);
    }
}
