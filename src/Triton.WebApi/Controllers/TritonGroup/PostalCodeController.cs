using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Triton.Interface.TritonGroup;
using Triton.Model.TritonExpress.Tables;

namespace Triton.WebApi.Controllers.TritonGroup
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostalCodeController : ControllerBase
    {
        private readonly IPostalCodes _postalCode;
        public PostalCodeController(IPostalCodes postalCodes )
        {
            _postalCode=postalCodes;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get Postalcode by name", Description = "Get Postalcode by name")]
        public async Task<ActionResult<List<PostalCodes>>> Get(string name)
        {
            return await _postalCode.GetPostalCodes(name);
        }

        [Route("GetPostalCodeByCode/{code}")]
        [HttpGet]
        [SwaggerOperation(Summary = "Get Postalcode by code", Description = "Get Postalcode by code")]
        public async Task<ActionResult<List<PostalCodes>>> GetPostalCodeByCode(string code)
        {
            return await _postalCode.GetPostalCodesByCode(code);
        }
    }
}