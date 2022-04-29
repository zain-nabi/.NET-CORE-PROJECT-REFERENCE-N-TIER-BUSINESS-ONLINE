using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Triton.Interface.HR;
using Triton.Model.LeaveManagement.Custom;
using Triton.Model.LeaveManagement.Tables;

namespace Triton.WebApi.Controllers.HR
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Queries used for Employees")]

    public class EmployeeController : ControllerBase
    {
        private readonly IEmployee _repo;

        public EmployeeController(IEmployee repo)
        {
            _repo = repo;
        }

        [HttpGet("{currentEmployeeCode}")]
        [SwaggerOperation(Summary = "Get - Gets the Employee details by employee code", Description = "Returns an Employee object")]
        public async Task<ActionResult<Employees>> Get(string currentEmployeeCode)
        {
            return await _repo.GetEmployee(currentEmployeeCode);
        }

        [HttpGet("ViaTritonSecurity/{tritonSecurityId}")]
        [SwaggerOperation(Summary = "Get - Gets the Employee details by old security", Description = "Returns an Employee object")]
        public async Task<ActionResult<Employees>> GetbyOldSecurityId(int tritonSecurityId)
        {
            return await _repo.GetEmployeeByOldUserId(tritonSecurityId);
        }

        [HttpGet("GetBranchManager/{costCentreId}")]
        [SwaggerOperation(Summary = "GetBranchManager - Gets the branch manager", Description = "Returns an Employee object")]
        public async Task<ActionResult<EmployeeUserMapModel>> GetBranchManager(int costCentreId)
        {
            return await _repo.GetBranchManager(costCentreId);
        }
    }
}