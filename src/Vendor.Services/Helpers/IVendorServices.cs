using System.Threading.Tasks;
using Triton.Model.CRM.Custom;
using Vendor.Services.CustomModels;
using Vendor.Services.Freightware.PROD.SetQuote;

namespace Vendor.Services.Helpers
{
    public interface IVendorServices
    {
        Task<VendorQuoteModel> ConvertFromFWQuote(SetQuoteResponseQuote fwQuote, VendorQuoteModel existQuote=null);
        Task<SetQuoteInQuote> ConvertToFWInQuote(VendorQuoteModel quote);
        Task<VendorQuoteModel> ConvertFromFWUATQuote(Freightware.UAT.SetQuote.SetQuoteResponseQuote fwQuote, VendorQuoteModel existQuote=null);
        Task<Freightware.UAT.SetQuote.SetQuoteInQuote> ConvertToFWUATInQuote(VendorQuoteModel quote);
        Freightware.UAT.SetWaybill.SetWaybillInWaybill ConvertDepotToDepot_To_FWWaybill(DepotToDepotModels depotToDepot);
        WaybillCustomerModel CovertFWWaybillToCustomerWaybill(dynamic outWaybill);
    }
}
