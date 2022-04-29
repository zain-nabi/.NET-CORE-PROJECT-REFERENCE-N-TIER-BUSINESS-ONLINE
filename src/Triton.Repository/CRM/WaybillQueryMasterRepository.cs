using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Triton.Core;
using Triton.Interface.Waybill;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.Tables;

namespace Triton.Repository.CRM
{
    public class WaybillQueryMasterRepository : IWaybillQueryMaster
    {
        private readonly IConfiguration _config;
        public WaybillQueryMasterRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<WaybillQueryMaster> Get(int waybillQueryMasterId)
        {
            const string sql = "SELECT * FROM WaybillQueryMaster WHERE WaybillQueryMasterID = @waybillQueryMasterId";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.QueryFirstOrDefault<WaybillQueryMaster>(sql, new { waybillQueryMasterId });
        }

        public async Task<List<WaybillQueryMaster_Waybills_Model>> GetWaybillQueryMaster(int userId, string queryStatusLcid, int? systemId)
        {
            var lstModel = new List<WaybillQueryMaster_Waybills_Model>();
            List<WaybillQueryMaster_Waybills_Model> data;

            await using (var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm)))
            {
                var sql = $@"SELECT * FROM WaybillQueryMaster WQ
                            INNER JOIN Waybills W ON W.WaybillID = WQ.WaybillID
                            INNER JOIN WaybillStatuss WS ON WS.WaybillStatusID = W.WaybillStatusID
                            INNER JOIN CustomerSiteMaps CS ON CS.CustomerSiteMapID = W.ReceiveCustomerSiteMapID
                            INNER JOIN Sites RS ON RS.SiteID = CS.SiteID
                            WHERE WQ.CreatedByUserID = {userId} AND QueryStatusLCID IN ({queryStatusLcid})
                            AND WQ.DeletedOn IS NULL";

                if (systemId == 0)
                {
                    sql = $@"SELECT * FROM WaybillQueryMaster WQ
                            INNER JOIN Waybills W ON W.WaybillID = WQ.WaybillID
                            INNER JOIN WaybillStatuss WS ON WS.WaybillStatusID = W.WaybillStatusID
                            INNER JOIN CustomerSiteMaps CS ON CS.CustomerSiteMapID = W.ReceiveCustomerSiteMapID
                            INNER JOIN Sites RS ON RS.SiteID = CS.SiteID
                            WHERE WQ.CreatedByUserID = {userId} AND QueryStatusLCID IN ({queryStatusLcid})
                            AND WQ.DeletedOn IS NULL
                            INSERT INTO TritonGroup.dbo.UserAudit (PageName, SystemID, CreatedOn, CreatedByUserID) VALUES ('Dashboard', 27, '" + DateTime.Now + "', " + userId + ")";
                }

                data = connection.Query<WaybillQueryMaster, Waybills, WayBillStatuss, Sites, List<WaybillQueryMaster_Waybills_Model>>(
                        sql, (waybillQueryMaster, waybills, waybillStatus, sites) =>
                        {
                            var model = new WaybillQueryMaster_Waybills_Model
                            {
                                WaybillQueryMaster = waybillQueryMaster,
                                Waybills = waybills,
                                WayBillStatuss = waybillStatus,
                                ReceiverSites = sites
                            };

                            lstModel.Add(model);
                            return lstModel;
                        },
                        splitOn: "WaybillQueryMasterID, WaybillID, WaybillStatusID, SiteID").FirstOrDefault();

            }
            return data;
        }

        public async Task<List<WaybillQuery>> GetWaybillQueryList(long waybillId)
        {
            const string sql = "SELECT * FROM WaybillQuery WHERE WaybillID = @waybillId AND DeletedOn IS NULL";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.Query<WaybillQuery>(sql, new { waybillId }).ToList();
        }

        public async Task<bool> PutWaybillQueryMaster(WaybillQueryMaster waybillQueryMaster)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return await connection.UpdateAsync(waybillQueryMaster);
        }

        public async Task<bool> PostWaybillQueryMaster(WaybillQueryMasterInsertModel model)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.Query<bool>("proc_WaybillQuery_Notifications_Insert", new { model.WaybillId, model.UserId, model.Query, model.QueryStatusLcid, model.CreatedByUserId, model.Resolution, model.SystemId }, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public async Task<bool> Delete(long waybillId, int userId)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.Query<bool>("proc_WaybillQuery_Delete", new { waybillId, userId }, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }
    }
}
