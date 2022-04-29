using System.Collections.Generic;
using System.Threading.Tasks;
using Triton.Model.FWWebservice.Custom;
using Triton.Model.TritonGroup.Tables;
using Vendor.Services.CustomModels;
using Vendor.Services.Freightware.PROD.GetChargesList;

namespace Triton.Interface.CRM
{
    public interface IQuotesAPI
    {
         Task<DocumentRepositories> GetQuoteDocument(int quoteID);
         Task<byte[]> EmailQuoteDocument(int quoteID,string email);
         Task<List<GetChargesListResponseChargesOutput>> GetQuoteSurcharges();
         Task<FWResponsePacket> PostQuoteUAT(VendorQuoteModel quoteModel,string dbName="CRM");
         Task<FWResponsePacket> PostQuoteProduction(VendorQuoteModel quoteModel, string dbName = "CRM");
        Task<byte[]> EmailQuoteDocument(int quoteId, string email, int UserID);
    }
}
