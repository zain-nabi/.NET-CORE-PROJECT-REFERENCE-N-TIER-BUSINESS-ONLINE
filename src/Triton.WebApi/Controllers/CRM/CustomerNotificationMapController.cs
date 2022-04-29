using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Triton.Interface.CRM;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.Tables;

namespace Triton.WebApi.Controllers.CRM
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerNotificationMapController : ControllerBase
    {
        private readonly ICustomerNotificationMap _customerNotificationMap;
        public CustomerNotificationMapController(ICustomerNotificationMap customerNotificationMap)
        {
            _customerNotificationMap = customerNotificationMap;

        }

        [HttpGet("{CustomerNotificationMapID}")]
        [SwaggerOperation(Summary = "Get Customer Notification Map By Id", Description = "Get Customer Notification Map By Id")]
        public async Task<CustomerNotificationMaps> GetById(int CustomerNotificationMapID)
        {
            return await _customerNotificationMap.GetCustomerNotificationMapsByCustomerNotificationMapID(CustomerNotificationMapID);
        }

        [HttpGet("CustomerNotificationMap")]
        [SwaggerOperation(Summary = "Get Customer Notification Map By Id", Description = "Get Customer Notification Map By Id")]
        public async Task<List<CustomerNotificationMaps>> GetAsync()
        {
            return await _customerNotificationMap.GetAsync();
        }
        [HttpGet]
        [SwaggerOperation(Summary = "Get customer notification by accountcode or customer name", Description = "get customer my account code or customer name")]
        public async Task<CustomerNotificationMapsModel> GetSearch(string AccountCode, string CustomerName)
        {
            return await _customerNotificationMap.GetCustomerNotificationMapsSearch(AccountCode, CustomerName);
        }
        [HttpGet("CustomerNotificationMap/{CustomerNoficationMapId}")]
        [SwaggerOperation(Summary ="Gets customer nofication map By Id and a list of account", Description ="return customer notification by Id a list of accountcodes and customer names")]
        public async Task<CustomerNotificationMapsEditModel>EditModel(int customerNotificationMapId)
        {
            return await _customerNotificationMap.GetCustomerNotificationMapsEditModel(customerNotificationMapId);
        }
        [HttpGet("CustomerNotificationMaps/CutomerNotificationMapWithAccountCodeAndCustomer/{CustomerNotificationMapId}")]
        [SwaggerOperation(Summary ="Gets account codes and customer names", Description ="returns list of accountcodes and CustomerNames")]
        public async Task<CustomerNotificationMapsEditModel>Edit(int CustomerNotificationMapId)
        {
            return await _customerNotificationMap.GetCustomerNotificationMapsEditModel(CustomerNotificationMapId);
        }
        [HttpGet("CustomerNotificationMap/ListOfCustomerAccountAndCustomerName")]
        [SwaggerOperation(Summary ="Gets a list of accountcodes and customer names",Description ="returns a list of accountcodes and names")]
        public async Task<CustomerNotificationMapsEditModel>Create()
        {
            return await _customerNotificationMap.GetCustomerNotificationMapsCreateModel();
        }

        [HttpPut("PutCustomerNotificationMap")]
        [SwaggerOperation(Summary = "Put - Updates the CustomerNotificationMap table", Description = "Returns a bool")]
        public async Task<ActionResult<bool>> Put(CustomerNotificationMaps customerNotificationMaps)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return await _customerNotificationMap.PutCustomerNotificationMaps(customerNotificationMaps);
        }
        [HttpPost("PostCustomerNotificationMap")]
        [SwaggerOperation(Summary = "Post - Insert into CustomerNotification Table", Description = "Returns a new Entry")]
        public async Task<ActionResult<long>> Post(CustomerNotificationMaps customerNotificationMaps)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return await _customerNotificationMap.PostCustomerNotificationMaps(customerNotificationMaps);
        }

    }
}
