using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triton.Core;
using Triton.Interface.TritonSecurity;
using Triton.Model.TritonSecurity.Tables;

namespace Triton.Repository.TritonSecurity
{
    public class BranchesRespository : IBranches
    {
        private readonly IConfiguration _config;

        public BranchesRespository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<List<Branches>> GetAllActiveBranches()
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonSecurity));
            {
                return connection.Query<Branches>("SELECT * FROM Branches WHERE Active = 1 ORDER BY BranchFullName").ToList();
            }
        }

        public async Task<List<Branches>> GetBranchesByBranchNameorFWDepotCode(string BranchSearch)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonSecurity));

            return connection.Query<Branches>("SELECT * FROM Branches WHERE BranchName = @BranchSearch or FWDepotCode = @BranchSearch or BranchFullName = @BranchSearch", new { BranchSearch }).ToList();
        }

        public async Task<Branches> GetQuestionnnaireBranch(int userId)
        {
            const string sql = @"SELECT DISTINCT B.*
                                FROM Branches B
                                INNER JOIN TritonOps.dbo.UserRoleBranchDepartmentMap URB ON URB.BranchID = B.BranchID
                                LEFT JOIN TritonGroup.dbo.Users U ON U.UserID = URB.UserID
                                WHERE U.UserID = @userId";

            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonSecurity));
            return connection.QueryFirst<Branches>(sql, new { userId });
        }

        public async Task<Branches> GetUserBranch(int userId/*, string OldUserID = "OldUserID"*/)
        {
            //if (OldUserID != null)
            //{

            //}
            const string sql = @"SELECT DISTINCT B.*
                                FROM Branches B
                                INNER JOIN TritonOps.dbo.UserRoleBranchDepartmentMap URB ON URB.BranchID = B.BranchID
                                LEFT JOIN TritonGroup.dbo.Users U ON U.OldUserID = URB.UserID
                                WHERE U.UserID = @userId";

            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonSecurity));
            return connection.QueryFirst<Branches>(sql, new { userId });
        }
    }
}
