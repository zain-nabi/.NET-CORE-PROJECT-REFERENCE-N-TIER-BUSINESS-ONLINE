using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Triton.Interface.CRM;
using Triton.Model.CRM.Tables;

namespace Triton.WebApi.Controllers.CRM
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuelSurchargeClassController : ControllerBase
    {
        private readonly IFuelSurchargeClass _fuelSurchargeClass;

        public FuelSurchargeClassController(IFuelSurchargeClass fuelSurchargeClass)
        {
            _fuelSurchargeClass = fuelSurchargeClass;
        }
        [HttpGet]
        [SwaggerOperation(Summary ="Get all FuelSurchargeClass",Description ="Returns List<FuelSurchargeClass>")]
        public async Task<ActionResult<List<FuelSurchargeClasss>>> Get()
        {
            return await _fuelSurchargeClass.GetAsync(); 
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Put - Updates the FuelSurchargeClass table", Description = "Returns a bool")]
        public async Task<ActionResult<bool>> Put(List<FuelSurchargeClasss> fuelSurchargeClass)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _fuelSurchargeClass.UpdateAsync(fuelSurchargeClass);
        }
    }
}