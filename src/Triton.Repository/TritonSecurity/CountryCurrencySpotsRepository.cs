using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triton.Core;
using Triton.Interface.TritonSecurity;
using Triton.Model.TritonSecurity.Custom;
using Triton.Model.TritonSecurity.Tables;

namespace Triton.Repository.TritonSecurity
{
    public class CountryCurrencySpotsRepository : ICountryCurrencySpots
    {
        private readonly IConfiguration _config;

        public CountryCurrencySpotsRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<List<CountryCurrencySpotsModel>> GetAsync()
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonSecurity));
            const string sql = "SELECT * FROM CountryCurrencySpots CCS INNER JOIN Countrys C ON C.CountryID = CCS.CountryID";
            var lstModel = new List<CountryCurrencySpotsModel>();

            var data = connection.Query<CountryCurrencySpots, Countrys, List<CountryCurrencySpotsModel>>(
                    sql, (countryCurrencySpots, country) =>
                    {
                        var model = new CountryCurrencySpotsModel
                        {
                            CountryCurrencySpots = countryCurrencySpots,
                            Countrys = country
                        };

                        lstModel.Add(model);
                        return lstModel;
                    },
                    splitOn: "CountryID").FirstOrDefault();
            return data;
        }

        public async Task<long> InsertListAsync(CountryCurrencySpots countryCurrencySpots)
        {
            return await RepositoryHelper.InsertAsync(countryCurrencySpots, _config.GetConnectionString(StringHelpers.Database.TritonSecurity));
        }

        public async Task<bool> UpdateAsync(List<CountryCurrencySpots> countryCurrencySpots)
        {
            return await RepositoryHelper.UpdateAsync(countryCurrencySpots, _config.GetConnectionString(StringHelpers.Database.TritonSecurity));
        }
    }
}
