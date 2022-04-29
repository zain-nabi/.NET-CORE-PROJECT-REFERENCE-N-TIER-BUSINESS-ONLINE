using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triton.Model.LeaveManagement.Custom;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Interface.HR
{
    public interface IQuestionnaire
    {
        Task<QuestionnaireModel> GetQuestionaireCreateModel(int questionnaireTemplateId);
        Task<QuestionnaireModel> GetQuestionaireModel(long questionnaireId);
        Task<int> GetExcessTempCountforTritonCovid(string employeeCode,DateTime createdDate);
        Task<QuestionnaireModel> FindTritonCovidQuestionaireModel(string employeeCode,DateTime createdDate);
        Task<QuestionnaireModel> FindByQuestionResponse(int questionId,string response,DateTime createdDate);
        Task<List<QuestionnaireSearchModel>> FindQuestionaireList(string identity,DateTime? forDate,int questionId=13);
        Task<long> Post(QuestionnairePostModel model);
        Task SendCovidEmails(long questionnairID);
        Task<DocumentRepositories> GetQuestionaireReport(int branchId,DateTime forDate);
    }
}
