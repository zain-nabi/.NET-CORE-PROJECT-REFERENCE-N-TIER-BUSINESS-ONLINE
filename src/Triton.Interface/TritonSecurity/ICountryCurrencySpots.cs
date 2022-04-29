using System.Collections.Generic;
using System.Threading.Tasks;
using Triton.Model.TritonSecurity.Custom;
using Triton.Model.TritonSecurity.Tables;

namespace Triton.Interface.TritonSecurity
{
    public interface ICountryCurrencySpots
    {
        Task<List<CountryCurrencySpotsModel>> GetAsync();

        Task<long> InsertListAsync(CountryCurrencySpots countryCurrencySpots);

        Task<bool> UpdateAsync(List<CountryCurrencySpots> countryCurrencySpots);
    }
}
