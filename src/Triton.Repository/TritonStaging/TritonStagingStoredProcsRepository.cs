using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triton.Model.TritonStaging.StoredProcs;
using Microsoft.Extensions.Configuration;

namespace Triton.Repository.TritonStaging
{
    public class TritonStagingStoredProcsRepository:ITritonStagingStoredProcs
    {
        private static IConfiguration _config;
        public TritonStagingStoredProcsRepository(IConfiguration config) { _config = config; }
        public async Task<proc_FMOEndorsements_Signature_Select> GetPODSignature(long waybillId)
        {
             await using var connection = Connection.GetOpenConnection(_config.GetConnectionString("TritonStaging"));
             return connection.Query<proc_FMOEndorsements_Signature_Select>("proc_FMOEndorsements_Signature_Select", new { waybillId }, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }
    }
}
