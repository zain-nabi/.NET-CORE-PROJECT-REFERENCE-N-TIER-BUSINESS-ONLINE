using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Triton.Interface.Mobile;
using Triton.Model.CRM.Custom;

namespace Triton.WebApi.Controllers.TritonMobile
{
    [Route("api/[controller]")]
    [ApiController]
    public class TritonScannerController : ControllerBase
    {
        private readonly ITritonScanner _tritonScannerRepo;

        public TritonScannerController(ITritonScanner tritonScannerRepo)
        {
            _tritonScannerRepo = tritonScannerRepo;
        }

        [HttpGet("{route}")]
        [SwaggerOperation(Summary = "Gets a subset of the delivery manifest for the scanners", Description = "Gets a subset of the delivery manifest for the scanners")]
        public async Task<ActionResult<List<DeviceDeliveryManifest>>> GetDeviceManifest(string route)
        {
            return await _tritonScannerRepo.GetDeviceManifest(route);
        }
    }
}