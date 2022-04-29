using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.Tables;
using Triton.Model.FWWebservice.Custom;

namespace Triton.Interface.CRM
{
    public interface ISites
    {
        Task<List<Sites>> GetSitesByCustomerId(int customerId,bool? savedSitesOnly=true,string dbName="CRM");
        Task<Sites> Get(int SiteId,string dbName="CRM");
        //Sites GetSiteFWBySiteID_CustomerID(int SiteID, int CustomerID);
        
        Task<bool> PutSite(Sites site,string dbName="CRM");
        Task<long> PostSite(Sites sites,string dbName="CRM");
        //bool PutBothSites(SiteFWUpdate site);
        //ComboResponsePacket PutBothSites(TritonSiteFWUpdate site);

        Task<List<Sites>> GetSitesBySiteNameAndSuburbForCustomer(string siteName, string suburb, int customerID, string dbName);
        Task<FWResponsePacket> SetSiteUAT(SiteCodeModel model, string dbName = "CRM");
        Task<FWResponsePacket> SetSiteProduction(SiteCodeModel model);        
        Task<long> PostSiteAsync(Sites model);
        //List<SiteFromRoute> GetCRMSitesByRouteIDandDates(int RouteID, DateTime FromDate, DateTime ToDate);
        //bool RemoveCRMCustomerSiteMap(SiteFWUpdate site);
        //ComboResponsePacket RemoveFWCustomerSite(TritonSiteFWUpdate site);
        //List<FWCustomerSites> GetFWCustomerSitesbyCustomerID(int customerID);
        //List<TritonSiteFromRoute> GetFWCustomerSitesByRouteIDandDates(int routeID, DateTime fromDate, DateTime toDate);
        //FWCustomerSites GetCRMFWCustomerSiteByFWCustomerSiteID(int fWCustomerSiteID);
    }
}
