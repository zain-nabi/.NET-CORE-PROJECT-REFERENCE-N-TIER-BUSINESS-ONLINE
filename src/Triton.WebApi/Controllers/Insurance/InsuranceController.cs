using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Triton.Interface.CRM;
using Triton.Model.CRM.Tables;
using Triton.Repository.CRM;

namespace Triton.WebApi.Controllers.Insurance
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Insurance Types")]
    public class InsuranceController : ControllerBase
    {
        private readonly IInsuranceTypes _insuranceType;

        public InsuranceController(IInsuranceTypes insuranceTypes)
        {
            _insuranceType = insuranceTypes;
        }

        #region Insurance Type
        [HttpGet("InsuranceTypes")]
        [SwaggerOperation(Summary ="Get All insurance Types",Description ="Get all insurance Types")]
        public async Task<ActionResult<List<InsuranceTypes>>> Get()
        {
            return await _insuranceType.GetInsuranceTypes(); 
        }

        [HttpGet("InsuranceTypes/{insuranceTypeId}")]
        [SwaggerOperation(Summary = "Get All insurance Types By Id", Description = "Get all insurance Types")]
        public async Task<ActionResult<InsuranceTypes>> GetInsuranceTypeById(int insuranceTypeId)
        {
            return await _insuranceType.GetInsuranceTypesById(insuranceTypeId);
        }


        [HttpPost("PostInsuranceTypes")]
        [SwaggerOperation(Summary = "Post - Creates a new insurance type", Description = "Returns a InsuranceTypeId")]
        public async Task<ActionResult<long>> Post(InsuranceTypes insuranceTypes)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _insuranceType.Post(insuranceTypes);
        }

        [HttpPut("PutInsuranceTypes")]
        [SwaggerOperation(Summary = "Post - Update a insurance type", Description = "Returns a InsuranceTypeId")]
        public async Task<ActionResult<bool>> Put(InsuranceTypes insuranceTypes)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _insuranceType.Put(insuranceTypes);
        }
        #endregion
    }
}
