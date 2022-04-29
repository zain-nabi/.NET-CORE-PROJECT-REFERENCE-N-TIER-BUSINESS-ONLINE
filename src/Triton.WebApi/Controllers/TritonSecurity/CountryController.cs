using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Triton.Interface.TritonSecurity;
using Triton.Model.TritonSecurity.Custom;
using Triton.Model.TritonSecurity.Tables;

namespace Triton.WebApi.Controllers.TritonSecurity
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryCurrencySpots _countryCurrencySpots;
        public CountryController(ICountryCurrencySpots countryCurrencySpots)
        {
            _countryCurrencySpots = countryCurrencySpots;
        }

        [HttpGet, Route("CountryCurrencySpots")]
        [SwaggerOperation(Summary = "Get - Gets all the Country Currency spots", Description = "Returns a List<CountryCurrencySpotsModel>")]
        public async Task<ActionResult<List<CountryCurrencySpotsModel>>> Get()
        {
            return await _countryCurrencySpots.GetAsync();
        }

        [HttpPost, Route("CountryCurrencySpots")]
        [SwaggerOperation(Summary = "Post - Creates a new CountryCurrencySpots items", Description = "Returns a <long>")]
        public async Task<ActionResult<long>> Post(CountryCurrencySpots countryCurrencySpots)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _countryCurrencySpots.InsertListAsync(countryCurrencySpots);
        }

        [HttpPut, Route("CountryCurrencySpots")]
        [SwaggerOperation(Summary = "Put - Updates an existing CountryCurrencySpots", Description = "Returns a boolean")]
        public async Task<ActionResult<bool>> Put(List<CountryCurrencySpots> countryCurrencySpots)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _countryCurrencySpots.UpdateAsync(countryCurrencySpots);
        }
    }
}