using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Triton.Interface.TritonGroup;
using Triton.Model.TritonGroup.Custom;
using Triton.Model.TritonGroup.Tables;

namespace Triton.WebApi.Controllers.TritonGroup
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("User / UserRole / Roles / UserMap")]
    public class ExternalUserController : ControllerBase
    {
        private readonly IExternalUser _externalUser;
        private readonly IExternalUserMap _externalUserMap;
        private readonly IExternalUserRole _externalUserRole;


        public ExternalUserController(IExternalUser externalUser, IExternalUserRole externalUserRole, IExternalUserMap externalUserMap)
        {
            _externalUser = externalUser;
            _externalUserMap = externalUserMap;
            _externalUserRole = externalUserRole;
        }

        #region ====================  User  ====================
        [HttpGet("{username}")]
        [SwaggerOperation(Summary = "FindByNameAsync - Gets the user by their username", Description = "Returns a single user")]
        public async Task<ActionResult<ExternalUser>> Get(string username)
        {
            return await _externalUser.FindByNameAsync(username);
        }

        [HttpGet("{userId:int}")]
        [SwaggerOperation(Summary = "FindByIdAsync - Gets the user by their UserID", Description = "Returns a single user")]
        public async Task<ActionResult<ExternalUser>> Get(int userId)
        {
            return await _externalUser.FindByIdAsync(userId);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "GetAllUsersInclLockedOut - Gets all the users including locked out users", Description = "Returns a list of users")]
        public async Task<ActionResult<List<ExternalUser>>> Get()
        {
            return await _externalUser.GetAllUsersInclLockedOut();
        }


        [HttpPost, Route("CreateAsync")]
        [SwaggerOperation(Summary = "CreateAsync - Creates a new user and returns the user object", Description = "Returns a user object")]
        public async Task<ActionResult<ExternalUser>> CreateAsync(ExternalUser user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _externalUser.CreateAsync(user);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Post - Creates a new user and returns a UserID", Description = "Returns a UserID")]
        public async Task<ActionResult<long>> Post(ExternalUser user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _externalUser.Post(user);
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Put - Updates an existing user", Description = "Returns a boolean")]
        public async Task<ActionResult<bool>> Put(ExternalUser user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _externalUser.PutUpdateAsync(user);
        }

        [HttpGet("CheckIfEmailExist/{email}")]
        [SwaggerOperation(Summary = "CheckIfEmailExist - Checks if email exists", Description = "Returns true/false")]
        public async Task<ActionResult<ExternalUser>> CheckIfEmailExist(string email)
        {
            return await _externalUser.CheckIfEmailExist(email);
        }

        [HttpGet("GetUserWithRoles")]
        [SwaggerOperation(Summary = "Users - Gets all users", Description = "Returns true/false")]
        public async Task<ActionResult<List<ExternalUserModel>>> GetUserWithRoles()
        {
            return await _externalUser.GetUserWithRoles();
        }

        [HttpGet("FindByExternalUserID/{externalUserID}")]
        [SwaggerOperation(Summary = "FindByExternalUserID - Finds external user by ID", Description = "Returns true/false")]
        public async Task<ActionResult<ExternalUserModel>> FindByExternalUserID(int externalUserID)
        {
            return await _externalUser.FindByExternalUserID(externalUserID);
        }

        #endregion

        #region ====================  UserRole  ====================
        [HttpGet("UserRoles/{userRoleId:int}")]
        [SwaggerOperation(Summary = "GetUserRole - Gets the UserRoles by UserRoleID", Description = "Returns a single UserRole")]
        public async Task<ActionResult<ExternalUserRole>> GetUserRole(int userRoleId)
        {
            return await _externalUserRole.GetUserRole(userRoleId);
        }

        [HttpPost("UserRoles")]
        [SwaggerOperation(Summary = "Post - Creates a new user role", Description = "Returns a long")]
        public async Task<ActionResult<long>> Post(ExternalUserRole userRole, string dbName)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _externalUserRole.Post(userRole, dbName);
        }

        [HttpPut("UserRoles")]
        [SwaggerOperation(Summary = "Put - Updates the user role", Description = "Returns a boolean")]
        public async Task<ActionResult<bool>> Put(ExternalUserRole userRole, string dbName)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _externalUserRole.Put(userRole, dbName);
        }

        [HttpGet("GetUserRoleByID/{externalUserID:int}")]
        [SwaggerOperation(Summary = "GetUserRoleByID - Gets the UserRoles by ExternalUserRoleID", Description = "Returns a single UserRole")]
        public async Task<ActionResult<ExternalUserRole>> GetUserRoleByID(int externalUserID)
        {
            return await _externalUserRole.GetUserRoleByID(externalUserID);
        }

        #endregion

        #region ====================  Roles  ====================
        [HttpGet, Route("Roles/{userId:int}/{dbName}")]
        [SwaggerOperation(Summary = "GetRolesByUserId - Gets all the roles for the users", Description = "Returns a list of Roles")]
        public async Task<ActionResult<List<Roles>>> Get(int userId, string dbName)
        {
            return await _externalUserRole.GetRolesByUserId(userId, dbName);
        }

        [HttpGet, Route("Roles/{roleIDs}/{dbName}")]
        [SwaggerOperation(Summary = "GetRolesByIds - Gets all the roles by ID's", Description = "Returns a list of Roles")]
        public async Task<ActionResult<List<Roles>>> Get(string roleIDs, string dbName)
        {
            return await _externalUserRole.GetRolesByIds(roleIDs, dbName);
        }

        [HttpGet, Route("GetActiveUserRoles")]
        [SwaggerOperation(Summary = "GetActiveUserRoles - Gets all the roles", Description = "Returns a list of Roles")]
        public async Task<ActionResult<List<Roles>>> GetActiveUserRoles()
        {
            return await _externalUserRole.GetActiveUserRoles();
        }
        #endregion

        #region ====================  UserMap  ====================
        [HttpGet("GetUserMap/{userMapId:int}")]
        [SwaggerOperation(Summary = "GetUserMap - Gets the user information based on the user type", Description = "Returns a single UserMap")]
        public async Task<ActionResult<ExternalUserMap>> GetUserMap(int userMapId)
        {
            return await _externalUserMap.GetUserMap(userMapId);
        }

        [HttpGet("UserMap/GetUserMapByUserType/{userTypeLcid:int}")]
        [SwaggerOperation(Summary = "GetUserMapByUserType - Gets the user information based on the user type", Description = "Returns a list of Users")]
        public async Task<ActionResult<List<ExternalUser>>> GetUserMapByUserType(int userTypeLcid)
        {
            return await _externalUserMap.GetUserMapByUserType(userTypeLcid);
        }

        [HttpGet("{userId:int}/GetUserCustomerMapModel")]
        [SwaggerOperation(Summary = "GetUserCustomerMapModel - Gets the user customer map with customer model", Description = "Returns a complex model that contains List<UserMap>, List<Customers>")]
        public async Task<ActionResult<ExternalUserMapCustomerModels>> GetUserCustomerMapModel(int userId)
        {
            return await _externalUserMap.GetUserCustomerMapModel(userId);
        }

        [HttpPost("UserMap")]
        [SwaggerOperation(Summary = "PostUserMap - Insert a new object into UserMaps", Description = "Returns a string indicating the status of the insert")]
        public async Task<ActionResult<string>> PostUserMap(ExternalUserMap userMap)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _externalUserMap.PostUserMapObject(userMap);
        }

        [HttpPut("UserMap")]
        [SwaggerOperation(Summary = "PutUserMap - Update an existing UserMap record", Description = "Returns a boolean")]
        public async Task<ActionResult<bool>> PutUserMap(ExternalUserMap userMap)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (userMap.ExternalUserID == 0) return BadRequest("UserID is 0");
            if (userMap.UserTypeLCID == 0) return BadRequest("UserTypeLCID is 0");
            if (userMap.CreatedByUserID == 0) return BadRequest("CreatedByUserID is 0");

            return await _externalUserMap.PutUserMap(userMap);
        }

        [HttpGet("GetUserMapCustomers/{externalUserID}")]
        [SwaggerOperation(Summary = "GetUserMapCustomers - Gets selected customers by ID", Description = "Returns a CustomerID and CustomerName")]
        public async Task<ActionResult<List<ExternalUserMapModel>>> GetUserMapCustomers(int externalUserID)
        {
            return await _externalUserMap.GetUserMapCustomers(externalUserID);
        }

        [HttpPut("UpdateExternalUserMap")]
        [SwaggerOperation(Summary = "UpdateExternalUserMap - Updates the externalusermap table", Description = "Returns true/false")]
        public async Task<ActionResult<bool>> UpdateExternalUserMap(ExternalUserMapModel model)
        {
            return await _externalUserMap.UpdateExternalUserMap(model);
        }

        [HttpPost("InsertExternalUserMap")]
        [SwaggerOperation(Summary = "InsertExternalUserMap - Inserts the externalusermap table", Description = "Returns true/false")]
        public async Task<ActionResult<string>> InsertExternalUserMap(ExternalUserMapModel model)
        {
            return await _externalUserMap.InsertExternalUserMap(model);
        }

        #endregion
    }
}
