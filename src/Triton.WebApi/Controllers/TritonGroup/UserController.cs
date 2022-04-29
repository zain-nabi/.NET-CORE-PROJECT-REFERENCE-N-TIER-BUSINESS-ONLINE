using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Triton.Interface.TritonGroup;
using Triton.Model.TritonGroup.Custom;
using Triton.Model.TritonGroup.StoredProcs;
using Triton.Model.TritonGroup.Tables;

namespace Triton.WebApi.Controllers.TritonGroup
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Users / Roles / UserRoles / UserMap / UserCustomerMap")]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IUserRole _userRole;
        private readonly IRole _role;
        private readonly IUserCustomerMap _userCustomerMap;
        private readonly IUserMap _userMap;

        public UserController(IUser user, IUserRole userRole, IRole role, IUserCustomerMap userCustomerMap, IUserMap userMap)
        {
            _user = user;
            _userRole = userRole;
            _role = role;
            _userCustomerMap = userCustomerMap;
            _userMap = userMap;
        }

        #region ====================  User  ====================
        [HttpGet("{username}")]
        [SwaggerOperation(Summary = "FindByNameAsync - Gets the user by their username", Description = "Returns a single user")]
        public async Task<ActionResult<Users>> Get(string username)
        {
            return await _user.FindByNameAsync(username);
        }

        [HttpGet("{userId:int}")]
        [SwaggerOperation(Summary = "FindByIdAsync - Gets the user by their UserID", Description = "Returns a single user")]
        public async Task<ActionResult<Users>> Get(int userId)
        {
            return await _user.FindByIdAsync(userId);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "GetAllUsersInclLockedOut - Gets all the users including locked out users", Description = "Returns a list of users")]
        public async Task<ActionResult<List<Users>>> Get()
        {
            return await _user.GetAllUsersInclLockedOut();
        }

        [HttpGet("GetUserWithRoles/{userId:int}/{roleIds?}")]
        [SwaggerOperation(Summary = "GetUserWithRoles - Gets the User/UserRoles and Roles for creating views", Description = "Returns a UserWithRoles model")]
        public async Task<ActionResult<UserWithRoles>> GetUserWithRoles(int userId, string roleIds = null)
        {
            return await _user.GetUserWithRoles(userId, roleIds);
        }

        [HttpGet("{sAmAccountName}/{database}")]
        [SwaggerOperation(Summary = "Users - Gets the Users information including employee, branch, roles etc", Description = "Returns a UserWithRoles model")]
        public async Task<ActionResult<UserInformation>> UserInformation(string sAmAccountName, string database)
        {
            return await _user.FindBysAmAccountName(sAmAccountName, database);
        }

        [HttpPost, Route("CreateAsync")]
        [SwaggerOperation(Summary = "CreateAsync - Creates a new user and returns the user object", Description = "Returns a user object")]
        public async Task<ActionResult<Users>> CreateAsync(Users user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _user.CreateAsync(user);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Post - Creates a new user and returns a UserID", Description = "Returns a UserID")]
        public async Task<ActionResult<long>> Post(Users user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _user.Post(user);
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Put - Updates an existing user", Description = "Returns a boolean")]
        public async Task<ActionResult<bool>> Put(Users user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _user.PutUpdateAsync(user);
        }
        #endregion

        #region ====================  UserRole  ====================
        [HttpGet("UserRoles/{userRoleId:int}")]
        [SwaggerOperation(Summary = "GetUserRole - Gets the UserRoles by UserRoleID", Description = "Returns a single UserRole")]
        public async Task<ActionResult<UserRoles>> GetUserRole(int userRoleId)
        {
            return await _userRole.GetUserRole(userRoleId);
        }

        [HttpPost("UserRoles")]
        [SwaggerOperation(Summary = "Post - Creates a new user role", Description = "Returns a long")]
        public async Task<ActionResult<long>> Post(UserRoles userRole, string dbName)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _userRole.Post(userRole, dbName);
        }

        [HttpPut("UserRoles")]
        [SwaggerOperation(Summary = "Put - Updates the user role", Description = "Returns a boolean")]
        public async Task<ActionResult<bool>> Put(UserRoles userRole, string dbName)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _userRole.Put(userRole, dbName);
        }

        #endregion

        #region ====================  Roles  ====================
        [HttpGet, Route("Roles/{userId:int}/{dbName}")]
        [SwaggerOperation(Summary = "GetRolesByUserId - Gets all the roles for the users", Description = "Returns a list of Roles")]
        public async Task<ActionResult<List<Roles>>> Get(int userId, string dbName)
        {
            return await _role.GetRolesByUserId(userId, dbName);
        }

        [HttpGet, Route("Roles/{roleIDs}/{dbName}")]
        [SwaggerOperation(Summary = "GetRolesByIds - Gets all the roles by ID's", Description = "Returns a list of Roles")]
        public async Task<ActionResult<List<Roles>>> Get(string roleIDs, string dbName)
        {
            return await _role.GetRolesByIds(roleIDs, dbName);
        }
        #endregion

        #region ====================  UserMap  ====================

        [HttpGet("GetUserMap/{userMapId:int}")]
        [SwaggerOperation(Summary = "GetUserMap - Gets the user information based on the user type", Description = "Returns a single UserMap")]
        public async Task<ActionResult<UserMap>> GetUserMap(int userMapId)
        {
            return await _userMap.GetUserMap(userMapId);
        }

        [HttpGet("UserMap/GetUserMapByUserType/{userTypeLcid:int}")]
        [SwaggerOperation(Summary = "GetUserMapByUserType - Gets the user information based on the user type", Description = "Returns a list of Users")]
        public async Task<ActionResult<List<Users>>> GetUserMapByUserType(int userTypeLcid)
        {
            return await _userMap.GetUserMapByUserType(userTypeLcid);
        }

        [HttpGet("{userId:int}/GetUserCustomerMapModel")]
        [SwaggerOperation(Summary = "GetUserCustomerMapModel - Gets the user customer map with customer model", Description = "Returns a complex model that contains List<UserMap>, List<Customers>")]
        public async Task<ActionResult<UserMapCustomerModels>> GetUserCustomerMapModel(int userId)
        {
            return await _userMap.GetUserCustomerMapModel(userId);
        }

        [HttpPost("UserMap")]
        [SwaggerOperation(Summary = "PostUserMap - Insert a new object into UserMaps", Description = "Returns a string indicating the status of the insert")]
        public async Task<ActionResult<string>> PostUserMap(UserMap userMap)
        {
            if (userMap.EmployeeID == null && userMap.CustomerID == null && userMap.SupplierID == null)
            {
                return BadRequest("One or more fields are not set");
            }

            //switch (userMap.UserTypeLCID)
            //{
            //    case 298: //Employee
            //        userMap.CustomerID = null;
            //        userMap.SupplierID = null;
            //        break;
            //    case 299: //Customer
            //    case 318: //CODCustomer
            //        userMap.EmployeeID = null;
            //        userMap.SupplierID = null;
            //        break;
            //    case 300: //Supplier
            //        userMap.CustomerID = null;
            //        userMap.EmployeeID = null;
            //        break;

            //    default:
            //        return BadRequest("UserTypeLCID is invalid");
            //};


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _userMap.PostUserMapObject(userMap);
        }

        [HttpPut("UserMap")]
        [SwaggerOperation(Summary = "PutUserMap - Update an existing UserMap record", Description = "Returns a boolean")]
        public async Task<ActionResult<bool>> PutUserMap(UserMap userMap)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (userMap.UserID == 0) return BadRequest("UserID is 0");
            if (userMap.UserTypeLCID == 0) return BadRequest("UserTypeLCID is 0");
            if (userMap.CreatedByUserID == 0) return BadRequest("CreatedByUserID is 0");
            if (userMap.SystemID == 0) return BadRequest("SystemID is 0");
            if (userMap.EmployeeID == null && userMap.CustomerID == null && userMap.SupplierID == null) return BadRequest("Either EmployeeID/CustomerID/Supplier is 0");

            return await _userMap.PutUserMap(userMap);
        }
        #endregion

        #region ====================  UserCustomerMap  ====================
        [HttpGet("{userId:int}/GetUserCustomerMap")]
        [SwaggerOperation(Summary = "GetUserCustomerMapByUserId - Gets all the CustomerID's assigned to the users", Description = "Returns a List<UserCustomerMap>")]
        public async Task<ActionResult<List<UserCustomerMap>>> GetUserCustomerMap(int userId)
        {
            return await _userCustomerMap.GetUserCustomerMapByUserId(userId);
        }
        #endregion

    }
}