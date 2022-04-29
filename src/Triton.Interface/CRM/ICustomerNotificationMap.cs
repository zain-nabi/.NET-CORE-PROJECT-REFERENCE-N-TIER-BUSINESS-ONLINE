using System.Collections.Generic;
using System.Threading.Tasks;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.Tables;

namespace Triton.Interface.CRM
{
    public interface ICustomerNotificationMap
    {
        Task<CustomerNotificationMaps> GetCustomerNotificationMapsByCustomerNotificationMapID(int CustomerNotificationMapID);
        Task<bool> PutCustomerNotificationMaps(CustomerNotificationMaps CustomerNotificationMap);
        Task<long> PostCustomerNotificationMaps(CustomerNotificationMaps CustomerNotificationMap);
        Task<List<CustomerNotificationMaps>> GetAsync();
        Task<CustomerNotificationMapsModel> GetComplexCustomerNotificationMapsbyCustomerNotificationMapID(int CustomerNotificationMapID);
        Task<CustomerNotificationMapsModel> GetCustomerNotificationMapsSearch(string CustomerName, string AccountCode);
        Task<CustomerNotificationMapsEditModel> GetCustomerNotificationMapsEditModel(int CustomerNotificationMapID);
        Task<CustomerNotificationMapsEditModel> GetCustomerNotificationMapsCreateModel();
    }
}
