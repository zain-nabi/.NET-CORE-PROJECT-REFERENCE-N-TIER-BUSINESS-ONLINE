using System.Collections.Generic;
using System.Threading.Tasks;
using Triton.Model.CRM.StoredProcs;
using Triton.Model.TritonGroup.Custom;
using Customers = Triton.Model.CRM.Tables.Customers;

namespace Triton.Interface.CRM
{
    public interface ICustomers
    {
        public Task<List<Customers>> GetAllActiveCustomers();

        public Task<Customers> GetCrmCustomerById(int id);
        Task<Customers> GetCRMCustomerByAccountCode(string accountCode,string dbName="CRM");

        Task<List<Customers>> FindCrmCustomer(string search);

        Task<bool> PutCustomer(Customers customer);

        Task<UserMapCustomerModels> GetUserMap_With_Customers(int userid);
        Task<List<Customers>> GetCrmCustomersByRepUserId(int userId);
        Task<List<Customers>> GetCrmInternalAccounts();
        Task<CustomerAssessment> GetCustomerAssessment(int customerId, string periodFrom, string periodTo, int? branch = null, int? rep = null);
        Task<List<Customers>> FindCrmCustomerByIds(string customerIds);
    }
}
