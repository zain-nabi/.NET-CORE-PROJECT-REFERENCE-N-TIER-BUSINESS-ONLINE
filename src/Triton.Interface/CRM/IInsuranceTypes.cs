using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triton.Model.CRM.Tables;

namespace Triton.Interface.CRM
{
    public interface IInsuranceTypes
    {
        Task<List<InsuranceTypes>> GetInsuranceTypes();
        Task<InsuranceTypes> GetInsuranceTypesById(int InsuranceTypeId);
        Task<long> Post(InsuranceTypes insuranceTypes);
        Task<bool> Put(InsuranceTypes insuranceTypes);
    }
}
