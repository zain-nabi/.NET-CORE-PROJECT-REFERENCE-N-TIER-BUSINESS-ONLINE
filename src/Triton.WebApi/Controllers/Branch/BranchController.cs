using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Triton.Interface.TritonSecurity;
using Triton.Model.TritonSecurity.Tables;

namespace Triton.WebApi.Controllers.Branch
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Branches")]

    public class BranchController : ControllerBase
    {
        private readonly IBranches _branch;
        public BranchController(IBranches branch)
        {
            _branch = branch;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "GetAllActiveBranches - Gets all the active branches", Description = "Returns a List<Branches>")]
        public async Task<ActionResult<List<Branches>>> GetAllActiveBranches()
        {
            return await _branch.GetAllActiveBranches();
        }

        [HttpGet("User/{userId}")]
        [SwaggerOperation(Summary = "GetUserBranch - Gets the BranchID of the user", Description = "Returns a single branch")]
        public async Task<ActionResult<Branches>> GetUserBranch(int userId)
        {
            return await _branch.GetUserBranch(userId);
        }
    }
}