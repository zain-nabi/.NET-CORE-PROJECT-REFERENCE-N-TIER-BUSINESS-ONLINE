using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.StoredProcs;
using Triton.Model.TritonOps.StoredProcs;

namespace Triton.Interface.BusinessOnline
{
    public interface IBusinessOnline
    {
        Task<List<proc_Waybills_CSA_Dashboard_Select>> GetCustomerDashboard(int userId, bool isTritonGroupUserId, DateTime date);
        Task<List<proc_CSA_WaybillList_Select>> GetWaybillsByType(string customerId, string type, bool mobile, DateTime? date);
        Task<proc_CSA_Customer_Select> GetDeliveryStatusCount(string customerIds);
        Task<CSADashboardModel> GetDashboardForCustomerMultiQuery(string customerIds, int userId, bool? isTritonGroupUserId, DateTime? date, string tableName);
        Task<List<proc_Customer_By_CustomerID_Tabs_Select>> GetCustomerDeliveriesByStatus(string customerIds, string type);
       
    }
}
