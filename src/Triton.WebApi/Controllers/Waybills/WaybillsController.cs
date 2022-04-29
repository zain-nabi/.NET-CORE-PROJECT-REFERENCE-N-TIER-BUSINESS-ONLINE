using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using Triton.Interface.CRM;
using Triton.Interface.Waybill;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.StoredProcs;
using Triton.Model.CRM.Tables;
using Triton.Model.CRM.Views;
using Triton.Model.FWWebservice.Custom;
using Triton.Model.TritonGroup.Tables;
using Triton.Model.TritonStaging.StoredProcs;
using Triton.Repository.TritonStaging;
using Triton.WebApi.Controllers.Freightware;
using Vendor.Services.Helpers;

namespace Triton.WebApi.Controllers.Waybills
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Waybills / E-Waybill / Image / InternalWaybill / WaybillQueryMaster / WaybillQuery")]

    public class WaybillsController : ControllerBase
    {

        private readonly IWaybill _waybill;
        private readonly ITritonStagingStoredProcs _stagingRepo;
        private readonly IWaybillQueryMaster _waybillQueryMaster;
        private readonly IConfiguration _configuration;
        private readonly IVendorServices _converters;
        private readonly ICustomers _customer;
        private readonly ITransportTypes _transportType;
        private readonly ISites _sites;


        public WaybillsController(IWaybill waybill, IConfiguration configuration, ITritonStagingStoredProcs stagingRepo, IVendorServices converters, IWaybillQueryMaster waybillQueryMaster, ICustomers customer, ITransportTypes transportTypes, ISites sites)
        {
            _waybill = waybill;
            _configuration = configuration;
            _stagingRepo = stagingRepo;
            _converters = converters;
            _waybillQueryMaster = waybillQueryMaster;
            _customer = customer;
            _transportType = transportTypes;
            _sites = sites;
        }

        #region ====================  Waybills  ====================
        [HttpGet("{waybillId:long}")]
        [SwaggerOperation(Summary = "GetWaybillById - Gets a single waybill by waybillId", Description = "Returns the Waybills object")]
        public async Task<ActionResult<Model.CRM.Tables.Waybills>> GetWaybillById(long waybillId)
        {
            if (waybillId == 0)
                return BadRequest("waybillId is required as a parameter");

            return await _waybill.GetWaybillById(waybillId);
        }


        [HttpGet("GetWaybillInfoById/{waybillId:long}")]
        [SwaggerOperation(Summary = "GetWaybillInfoById - Gets a single waybill by waybillId", Description = "Returns the Waybills object")]
        public async Task<ActionResult<WayBillInfoModel>> GetWaybillInfoById(long waybillId)
        {
            if (waybillId == 0)
                return BadRequest("waybillId is required as a parameter");

            return await _waybill.GetWaybillInfoById(waybillId);
        }


        [HttpGet("GetWaybillViewList/{customerIds}")]
        [SwaggerOperation(Summary = "GetWaybillViewList - Gets waybill view", Description = "Returns List<vwOpsWaybills> object")]
        public async Task<ActionResult<List<vwOpsWaybills>>> GetWaybillViewList(string customerIds, string waybillNo, string customerXRef, int? waybillId)
        {
            if (waybillNo == null && customerXRef == null && waybillId == null)
                return BadRequest("WaybillNo or CustomerXRef or WaybillId");

            return await _waybill.GetWaybillViewList(customerIds, waybillNo, customerXRef, waybillId);
        }


        [HttpGet("{WaybillNo}")]
        [SwaggerOperation(Summary = "GetWaybillsbyWaybillNo", Description = "get waybill by waybillno")]
        public async Task<ActionResult<List<vwOpsWaybills>>> GetWaybillsbyWaybillNo(string WaybillNo)
        {
            return await _waybill.GetWaybillsByWaybillNo(WaybillNo);
        }

        [HttpGet("{waybillNo}/{dbName}")]
        [SwaggerOperation(Summary = "GetWaybillByWaybillNo", Description = "Get waybill by WaybillNo")]
        public async Task<ActionResult<Model.CRM.Tables.Waybills>> GetWaybillByWaybillNo(string waybillNo, string dbName = "CRM")
        {
            return await _waybill.GetWaybillByWaybillNo(waybillNo, dbName);
        }
        #endregion




        #region ====================  WaybillQueryMaster  ====================
        [HttpGet("WaybillQueryMaster/{waybillQueryMasterId:int}")]
        [SwaggerOperation(Summary = "Get - Get the single WaybillQueryMaster", Description = "Returns WaybillQueryMaster")]
        public async Task<ActionResult<WaybillQueryMaster>> Get(int waybillQueryMasterId)
        {
            return await _waybillQueryMaster.Get(waybillQueryMasterId);
        }

        [HttpGet("WaybillQueryMaster/{userId}/{queryStatusLcid}")]
        [SwaggerOperation(Summary = "GetWaybillQueryMaster - Get the queries associated with the waybill", Description = "Returns List<WaybillQueryMaster_Waybills_Model>")]
        public async Task<ActionResult<List<WaybillQueryMaster_Waybills_Model>>> GetWaybillQueryMaster(int userId, string queryStatusLcid, int? systemId)
        {
            return await _waybillQueryMaster.GetWaybillQueryMaster(userId, queryStatusLcid, systemId);
        }

        [HttpGet("WaybillQuery/{waybillId:long}")]
        [SwaggerOperation(Summary = "GetWaybillQueryList - Get the queries associated with the waybill", Description = "Returns List<WaybillQuery>")]
        public async Task<ActionResult<List<WaybillQuery>>> GetWaybillQueryList(long waybillId)
        {
            return await _waybillQueryMaster.GetWaybillQueryList(waybillId);
        }


        [HttpPut("WaybillQueryMaster")]
        [SwaggerOperation(Summary = "PutWaybillQueryMaster - Updates the WaybillQueryMaster table", Description = "Returns a bool")]
        public async Task<ActionResult<bool>> PutWaybillQueryMaster(WaybillQueryMaster waybillQueryMaster)
        {
            return await _waybillQueryMaster.PutWaybillQueryMaster(waybillQueryMaster);
        }


        [HttpPost("WaybillQueryMaster")]
        [SwaggerOperation(Summary = "PostWaybillQueryMaster - Insert into the WaybillQueryMaster table", Description = "Returns a bool")]
        public async Task<ActionResult<bool>> PostWaybillQueryMaster(WaybillQueryMasterInsertModel model)
        {
            if (ModelState.IsValid)
                return await _waybillQueryMaster.PostWaybillQueryMaster(model);

            return BadRequest("Model is invalid");
        }

        [HttpDelete("WaybillQueryMaster/{waybillId:long}/{userId:int}")]
        [SwaggerOperation(Summary = "Delete - Deletes a record WaybillQueryMaster / WaybillQuery", Description = "Returns bool")]
        public async Task<ActionResult<bool>> Delete(long waybillId, int userId)
        {
            return await _waybillQueryMaster.Delete(waybillId, userId);
        }
        #endregion

        [HttpGet("WaybillStickers/Image/{waybillNo}")]
        [SwaggerOperation(Summary = "Get Stickers For a Waybill", Description = "Retreive the sticker pdf for a waybill by waybillno, optional test")]
        public async Task<ActionResult<DocumentRepositories>> GetStickersForWaybillasPDF(string waybillNo, bool test = false)
        {
            var client = new WebClient
            {
                Credentials = new NetworkCredential("servicea", "S3rv1cEA2021", "tritonexpress")
            };

            var report = await client.DownloadDataTaskAsync($"http://Tiger/ReportServer?/CRMReports/Waybill{ (test ? "Test" : "") }APIStickers&WaybillNo={waybillNo}&rs:ClearSession=true&rs:Command=Render&rs:Format=PDF");
            DocumentRepositories doc = new DocumentRepositories
            {
                ImgContentType = "application/pdf",
                ImgData = report,
                ImgName = $"{waybillNo}Stickers.pdf",
                ImgLength = report.Length
            };
            return Ok(doc);
        }

        [HttpGet("WaybillStickers/POD/{waybillNo}/Pages/{pages}")]
        [SwaggerOperation(Summary = "Get POD Stickers For a Waybill", Description = "Retreive the pod sticker pdf for a waybill by waybillno,with page count")]
        public async Task<ActionResult<DocumentRepositories>> GetPODStickersForWaybillasPDF(string waybillNo, int pages = 1)// bool test = false)
        {
            var client = new WebClient
            {
                Credentials = new NetworkCredential("servicea", "S3rv1cEA2021", "tritonexpress")
            };

            var report = await client.DownloadDataTaskAsync($"http://Tiger/ReportServer?/CRMReports/WaybillPODSticker&WaybillNo={waybillNo}&Pages={pages}&rs:ClearSession=true&rs:Command=Render&rs:Format=PDF");
            DocumentRepositories doc = new DocumentRepositories
            {
                ImgContentType = "application/pdf",
                ImgData = report,
                ImgName = $"{waybillNo}PODStickers.pdf",
                ImgLength = report.Length
            };
            return Ok(doc);
        }

        [HttpGet("UAT/E-Waybill/Image/{waybillNo}")]
        [SwaggerOperation(Summary = "Get E-Waybill For a Waybill in UAT", Description = "Retreives the e-waybill document for a waybill, locked to UAT")]
        public async Task<ActionResult<DocumentRepositories>> GetEWaybillasPDFUAT(string waybillNo)
        {
            //Retreive latest data from FW and store
            var fc = new FreightwareController(_configuration, _waybill, _converters, _customer, _transportType, _sites);
            var fwwaybill = await fc.GetWaybillUAT(waybillNo);
            //Get our version of the waybill so we can pass the id
            var waybill = await _waybill.GetWaybillByWaybillNo(waybillNo, "CRMTest");
            //Store the lines
            var storeWaybillLines = await _waybill.StoreWaybillLines(fwwaybill.Value.DataObject, waybill.WaybillID, "CRMTest");

            //Now retrieve the image as all reference data is good.
            var client = new WebClient
            {
                Credentials = new NetworkCredential("servicea", "S3rv1cEA2021", "tritonexpress")
            };

            var report = await client.DownloadDataTaskAsync($"http://Tiger/ReportServer?/CRMReports/TritonInternalWaybillTest&WaybillID={waybill.WaybillID}&rs:ClearSession=true&rs:Command=Render&rs:Format=PDF");
            var doc = new DocumentRepositories
            {
                ImgContentType = "application/pdf",
                ImgData = report,
                ImgName = $"{waybillNo}.pdf",
                ImgLength = report.Length
            };
            return Ok(doc);
        }

        [HttpGet("E-Waybill/Image/{waybillNo}")]
        [SwaggerOperation(Summary = "Get E-Waybill For a Waybill", Description = "Retreives the e-waybill document for a waybill, production only")]
        public async Task<ActionResult<DocumentRepositories>> GetEWaybillasPDF(string waybillNo)
        {

            //Retrieve latest data from FW and store
            var fc = new FreightwareController(_configuration, _waybill, _converters, _customer, _transportType, _sites);
            var fwwaybill = await fc.GetWaybill(waybillNo);
            //Get our version of the waybill so we can pass the id
            var waybill = await _waybill.GetWaybillByWaybillNo(waybillNo, "CRM");
            //Store the lines
            var storeWaybillLines = await _waybill.StoreWaybillLines(fwwaybill.Value.DataObject, waybill.WaybillID, "CRM");

            var client = new WebClient
            {
                Credentials = new NetworkCredential("servicea", "S3rv1cEA2021", "tritonexpress")
            };

            var report = client.DownloadData($"http://Tiger/ReportServer?/CRMReports/TritonInternalWaybill&WaybillID={waybill.WaybillID}&rs:ClearSession=true&rs:Command=Render&rs:Format=PDF");
            var doc = new DocumentRepositories
            {
                ImgContentType = "application/pdf",
                ImgData = report,
                ImgName = $"{waybillNo}.pdf",
                ImgLength = report.Length
            };
            return Ok(doc);
        }

        [HttpGet("Image/Signature/{waybillNo}")]
        [SwaggerOperation(Summary = "Get signature image a Waybill", Description = "Retreives a signature image if available from the FMO system")]
        public async Task<ActionResult<proc_FMOEndorsements_Signature_Select>> PODSignature(string waybillNo)
        {
            try
            {
                var waybill = await _waybill.GetWaybillByWaybillNo(waybillNo);
                if (waybill == null)
                    return NotFound();
                else return await _stagingRepo.GetPODSignature(waybill.WaybillID);
            }
            catch
            {
                return NotFound();
            }
        }

        #region Internal Waybills
        [HttpGet("InternalWaybill/{internalWaybillId}")]
        [SwaggerOperation(Summary = "Get InternalWaybill object", Description = "Retreives an internal waybill by id,optional database")]
        public async Task<ActionResult<InternalWaybills>> GetInternalWaybill(long internalWaybillID, string DBName = "CRM")
        {
            return await _waybill.GetInternalWaybill(internalWaybillID, DBName);
        }

        [HttpGet("InternalWaybill/{waybillno}")]
        [SwaggerOperation(Summary = "Get InternalWaybill object by Reference", Description = "Retreives an internal waybill by reference,optional database")]
        public async Task<ActionResult<InternalWaybills>> GetInternalWaybillByReference(string waybillNo, string dbName = "CRM")
        {
            return await _waybill.GetInternalWaybillByReference(waybillNo, dbName);
        }

        [HttpPost("InternalWaybill/")]
        [SwaggerOperation(Summary = "Posts an InternalWaybill", Description = "Creates an internal waybill record, optional database")]
        public async Task<ActionResult<FWResponsePacket>> PostInternalWaybill(TritonWaybillSubmitModels model, string dbName = "CRM")
        {
            //using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var iWaybillId = await _waybill.PostInternalWaybill(new InternalWaybills
                {
                    CreatedOn = DateTime.Now,
                    CreatedByUserID = model.CreatedByUserID,
                    CreatedByGroupUserID = model.CreatedByUserID,
                    ReceiverCell = model.ReceiverContactCell,
                    ReceiverContact = model.ReceiverContactName,
                    ReceiverEmail = model.ReceiverContactEmail,
                    ReceiverTel = model.ReceiverContactCell,
                    SenderCell = model.SenderContactCell,
                    SenderContact = model.SenderContactName,
                    SenderEmail = model.SenderContactEmail,
                    SenderTel = model.SenderContactCell
                }, dbName);
                if (iWaybillId != 0)
                {
                    //Retreive the recently stored internal waybill
                    var iWaybill = await _waybill.GetInternalWaybill(iWaybillId, dbName);

                    //Update the submission model with the reference 
                    model.WaybillNo = iWaybill.ReferenceNo;

                    //Post it to Freightware
                    var fc = new FreightwareController(_configuration, _waybill, _converters, _customer, _transportType, _sites);
                    var packet = (await fc.PostWaybill(model)).Value;

                    if (packet.ReturnCode == "0000")
                    {
                        var savedWaybillId = await _waybill.PostWaybillFromFWWS(_converters.CovertFWWaybillToCustomerWaybill(packet.DataObject), dbName);
                        var pWaybill = await _waybill.GetWaybillByWaybillNo(iWaybill.ReferenceNo, dbName);
                        if (pWaybill != null)
                        {
                            iWaybill.WaybillID = pWaybill.WaybillID;
                            await _waybill.PutInternalWaybill(iWaybill, dbName);
                        }
                        //transaction.Complete();

                        #region SendEmail - Commented Out
                        //var x = await GetAsyncRequest<TritonModel.TritonGroup.Tables.DocumentRepositories>("Waybills", "GetEWaybillasPDFUAT", $"WaybillNo={model.WaybillNo}");
                        //List<String> sb = new List<String> { profileData.GroupEmail };
                        //if (!String.IsNullOrEmpty(model.SenderContactEmail) && model.SenderContactEmail != profileData.GroupEmail)
                        //    sb.Add(model.SenderContactEmail);
                        //if (!String.IsNullOrEmpty(model.ReceiverContactEmail) && model.ReceiverContactEmail != profileData.GroupEmail)
                        //    sb.Add(model.ReceiverContactEmail);
                        //EmailSender.SendEmail(sb.ToArray(), "no-reply@tritonexpress.co.za", $"Please find attached a copy of the waybill document for {model.WaybillNo}. Also note the PIN for this transaction is {iWaybill.Pin}.",
                        //    $"Attention for Waybill {model.WaybillNo}", ConfigurationManager.AppSettings["SMTP"], new List<System.Net.Mail.Attachment>{
                        //        new System.Net.Mail.Attachment(new MemoryStream(x.ImgData),x.ImgName,x.ImgContentType)
                        //        }); ;

                        #endregion

                        return new FWResponsePacket
                        {
                            ReturnCode = "0000",
                            ReturnMessage = "Waybill Created Successfully",
                            DataObject = iWaybill,
                            Reference = iWaybill.ReferenceNo
                        };
                    }

                    return new FWResponsePacket
                    {
                        ReturnCode = packet.ReturnCode,
                        ReturnMessage = $"Freighware Failure - {packet.ReturnMessage}",
                    };
                }

                return new FWResponsePacket
                {
                    ReturnCode = "9999",
                    ReturnMessage = "Internal Error - Waybill Not Created"
                };

            }
            catch (Exception x)
            {
                return new FWResponsePacket
                {
                    ReturnCode = "9999",
                    ReturnMessage = $"Internal Error - Waybill Not Created - {x.Message} - {x.InnerException}"
                };
            }
        }

        [HttpPost("UAT/InternalWaybill/")]
        [SwaggerOperation(Summary = "Posts an InternalWaybill UAT", Description = "Creates an internal UAT waybill record,optional database")]
        public async Task<ActionResult<FWResponsePacket>> PostInternalWaybillUAT(TritonWaybillSubmitModels model, string dbName = "CRM")
        {
            //using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var iWaybillId = await _waybill.PostInternalWaybill(new InternalWaybills
                {
                    CreatedOn = DateTime.Now,
                    CreatedByUserID = model.CreatedByUserID,
                    CreatedByGroupUserID = model.CreatedByUserID,
                    ReceiverCell = model.ReceiverContactCell,
                    ReceiverContact = model.ReceiverContactName,
                    ReceiverEmail = model.ReceiverContactEmail,
                    ReceiverTel = model.ReceiverContactCell,
                    SenderCell = model.SenderContactCell,
                    SenderContact = model.SenderContactName,
                    SenderEmail = model.SenderContactEmail,
                    SenderTel = model.SenderContactCell
                }, dbName);

                if (iWaybillId == 0)
                    return new FWResponsePacket
                    {
                        ReturnCode = "9999",
                        ReturnMessage = "Internal Error - Waybill Not Created"
                    };

                //Retrieve the recently stored internal waybill
                var iWaybill = await _waybill.GetInternalWaybill(iWaybillId, dbName);

                //Update the submission model with the reference 
                model.WaybillNo = iWaybill.ReferenceNo;

                //Post it to Freightware
                var fc = new FreightwareController(_configuration, _waybill, _converters, _customer, _transportType, _sites);

                var packet = (await fc.PostWaybillUAT(model)).Value;
                var savedWaybillId = await _waybill.PostWaybillFromFWWS(_converters.CovertFWWaybillToCustomerWaybill(packet.DataObject), dbName);

                if (packet.ReturnCode == "0000")
                {
                    var pWaybill = await _waybill.GetWaybillByWaybillNo(iWaybill.ReferenceNo, dbName);
                    if (pWaybill != null)
                    {
                        iWaybill.WaybillID = pWaybill.WaybillID;
                        await _waybill.PutInternalWaybill(iWaybill, dbName);
                    }
                    //transaction.Complete();

                    #region SendEmail - Commented Out
                    //var x = await GetAsyncRequest<TritonModel.TritonGroup.Tables.DocumentRepositories>("Waybills", "GetEWaybillasPDFUAT", $"WaybillNo={model.WaybillNo}");
                    //List<String> sb = new List<String> { profileData.GroupEmail };
                    //if (!String.IsNullOrEmpty(model.SenderContactEmail) && model.SenderContactEmail != profileData.GroupEmail)
                    //    sb.Add(model.SenderContactEmail);
                    //if (!String.IsNullOrEmpty(model.ReceiverContactEmail) && model.ReceiverContactEmail != profileData.GroupEmail)
                    //    sb.Add(model.ReceiverContactEmail);
                    //EmailSender.SendEmail(sb.ToArray(), "no-reply@tritonexpress.co.za", $"Please find attached a copy of the waybill document for {model.WaybillNo}. Also note the PIN for this transaction is {iWaybill.Pin}.",
                    //    $"Attention for Waybill {model.WaybillNo}", ConfigurationManager.AppSettings["SMTP"], new List<System.Net.Mail.Attachment>{
                    //        new System.Net.Mail.Attachment(new MemoryStream(x.ImgData),x.ImgName,x.ImgContentType)
                    //        }); ;

                    #endregion

                    return new FWResponsePacket
                    {
                        ReturnCode = "0000",
                        ReturnMessage = "Waybill Created Successfully",
                        DataObject = iWaybill
                    };
                }

                return new FWResponsePacket
                {
                    ReturnCode = packet.ReturnCode,
                    ReturnMessage = $"Freightware Failure - {packet.ReturnMessage}",
                };

            }
            catch (Exception x)
            {
                return new FWResponsePacket
                {
                    ReturnCode = "9999",
                    ReturnMessage = $"Internal Error - Waybill Not Created - {x.Message} - {x.InnerException}"
                };
            }
        }

        #endregion

        #region CreditNotes
        [HttpGet("GetCreditNotePage/{waybillNo}")]
        [SwaggerOperation(Summary = "GetCreditNotePage - Gets the information for the CRNoteTemp tables", Description = "Returns the CreditNotePageModel object")]
        public async Task<ActionResult<CreditNotePageModel>> GetCreditNotePage(string waybillNo)
        {
            if (waybillNo == null)
                return BadRequest("waybillNo is required as a parameter");

            return await _waybill.GetCreditNotePage(waybillNo);
        }
        #endregion
    }
}