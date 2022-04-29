using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triton.Model.CRM.Custom;
using Triton.Model.FWWebservice.Custom;
using Vendor.Services.Freightware.PROD.GetPcodeList;

namespace Triton.Interface.Freightware
{
    public interface IFreightware
    {
        //QuoteModels ConvertFromFWQuote(SetQuote.SetQuoteResponseQuote fwQuote,QuoteModels existQuote=null);
        //QuoteModels ConvertFromFWUATQuote(SetQuoteUAT.SetQuoteResponseQuote fwQuote,QuoteModels existQuote=null);
        Task<Vendor.Services.Freightware.PROD.SetQuote.SetQuoteInQuote> ConvertToFWInQuote(QuoteModels quote);
        Task<Vendor.Services.Freightware.UAT.SetQuote.SetQuoteInQuote> ConvertToFWUATInQuote(QuoteModels quote);
        Task<Vendor.Services.Freightware.PROD.SetWaybill.SetWaybillInWaybill> ConvertDepotToDepot_To_FWWaybill(DepotToDepotModels depotToDepot);

        Task<List<GetPcodeListResponsePcodeOutput>> GetPostalList(string SearchPhrase);
        Task<List<GetPcodeListResponsePcodeOutput>> GetPostalListByCode(string Code);

        Task<FWResponsePacket> GetFWWSWaybill(string waybillNo);

        Task<FWResponsePacket> GetFWWSWaybillUAT(string waybillNo);
        Task<List<CustomerPostalCodes>> GetPostalListForCustomer(string SearchPhrase);

        Task<List<CustomerPostalCodes>> GetPostalListByCodeForCustomer(string Code);

        Task<FWResponsePacket> PostWaybillUAT(CustomerWaybillSubmitModels waybill);
        Task<FWResponsePacket> PostWaybillLIVE(CustomerWaybillSubmitModels waybill);
        Task<FWResponsePacket> SetPODLive(FWPodDetails podDetails);
        Task<FWResponsePacket> SetPODUAT (FWPodDetails podDetails);

        Task<List<Vendor.Services.Freightware.UAT.GetCustRateAreaList.GetCustRateAreaListResponseCustRateAreaOutput>> GetFWCustomerRateAreasUAT(string Code);

        Task<FWResponsePacket> SetFWCustRateAreaUAT(Vendor.Services.Freightware.UAT.SetCustRateArea.SetCustRateAreaInCustRateArea area);

        Task<FWResponsePacket> SetFWCustomerRateUAT(Vendor.Services.Freightware.UAT.SetAccountRate.SetAccountRateInRate rate);

        Task<List<Vendor.Services.Freightware.PROD.GetCustRateAreaList.GetCustRateAreaListResponseCustRateAreaOutput>> GetFWCustomerRateAreas(string Code);

        Task<FWResponsePacket> SetFWCustRateArea(Vendor.Services.Freightware.PROD.SetCustRateArea.SetCustRateAreaInCustRateArea area);

        Task<FWResponsePacket> SetFWCustomerRate(Vendor.Services.Freightware.PROD.SetAccountRate.SetAccountRateInRate rate);

        //WayBillInfoModel Convert_FWWaybill_to_ComplexWaybill(Waybill fwWaybill);

        Task<FWResponsePacket> SetCustFWRateIncreaseUAT(FWIncreaseRatesModel model);
        Task<FWResponsePacket> SetCustFWRateIncrease(FWIncreaseRatesModel model);
        Task<FWResponsePacket> GetAccountRatesUAT(string accountCode, bool? active);

        Task<FWResponsePacket> GetSiteList(string customerCode, string siteCode);
        Task<FWResponsePacket> GetSiteListUAT(string customerCode, string siteCode);

        Task<FWResponsePacket> SetSiteUAT(SiteCodeModel model);
        Task<FWResponsePacket> SetSiteProduction(SiteCodeModel model);
    }
}
