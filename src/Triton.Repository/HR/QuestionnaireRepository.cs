using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Triton.Core;
using Triton.Interface.HR;
using Triton.Interface.TritonGroup;
using Triton.Interface.TritonSecurity;
using Triton.Model.Dashboard;
using Triton.Model.HelpDesk.StoredProcs;
using Triton.Model.LeaveManagement.Custom;
using Triton.Model.LeaveManagement.Tables;
using Triton.Model.TritonGroup.Tables;
using Triton.Model.TritonStaging.External.RWS;

namespace Triton.Repository.HR
{
    public class QuestionnaireRepository:IQuestionnaire
    {
        private static IConfiguration _config;
        private static IEmployee _employee;
        private static IUser _user;
        private static IBranches _branches;
        public QuestionnaireRepository(IConfiguration config,IEmployee employee,IUser user, IBranches branches) { _config = config; _employee=employee;_user=user; _branches = branches; }

        public async Task<QuestionnaireModel> GetQuestionaireCreateModel(int questionnaireTemplateId)
        {
                await using var connection = Connection.GetOpenConnection(_config.GetConnectionString("LeaveManagement"));
                QuestionnaireModel qm = new QuestionnaireModel();
                var sql = string.Format(@"  select * from QuestionnaireTemplates QT where QT.QuestionnaireTemplateID={0}
                                        	select Q.*,CLC.*,FLC.*,null as QuestionnaireAnswerMapId
                                                
	                                           from QuestionnaireTemplateMaps QTM 
                                               inner join QuestionnaireQuestions Q on QTM.QuestionnaireQuestionID=q.QuestionnaireQuestionID and QTM.DeletedOn is  null and Q.DeletedOn is  null
                                               inner join TritonGroup..Lookupcodes CLC on Q.QuestionCategoryLCID=CLC.LookupcodeId
                                               inner join TritonGroup..Lookupcodes FLC on Q.QuestionFieldTypeLCID=FLC.LookupcodeId
                                                where QuestionnaireTemplateID={0} order by QTM.[Sequence]
                                            ", questionnaireTemplateId);

                using (var multi = connection.QueryMultiple(sql))
                {
                    
                    qm.Template = multi.ReadFirstOrDefault<QuestionnaireTemplates>();
                    qm.QuestionsandAnswers = multi.Read<QuestionnaireQuestions,LookUpCodes,LookUpCodes,QuestionnaireAnswerMaps,QuestionAnswerModel>(
                        questions,
                        "LookupCodeID,LookupCodeID,QuestionnaireAnswerMapId"
                        ).ToList();
                    qm.Questionnaire = new Questionnaires
                    {
                        QuestionnaireTemplateId=questionnaireTemplateId
                    };

                }

                return qm;
        }

        public async Task<QuestionnaireModel> GetQuestionaireModel(long questionnaireId)
        {
                await using var connection = Connection.GetOpenConnection(_config.GetConnectionString("LeaveManagement"));
                QuestionnaireModel qm = new QuestionnaireModel();
                var sql = string.Format(@"  select QT.* from QuestionnaireTemplates QT inner join Questionnaires Q on QT.QuestionnaireTemplateId=Q.QuestionnaireTemplateID where Q.QuestionnaireID={0}
                                        	select Q.*,CLC.*,FLC.*,QMA.*
                                               from Questionnaires 
	                                           inner join QuestionnaireTemplateMaps QTM on Questionnaires.QuestionnaireTemplateID=QTM.QuestionnaireTemplateID
                                               inner join QuestionnaireQuestions Q on QTM.QuestionnaireQuestionID=q.QuestionnaireQuestionID and QTM.DeletedOn is  null and Q.DeletedOn is  null
                                               inner join TritonGroup..Lookupcodes CLC on Q.QuestionCategoryLCID=CLC.LookupcodeId
                                               inner join TritonGroup..Lookupcodes FLC on Q.QuestionFieldTypeLCID=FLC.LookupcodeId
                                               left outer join QuestionnaireAnswerMaps QMA on Questionnaires.QuestionnaireID=QMA.QuestionnaireId and Q.QuestionnaireQuestionID=QMA.QuestionId
                                                where Questionnaires.QuestionnaireID={0} order by QTM.[Sequence]
                                            select * from Questionnaires where QuestionnaireID={0}
                                            ", questionnaireId);

                using (var multi = connection.QueryMultiple(sql))
                {
                    
                    qm.Template = multi.ReadFirstOrDefault<QuestionnaireTemplates>();
                    qm.QuestionsandAnswers = multi.Read<QuestionnaireQuestions,LookUpCodes,LookUpCodes,QuestionnaireAnswerMaps,QuestionAnswerModel>(
                        questions,
                        "LookupCodeID,LookupCodeID,QuestionnaireAnswerMapId"
                        ).ToList();
                    qm.Questionnaire = multi.ReadFirstOrDefault<Questionnaires>();
                }

                return qm;
        }

        public async Task<QuestionnaireModel> FindTritonCovidQuestionaireModel(string EmployeeCode,DateTime CreatedDate)
        {
                await using var connection = Connection.GetOpenConnection(_config.GetConnectionString("LeaveManagement"));
                QuestionnaireModel qm = new QuestionnaireModel();
                var sql = string.Format(@"  select QT.* from QuestionnaireTemplates QT 
                                                inner join Questionnaires Q on QT.QuestionnaireTemplateId=Q.QuestionnaireTemplateID 
                                                inner join QuestionnaireAnswerMaps QMA on Q.QuestionnaireID=QMA.QuestionnaireID and QMA.QuestionId=13 where QMA.Response='{0}' and cast(QMA.CreatedOn as Date)=cast('{1}' as date) 
                                        	select Q.*,CLC.*,FLC.*,QMA.*
                                               from Questionnaires 
	                                           inner join QuestionnaireTemplateMaps QTM on Questionnaires.QuestionnaireTemplateID=QTM.QuestionnaireTemplateID
                                               inner join QuestionnaireQuestions Q on QTM.QuestionnaireQuestionID=q.QuestionnaireQuestionID and QTM.DeletedOn is  null and Q.DeletedOn is  null
                                               inner join TritonGroup..Lookupcodes CLC on Q.QuestionCategoryLCID=CLC.LookupcodeId
                                               inner join TritonGroup..Lookupcodes FLC on Q.QuestionFieldTypeLCID=FLC.LookupcodeId
                                               left outer join QuestionnaireAnswerMaps QMA on Questionnaires.QuestionnaireID=QMA.QuestionnaireId and Q.QuestionnaireQuestionID=QMA.QuestionId
                                                where Questionnaires.QuestionnaireID in
                                                (
                                                    select QMA.QuestionnaireID from QuestionnaireAnswerMaps QMA
                                                    where QMA.QuestionId=13 and QMA.Response='{0}' and cast(CreatedOn as Date)=cast('{1}' as date) 
                                                 )
                                            select * from Questionnaires where QuestionnaireID in
                                                (
                                                    select QMA.QuestionnaireID from QuestionnaireAnswerMaps QMA
                                                    where QMA.QuestionId=13 and QMA.Response='{0}' and cast(CreatedOn as Date)=cast('{1}' as date) 
                                                 )
                                            ", EmployeeCode,CreatedDate);

                using (var multi = connection.QueryMultiple(sql))
                {
                    
                    qm.Template = multi.ReadFirstOrDefault<QuestionnaireTemplates>();
                    qm.QuestionsandAnswers = multi.Read<QuestionnaireQuestions,LookUpCodes,LookUpCodes,QuestionnaireAnswerMaps,QuestionAnswerModel>(
                        questions,
                        "LookupCodeID,LookupCodeID,QuestionnaireAnswerMapId"
                        ).ToList();
                    qm.Questionnaire = multi.ReadFirstOrDefault<Questionnaires>();
                }

                return qm;
        }

        public async Task<QuestionnaireModel> FindByQuestionResponse(int questionId,string response,DateTime CreatedDate)
        {
                await using var connection = Connection.GetOpenConnection(_config.GetConnectionString("LeaveManagement"));
                QuestionnaireModel qm = new QuestionnaireModel();
                var sql = string.Format(@"  select QT.* from QuestionnaireTemplates QT 
                                                inner join Questionnaires Q on QT.QuestionnaireTemplateId=Q.QuestionnaireTemplateID 
                                                inner join QuestionnaireAnswerMaps QMA on Q.QuestionnaireID=QMA.QuestionnaireID and QMA.QuestionId={0} where QMA.Response='{1}' and cast(QMA.CreatedOn as Date)=cast('{2}' as date) 
                                        	select Q.*,CLC.*,FLC.*,QMA.*
                                               from Questionnaires 
	                                           inner join QuestionnaireTemplateMaps QTM on Questionnaires.QuestionnaireTemplateID=QTM.QuestionnaireTemplateID
                                               inner join QuestionnaireQuestions Q on QTM.QuestionnaireQuestionID=q.QuestionnaireQuestionID and QTM.DeletedOn is  null and Q.DeletedOn is  null
                                               inner join TritonGroup..Lookupcodes CLC on Q.QuestionCategoryLCID=CLC.LookupcodeId
                                               inner join TritonGroup..Lookupcodes FLC on Q.QuestionFieldTypeLCID=FLC.LookupcodeId
                                               left outer join QuestionnaireAnswerMaps QMA on Questionnaires.QuestionnaireID=QMA.QuestionnaireId and Q.QuestionnaireQuestionID=QMA.QuestionId
                                                where Questionnaires.QuestionnaireID in
                                                (
                                                    select QMA.QuestionnaireID from QuestionnaireAnswerMaps QMA
                                                    where QMA.QuestionId={0} and QMA.Response='{1}' and cast(CreatedOn as Date)=cast('{2}' as date) 
                                                 )
                                            select * from Questionnaires where QuestionnaireID in
                                                (
                                                    select QMA.QuestionnaireID from QuestionnaireAnswerMaps QMA
                                                    where QMA.QuestionId={0} and QMA.Response='{1}' and cast(CreatedOn as Date)=cast('{2}' as date) 
                                                 )
                                            ", questionId,response,CreatedDate);

                using (var multi = connection.QueryMultiple(sql))
                {
                    
                    qm.Template = multi.ReadFirstOrDefault<QuestionnaireTemplates>();
                    qm.QuestionsandAnswers = multi.Read<QuestionnaireQuestions,LookUpCodes,LookUpCodes,QuestionnaireAnswerMaps,QuestionAnswerModel>(
                        questions,
                        "LookupCodeID,LookupCodeID,QuestionnaireAnswerMapId"
                        ).ToList();
                    qm.Questionnaire = multi.ReadFirstOrDefault<Questionnaires>();
                }

                return qm;
        }
        public async Task<int> GetExcessTempCountforTritonCovid(string employeeCode,DateTime forDate)
        {
                await using var connection = Connection.GetOpenConnection(_config.GetConnectionString("LeaveManagement"));
                return connection.ExecuteScalar<int>($"proc_GetCountTempRecordsAboveThreshold",new { employeeCode,forDate },null,null,System.Data.CommandType.StoredProcedure);
        }

        public async Task<List<QuestionnaireSearchModel>> FindQuestionaireList(string identity,DateTime? forDate,int questionId=13)
        {
             await using var connection = Connection.GetOpenConnection(_config.GetConnectionString("LeaveManagement"));
             return connection.Query<QuestionnaireSearchModel>("procr_GetQuestionairesSearch",new { Identity = identity, fordate=forDate,questionId=questionId }, commandType: CommandType.StoredProcedure).ToList();
        }
        public async Task<long> Post(QuestionnairePostModel model)
        {
            //Check if Questionnaire BranchId has been set.
            if (!model.BranchId.HasValue)
            {
                //We need to check the creator if security - Role 12
                 var creatorRole = await _user.GetUserWithRoles(model.CreatedByGroupUserId,"12");
                 if (creatorRole.UserRoles!=null)
                    model.BranchId=creatorRole.UserRoles.BranchId;
                 else
                 {
                    //use the normal users branch as per the employee
                    var creator = await _user.FindByIdAsync(model.CreatedByGroupUserId);
                    var employee = await _employee.GetEmployeeByOldUserId(creator.OldUserID.Value);
                    model.BranchId=employee.BranchID;
                 }
            }

            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString("LeaveManagement"));
            var transaction=connection.BeginTransaction();
            try
            {
                //We need to create multiple parts, yes I could do it as a store proc maybe later.
                //Create the Questionaire
                var questionnaire = new Questionnaires
                {
                    QuestionnaireTemplateId=model.QuestionnaireTemplateId,
                    CreatedByGroupUserId=model.CreatedByGroupUserId,
                    CreatedOn=model.CreatedOn,
                    BranchId=model.BranchId
                };
               var questionnaireId = connection.Insert(questionnaire,transaction);
               foreach(QuestionnaireAnswerMaps map in model.Answers)
               {
                    map.QuestionnaireId=questionnaireId;
                    map.CreatedByGroupUserId=model.CreatedByGroupUserId;
                    map.CreatedOn=model.CreatedOn;
                    connection.Insert(map,transaction);
                    
               }
               transaction.Commit();
               return questionnaireId;
            }
            catch (TransactionAbortedException ex)
            {
                transaction.Rollback();
                throw ex;
            }
            catch (ApplicationException ex)
            {
                transaction.Rollback();
                throw ex;
            }
            catch( Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }
        private QuestionAnswerModel questions(QuestionnaireQuestions question, LookUpCodes category,LookUpCodes fieldType,QuestionnaireAnswerMaps answer)
        {

            if (answer==null)
                answer=new QuestionnaireAnswerMaps {QuestionId=question.QuestionnaireQuestionId };
            return new QuestionAnswerModel
            {
                Answer=answer,
                Question=question,
                QuestionCategoryCode=category,
                QuestionFieldType=fieldType
            };
        }

        public async Task SendCovidEmails(long questionnairId)
        {
            var model = await GetQuestionaireModel(questionnairId);
            var body="";
            //First Check the tempreture - Question 5
            var tempAnswer = model.QuestionsandAnswers.Find(x=>x.Answer.QuestionId==5);
            var baseTemp = _config.GetSection("Corvid-19").GetValue<decimal>("temperature");
            if (Convert.ToDecimal(tempAnswer.Answer.Response)>baseTemp)
            {
                if (model.Questionnaire.QuestionnaireTemplateId==1) //If Triton we need to fetch employee details
                {
                    var employeeCode = model.QuestionsandAnswers.Find(x=>x.Answer.QuestionId==13).Answer.Response;
                    var employee = await _employee.GetEmployee(employeeCode);
                    var employeeManager = await _employee.GetBranchManager(employee.BranchID.Value);
                    var userbranch = await _branches.GetQuestionnnaireBranch(employee.UserID.Value);
                    body = $"Please note that employee {employee.LeaveDisplayName} from {userbranch.BranchFullName} has recorded an excess temperature of {tempAnswer.Answer.Response} today.";
                    await Email.SendIntraSystemEmail(new[] { employeeManager.Users.Email },new []{"nikio@tritonexpress.co.za","biancaV@tritonexpress.co.za" }, "administrator@tritonexpress.co.za", body, $"Employee {employee.LeaveDisplayName} with high temperature.", $"{_config.GetSection("smtp").GetSection("ip").Value}", null);
                    if (model.Questionnaire.BranchId==8)
                        await Email.SendIntraSystemEmail(new[] { "#Cov19JNB@tritonexpress.co.za" },null, "administrator@tritonexpress.co.za", body, $"Employee {employee.LeaveDisplayName} with high temperature.", $"{_config.GetSection("smtp").GetSection("ip").Value}", null);

                }
                else //if a visitor just send a email with their details
                {
                    var visitorName = model.QuestionsandAnswers.Find(x=>x.Answer.QuestionId==1).Answer.Response;
                    var visitorContact = model.QuestionsandAnswers.Find(x=>x.Answer.QuestionId==2).Answer.Response;
                    var visitorCompany = model.QuestionsandAnswers.Find(x=>x.Answer.QuestionId==3).Answer.Response;
                    var visitorCompanyContact= model.QuestionsandAnswers.Find(x=>x.Answer.QuestionId==4).Answer.Response;
                    
                    body = $"Please note that a visitor {visitorName} from has recorded an excess temperature of {tempAnswer.Answer.Response} today. The visitors details are <br><br> Name: {visitorName} <br><br> Contact: {visitorContact} <br><br> Company: {visitorCompany} <br><br> Company Contact: { visitorCompanyContact} ";
                    await Email.SendIntraSystemEmail(new []{"nikio@tritonexpress.co.za","biancaV@tritonexpress.co.za" }, null, "administrator@tritonexpress.co.za", body, $"Visitor {visitorName} with high temperature. {DateTime.Now}", $"{_config.GetSection("smtp").GetSection("ip").Value}", null);
                    if (model.Questionnaire.BranchId==8)
                        await Email.SendIntraSystemEmail(new []{ "#Cov19JNB@tritonexpress.co.za" }, null, "administrator@tritonexpress.co.za", body, $"Visitor {visitorName} with high temperature. {DateTime.Now}", $"{_config.GetSection("smtp").GetSection("ip").Value}", null);
                }
            }
            
            //Second Check if any questions answered with triggers
            var positiveAnswers = model.QuestionsandAnswers.FindAll(x=>x.Question.QuestionFieldTypeLCId==388 && x.Answer.Response=="Yes"?true:false);
            
            if (positiveAnswers.Count()>0)
            {
                foreach(QuestionAnswerModel item in positiveAnswers)
                {
                    body=body+$" {item.Question.QuestionText} - {item.Answer.Response} <br><br>";
                }
                if (model.Questionnaire.QuestionnaireTemplateId==1) //If Triton we need to fetch employee details
                {
                    var employeeCode = model.QuestionsandAnswers.Find(x=>x.Answer.QuestionId==13).Answer.Response;
                    var employee = await _employee.GetEmployee(employeeCode);
                    var employeeManager = await _employee.GetBranchManager(employee.BranchID.Value);
                    body = $"Please note that employee {employee.LeaveDisplayName} has reported a postive response to certain questions today." + body;
                    await Email.SendIntraSystemEmail(new[] { employeeManager.Users.Email },new []{"nikio@tritonexpress.co.za","biancaV@tritonexpress.co.za" }, "administrator@tritonexpress.co.za", body, $"Employee {employee.LeaveDisplayName} with positive response. {DateTime.Now}", $"{_config.GetSection("smtp").GetSection("ip").Value}", null);
                     if (model.Questionnaire.BranchId==8)
                        await Email.SendIntraSystemEmail(new[] {"#Cov19JNB@tritonexpress.co.za" },new []{"nikio@tritonexpress.co.za","biancaV@tritonexpress.co.za" }, "administrator@tritonexpress.co.za", body, $"Employee {employee.LeaveDisplayName} with positive response. {DateTime.Now}", $"{_config.GetSection("smtp").GetSection("ip").Value}", null);
                }
                else //if a visitor just send a email with their details
                {
                    var visitorName = model.QuestionsandAnswers.Find(x=>x.Answer.QuestionId==1).Answer.Response;
                    var visitorContact = model.QuestionsandAnswers.Find(x=>x.Answer.QuestionId==2).Answer.Response;
                    var visitorCompany = model.QuestionsandAnswers.Find(x=>x.Answer.QuestionId==3).Answer.Response;
                    var visitorCompanyContact= model.QuestionsandAnswers.Find(x=>x.Answer.QuestionId==4).Answer.Response;
                    body = $"Please note that a visitor {visitorName} has reported a positive response to certain questions today. The visitors details are <br><br> Name: {visitorName} <br><br> Contact: {visitorContact} <br><br> Company: {visitorCompany} <br><br> Company Contact: { visitorCompanyContact} <br><br> And the positive responsed were: <br><br>" +body;
                    await Email.SendIntraSystemEmail(new []{"nikio@tritonexpress.co.za","biancaV@tritonexpress.co.za" }, null, "administrator@tritonexpress.co.za", body, $"Visitor {visitorName} with postive responses. {DateTime.Now}", $"{_config.GetSection("smtp").GetSection("ip").Value}", null);
                     if (model.Questionnaire.BranchId==8)
                        await Email.SendIntraSystemEmail(new []{"#Cov19JNB@tritonexpress.co.za" }, null, "administrator@tritonexpress.co.za", body, $"Visitor {visitorName} with postive responses. {DateTime.Now}", $"{_config.GetSection("smtp").GetSection("ip").Value}", null);
                }
            }
            //Third Check Triton Staff only, repeated high tempreture
            //check the employee number of tries
            if (model.Questionnaire.QuestionnaireTemplateId==1)
            { var attempt = await GetExcessTempCountforTritonCovid(model.QuestionsandAnswers.Find(x=>x.Answer.QuestionId==13).Answer.Response, DateTime.Now);

                if (attempt > 3)
                {
                    var employeeCode = model.QuestionsandAnswers.Find(x=>x.Answer.QuestionId==13).Answer.Response;
                    var employee = await _employee.GetEmployee(employeeCode);
                    var employeeManager = await _employee.GetBranchManager(employee.BranchID.Value);
                    body = $"Please note that employee {employee.LeaveDisplayName} has recorded an excess temperature more than {attempt} times today.";
                    //await Email.SendIntraSystemEmail(new[]{ manager.Users.Email,"nikio@tritonexpress.co.za"},null,"administrator@tritonexpress.co.za", body,"Employee repeated high temperature.",$"{_configuration.GetSection("smtp").GetSection("ip").Value}",null);
                    await Email.SendIntraSystemEmail(new[] { employeeManager.Users.Email },new []{"nikio@tritonexpress.co.za","biancaV@tritonexpress.co.za" }, "administrator@tritonexpress.co.za", body, $"Employee {employee.LeaveDisplayName} repeated high temperature. {DateTime.Now}", $"{_config.GetSection("smtp").GetSection("ip").Value}", null);
                     if (model.Questionnaire.BranchId==8)
                        await Email.SendIntraSystemEmail(new[] {"#Cov19JNB@tritonexpress.co.za" },new []{"nikio@tritonexpress.co.za","biancaV@tritonexpress.co.za" }, "administrator@tritonexpress.co.za", body, $"Employee {employee.LeaveDisplayName} repeated high temperature. {DateTime.Now}", $"{_config.GetSection("smtp").GetSection("ip").Value}", null);
                }
            }

        }

        [Obsolete("Only used in API and Service Layer")]
        public Task<DocumentRepositories> GetQuestionaireReport(int branchId, DateTime forDate)
        {
            throw new NotImplementedException();
        }

       
    }
}
