using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Interface.TritonGroup
{
    public interface ICustomerSiteMaps
    {
        Task<bool> PostCustomerSiteMap(CustomerSiteMaps customerSiteMap,String dbName="TritonGroup");
        Task<bool> PutCustomerSiteMap(CustomerSiteMaps customerSiteMap, String dbName = "TritonGroup");

        Task<CustomerSiteMaps> GetCustomerSiteMap(long customerSiteMapID, String dbName = "TritonGroup");

        Task<CustomerSiteMaps> GetCustomerSiteMapsByCustomerID_And_SiteID(int customerID, int siteID, String dbName = "TritonGroup");
    }
}
