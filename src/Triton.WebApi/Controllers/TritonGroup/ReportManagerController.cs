using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Triton.Interface.TritonGroup;
using Triton.Model.TritonGroup.Tables;

namespace Triton.WebApi.Controllers.TritonGroup
{
    [Route("api/[controller]")]
    [ApiController]
    // [SwaggerTag("ReportManager")]
    public class ReportManagerController : ControllerBase
    {
        private readonly IReportManager _reportManager;

        public ReportManagerController(IReportManager reportManager)
        {
            _reportManager = reportManager;
        }

        [HttpGet("{roleIds}")]
        [SwaggerOperation(Summary = "Get - Gets all the reports by roleIds", Description = "Returns a List<ReportManager>")]
        public async Task<ActionResult<List<ReportManager>>> Get(string roleIds)
        {
            return await _reportManager.Get(roleIds);
        }

        [HttpGet("{systemId:int}/{categoryLciDs:int}/{roleIds}")]
        [SwaggerOperation(Summary = "Get - Gets all the reports by categoryLciDs, systemId, roleIds", Description = "Returns a List<ReportManager>")]
        public async Task<ActionResult<List<ReportManager>>> Get(int systemId, string categoryLciDs, string roleIds)
        {
            return await _reportManager.Get(systemId, categoryLciDs, roleIds);
        }
        [HttpGet("{reportManagerId}")]
        [SwaggerOperation(Summary = "Get - Gets Report Manager", Description = "Returns <ReportManager>")]
        public async Task<ActionResult<ReportManager>> GetReport(int reportManagerId)
        {
            return await _reportManager.GetReport(reportManagerId);
        }
    }
}
