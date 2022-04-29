using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using Triton.Interface.HR;
using Triton.Model.LeaveManagement.Custom;
using Triton.Model.TritonGroup.Tables;

namespace Triton.WebApi.Controllers.HR
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Questionnaire")]
    public class QuestionnaireController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IQuestionnaire _questionnaire;

        public QuestionnaireController(IConfiguration configuration,IQuestionnaire questionnaire)
        {
            _configuration=configuration;
            _questionnaire=questionnaire;

        }

        [HttpGet("Create/{questionnaireTemplateId}")]
        [SwaggerOperation(Summary = "Get create models - Retreives a predefined object for creation", Description = "Retreives a predefined object for creation")]
        public async Task<ActionResult<QuestionnaireModel>> GetQuestionnaireCreateModel(int questionnaireTemplateId)
        {
            if (questionnaireTemplateId == 0) 
                return BadRequest("a templateid is required as a parameter");

            return await _questionnaire.GetQuestionaireCreateModel(questionnaireTemplateId);
        }

        [HttpGet("{questionnaireId}")]
        [SwaggerOperation(Summary = "Get a questionnaire - Retreives a complex object ", Description = "Retreives a complex object")]
        public async Task<ActionResult<QuestionnaireModel>> Get(long questionnaireId)
        {
            if (questionnaireId == 0) 
                return BadRequest("a questionnaireId is required as a parameter");

            return await _questionnaire.GetQuestionaireModel(questionnaireId);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Post a Questionaire ", Description = "Posts a complex Questionaire model")]
        public async Task<ActionResult<long>> Post(QuestionnairePostModel model)
        {
            if (model==null)
                return BadRequest("model is invalid or null");
            var questionnaireId = await _questionnaire.Post(model);
            if (model.QuestionnaireTemplateId==1 || model.QuestionnaireTemplateId==2) //Check if a COVID-19 Questionnaire
                await _questionnaire.SendCovidEmails(questionnaireId);
            return questionnaireId;
        }

        [HttpGet("Find/{QuestionId}/Question/{response}/Date/{createdDate}")]
        [SwaggerOperation(Summary = "find a questionnaire - Retreives a complex object ", Description = "Search by question, response and date ,retreives a complex object")]
        public async Task<ActionResult<QuestionnaireModel>> Find(int questionId,string response,DateTime CreatedDate)
        {
            if (string.IsNullOrEmpty(response)) 
                return BadRequest("a questionid, response and date are required as parameters");

            return await _questionnaire.FindByQuestionResponse(questionId,response,CreatedDate);
        }

        [HttpGet("Find/TritonCovid/{employeeCode}/Date/{createdDate}")]
        [SwaggerOperation(Summary = "find a Triton Covid questionnaire - Retreives a complex object ", Description = "Search by EmployeeCode and date ,retreives a complex object")]
        public async Task<ActionResult<QuestionnaireModel>> FindforTritonCovidByEmployeeCode(string employeeCode,DateTime createdDate)
        {
            if (string.IsNullOrEmpty(employeeCode)) 
                return BadRequest("a employeecode is required as a parameter");

            return await _questionnaire.FindTritonCovidQuestionaireModel(employeeCode,createdDate);
        }

        [HttpGet("Find/List")]
        [SwaggerOperation(Summary = "Get a list of questionnaires ", Description = "Search for a list by a identity and/or date")]
        public async Task<ActionResult<List<QuestionnaireSearchModel>>> FindQuestionaireList(string identity,DateTime? forDate,int questionId=13)
        {
            if (string.IsNullOrEmpty(identity)&&!forDate.HasValue) 
                return BadRequest("you must pass either an identity or a date at least");

            return await _questionnaire.FindQuestionaireList(identity,forDate,questionId);
        }
        
        [HttpGet("Find/TritonCovid/Temp/{employeeCode}/{createdDate}")]
        [SwaggerOperation(Summary = "High Tempreture count for an employee - Retreives a int count ", Description = "Search by EmployeeCode and date ,retreives a count of tempretures above threshold")]
        public async Task<ActionResult<int>> GetExcessTempCountforTritonCovid(string employeeCode,DateTime createdDate)
        {
            if (string.IsNullOrEmpty(employeeCode)) 
                return BadRequest("a employeecode is required as a parameter");

            return await _questionnaire.GetExcessTempCountforTritonCovid(employeeCode,createdDate);
        }

        [HttpGet("Report/Branch/{branchId}/Date/{forDate}")]
        [SwaggerOperation(Summary = "Get a questionnaire report", Description = "Get a excel version of the questionnaire report, pass branchid of 0 for all branches")]
        public async Task<ActionResult<DocumentRepositories>> GetQuestionaireReport(int branchId,DateTime forDate)// bool test = false)
        {
            var client = new WebClient
            {
                Credentials = new NetworkCredential("servicea", "S3rv1cEA2021", "tritonexpress")
            };
            
            var report = await client.DownloadDataTaskAsync($"http://Tiger/ReportServer?/Leave/QuestionnaireRegister&BranchId={branchId}&ForDate={forDate:yyyy-MM-dd}&rs:ClearSession=true&rs:Command=Render&rs:Format=excel");
            DocumentRepositories doc = new DocumentRepositories
            {
                ImgContentType = "application/xls",
                ImgData = report,
                ImgName = "QuestionnaireReport.xls",
                ImgLength = report.Length
            };
            return Ok(doc);
        }
    }
}