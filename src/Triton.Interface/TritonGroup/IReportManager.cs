using System.Collections.Generic;
using System.Threading.Tasks;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Interface.TritonGroup
{
    public interface IReportManager
    {
        /// <summary>
        /// This method returns a List of object ReportManager based on the user's roles
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        Task<List<ReportManager>> Get(string roleIds);

        /// <summary>
        /// This method returns a List of object ReportManager based on the system , category and user's roles
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="categoryLciDs"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        Task<List<ReportManager>> Get(int systemId, string categoryLciDs, string roleIds);
        Task<ReportManager> GetReport(int reportManagerId);
    }
}
