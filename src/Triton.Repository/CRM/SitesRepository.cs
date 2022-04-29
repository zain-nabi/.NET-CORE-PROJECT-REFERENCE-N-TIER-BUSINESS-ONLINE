using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Triton.Core;
using Triton.Interface.CRM;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.Tables;
using Triton.Model.FWWebservice.Custom;
using Vendor.Services.Freightware.PROD.GetPcodeList;

namespace Triton.Repository.CRM
{
    public class SitesRepository : ISites
    {
        //private readonly ICRMStoredProcs _cRMStoredProcs = new CRMStoredProcsRepository ();

        //private class FWSiteObject
        //{
        //    public SetSiteCode.SetSiteCodeResponseOutSiteCode FwSite { get; set; }
        //    public string IOSessionData { get; set; }
        //    public string ReturnCode { get; set; } = "";
        //    public string ReturnMessage { get; set; } = "";
        //}

        private readonly IConfiguration _config;
        private readonly string inUserid;
        private readonly string inPassword;
        private readonly string inRequestCtr;
        private readonly string inUATUserid;
        private readonly string inUATPassword;

        public SitesRepository(IConfiguration configuration)
        {
            _config = configuration;
            inUserid = configuration.GetSection("Freightware").GetSection("InUserid").Value;
            inPassword = configuration.GetSection("Freightware").GetSection("InPassword").Value;
            inRequestCtr = configuration.GetSection("Freightware").GetSection("InRequestCtr").Value;
            inUATUserid = configuration.GetSection("Freightware").GetSection("InUATUserid").Value;
            inUATPassword = configuration.GetSection("Freightware").GetSection("InUATPassword").Value;
        }

        public async Task<Sites> Get(int siteId, string dbName = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            return connection.Query<Sites>(@"SELECT * FROM Sites S  WHERE SiteID = @SiteID ORDER BY Address4, PostalCode", new { siteId }).FirstOrDefault();
        }
        public async Task<List<Sites>> GetSitesByCustomerId(int customerID, bool? savedSitesOnly = true, string dbName = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            string sql = "";
            if (savedSitesOnly.Value)
                sql = " and SiteName is not null and SiteName<>'' ";
            return connection.Query<Sites>(@"SELECT * FROM Sites S INNER JOIN CustomerSiteMaps CS ON CS.SiteID = S.SiteID WHERE DeletedOn is null and CustomerID = @CustomerID" + sql + " ORDER BY Address4, PostalCode", new { customerID }).ToList();
        }

        public async Task<bool> PutSite(Sites site, string dbName = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            return connection.Update(site);
        }

        public async Task<long> PostSite(Sites site, string dbName = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            return connection.Insert(site);
        }

        public async Task<List<Sites>> GetSitesBySiteNameAndSuburbForCustomer(string siteName, string suburb, int customerID, string dbName)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            return connection.Query<Sites>(@"SELECT S.* FROM Sites S
                                                LEFT JOIN CustomerSiteMaps CSM ON CSM.SiteID = S.SiteID
                                                LEFT JOIN Customers C ON C.CustomerID = CSM.CustomerID
                                                WHERE CSM.CustomerID = @CustomerID and S.SiteCustomerName=@SiteName and S.Address4=@Suburb
                                                ORDER BY SiteCustomerName", new { customerID, siteName, suburb }).ToList();
        }

        public async Task<FWResponsePacket> SetSiteUAT(SiteCodeModel model, string dbName = "CRM")
        {
            string spIOSessionData = "";

            Vendor.Services.Freightware.UAT.SetSiteCode.FWV6WEBPortClient client = new Vendor.Services.Freightware.UAT.SetSiteCode.FWV6WEBPortClient(Vendor.Services.Freightware.UAT.SetSiteCode.FWV6WEBPortClient.EndpointConfiguration.FWV6WEBSOAP11Port);
            Vendor.Services.Freightware.UAT.SetSiteCode.SetSiteCodeRequest request = new Vendor.Services.Freightware.UAT.SetSiteCode.SetSiteCodeRequest
            {
                InAction = new Vendor.Services.Freightware.UAT.SetSiteCode.SetSiteCodeInAction
                {
                    InAdd = true,
                    InAddSpecified = true
                },
                InSiteCode = new Vendor.Services.Freightware.UAT.SetSiteCode.SetSiteCodeInSiteCode
                {
                    AccountCode = model.AccountCode,
                    SiteCode = model.SiteName,
                    SiteName = model.SiteCustomerName,
                    PhysicalAddress1 = model.Address1,
                    PhysicalAddress2 = model.Address2,
                    PhysicalAddress3 = model.Address3,
                    PhysicalSuburb = model.Address4,
                    PhysicalPostCode = model.PostalCode,
                    TelephoneNumber = model.TelNo,
                    ContactName = model.Contact
                },
                InUserid = inUATUserid,
                InPassword = inUATPassword,
                InRequestCtr = inRequestCtr,
                IoSessionData = spIOSessionData
            };


            var response = new FWResponsePacket();
            try
            {
                var siteCodeResponse = await client.SetSiteCodeAsync(request);

                response.ReturnCode = siteCodeResponse.ReturnCode;
                response.ReturnMessage = siteCodeResponse.ReturnMessage;
                //Sites site = new Sites();
                //if (siteCodeResponse.ReturnCode == "0000")
                //{
                //    site.SiteName = model.SiteName;
                //    site.SiteCustomerName = model.SiteCustomerName;
                //    site.Address1 = model.Address1;
                //    site.Address2 = model.Address2;
                //    site.Address3 = model.Address3;
                //    site.Address4 = model.Address4;
                //    site.PostalCode = model.PostalCode;
                //    site.RateAreaID = model.RateAreaID;
                //    site.Longitude = model.Longitude;
                //    site.Lattitude = model.Lattitude;
                //    site.TelNo = model.TelNo;
                //    site.CellNo = model.CellNo;
                //    site.What3Words = model.What3Words;
                //    site.LastVerifiedOn = model.LastVerifiedOn;
                //    site.LastVerifiedBy = model.LastVerifiedBy;
                //    site.Email = model.Email;
                //    site.Contact = model.Contact;

                //    await PostSiteAsync(site);
                //}
            }
            catch (Exception x)
            {
                response.ReturnCode = "Internal Error";
                response.ReturnMessage = x.Message + " - " + x.InnerException;
            }
            return response;
        }

        public async Task<FWResponsePacket> SetSiteProduction(SiteCodeModel model)
        {
            string spIOSessionData = "";

            var client = new Vendor.Services.Freightware.PROD.SetSiteCode.FWV6WEBPortClient(Vendor.Services.Freightware.PROD.SetSiteCode.FWV6WEBPortClient.EndpointConfiguration.FWV6WEBSOAP11Port);
            var request = new Vendor.Services.Freightware.PROD.SetSiteCode.SetSiteCodeRequest
            {
                InAction = new Vendor.Services.Freightware.PROD.SetSiteCode.SetSiteCodeInAction
                {
                    InAdd = true,
                    InAddSpecified = true
                },
                InSiteCode = new Vendor.Services.Freightware.PROD.SetSiteCode.SetSiteCodeInSiteCode
                {
                    AccountCode = model.AccountCode,
                    SiteCode = model.SiteName,
                    SiteName = model.SiteCustomerName,
                    PhysicalAddress1 = model.Address1,
                    PhysicalAddress2 = model.Address2,
                    PhysicalAddress3 = model.Address3,
                    PhysicalSuburb = model.Address4,
                    PhysicalPostCode = model.PostalCode,
                    TelephoneNumber = model.TelNo,
                    ContactName = model.Contact
                },
                InUserid = inUserid,
                InPassword = inPassword,
                InRequestCtr = inRequestCtr,
                IoSessionData = spIOSessionData
            };


            var response = new FWResponsePacket();
            try
            {
                var siteCodeResponse = await client.SetSiteCodeAsync(request);

                response.ReturnCode = siteCodeResponse.ReturnCode;
                response.ReturnMessage = siteCodeResponse.ReturnMessage;
                //Sites site = new Sites();
                //if (siteCodeResponse.ReturnCode == "0000")
                //{
                //    site.SiteName = model.SiteName;
                //    site.SiteCustomerName = model.SiteCustomerName;
                //    site.Address1 = model.Address1;
                //    site.Address2 = model.Address2;
                //    site.Address3 = model.Address3;
                //    site.Address4 = model.Address4;
                //    site.PostalCode = model.PostalCode;
                //    site.RateAreaID = model.RateAreaID;
                //    site.Longitude = model.Longitude;
                //    site.Lattitude = model.Lattitude;
                //    site.TelNo = model.TelNo;
                //    site.CellNo = model.CellNo;
                //    site.What3Words = model.What3Words;
                //    site.LastVerifiedOn = model.LastVerifiedOn;
                //    site.LastVerifiedBy = model.LastVerifiedBy;
                //    site.Email = model.Email;
                //    site.Contact = model.Contact;

                //    await PostSiteAsync(site);
                //}
            }
            catch (Exception x)
            {
                response.ReturnCode = "Internal Error";
                response.ReturnMessage = x.Message + " - " + x.InnerException;
            }
            return response;
        }

        public async Task<long> PostSiteAsync(Sites model)
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            var postalRequest = new GetPcodeListRequest
            {
                InUserid = inUserid,
                InPassword = inPassword,
                Sequence = new GetPcodeListSequence
                {
                    ByNamePcode = false,
                    ByNamePcodeSpecified = false,
                    ByBranchPcode = false,
                    ByBranchPcodeSpecified = false,
                    ByHubBranchAreaPcode = false,
                    ByHubBranchAreaPcodeSpecified = false,
                    ByPcode = true,
                    ByPcodeSpecified = true,
                    ByPcodeName = false,
                    ByPcodeNameSpecified = false
                },
                StartValues = new GetPcodeListStartValues
                {
                    PostCode = model.PostalCode + "*"
                }
            };

            var client = new Vendor.Services.Freightware.PROD.GetPcodeList.FWV6WEBPortClient(Vendor.Services.Freightware.PROD.GetPcodeList.FWV6WEBPortClient.EndpointConfiguration.FWV6WEBSOAP11Port);
            var pc = await client.GetPcodeListAsync(postalRequest);
            string RateArea = null;
            foreach (var item in pc.PcodeOutputs.Take(1))
            {
                RateArea = item.Area;
            }

            model.RateAreaID = connection.Query<RateAreas>(@"SELECT RA.RateAreaID FROM CRM.dbo.RateAreas RA WHERE RA.ShortCode = @RateArea", new { RateArea }).FirstOrDefault().RateAreaID;
            var siteId = connection.Insert(model);
            return siteId;
        }

        //public List<SiteFromRoute> GetCRMSitesByRouteIDandDates(int RouteID, DateTime FromDate,DateTime ToDate)
        //{
        //    using (var connection = Connection.GetOpenConnection(ConfigurationManager.ConnectionStrings["CRM"].ConnectionString))
        //    {
        //        return connection.Query<SiteFromRoute>(@"proc_getCRMSitesByRouteIDandDates", new { RouteID,FromDate,ToDate }, commandType: CommandType.StoredProcedure).ToList();
        //    }
        //}

        //public Sites GetSiteFWBySiteID_CustomerID(int SiteID,int CustomerID)
        //{
        //    return CheckSiteFW(GetSiteBySiteID(SiteID), CustomerID);
        //}

        //private FWSiteObject GetFWSite(string siteCode, string stCustomerCode)
        //{

        //    string username = Properties.Resources.FWWebServiceUser;        //These paramaters are set in the resources tab of the project or the resources file
        //    string password = Properties.Resources.FWWebServicePassword;    //If added to the app.config, it will not pickup as the calling projects config will be used over the reference dll.
        //    string IOSessionData = "";

        //    SetSiteCode.SetSiteCode ssc = new SetSiteCode.SetSiteCode();
        //    SetSiteCode.SetSiteCodeInAction sscAction = new SetSiteCode.SetSiteCodeInAction
        //    {
        //        InGet = true,
        //        InGetSpecified = true
        //    };

        //    SetSiteCode.SetSiteCodeInSiteCode inSiteObj = new SetSiteCode.SetSiteCodeInSiteCode
        //    {
        //        AccountCode = stCustomerCode.ToUpper(),
        //        SiteCode = siteCode
        //    };

        //    ssc.CallSetSiteCode(username, password, stCustomerCode.ToUpper(), "1", ref IOSessionData, sscAction, inSiteObj, out string returnCode, out string returnMessage, out SetSiteCode.SetSiteCodeResponseOutSiteCode outSiteObj);
        //    ssc.Dispose();
        //    return new FWSiteObject { FwSite = outSiteObj, IOSessionData = IOSessionData, ReturnCode=returnCode,ReturnMessage=returnMessage };
        //}

        //private TVP_FWCustomerSites GetFWSiteForTvpUpdate(string siteCode, string stCustomerCode)
        //{

        //    string username = Properties.Resources.FWWebServiceUser;        //These paramaters are set in the resources tab of the project or the resources file
        //    string password = Properties.Resources.FWWebServicePassword;    //If added to the app.config, it will not pickup as the calling projects config will be used over the reference dll.
        //    string IOSessionData = "";

        //    GetSites.GetSiteList g = new GetSites.GetSiteList();
        //    GetSites.GetSiteListStartValues sv = new GetSites.GetSiteListStartValues
        //    {
        //        SiteCode=siteCode,CustomerCode=stCustomerCode
        //    };
        //    g.CallGetSiteList(username, password, stCustomerCode, "30", ref IOSessionData,null,sv, out string returnCode, out string returnMessage, out GetSites.GetSiteListResponseSiteOutput[] fwSites);

        //    if (fwSites != null && fwSites.Count() > 0 && fwSites[0].CustomerCode==stCustomerCode && fwSites[0].SiteCode==siteCode)
        //       return _cRMStoredProcs.ConvertFWSiteToTVP(fwSites[0]);
        //    else return null;
        //}

        //private Sites CheckSiteFW(Sites selectedSite, int? CustomerID)
        //{
        //    //This method assumes that it is being passed a FULL site object so it does not reload the site.
        //    TritonRepository.CRM.Tables.ICustomers custRepo = new TritonRepository.CRM.Tables.CustomersRepository();
        //    var _customer = custRepo.GetCRMCustomerByID(CustomerID.Value);
        //    FWSiteObject fwSite = GetFWSite(selectedSite.SiteName, _customer.AccountCode);
        //    //Ensure that stored value match latest Freightware Values
        //    if (fwSite != null && fwSite.ReturnCode=="0000")
        //    {
        //        selectedSite.SiteName = fwSite.FwSite.SiteCode;
        //        selectedSite.SiteCustomerName = fwSite.FwSite.SiteName;
        //        selectedSite.Address1 = fwSite.FwSite.PhysicalAddress1;
        //        selectedSite.Address2 = fwSite.FwSite.PhysicalAddress2;
        //        selectedSite.Address3 = fwSite.FwSite.PhysicalAddress3;
        //        selectedSite.Address4 = fwSite.FwSite.PhysicalSuburb;
        //        selectedSite.PostalCode = fwSite.FwSite.PhysicalPostCode;
        //        selectedSite.TelNo = fwSite.FwSite.TelephoneNumber;
        //        selectedSite.CellNo = fwSite.FwSite.FaxNumber;
        //        selectedSite.Lattitude = fwSite.FwSite.Latitude;
        //        selectedSite.Longitude = fwSite.FwSite.Longitude;
        //        selectedSite.Email = fwSite.FwSite.Email;
        //        selectedSite.Contact = fwSite.FwSite.ContactName;
        //        PutSite(selectedSite); //Ensure databases has all saved changes.

        //    }
        //    else selectedSite.FWActive = false;
        //    return selectedSite;
        //}

        //private FWResponsePacket SetFWSite(Sites output, int CustomerID)
        //{

        //    string username = Properties.Resources.FWWebServiceUser;        //These paramaters are set in the resources tab of the project or the resources file
        //    string password = Properties.Resources.FWWebServicePassword;    //If added to the app.config, it will not pickup as the calling projects config will be used over the reference dll.
        //    string IOSessionData = "";

        //    TritonRepository.CRM.Tables.ICustomers custRepo = new TritonRepository.CRM.Tables.CustomersRepository();
        //    var _customer = custRepo.GetCRMCustomerByID(CustomerID);
        //    FWSiteObject latestFWSite = GetFWSite(output.SiteName, _customer.AccountCode);
        //    SetSiteCode.SetSiteCode ssc = new SetSiteCode.SetSiteCode();
        //    SetSiteCode.SetSiteCodeInAction sscAction = new SetSiteCode.SetSiteCodeInAction();
        //    if (latestFWSite.ReturnCode!="0000")
        //    {
        //        sscAction.InAdd = true;
        //        sscAction.InAddSpecified = true;
        //        latestFWSite = new FWSiteObject
        //        {
        //            FwSite=new SetSiteCode.SetSiteCodeResponseOutSiteCode{
        //               AccountCode="NATIONAL",
        //               SiteCode=output.SiteName
        //           }
        //        };
        //    }
        //    else
        //    {
        //        sscAction.InModify = true;
        //        sscAction.InModifySpecified = true;
        //        IOSessionData = latestFWSite.IOSessionData;
        //    }





        //    SetSiteCode.SetSiteCodeInSiteCode InSite = (SetSiteCode.SetSiteCodeInSiteCode)latestFWSite.FwSite;

        //    InSite.Latitude = output.Lattitude;
        //    InSite.Longitude = output.Longitude;
        //    InSite.PhysicalAddress1 = output.Address1;
        //    InSite.PhysicalAddress2 = output.Address2;
        //    InSite.PhysicalAddress3 = output.Address3;
        //    InSite.PhysicalSuburb = output.Address4;
        //    InSite.PhysicalPostCode = output.PostalCode;
        //    InSite.CustomerContractNo = output.CellNo;
        //    InSite.SiteName = output.SiteCustomerName;
        //    InSite.TelephoneNumber = output.TelNo;
        //    InSite.Email = output.Email;
        //    InSite.ContactName = output.Contact;

        //    ssc.CallSetSiteCode(username, password, InSite.AccountCode, "1", ref IOSessionData, sscAction, InSite, out string returnCode, out string returnMessage, out SetSiteCode.SetSiteCodeResponseOutSiteCode outSiteObj);
        //    ssc.Dispose();
        //    return new FWResponsePacket
        //    {
        //        DataObject=outSiteObj,
        //        IOSessionData=IOSessionData,
        //        ReturnCode=returnCode,
        //        ReturnMessage=returnMessage
        //    };

        //}

        //private  FWResponsePacket SetFWSite(FWCustomerSites output, int CustomerID)
        //{

        //    string username = Properties.Resources.FWWebServiceUser;        //These paramaters are set in the resources tab of the project or the resources file
        //    string password = Properties.Resources.FWWebServicePassword;    //If added to the app.config, it will not pickup as the calling projects config will be used over the reference dll.
        //    string IOSessionData = "";

        //    TritonRepository.CRM.Tables.ICustomers custRepo = new TritonRepository.CRM.Tables.CustomersRepository();
        //    var _customer = custRepo.GetCRMCustomerByID(CustomerID);
        //    FWSiteObject latestFWSite = GetFWSite(output.SiteCode, _customer.AccountCode);
        //    SetSiteCode.SetSiteCode ssc = new SetSiteCode.SetSiteCode();
        //    SetSiteCode.SetSiteCodeInAction sscAction = new SetSiteCode.SetSiteCodeInAction();
        //    if (latestFWSite.ReturnCode!="0000")
        //    {
        //        sscAction.InAdd = true;
        //        sscAction.InAddSpecified = true;
        //        latestFWSite = new FWSiteObject
        //        {
        //            FwSite=new SetSiteCode.SetSiteCodeResponseOutSiteCode{
        //               AccountCode=_customer.AccountCode,
        //               SiteCode=output.SiteCode
        //           }
        //        };
        //    }
        //    else
        //    {
        //        sscAction.InModify = true;
        //        sscAction.InModifySpecified = true;
        //        IOSessionData = latestFWSite.IOSessionData;
        //    }

        //    SetSiteCode.SetSiteCodeInSiteCode InSite = (SetSiteCode.SetSiteCodeInSiteCode)latestFWSite.FwSite;

        //    InSite.Latitude = output.Latitude;
        //    InSite.Longitude = output.Longitude;
        //    InSite.PhysicalAddress1 = output.StreetAdd1;
        //    InSite.PhysicalAddress2 = output.StreetAdd2;
        //    InSite.PhysicalAddress3 = output.StreetAdd3;
        //    InSite.PhysicalSuburb = output.StreetAdd4;
        //    InSite.PhysicalPostCode = output.StreetAdd5;
        //    InSite.SiteName = output.SiteName;
        //    InSite.TelephoneNumber = output.TelephoneNo;
        //    InSite.Email = output.Email;
        //    InSite.ContactName = output.ContactPerson;
        //    InSite.FaxNumber=output.Fax;
        //    if (!String.IsNullOrEmpty(output.StatusCode))
        //        InSite.SiteStatus=output.StatusCode;
        //    ssc.CallSetSiteCode(username, password, InSite.AccountCode, "1", ref IOSessionData, sscAction, InSite, out string returnCode, out string returnMessage, out SetSiteCode.SetSiteCodeResponseOutSiteCode outSiteObj);
        //    ssc.Dispose();

        //    return new FWResponsePacket
        //    {
        //        DataObject=outSiteObj,
        //        IOSessionData=IOSessionData,
        //        ReturnCode=returnCode,
        //        ReturnMessage=returnMessage
        //    };

        //}
        //public bool PutBothSites(SiteFWUpdate site)
        //{
        //    FWResponsePacket savedInFreightware = SetFWSite(site.SelectedSite,site.CustomerID);
        //    if (savedInFreightware.ReturnCode=="0000")
        //    {
        //        using (var connection = Connection.GetOpenConnection(ConfigurationManager.ConnectionStrings["CRM"].ConnectionString))
        //        {
        //            return connection.Update(site.SelectedSite);
        //        }
        //    }
        //    else return false;
        //}

        //public ComboResponsePacket PutBothSites(TritonSiteFWUpdate site)
        //{
        //    ComboResponsePacket responsePacket = new ComboResponsePacket();
        //    FWResponsePacket savedInFreightware = SetFWSite(site.SelectedSite,site.CustomerID);
        //    bool success=false;
        //    responsePacket.FWResponse=savedInFreightware;
        //    if (savedInFreightware.ReturnCode=="0000")
        //    {
        //        try
        //        {

        //            success= _cRMStoredProcs.PostFWCustomerSite(new List<TritonModel.CRM.StoredProcs.TVP_FWCustomerSites>(){  GetFWSiteForTvpUpdate(savedInFreightware.DataObject.SiteCode,savedInFreightware.DataObject.AccountCode) });
        //            if (success)
        //            using (var connection = Connection.GetOpenConnection(ConfigurationManager.ConnectionStrings["CRM"].ConnectionString))
        //            {
        //                connection.Execute(@"   Update FWCustomerSites 
        //                                        set What3Words=@What3Words,LastVerifiedBy=@LastVerifiedBy,LastVerifiedOn=@LastVerifiedOn,DeliveryManifestLineItemID=@DeliveryManifestLineItemID,GoogleSuburb=@GoogleSuburb,GoogleCity=@GoogleCity,GooglePostalCode=@GooglePostalCode   
        //                                        WHERE CustomerID=@CustomerID and SiteCode=@SiteCode", 
        //                    new { site.SelectedSite.What3Words,site.SelectedSite.LastVerifiedBy,site.SelectedSite.LastVerifiedOn,site.SelectedSite.DeliveryManifestLineItemID,site.SelectedSite.GoogleSuburb,site.SelectedSite.GoogleCity,site.SelectedSite.GooglePostalCode,site.CustomerID,site.SelectedSite.SiteCode });
        //            }
        //            responsePacket.success=success;
        //        }
        //        catch ( Exception x)
        //        {
        //            success=false;
        //            responsePacket.success=success;
        //            responsePacket.Exception=x.Message;
        //            responsePacket.InnerException=x.InnerException.Message;
        //        }
        //    }
        //    return responsePacket;
        //}



        //public bool RemoveCRMCustomerSiteMap(SiteFWUpdate site)
        //{
        //    using (var connection = Connection.GetOpenConnection(ConfigurationManager.ConnectionStrings["CRM"].ConnectionString))
        //    {
        //        try
        //        {
        //            connection.Execute($"update CustomerSiteMaps set deletedOn='{DateTime.Now.ToString("yyyy-MM-dd")}',deletedByUserID=7 where CustomerID={site.CustomerID} and SiteID={site.SelectedSite.SiteID}");
        //            return true;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //}

        //public ComboResponsePacket RemoveFWCustomerSite(TritonSiteFWUpdate site)
        //{
        //    return PutBothSites(site);
        //}

        //public List<FWCustomerSites> GetFWCustomerSitesbyCustomerID(int customerID)
        //{
        //    using (var connection = Connection.GetOpenConnection(ConfigurationManager.ConnectionStrings["CRM"].ConnectionString))
        //    {
        //        return connection.Query<FWCustomerSites>(@"SELECT * FROM FWCustomerSites S  WHERE CustomerID = @CustomerID and StatusCode='1' ORDER BY StreetAdd4, StreetAdd5", new { customerID }).ToList();
        //    }
        //}

        //public List<TritonSiteFromRoute> GetFWCustomerSitesByRouteIDandDates(int routeID, DateTime fromDate, DateTime toDate)
        //{
        //    using (var connection = Connection.GetOpenConnection(ConfigurationManager.ConnectionStrings["CRM"].ConnectionString))
        //    {
        //        return connection.Query<TritonSiteFromRoute>(@"proc_getFWCustomerSitesByRouteIDandDates", new { routeID, fromDate, toDate }, commandType: CommandType.StoredProcedure).ToList();
        //    }
        //}

        //public FWCustomerSites GetCRMFWCustomerSiteByFWCustomerSiteID(int fWCustomerSiteID)
        //{
        //    using (var connection = Connection.GetOpenConnection(ConfigurationManager.ConnectionStrings["CRM"].ConnectionString))
        //    {
        //        return connection.Query<FWCustomerSites>(@"SELECT * FROM FWCustomerSites S  WHERE FWCustomerSiteID = @fwCustomerSiteID ORDER BY StreetAdd4, StreetAdd5", new { fWCustomerSiteID }).FirstOrDefault();
        //    }
        //}
    }
}
