using System.Threading.Tasks;
using Triton.Model.LeaveManagement.Custom;
using Triton.Model.LeaveManagement.Tables;

namespace Triton.Interface.HR
{
    public interface IEmployee
    {
        Task<Employees> GetEmployee(string currentEmployeeCode);
        Task<EmployeeUserMapModel> GetBranchManager(int costCentreId);
        Task<Employees> GetEmployeeByOldUserId(int tritonSecurityUserId);
    }
}
