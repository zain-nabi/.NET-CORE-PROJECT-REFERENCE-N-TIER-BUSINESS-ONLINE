using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Triton.Interface.BusinessOnline;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.StoredProcs;
using Triton.Model.TritonOps.StoredProcs;

namespace Triton.WebApi.Controllers.BusinessOnline
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Queries used within the BusinessOnline, CSA Dashboard")]

    public class BusinessOnlineController : ControllerBase
    {
        private readonly IBusinessOnline _repo;

        public BusinessOnlineController(IBusinessOnline repo)
        {
            _repo = repo;
        }

        [HttpGet("{userId:int}/{isTritonGroupUserId:bool}/{date}")]
        [SwaggerOperation(Summary = "GetCustomerDashboard - Gets the summary of waybills for CSA or customers", Description = "Returns a List<proc_Waybills_CSA_Dashboard_Select>")]
        public async Task<ActionResult<List<proc_Waybills_CSA_Dashboard_Select>>> Get(int userId, bool isTritonGroupUserId, DateTime date)
        {
            return await _repo.GetCustomerDashboard(userId, isTritonGroupUserId, date);
        }

        [HttpGet("Waybills/{customerId}/{type}/{mobile:bool}")]
        [SwaggerOperation(Summary = "GetWaybillsByType - Get all the waybill information by category for CSA or customers", Description = "Returns a List<proc_CSA_WaybillList_Select>")]
        public async Task<ActionResult<List<proc_CSA_WaybillList_Select>>> Get(string customerId, string type, bool mobile, DateTime? date)
        {
            return await _repo.GetWaybillsByType(customerId, type, mobile, date);
        }

        [HttpGet("Waybills/{customerIds}")]
        [SwaggerOperation(Summary = "GetDeliveryStatusCount - Waybill Status Summary for CSA or customers", Description = "Returns proc_CSA_Customer_Select")]
        public async Task<ActionResult<proc_CSA_Customer_Select>> Get(string customerIds)
        {
            return await _repo.GetDeliveryStatusCount(customerIds);
        }

        [HttpGet("Waybills/{customerIds}/{userId:int}")]
        [SwaggerOperation(Summary = "GetDashboardForCustomerMultiQuery - Multi-query to get all the information for CSA/Customer dashboard", Description = "Returns a multi-query CSADashboardModel")]
        public async Task<ActionResult<CSADashboardModel>> Get(string customerIds, int userId, bool? isTritonGroupUserId, DateTime? date, string tableName)
        {
            return await _repo.GetDashboardForCustomerMultiQuery(customerIds, userId, isTritonGroupUserId, date, tableName);
        }

        [HttpGet("Waybills/{customerIds}/{type}")]
        [SwaggerOperation(Summary = "GetCustomerDeliveriesByStatus - Gets a waybill list by the customerId's and type such as Bookings etc", Description = "Returns a List<proc_Customer_By_CustomerID_Tabs_Select>")]
        public async Task<ActionResult<List<proc_Customer_By_CustomerID_Tabs_Select>>> Get(string customerIds, string type)
        {
            return await _repo.GetCustomerDeliveriesByStatus(customerIds, type);
        }
    }
}