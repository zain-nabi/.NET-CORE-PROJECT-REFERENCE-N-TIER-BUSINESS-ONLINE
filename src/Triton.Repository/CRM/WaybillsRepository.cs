using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Triton.Core;
using Triton.Interface.CRM;
using Triton.Interface.TritonGroup;
using Triton.Interface.TritonSecurity;
using Triton.Interface.Waybill;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.StoredProcs;
using Triton.Model.CRM.Tables;
using Triton.Model.CRM.Views;
using Triton.Model.LeaveManagement.Tables;
using Triton.Model.TritonExpress.Tables;
using Triton.Model.TritonOps.Tables;
using Triton.Model.TritonStaging.Tables;

namespace Triton.Repository.CRM
{
    public class WaybillsRepository : IWaybill
    {
        private readonly IConfiguration _config;
        private readonly ICustomers _customerRepo;
        private readonly ISites _siteRepo;
        private readonly IPostalCodes _postalCodeRepo;
        private readonly ITransportTypes _transportRepo;
        private readonly IBranches _branchRepo;
        private readonly ICustomerSiteMaps _customerMapRepo;

        public WaybillsRepository(IConfiguration configuration, ICustomers customer, ISites sites, IPostalCodes postalCodes, ITransportTypes transportTypes, IBranches branches, ICustomerSiteMaps customerSiteMaps)
        {
            _config = configuration;
            _customerRepo = customer;
            _siteRepo = sites;
            _postalCodeRepo = postalCodes;
            _transportRepo = transportTypes;
            _branchRepo = branches;
            _customerMapRepo = customerSiteMaps;
        }

        public async Task<List<vwOpsWaybills>> GetWaybillViewList(string customerIds, string waybillNo, string customerXRef, int? waybillId)
        {
            const string sql = "proc_GetWaybillByCustomerID";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.Query<vwOpsWaybills>(sql, new { CustomerID = customerIds, waybillNo, customerXRef, waybillId }, commandType: CommandType.StoredProcedure).ToList();
        }

        public async Task<Waybills> GetWaybillById(long waybillId)
        {
            const string sql = "SELECT * FROM Waybills WHERE WaybillID = @waybillId";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.QueryFirstOrDefault<Waybills>(sql, new { waybillId });
        }

        public async Task<Waybills> GetWaybillByWaybillNo(string waybillNo, string dbName = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            return connection.Query<Triton.Model.CRM.Tables.Waybills>(@"SELECT * FROM Waybills WHERE waybillNo = @waybillNo", new { waybillNo }).FirstOrDefault();
        }

        public async Task<WayBillInfoModel> GetWaybillInfoById(long waybillId)
        {
            var model = new WayBillInfoModel();
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            const string sql = @"EXEC proc_WaybillInfo_Select @WaybillID";

            using var multi = await connection.QueryMultipleAsync(sql, new { waybillId }, commandTimeout: 5000);
            model.Waybills = multi.Read<Waybills>().FirstOrDefault();
            model.Customers = multi.Read<Customers>().FirstOrDefault();
            model.WaybillTrackandTracesModel = multi.Read<WaybillTrackandTracesModel>().ToList();
            model.WaybillOpsNotes = multi.Read<WaybillOpsNotes>().ToList();
            model.CustomerSpecialArangements = multi.Read<CustomerSpecialArangements>().ToList();
            model.WaybillSurchargeModel = multi.Read<WaybillSurchargeModel>().ToList();
            model.ReceiverSites = multi.Read<Sites>().FirstOrDefault();
            model.SenderSites = multi.Read<Sites>().FirstOrDefault();
            model.FMOEndorsements = multi.Read<FMOEndorsements>().FirstOrDefault();
            model.WayBillStatuss = multi.Read<WayBillStatuss>().FirstOrDefault();
            model.TransportTypes = multi.Read<TransportTypes>().FirstOrDefault();
            model.WaybillQuery = multi.Read<WaybillQuery>().ToList();
            model.WaybillQueryMaster = multi.Read<WaybillQueryMaster>().FirstOrDefault();
            return model;
        }


        public async Task<List<vwOpsWaybills>> GetWaybillsByWaybillNo(string waybillNo)
        {
            const string sql = "proc_SELECT_FROM_vwOpsWaybills";
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.QueryFirst<List<vwOpsWaybills>>(sql, new { waybillNo }, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> StoreWaybillLines(dynamic waybill, long waybillId, string dbName = "CRM")
        {
            using var transaction = new TransactionScope();
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            bool result;
            try
            {
                connection.Execute($"Update WaybillFWLines set DeletedOn=getdate() where WaybillID={waybillId}", null);
                foreach (var line in waybill.WaybillLines)
                {
                    var waybillFwLine = new WaybillFWLine
                    {
                        WaybillID = waybillId,
                        LineChargeableUnits = line.LineChargeableUnits,
                        LineHgt = line.LineHgt,
                        LineItemChrg = line.LineItemChrg,
                        LineItemDesc = line.LineItemDesc,
                        LineItemLoadType = line.LineItemLoadType,
                        LineItemMass = line.LineItemMass,
                        LineItemQty = line.LineItemQty,
                        LineItemType = line.LineItemType,
                        LineLabelPrinted = line.LineLabelPrinted,
                        LineLen = line.LineLen,
                        LineNo = line.LineNo,
                        LineBth = line.LineBth,
                        LineParcelFrom = line.LineParcelFrom,
                        LineParcelTo = line.LineParcelTo,
                        LineParcelType = line.LineParcelType,
                        LineProdID = line.LineProdId,
                        LineVolWeight = line.LineVolWeight
                    };
                    connection.Insert(waybillFwLine);
                }
                transaction.Complete();
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public async Task<InternalWaybills> GetInternalWaybill(long internalWaybillID, string dbName = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            return connection.Query<Triton.Model.CRM.Tables.InternalWaybills>(@"SELECT * FROM InternalWaybills WHERE InternalWaybillID = @InternalWaybillID", new { internalWaybillID }).FirstOrDefault();
        }

        public async Task<InternalWaybills> GetInternalWaybillByReference(string waybillNo, string dbName = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            return connection.Query<Triton.Model.CRM.Tables.InternalWaybills>(@"SELECT * FROM InternalWaybills WHERE ReferenceNo = @WaybillNo", new { waybillNo }).FirstOrDefault();
        }

        public async Task<long> PostInternalWaybill(InternalWaybills internalWaybill, string dbName = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            return connection.Insert(internalWaybill);
        }

        public async Task<bool> PutInternalWaybill(InternalWaybills internalWaybill, string dbName = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            {
                bool _result = false;
                var waybill = await GetWaybillByWaybillNo(internalWaybill.ReferenceNo);
                if (waybill != null)
                {
                    internalWaybill.ReferenceNo = waybill.WaybillNo;
                    _result = connection.Update<InternalWaybills>(internalWaybill);
                }
                return _result;
            }
        }

        // The below methods are only used to create "light" entries into the system so that stickers can be generated via the API. Main waybill creation should be from the xml dump
        public async Task<long> PostWaybillFromFWWS(WaybillCustomerModel Waybill, string dbName = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            {
                var customer = await _customerRepo.GetCRMCustomerByAccountCode(Waybill.AcountCode);
                var FromSite = (await _siteRepo.GetSitesBySiteNameAndSuburbForCustomer(Waybill.SenderName, Waybill.SenderSuburbCode, customer.CustomerID, dbName)).FirstOrDefault();
                var ToSite = (await _siteRepo.GetSitesBySiteNameAndSuburbForCustomer(Waybill.ReceiverName, Waybill.ReceiverSuburbCode, customer.CustomerID, dbName)).FirstOrDefault();
                var fromcode = (await _postalCodeRepo.GetPostalCodesByCode(Waybill.SenderPostalCode)).FirstOrDefault();
                var tocode = (await _postalCodeRepo.GetPostalCodesByCode(Waybill.ReceiverPostalCode)).FirstOrDefault();
                if (FromSite == null)
                {
                    FromSite = new Sites
                    {
                        SiteCustomerName = Waybill.SenderName,
                        SiteName = customer.AccountCode + " " + Waybill.SenderName,
                        Address1 = Waybill.SenderAddress1,
                        Address2 = Waybill.SenderAddress2,
                        Address3 = Waybill.SenderAddress3,
                        Address4 = Waybill.SenderSuburbCode,
                        PostalCode = Waybill.SenderPostalCode,
                        CellNo = Waybill.SenderCell,
                        Contact = Waybill.SenderContact,
                        Email = Waybill.SenderEmail,
                        TelNo = Waybill.SenderTelNo,
                        RateAreaID = fromcode.RateAreaID.Value
                    };
                    FromSite.SiteID = Convert.ToInt32(await _siteRepo.PostSite(FromSite, dbName));

                }
                if (ToSite == null)
                {
                    ToSite = new Sites
                    {
                        SiteCustomerName = Waybill.ReceiverName,
                        SiteName = customer.AccountCode + " " + Waybill.ReceiverName,
                        Address1 = Waybill.ReceiverAddress1,
                        Address2 = Waybill.ReceiverAddress2,
                        Address3 = Waybill.ReceiverAddress3,
                        Address4 = Waybill.ReceiverSuburbCode,
                        PostalCode = Waybill.ReceiverPostalCode,
                        CellNo = Waybill.ReceiverCell,
                        Contact = Waybill.ReceiverContact,
                        Email = Waybill.ReceiverEmail,
                        TelNo = Waybill.ReceiverTelNo,
                        RateAreaID = tocode.RateAreaID.Value

                    };
                    ToSite.SiteID = Convert.ToInt32(await _siteRepo.PostSite(ToSite, dbName));
                }
                var frommap = await _customerMapRepo.GetCustomerSiteMapsByCustomerID_And_SiteID(customer.CustomerID, FromSite.SiteID, dbName);
                if (frommap == null)
                {
                    frommap = new Triton.Model.TritonGroup.Tables.CustomerSiteMaps
                    {
                        CustomerID = customer.CustomerID,
                        SiteID = FromSite.SiteID,
                        CreatedOn = DateTime.Now
                    };
                    await _customerMapRepo.PostCustomerSiteMap(frommap, dbName);
                    frommap = await _customerMapRepo.GetCustomerSiteMapsByCustomerID_And_SiteID(customer.CustomerID, FromSite.SiteID, dbName);
                }

                Triton.Model.TritonGroup.Tables.CustomerSiteMaps tomap = await _customerMapRepo.GetCustomerSiteMapsByCustomerID_And_SiteID(customer.CustomerID, ToSite.SiteID, dbName);
                if (tomap == null)
                {
                    tomap = new Triton.Model.TritonGroup.Tables.CustomerSiteMaps
                    {
                        CustomerID = customer.CustomerID,
                        SiteID = ToSite.SiteID,
                        CreatedOn = DateTime.Now
                    };
                    await _customerMapRepo.PostCustomerSiteMap(tomap, "CRM");
                    tomap = await _customerMapRepo.GetCustomerSiteMapsByCustomerID_And_SiteID(customer.CustomerID, ToSite.SiteID, "CRM");
                }



                var newWaybill = new Waybills
                {
                    CustomerID = customer.CustomerID,
                    ChargeMass = Waybill.TotalChargeUnits,
                    SendCustomerSiteMapID = frommap.CustomerSiteMapID,
                    ReceiveCustomerSiteMapID = tomap.CustomerSiteMapID,
                    TotalQty = Convert.ToInt32(Waybill.TotalQty),
                    TotalValue = Waybill.TotalValue,
                    TotalMass = Waybill.TotalMass,
                    FromBranchID = (await _branchRepo.GetBranchesByBranchNameorFWDepotCode(fromcode.BranchCode)).FirstOrDefault().BranchID,
                    ToBranchID = (await _branchRepo.GetBranchesByBranchNameorFWDepotCode(tocode.BranchCode)).FirstOrDefault().BranchID,
                    WaybillDate = Waybill.WaybillDate,
                    WaybillNo = Waybill.WaybillNo,
                    ServiceTypeID = (await _transportRepo.GetAllTransportTypes()).Find(x => x.Description.Trim() == Waybill.ServiceType.Trim()).TransportTypeID,
                    WaybillStatusID = 12
                };
                return connection.Insert(newWaybill);
            }
        }

        public async Task<CreditNotePageModel> GetCreditNotePage(string waybillNo)
        {
            var model = new CreditNotePageModel();
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            using var multi = await connection.QueryMultipleAsync("proc_Waybill_CRNoteTemp_Select", new {waybillNo}, commandType: CommandType.StoredProcedure);

            model.WaybillCreditModel = multi.Read<WaybillCreditModel>().First();
            model.Employees = multi.Read<Employees>().ToList();
            model.ReasonCodeList = multi.Read<Model.TritonGroup.Tables.LookUpCodes>().ToList();
            return model;
        }
    }
}
