using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Expressions;
using Swashbuckle.AspNetCore.Annotations;
using Triton.Interface.CRM;
using Triton.Interface.Waybill;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.StoredProcs;
using Triton.Model.CRM.Tables;
using Triton.Model.FWWebservice.Custom;
using Triton.Model.TritonGroup.Tables;
using Triton.WebApi.Controllers.Freightware;
using Vendor.Services.CustomModels;
using Vendor.Services.Freightware.PROD.GetChargesList;
using Vendor.Services.Freightware.PROD.GetStatement;
using Vendor.Services.Freightware.PROD.SetCustRateIncreasePerc;
using Vendor.Services.Helpers;

namespace Triton.WebApi.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Customers / Quotations/ Invoice")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomers _customerRepo;
        private readonly IVendorServices _converter;
        private readonly IConfiguration _configuration;
        private readonly IQuotes _quotesRepo;
        private readonly IWaybill _waybillrepo;
        private readonly IInvoices _invoice;
        private readonly ITransportTypes _transportType;
        private readonly ISites _sites;

        public CustomerController(ICustomers customerRepo, IVendorServices converter, IConfiguration configuration, IQuotes quoteRepo, IWaybill waybillRepo, IInvoices invoices, ITransportTypes transportTypes, ISites sites)
        {
            _customerRepo = customerRepo;
            _configuration = configuration;
            _converter = converter;
            _quotesRepo = quoteRepo;
            _waybillrepo = waybillRepo;
            _invoice = invoices;
            _transportType = transportTypes;
            _sites = sites;
        }

        #region Customers

        [HttpGet]
        [SwaggerOperation(Summary = "Gets all active customers from CRM", Description = "Gets all active customers from CRM")]
        public async Task<ActionResult<List<Model.CRM.Tables.Customers>>> Get()
        {
            return await _customerRepo.GetAllActiveCustomers();
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Gets a CRM customer by CustomerID", Description = "Gets a CRM customer by CustomerID")]
        public async Task<ActionResult<Model.CRM.Tables.Customers>> Get(int id)
        {
            return await _customerRepo.GetCrmCustomerById(id);
        }

        [HttpGet("FindByAccountCode/{accountCode}")]
        [SwaggerOperation(Summary = "Gets a CRM customer by AccountCode", Description = "Gets a CRM customer by AccountCode")]
        public async Task<ActionResult<Model.CRM.Tables.Customers>> FindByAccountCode(string accountCode)
        {
            return await _customerRepo.GetCRMCustomerByAccountCode(accountCode);
        }

        [HttpGet("{search}")]
        [SwaggerOperation(Summary = "Search for a customer by AccountCode or Name", Description = "Search for a customer by AccountCode or Name")]
        public async Task<ActionResult<List<Model.CRM.Tables.Customers>>> FindCrmCustomer(string search)
        {
            return await _customerRepo.FindCrmCustomer(search);
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Update an existing customer", Description = "Update an existing customer")]
        public async Task<ActionResult<bool>> PutCustomer(Model.CRM.Tables.Customers customer)
        {
            return await _customerRepo.PutCustomer(customer);
        }

        [HttpGet, Route("RepUser/{userId}")]
        public async Task<ActionResult<List<Model.CRM.Tables.Customers>>> GetCrmCustomersByRepUserId(int userId)
        {
            return await _customerRepo.GetCrmCustomersByRepUserId(userId);
        }

        [HttpGet, Route("Internal")]

        public async Task<ActionResult<List<Model.CRM.Tables.Customers>>> GetCrmInternalAccounts()
        {
            return await _customerRepo.GetCrmInternalAccounts();
        }

        [HttpGet("GetCustomerAssessment/{customerId:int}")]
        [SwaggerOperation(Summary = "GetCustomerAssessment - Gets the customer waybill assessment", Description = "Returns a CustomerAssessment object")]
        public async Task<ActionResult<CustomerAssessment>> GetCustomerAssessment(int customerId, string periodFrom, string periodTo, int? branch = null, int? rep = null)
        {
            return await _customerRepo.GetCustomerAssessment(customerId, periodFrom, periodTo, branch, rep);
        }

        [HttpGet("FindCrmCustomerByIds/{customerIds}")]
        [SwaggerOperation(Summary = "FindCrmCustomerByIds - Finds a list of customers by multiple Id's", Description = "Returns a List<Customers> object")]
        public async Task<ActionResult<List<Model.CRM.Tables.Customers>>> FindCrmCustomerByIds(string customerIds)
        {
            return await _customerRepo.FindCrmCustomerByIds(customerIds);
        }
        #endregion

        #region Quotations
        /// <summary>
        /// Gets available surcharges
        /// </summary>
        /// <returns></returns>
        [HttpGet("Quotations/Surcharges")]
        [SwaggerOperation(Summary = "Surcharges", Description = "Get available surcharge for quotations")]
        public async Task<ActionResult<List<GetChargesListResponseChargesOutput>>> GetSurcharges()
        {
            var fc = new FreightwareController(_configuration, _waybillrepo, _converter, _customerRepo, _transportType, _sites);
            var charges = await fc.Get();
            //Filter only for the ones we want for Quotes ie Sat Collection , delivery etc
            List<GetChargesListResponseChargesOutput> chargeList = charges.Value.DataObject;
            //var filtered = chargeList.FindAll(x => x.Heading.Contains("SAT") || x.Heading.Contains("EARLY"));
            //var t = chargeList.FindAll(x => x.CountryCode != "SA");
            return chargeList;
        }

        /// <summary>
        /// Gets available surcharges
        /// </summary>
        /// <returns></returns>
        [HttpGet("Quotations/{quoteId:long}")]
        [SwaggerOperation(Summary = "Get a Quote", Description = "Get a quote by id, optionaly passing the database")]
        public async Task<ActionResult<VendorQuoteModel>> GetQuoteByID(long quoteId, string dbName = "CRM")
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return await _quotesRepo.GetQuoteByID(quoteId, dbName);
        }

        /// <summary>
        /// Gets available surcharges
        /// </summary>
        /// <returns></returns>
        [HttpGet("Quotations/{quoteNumber}")]
        [SwaggerOperation(Summary = "Get a quote by reference", Description = "Get a quote by reference no, optionally passing the database")]
        public async Task<ActionResult<VendorQuoteModel>> GetQuoteByQuoteNumber(string quoteNumber, string dbName = "CRM")
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return await _quotesRepo.GetQuoteByQuoteNumber(quoteNumber, dbName);

        }

        /// <summary>
        /// Gets available surcharges
        /// </summary>
        /// <returns></returns>
        [HttpGet("{customerId}/Quotations")]
        [SwaggerOperation(Summary = "Search quotes", Description = "Search quotes for a customer by reference and/or dates")]
        public async Task<ActionResult<VendorQuoteSearchModel>> GetQuotebyCustomerIdOptRefandDates(int customerId, string referance, DateTime? from, DateTime? to, string dbName = "CRM")
        {
            return await _quotesRepo.GetQuotesbyCustomerIdOptRefandDates(customerId, referance, from, to, dbName);
        }

        /// <summary>
        /// Post a Quote
        /// </summary>
        /// <returns></returns>
        [HttpPost("Quotations")]
        [SwaggerOperation(Summary = "Post Quote", Description = "Post a Quote Object")]
        public async Task<ActionResult<long>> Post(VendorQuoteModel model)
        {
            var fwQuote = await _converter.ConvertToFWUATInQuote(model);
            //var quote = _converter.ConvertFromFWUATQuote(fwQuote);
            return 1;
        }

        /// <summary>
        /// Post a Quote in UAT
        /// </summary>
        /// <returns></returns>
        [HttpPost("Quotations/UAT")]
        [SwaggerOperation(Summary = "Post Quote in UAT", Description = "Post a Quote Object in UAT")]
        public async Task<ActionResult<FWResponsePacket>> PostUAT(VendorQuoteModel model)
        {
            var fwQuote = await _converter.ConvertToFWUATInQuote(model);
            var fc = new FreightwareController(_configuration, _waybillrepo, _converter, _customerRepo, _transportType, _sites);
            var fwResponse = await fc.SetQuoteUAT(fwQuote);
            // long quoteId = 0;

            if (fwResponse.Value.ReturnCode != "0000") return fwResponse;
            var crmQuote = await _converter.ConvertFromFWUATQuote(fwResponse.Value.DataObject);
            try
            {
                crmQuote.Quote.CreatedByTSUserID = model.Quote.CreatedByTSUserID;
                crmQuote.Quote.CreatedOn = model.Quote.CreatedOn;
                var quoteId = await _quotesRepo.PostQuote(crmQuote, "CRM");

                if (quoteId > 0)
                {
                    return fwResponse;
                }
            }
            catch
            {
                //throw x;
            }

            return fwResponse;
        }

        /// <summary>
        /// Post a quote to Production
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Quotations/Production")]
        [SwaggerOperation(Summary = "Post Quote in Production", Description = "Post a Quote Object in Production")]
        public async Task<ActionResult<FWResponsePacket>> PostProduction(VendorQuoteModel model)
        {
            var fwQuote = await _converter.ConvertToFWInQuote(model);
            var fc = new FreightwareController(_configuration, _waybillrepo, _converter, _customerRepo, _transportType,_sites);
            var fwResponse = await fc.SetQuoteProduction(fwQuote);

            // long quoteId = 0;

            if (fwResponse.Value.ReturnCode != "0000") return fwResponse;
            var crmQuote = await _converter.ConvertFromFWQuote(fwResponse.Value.DataObject, model);
            try
            {


                crmQuote.Quote.SenLatitude = model.Quote.SenLatitude;
                crmQuote.Quote.SenLongitude = model.Quote.SenLongitude;
                crmQuote.Quote.SenPlusCode = model.Quote.SenPlusCode;
                crmQuote.Quote.RecLatitude = model.Quote.RecLatitude;
                crmQuote.Quote.RecLongitude = model.Quote.RecLongitude;
                crmQuote.Quote.RecPlusCode = model.Quote.RecPlusCode;
                crmQuote.Quote.CreatedByTSUserID = model.Quote.CreatedByTSUserID;
                crmQuote.Quote.CreatedOn = model.Quote.CreatedOn;
                crmQuote.Quote.RecName = model.Quote.RecName;
                crmQuote.Quote.RecAdd1 = model.Quote.RecAdd1;
                crmQuote.Quote.RecAdd2 = model.Quote.RecAdd2;
                crmQuote.Quote.RecAdd3 = model.Quote.RecAdd3;
                crmQuote.Quote.RecAdd4 = model.Quote.RecAdd4;
                crmQuote.Quote.RecAdd5 = model.Quote.RecAdd5;
                crmQuote.Quote.SenName = model.Quote.SenName;
                crmQuote.Quote.SenAdd1 = model.Quote.SenAdd1;
                crmQuote.Quote.SenAdd2 = model.Quote.SenAdd2;
                crmQuote.Quote.SenAdd3 = model.Quote.SenAdd3;
                crmQuote.Quote.SenAdd4 = model.Quote.SenAdd4;
                crmQuote.Quote.SenAdd5 = model.Quote.SenAdd5;
                var quoteId = await _quotesRepo.PostQuote(crmQuote, "CRM");

                if (quoteId > 0)
                {
                    return fwResponse;
                }
            }
            catch
            {
                //throw x;
            }

            return fwResponse;
        }

        /// <summary>
        /// Get quote as a PDF
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Quotations/GetQuoteDocument/{QuoteID}")]
        [SwaggerOperation(Summary = "Get Quote PDF", Description = "get Quote as a PDF")]

        public ActionResult<DocumentRepositories> GetQuoteDocument(int quoteId)
        {
            var client = new WebClient();
            {
                client.Credentials = new NetworkCredential("servicea", "S3rv1cEA2021", "tritonexpress");
            };
            var report = client.DownloadData(@"http://tiger/ReportServer/Pages/ReportViewer.aspx?/CRMReports/QuoteNewFormat&QuoteID=" + quoteId + "&rs:ClearSession=true&rs:Command=Render&rs:Format=PDF");
            var doc = new DocumentRepositories
            {
                ImgContentType = "application/pdf",
                ImgData = report,
                ImgName = "Quote.pdf",
                ImgLength = report.Length
            };
            return Ok(doc);
        }
        [HttpGet]
        [Route("Quotations/EmailQuoteDocument/{QuoteID}/{email}/{CreatedBy}")]
        [SwaggerOperation(Summary = "Get Quote PDF Email", Description = "get Quote as a PDF email")]
        public ActionResult<DocumentRepositories> EmailQuoteDocument(int QuoteID, string email, int CreatedBy)
        {
            var client = new WebClient();
            {
                client.Credentials = new NetworkCredential("servicea", "S3rv1cEA2021", "tritonexpress");
            };
            var report = client.DownloadData(@"http://tiger/ReportServer/Pages/ReportViewer.aspx?/CRMReports/QuoteNewFormat&QuoteID=" + QuoteID + "&rs:ClearSession=true&rs:Command=Render&rs:Format=PDF");
            var doc = new DocumentRepositories
            {
                ImgContentType = "application/pdf",
                ImgData = report,
                ImgName = "Quote.pdf",
                ImgLength = report.Length
            };
            var stream = new MemoryStream(report, 0, report.Length, false, true);
            var attachments = new List<System.Net.Mail.Attachment>();
            attachments.Add(new System.Net.Mail.Attachment(stream, "quote.pdf"));
            Core.Email.SendIntraSystemEmail(new[] { email }, null, "administrator@tritonexpress.co.za", "Please find attached the requested quotation", "Requested Quote document", "texdcmailmbx01", attachments, null);
            QuotationTracker quotationTracker = new QuotationTracker();
            quotationTracker.QuoteID = QuoteID;
            quotationTracker.From = "administrator@tritonexpress.co.za";
            quotationTracker.To = email;
            quotationTracker.CreatedBy = CreatedBy;
            quotationTracker.CreatedOn = DateTime.Now;
            _quotesRepo.PostQuotationEmail(quotationTracker);
            return Ok();
        }
        #endregion

        #region Invoice
        [HttpGet("Invoice/{invoiceId}"), SwaggerOperation(Summary = "Get Invoice By Invoice", Description = "Get InvoiceBy ID")]
        public async Task<ActionResult<Invoices>> GetInvoice(int invoiceId)
        {
            return await _invoice.GetInvoicesById(invoiceId);
        }

        [HttpGet("Invoice/"),
         SwaggerOperation(Summary = "Get Invoice By InvoiceNo", Description = "get INvoice By No")]
        public async Task<ActionResult<InvoiceSearchModel>> FindInvoice(string InvoiceNo, int CustomerID, DateTime? StartDate, DateTime? EndDate)
        {
            return await _invoice.GetInvoiceNo(InvoiceNo, CustomerID, StartDate, EndDate);
        }

        [HttpGet("{customerId}/Invoice/Document")]
        [SwaggerOperation(Summary = "Get excel invoice/s", Description = "Generate an excel version of an invoice")]
        public async Task<ActionResult<DocumentRepositories>> GetExcelInvoice(int customerId, string invoiceNumber, DateTime? startDate = null, DateTime? endDate = null)
        {
            var client = new WebClient
            {
                Credentials = new NetworkCredential("servicea", "S3rv1cEA2021", "tritonexpress")
            };

            string query = "";
            query = query + $"&InvoiceNumber{ (!String.IsNullOrEmpty(invoiceNumber) ? $"={invoiceNumber}" : ":Isnull=True")}"; ;
            query = query + $"&CustomerID={customerId}";
            query = query + $"&StartDate{ (startDate.HasValue ? $"={startDate.Value.ToString("yyyy-MM-dd")}" : ":isnull=true")}" + $"&EndDate{ (endDate.HasValue ? $"={endDate.Value.ToString("yyyy-MM-dd")}" : ":isnull=true")}";
            var report = await client.DownloadDataTaskAsync($"http://tiger/ReportServer/Pages/ReportViewer.aspx?/CRMReports/CustomerInvoices{query}&rs:ParameterLanguage=&rc:Parameters=Collapsed&rs:Command=Render&rs:Format=EXCEL&rs:ClearSession=truel");
            DocumentRepositories doc = new DocumentRepositories
            {
                ImgContentType = "application/xls",
                ImgData = report,
                ImgName = $"{invoiceNumber}.xlsx",
                ImgLength = report.Length
            };
            return Ok(doc);
        }

        #endregion

        #region Statements
        [HttpGet("{customerId}/Statement/{period}"), SwaggerOperation(Summary = "Get a customers statement for a period", Description = "Get a customers statement for a period")]
        public async Task<ActionResult<GetStatementResponseStatementOutput>> GetCustomerStatement(int customerId, DateTime period)
        {
            var customer = await _customerRepo.GetCrmCustomerById(customerId);
            FreightwareController fc = new FreightwareController(_configuration, _waybillrepo, _converter, _customerRepo, _transportType, _sites);
            var response = await fc.GetCustomerStatement(customer.AccountCode, period);
            return response.Value.DataObject;
        }

        #endregion
    }
}