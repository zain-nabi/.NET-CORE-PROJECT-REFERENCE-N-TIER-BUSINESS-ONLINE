using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Triton.Core;
using Triton.Interface.HR;
using Triton.Model.LeaveManagement.Custom;
using Triton.Model.LeaveManagement.Tables;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Repository.HR
{
    public class EmployeeRepository : IEmployee
    {
        private readonly IConfiguration _config;
        public EmployeeRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<Employees> GetEmployee(string currentEmployeeCode)
        {
            const string sql = "SELECT * FROM Employees WHERE CurrentEmployeeCode = @currentEmployeeCode";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.LeaveManagement));
            return connection.QueryFirst<Employees>(sql, new { currentEmployeeCode });
        }

        public async Task<Employees> GetEmployeeByOldUserId(int tritonSecurityUserId)
        {
            const string sql = "SELECT E.* FROM Employees E inner join EmployeeUserMap EM on EM.EmployeeID=E.EmployeeID WHERE EM.UserId = @tritonSecurityUserId";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.LeaveManagement));
            return connection.QueryFirst<Employees>(sql, new { tritonSecurityUserId });
        }

        public async Task<EmployeeUserMapModel> GetBranchManager(int costCentreId)
        {
            const string sql = @"SELECT * FROM Employees E 
                                INNER JOIN orgOrganogram O ON O.EmployeeID = E.EmployeeID
                                INNER JOIN EmployeeUserMap EUM ON EUM.EmployeeID = E.EmployeeID
                                INNER JOIN TritonGroup.dbo.Users U ON U.OldUserID = EUM.UserID
                                WHERE orgJobTitle = 43 
                                AND CostCentreID = @costCentreId
                                AND orgActive = 1";

            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.LeaveManagement));

            return connection.Query<Employees, orgOrganogram, EmployeeUserMap, Users, EmployeeUserMapModel>(sql,
                (employee, organogram, employeeUserMap, user) =>
                    new EmployeeUserMapModel
                    {
                        Employees = employee,
                        Organogram = organogram,
                        EmployeeUserMap = employeeUserMap,
                        Users = user
                    },
                new { costCentreId },
                splitOn: "EmployeeID, UserID").FirstOrDefault();
        }
    }
}
