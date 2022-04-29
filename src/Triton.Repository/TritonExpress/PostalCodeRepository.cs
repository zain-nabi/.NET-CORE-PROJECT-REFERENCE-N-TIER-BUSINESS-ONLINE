using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triton.Core;
using Triton.Interface.TritonGroup;
using Triton.Model.TritonExpress.Tables;

namespace Triton.Repository.TritonExpress
{
    public class PostalCodesRepository : IPostalCodes
    {
        private readonly IConfiguration _config;
        public PostalCodesRepository(IConfiguration configuration) { _config = configuration; }

        public async Task<List<PostalCodes>> GetAllPostalCodes(string branchCode)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonExpress));
            return connection.Query<PostalCodes>("SELECT * FROM PostalCodes WHERE BranchCode = @BranchCode", new { branchCode }).ToList();
        }


        public async Task<bool> PutPostalCodes(PostalCodes postalCodes)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonExpress));
            return connection.Update(postalCodes);
        }

        public async Task<List<PostalCodes>> GetPostalCodes(string name)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonExpress));
                return connection.Query<PostalCodes>("proc_PostalCodes_ByName_Select", new { name },
                    commandType: CommandType.StoredProcedure).ToList();
        }

        public async Task<PostalCodes> GetSenderPostCodeName(string date, DateTime collectionDate, string senderPostCodeName)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonExpress));
                return connection
                .Query<PostalCodes>(
                    "SELECT *, DATENAME (DW, @Date) AS NameOfDay, DDATEDIFF(D, @CollectionDate, @Date)As NumberOfDays FROM TritonExpress.dbo.PostalCodes WHERE Name = @SenderPostCodeName",
                    new { date, collectionDate, senderPostCodeName })
                .FirstOrDefault();
        }

        public async Task<List<PostalCodes>> GetPostalCodesByCode(string code)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.TritonExpress));
                return connection.Query<PostalCodes>("SELECT * FROM PostalCodes WHERE PostalCode = @Code", new { code }).ToList();
        }


    }
}
