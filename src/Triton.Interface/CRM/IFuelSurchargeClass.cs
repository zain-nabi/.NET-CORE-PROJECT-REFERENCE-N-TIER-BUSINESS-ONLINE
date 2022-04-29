using System.Collections.Generic;
using System.Threading.Tasks;
using Triton.Model.CRM.Tables;

namespace Triton.Interface.CRM
{
    public interface IFuelSurchargeClass
    {
        Task<List<FuelSurchargeClasss>> GetAsync();
        Task<bool> UpdateAsync(List<FuelSurchargeClasss> fuelSurchargeClass);
    }
}
