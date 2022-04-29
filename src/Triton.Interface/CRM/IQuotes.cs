using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.Tables;
using Vendor.Services.CustomModels;

namespace Triton.Interface.CRM
{
    public interface IQuotes
    {
        Task<long> PostQuote(VendorQuoteModel quoteModel,string dbName="CRM");
        Task<long> PostQuotationEmail(QuotationTracker quotationTracker, string dbName = "CRM");
        Task<VendorQuoteModel> GetQuoteByID(long quoteID,string dbName="CRM");
        Task<VendorQuoteModel> GetQuoteByQuoteNumber(string quoteNumber,string dbName="CRM");
        Task<VendorQuoteSearchModel> GetQuotesbyCustomerIdOptRefandDates(int customerId,string referance,DateTime? from,DateTime? to,string dbName="CRM");
        //Task<TransportPriceResultModels> GetTransportPrice(TransportPriceSubmitModels quote);
       
    }
}
