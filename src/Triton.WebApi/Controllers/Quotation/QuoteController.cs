using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using Triton.WebApi.Controllers.Freightware;
using Vendor.Services.CustomModels;
using Vendor.Services.Freightware.PROD.GetChargesList;
using Vendor.Services.Helpers;

namespace Triton.WebApi.Controllers.Quotation
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IVendorServices _converter;

        public QuotesController(IConfiguration config,IVendorServices converter)
        {
            _configuration=config;
            _converter=converter;
        }

        /// <summary>
        /// Gets available surcharges
        /// </summary>
        /// <returns></returns>
        [HttpGet("Surcharges")]
        [SwaggerOperation(Summary = "Surcharges", Description = "Get available surcharge for quotations" )]
        public async Task<ActionResult<List<GetChargesListResponseChargesOutput>>> GetSurcharges()
        {
            FreightwareController fc = new FreightwareController(_configuration);
            var charges = await fc.Get();
            //Filter only for the ones we want for Quotes ie Sat Collection , delivery etc
            List<GetChargesListResponseChargesOutput> chargeList = charges.Value.DataObject;
            var filtered = chargeList.FindAll(x=> x.Heading.Contains("SAT") || x.Heading.Contains("EARLY"));
            return filtered;
        }

         /// <summary>
        /// Post a Quote
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Post Quote", Description = "Post a Quote Object" )]

        public async Task<ActionResult<long>> Post(VendorQuoteModel model)
        {
            var fwQuote =await _converter.ConvertToFWUATInQuote(model);
            //var quote = _converter.ConvertFromFWUATQuote(fwQuote);
            return 1;
        }
    }
}