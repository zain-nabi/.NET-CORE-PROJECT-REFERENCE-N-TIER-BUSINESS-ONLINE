using System;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Triton.Model.FWWebservice.Custom;
using Vendor.Services.Freightware.PROD.GetChargesList;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;
using Vendor.Services.Freightware.PROD.GetSiteList;
using Triton.Model.CRM.Custom;
using Triton.Interface.Waybill;
using Vendor.Services.Helpers;
using Vendor.Services.Freightware.PROD.GetPcodeList;
using Vendor.Services.Freightware.UAT.SetCollection;
using Triton.Model.CRM.Tables;
using Triton.Interface.CRM;
using Vendor.Services.Freightware.PROD.SetQuote;

namespace Triton.WebApi.Controllers.Freightware
{
    [Route("api/[controller]")]
    [ApiController]
    public class FreightwareController : ControllerBase
    {
        private readonly string inUserid;
        private readonly string inPassword;
        private readonly string inRequestCtr;
        private readonly string inUATUserid;
        private readonly string inUATPassword;
        private readonly IWaybill _waybillsRepo;
        private readonly IVendorServices _converter;
        private readonly ICustomers _customer;
        private readonly ITransportTypes _transportType;
        private readonly ISites _sites;


        public FreightwareController(IConfiguration config, IWaybill waybills, IVendorServices converters,ICustomers customers,ITransportTypes transportTypes, ISites sites)
        {
            inUserid = config.GetSection("Freightware").GetSection("InUserid").Value;
            inPassword = config.GetSection("Freightware").GetSection("InPassword").Value;
            inRequestCtr = config.GetSection("Freightware").GetSection("InRequestCtr").Value;
            inUATUserid = config.GetSection("Freightware").GetSection("InUATUserid").Value;
            inUATPassword = config.GetSection("Freightware").GetSection("InUATPassword").Value;
            _waybillsRepo = waybills;
            _converter = converters;
            _customer=customers;
            _transportType=transportTypes;
            _sites = sites;
        }

        [HttpGet("Surcharges")]
        [SwaggerOperation(Summary = "Freightware Surcharge", Description = "Get currently active surcharges.")]
        public async Task<ActionResult<FWResponsePacket>> Get()
        {
            var request = new GetChargesListRequest
            {
                InUserid = inUserid,
                InPassword = inPassword,
                InRequestCtr = inRequestCtr,
                Sequence = new GetChargesListSequence
                {
                    ByTypeCodeSpecified = false
                },
                EffectiveDate = DateTime.Now.ToString("yyyyMMdd")
            };
            var client = new Vendor.Services.Freightware.PROD.GetChargesList.FWV6WEBPortClient(Vendor.Services.Freightware.PROD.GetChargesList.FWV6WEBPortClient.EndpointConfiguration.FWV6WEBSOAP11Port);
            var response = new FWResponsePacket();
            try
            {
                List<GetChargesListResponseChargesOutput> filtered = new List<GetChargesListResponseChargesOutput>();
                var x = await client.GetChargesListAsync(request);
                //Store initial Results
                if (x.ReturnCode == "0000" || x.ReturnCode == "0130")
                    filtered = (from c in x.ChargesOutputs
                                select c
                                ).ToList();

                //Get remaining charges if anyes
                while (x.ReturnCode == "0130")
                {
                    request.IoSessionData = x.IoSessionData;
                    x = await client.GetChargesListAsync(request);
                    foreach (var v in x.ChargesOutputs)
                    {
                        filtered.Add(v);
                    }
                }

                response.ReturnCode = x.ReturnCode;
                response.ReturnMessage = x.ReturnMessage;
                response.DataObject = filtered;
            }
            catch (Exception x)
            {
                response.ReturnCode = "Internal Error";
                response.ReturnMessage = x.Message + " - " + x.InnerException;
            }
            return response;
        }

        [HttpGet("CustomerSites/Customer/{customerCode}/SiteCode/{siteCode}")]
        [SwaggerOperation(Summary = "Get list of Customer Sites", Description = "Get a list of customer sites.")]
        public async Task<ActionResult<FWResponsePacket>> GetSiteList(string customerCode, string siteCode)
        {
            var request = new GetSiteListRequest
            {
                InUserid = inUserid,
                InPassword = inPassword,
                InRequestCtr = inRequestCtr,
                StartValues = new GetSiteListStartValues
                {
                    CustomerCode = customerCode.ToUpper(),
                    SiteCode = siteCode.ToUpper() + "*"
                },
                Sequence = new GetSiteListSequence
                {
                    ByCustomerSite = true,
                    ByCustomerSiteSpecified = true,
                    ByNameSite = true,
                    ByNameSiteSpecified = true
                }
            };

            var client = new Vendor.Services.Freightware.PROD.GetSiteList.FWV6WEBPortClient(Vendor.Services.Freightware.PROD.GetSiteList.FWV6WEBPortClient.EndpointConfiguration.FWV6WEBSOAP11Port);
            var response = new FWResponsePacket();
            try
            {
                List<GetSiteListResponseSiteOutput> filtered = new List<GetSiteListResponseSiteOutput>();
                var x = await client.GetSiteListAsync(request);
                //Store initial Results
                if (x.ReturnCode == "0000" || x.ReturnCode == "0130")
                    filtered = (from c in x.SiteOutputs
                                select c
                                ).ToList();

                //Get remaining charges if anyes
                while (x.ReturnCode == "0130")
                {
                    request.IoSessionData = x.IoSessionData;
                    x = await client.GetSiteListAsync(request);
                    foreach (var v in x.SiteOutputs)
                    {
                        filtered.Add(v);
                    }
                    request.IoSessionData = x.IoSessionData;
                }

                response.ReturnCode = x.ReturnCode;
                response.ReturnMessage = x.ReturnMessage;
                response.DataObject = filtered;
            }
            catch (Exception x)
            {
                response.ReturnCode = "Internal Error";
                response.ReturnMessage = x.Message + " - " + x.InnerException;
            }
            return response;
        }

        [HttpGet("UAT/CustomerSites/Customer/{customerCode}/SiteCode/{siteCode}")]
        [SwaggerOperation(Summary = "Get list of Customer Sites", Description = "Get a list of customer sites.")]
        public async Task<ActionResult<FWResponsePacket>> GetSiteListUAT(string customerCode, string siteCode)
        {
            var request = new Vendor.Services.Freightware.UAT.GetSiteList.GetSiteListRequest
            {
                InUserid = inUATUserid,
                InPassword = inUATPassword,
                InRequestCtr = inRequestCtr,
                StartValues = new Vendor.Services.Freightware.UAT.GetSiteList.GetSiteListStartValues
                {
                    CustomerCode = customerCode.ToUpper(),
                    SiteCode = siteCode.ToUpper() + "*"
                },
                Sequence = new Vendor.Services.Freightware.UAT.GetSiteList.GetSiteListSequence
                {
                    ByCustomerSite = true,
                    ByCustomerSiteSpecified = true,
                    ByNameSite = true,
                    ByNameSiteSpecified = true
                }
            };

            var client = new Vendor.Services.Freightware.UAT.GetSiteList.FWV6WEBPortClient(Vendor.Services.Freightware.UAT.GetSiteList.FWV6WEBPortClient.EndpointConfiguration.FWV6WEBSOAP11Port);
            var response = new FWResponsePacket();
            try
            {
                List<Vendor.Services.Freightware.UAT.GetSiteList.GetSiteListResponseSiteOutput> filtered = new List<Vendor.Services.Freightware.UAT.GetSiteList.GetSiteListResponseSiteOutput>();
                var x = await client.GetSiteListAsync(request);
                //Store initial Results
                if (x.ReturnCode == "0000" || x.ReturnCode == "0130")
                    filtered = (from c in x.SiteOutputs
                                select c
                                ).ToList();

                //Get remaining charges if anyes
                while (x.ReturnCode == "0130")
                {
                    request.IoSessionData = x.IoSessionData;
                    x = await client.GetSiteListAsync(request);
                    foreach (var v in x.SiteOutputs)
                    {
                        filtered.Add(v);
                    }
                    request.IoSessionData = x.IoSessionData;
                }

                response.ReturnCode = x.ReturnCode;
                response.ReturnMessage = x.ReturnMessage;
                response.DataObject = filtered;
            }
            catch (Exception x)
            {
                response.ReturnCode = "Internal Error";
                response.ReturnMessage = x.Message + " - " + x.InnerException;
            }
            return response;
        }

        [HttpPost("Quotes/UAT")]
        [SwaggerOperation(Summary = "Post a quote to UAT ", Description = "Post a quote to UAT")]
        public async Task<ActionResult<FWResponsePacket>> SetQuoteUAT(Vendor.Services.Freightware.UAT.SetQuote.SetQuoteInQuote setQuoteInQuote)
        {

            var request = new Vendor.Services.Freightware.UAT.SetQuote.SetQuoteRequest
            {
                InUserid = inUATUserid,
                InPassword = inUATPassword,
                InRequestCtr = inRequestCtr,
                InAction = new Vendor.Services.Freightware.UAT.SetQuote.SetQuoteInAction
                {
                    InAddSpecified = true,
                    InAdd = true
                },
                InQuote = setQuoteInQuote
            };
            var client = new Vendor.Services.Freightware.UAT.SetQuote.FWV6WEBPortClient(Vendor.Services.Freightware.UAT.SetQuote.FWV6WEBPortClient.EndpointConfiguration.FWV6WEBSOAP11Port);
            var response = new FWResponsePacket();
            try
            {
                var x = await client.SetQuoteAsync(request);

                response.ReturnCode = x.ReturnCode;
                response.ReturnMessage = x.ReturnMessage;
                response.DataObject = x.Quote;
                response.Reference = x.Quote.QuoteNo;
            }
            catch (Exception x)
            {
                response.ReturnCode = "Internal Error";
                response.ReturnMessage = x.Message + " - " + x.InnerException;
            }
            return response;
        }

        [HttpPost("Quotes/Production")]
        [SwaggerOperation(Summary = "Post a quote to Production ", Description = "Post a quote to Production")]
        public async Task<ActionResult<FWResponsePacket>> SetQuoteProduction(Vendor.Services.Freightware.PROD.SetQuote.SetQuoteInQuote setQuoteInQuote)
        {

            Customers Customer = await _customer.GetCRMCustomerByAccountCode(setQuoteInQuote.CustCode);

            Vendor.Services.Freightware.PROD.SetQuote.SetQuoteInQuote sq = new Vendor.Services.Freightware.PROD.SetQuote.SetQuoteInQuote();
            Vendor.Services.Freightware.PROD.SetQuote.SetQuoteInAction sqAction = new Vendor.Services.Freightware.PROD.SetQuote.SetQuoteInAction();
            if (Customer.FWPriceCheckQuoteNo == null || Customer.FWPriceCheckQuoteNo.Length == 0)
            {
                sqAction.InAddSpecified = true;
                sqAction.InAdd = true;
            }
            else
            {
                sqAction.InModifySpecified = true;
                sqAction.InModify = true;
            }

            SetQuoteInQuote inQuoteObj = new SetQuoteInQuote
            {
                DateQuote = DateTime.Now.ToString("yyyyMMdd"),
                CustCode = setQuoteInQuote.CustCode,
                ServiceType = setQuoteInQuote.ServiceType,
                ReceiverInformation = new SetQuoteInQuoteReceiverInformation
                {
                    RecAdd4 = setQuoteInQuote.ReceiverInformation.RecAdd4,
                    RecAdd5 = setQuoteInQuote.ReceiverInformation.RecAdd5
                },
                SenderInformation = new SetQuoteInQuoteSenderInformation
                {
                    SenAdd4 = setQuoteInQuote.SenderInformation.SenAdd4,
                    SenAdd5 = setQuoteInQuote.SenderInformation.SenAdd5
                }
            };

            inQuoteObj = setQuoteInQuote;


            var request = new Vendor.Services.Freightware.PROD.SetQuote.SetQuoteRequest
            {
                InUserid = inUserid,
                InPassword = inPassword,
                InRequestCtr = inRequestCtr,
                InAction = new Vendor.Services.Freightware.PROD.SetQuote.SetQuoteInAction
                {
                    InAddSpecified = true,
                    InAdd = true
                },
                InQuote = inQuoteObj
            };

            var client = new Vendor.Services.Freightware.PROD.SetQuote.FWV6WEBPortClient(Vendor.Services.Freightware.PROD.SetQuote.FWV6WEBPortClient.EndpointConfiguration.FWV6WEBSOAP12Port);
            var response = new FWResponsePacket();
            try
            {
                var x = await client.SetQuoteAsync(request);

                response.ReturnCode = x.ReturnCode;
                response.ReturnMessage = x.ReturnMessage;
                response.DataObject = x.Quote;
                response.Reference = x.Quote?.QuoteNo;
            }
            catch (Exception x)
            {
                response.ReturnCode = "Internal Error";
                response.ReturnMessage = x.Message + " - " + x.InnerException;
            }
            return response;
        }


        [HttpGet("UAT/Waybill/")]
        [SwaggerOperation(Summary = "Get UAT waybill", Description = "Get a uat waybill.")]
        public async Task<ActionResult<FWResponsePacket>> GetWaybillUAT(string waybillNo)
        {
            var request = new Vendor.Services.Freightware.UAT.GetWaybill.GetWaybillRequest
            {
                InUserid = inUATUserid,
                InPassword = inUATPassword,
                InRequestCtr = inRequestCtr,
                InWaybillNo = waybillNo
            };

            var client = new Vendor.Services.Freightware.UAT.GetWaybill.FWV6WEBPortClient(Vendor.Services.Freightware.UAT.GetWaybill.FWV6WEBPortClient.EndpointConfiguration.FWV6WEBSOAP11Port);
            var response = new FWResponsePacket();
            try
            {
                var x = await client.GetWaybillAsync(request);

                response.ReturnCode = x.ReturnCode;
                response.ReturnMessage = x.ReturnMessage;
                response.DataObject = x.Waybill;
            }
            catch (Exception x)
            {
                response.ReturnCode = "Internal Error";
                response.ReturnMessage = x.Message + " - " + x.InnerException;
            }
            return response;
        }


        [HttpGet("Waybill/")]
        [SwaggerOperation(Summary = "Get waybill", Description = "Get a waybill.")]
        public async Task<ActionResult<FWResponsePacket>> GetWaybill(string waybillNo)
        {
            var request = new Vendor.Services.Freightware.PROD.GetWaybill.GetWaybillRequest
            {
                InUserid = inUserid,
                InPassword = inPassword,
                InRequestCtr = inRequestCtr,
                InWaybillNo = waybillNo
            };

            var client = new Vendor.Services.Freightware.PROD.GetWaybill.FWV6WEBPortClient(Vendor.Services.Freightware.PROD.GetWaybill.FWV6WEBPortClient.EndpointConfiguration.FWV6WEBSOAP11Port);
            var response = new FWResponsePacket();
            try
            {
                var x = await client.GetWaybillAsync(request);

                response.ReturnCode = x.ReturnCode;
                response.ReturnMessage = x.ReturnMessage;
                response.DataObject = x.Waybill;
            }
            catch (Exception x)
            {
                response.ReturnCode = "Internal Error";
                response.ReturnMessage = x.Message + " - " + x.InnerException;
            }
            return response;
        }
        
        [HttpPost("UAT/Waybill/")]
        [SwaggerOperation(Summary = "Post UAT waybill", Description = "Post a UAT waybill.")]
        public async Task<ActionResult<FWResponsePacket>> PostWaybillUAT(CustomerWaybillSubmitModels model, string dbName = "CRMTest")
        {
            var spIOSessionData = "";

            var client = new Vendor.Services.Freightware.UAT.SetWaybill.FWV6WEBPortClient(Vendor.Services.Freightware.UAT.SetWaybill.FWV6WEBPortClient.EndpointConfiguration.FWV6WEBSOAP11Port);
            var request = new Vendor.Services.Freightware.UAT.SetWaybill.SetWaybillRequest
            {
                InAction = new Vendor.Services.Freightware.UAT.SetWaybill.SetWaybillInAction
                {
                    InAdd = true,
                    InAddSpecified = true
                },
                InWaybill = new Vendor.Services.Freightware.UAT.SetWaybill.SetWaybillInWaybill
                {
                    DateWaybill = DateTime.Now.ToString("yyyyMMdd"),
                    CustCode = model.CustCode,//quote.CustCode,
                    ServiceType = model.ServiceType,//quote.ServiceType,
                    RecName = model.ReceiverName,
                    RecAdd1 = model.ReceiverAddress1,
                    RecAdd2 = model.ReceiverAddress2,
                    RecAdd3 = model.ReceiverAddress3,
                    RecAdd4 = model.ReceiverSuburb,
                    RecAdd5 = model.ReceiverPostalCode,
                    RecCellNo = model.ReceiverContactCell,
                    RecEmail = model.ReceiverContactEmail,
                    RecContact = model.ReceiverContactName,
                    RecTelNo = model.ReceiverContactCell,
                    RecSite = model.ReceiverCode,
                    SenName = model.SenderName,
                    SenAdd1 = model.SenderAddress1,
                    SenAdd2 = model.SenderAddress2,
                    SenAdd3 = model.SenderAddress3,
                    SenAdd4 = model.SenderSuburb,
                    SenAdd5 = model.SenderPostalCode,
                    SenCell = model.SenderContactCell,
                    SenTelNo = model.SenderContactCell,
                    SenEmail = model.SenderContactEmail,
                    SenContact = model.SenderContactName,
                    SendSite = model.SenderCode,
                    WaybillNo = model.WaybillNo,
                    CustXref = model.CustXRef,
                },
                InUserid = inUATUserid,
                InPassword = inUATPassword,
                InRequestCtr = inRequestCtr
            };

            var inLines = new List<Vendor.Services.Freightware.UAT.SetWaybill.SetWaybillInWaybillWaybillLine>();
            var lineCounter = 1;
            var inParcels = new List<Vendor.Services.Freightware.UAT.SetParcel.SetParcelInParcel>();
            var parcelCounter = 1;
            foreach (TransportPriceLineItemModels line in model.Lines)
            {
                inLines.Add(new Vendor.Services.Freightware.UAT.SetWaybill.SetWaybillInWaybillWaybillLine
                {
                    LineBth = line.LineBreadth.ToString(),
                    LineHgt = line.LineHeight.ToString(),
                    LineLen = line.LineLength.ToString(),
                    LineItemQty = line.LineQty.ToString(),
                    LineItemMass = line.LineMass.ToString(),
                    LineNo = lineCounter.ToString(),
                    LineLabelPrinted = model.WaybillNo + " " + lineCounter.ToString("00#"),
                    LineItemDesc = line.Description

                });

                //Add to the parcel collection
                if (line.Parcels == null || line.Parcels.Count == 0) //Create the parcel objects if default
                {
                    for (var i = 1; i <= line.LineQty; i++)
                    {
                        //Create a parcel for every item in the line qty
                        inParcels.Add(new Vendor.Services.Freightware.UAT.SetParcel.SetParcelInParcel
                        {
                            ParcelNo = model.WaybillNo.PadRight(line.TotalParcelNoLength - 7, ' ') + " " + parcelCounter.ToString("00#"),
                            WaybillItemNo = lineCounter.ToString(),
                            WaybillNo = model.WaybillNo
                        });
                        parcelCounter++;
                    }
                }
                else
                {
                    foreach (TransportPriceLineItemParcelModels parcel in line.Parcels)
                    {
                        inParcels.Add(new Vendor.Services.Freightware.UAT.SetParcel.SetParcelInParcel
                        {
                            ParcelNo = parcel.ParcelNo,
                            WaybillNo = model.WaybillNo,
                            WaybillItemNo = lineCounter.ToString(),
                            ParcelBth = parcel.ParcelBreadth.ToString(),
                            ParcelHgt = parcel.ParcelHeight.ToString(),
                            ParcelLen = parcel.ParcelLength.ToString(),
                            ParcelMass = parcel.ParcelMass.ToString()
                        });
                    }
                }
                lineCounter++;
            }
            request.InWaybill.WaybillLines = inLines.ToArray();
            //We need to check if the parcel numbers are deemed valid first
            var parcelClient = new Vendor.Services.Freightware.UAT.SetParcel.FWV6WEBPortClient(Vendor.Services.Freightware.UAT.SetParcel.FWV6WEBPortClient.EndpointConfiguration.FWV6WEBSOAP11Port);

            foreach (var parcel in inParcels)
            {
                if (model.Lines[Convert.ToInt32(parcel.WaybillItemNo) - 1].Parcels == null)
                    parcel.ParcelNo = (await GetPostalListByCode(request.InWaybill.RecAdd5))[0].Branch + parcel.ParcelNo;
                var parcelRequest = new Vendor.Services.Freightware.UAT.SetParcel.SetParcelRequest
                {
                    InAction = new Vendor.Services.Freightware.UAT.SetParcel.SetParcelInAction
                    {
                        InGetSpecified = true,
                        InGet = true
                    },
                    InUserid = inUATUserid,
                    InPassword = inUATPassword,
                    InRequestCtr = inRequestCtr,
                    InParcel = parcel,
                    IoSessionData = spIOSessionData
                };
                var parcelOutput = await parcelClient.SetParcelAsync(parcelRequest);
                switch (parcelOutput.ReturnCode)
                {
                    case "0000":
                        return new FWResponsePacket
                        {
                            ReturnCode = "Error",
                            ReturnMessage = "A parcel with this numeric sequence already exists, not able to add",
                            DataObject = null
                        };
                    case "9998":
                        if (!parcelOutput.ReturnMessage.Contains("does not exist"))
                            return new FWResponsePacket
                            {
                                ReturnCode = "Error",
                                ReturnMessage = "A parcel with this numeric sequence already exists, not able to add",
                                DataObject = null
                            };
                        break;
                    default:
                        return new FWResponsePacket
                        {
                            ReturnCode = parcelOutput.ReturnCode,
                            ReturnMessage = parcelOutput.ReturnMessage,
                            DataObject = null
                        };
                }
            }

            //We can assume if it reaches here that the parcel numbers have passed validation.
            var waybillResponse = await client.SetWaybillAsync(request);

            var parcelError = "";

            if (waybillResponse.ReturnCode == "0000")
            {
               
                //Now that the waybill exists, we can create the parcels
                foreach (var parcel in inParcels)
                {
                    var parcelRequest = new Vendor.Services.Freightware.UAT.SetParcel.SetParcelRequest
                    {
                        InAction = new Vendor.Services.Freightware.UAT.SetParcel.SetParcelInAction
                        {
                            InAddSpecified = true,
                            InAdd = true
                        },
                        InUserid = inUATUserid,
                        InPassword = inUATPassword,
                        InRequestCtr = inRequestCtr,
                        InParcel = parcel,
                        IoSessionData = spIOSessionData
                    };
                    var parcelOutput = await parcelClient.SetParcelAsync(parcelRequest);

                    if (parcelOutput.ReturnCode != "0000")
                    {
                        parcelError = parcelError + " Parcel Error :" + parcelOutput.ReturnCode + " - " + parcelOutput.ReturnMessage + ". ";
                    }

                }
            }


            return new FWResponsePacket
            {
                ReturnCode = waybillResponse.ReturnCode,
                ReturnMessage = waybillResponse.ReturnMessage + parcelError,
                DataObject = waybillResponse.ReturnCode == "0000" ? waybillResponse.Waybill : null
            };
        }
        
        [HttpPost("Waybill/")]
        [SwaggerOperation(Summary = "Post waybill", Description = "Post a waybill.")]
        public async Task<ActionResult<FWResponsePacket>> PostWaybill(CustomerWaybillSubmitModels model, string dbName = "CRM")
        {
            var spIOSessionData = "";

            var client = new Vendor.Services.Freightware.PROD.SetWaybill.FWV6WEBPortClient(Vendor.Services.Freightware.PROD.SetWaybill.FWV6WEBPortClient.EndpointConfiguration.FWV6WEBSOAP11Port);
            var request = new Vendor.Services.Freightware.PROD.SetWaybill.SetWaybillRequest
            {
                InAction = new Vendor.Services.Freightware.PROD.SetWaybill.SetWaybillInAction
                {
                    InAdd = true,
                    InAddSpecified = true
                },
                InWaybill = new Vendor.Services.Freightware.PROD.SetWaybill.SetWaybillInWaybill
                {
                    DateWaybill = DateTime.Now.ToString("yyyyMMdd"),
                    CustCode = model.CustCode,//quote.CustCode,
                    ServiceType = model.ServiceType,//quote.ServiceType,
                    RecName = model.ReceiverName,
                    RecAdd1 = model.ReceiverAddress1,
                    RecAdd2 = model.ReceiverAddress2,
                    RecAdd3 = model.ReceiverAddress3,
                    RecAdd4 = model.ReceiverSuburb,
                    RecAdd5 = model.ReceiverPostalCode,
                    RecCellNo = model.ReceiverContactCell,
                    RecEmail = model.ReceiverContactEmail,
                    RecContact = model.ReceiverContactName,
                    RecTelNo = model.ReceiverContactCell,
                    RecSite = model.ReceiverCode,
                    SenName = model.SenderName,
                    SenAdd1 = model.SenderAddress1,
                    SenAdd2 = model.SenderAddress2,
                    SenAdd3 = model.SenderAddress3,
                    SenAdd4 = model.SenderSuburb,
                    SenAdd5 = model.SenderPostalCode,
                    SenCell = model.SenderContactCell,
                    SenTelNo = model.SenderContactCell,
                    SenEmail = model.SenderContactEmail,
                    SenContact = model.SenderContactName,
                    SendSite = model.SenderCode,
                    WaybillNo = model.WaybillNo,
                    CustXref = model.CustXRef,
                },
                InUserid = inUserid,
                InPassword = inPassword,
                InRequestCtr = inRequestCtr
            };

            var inLines = new List<Vendor.Services.Freightware.PROD.SetWaybill.SetWaybillInWaybillWaybillLine>();
            var lineCounter = 1;
            var inParcels = new List<Vendor.Services.Freightware.PROD.SetParcel.SetParcelInParcel>();
            var parcelCounter = 1;
            foreach (var line in model.Lines)
            {
                inLines.Add(new Vendor.Services.Freightware.PROD.SetWaybill.SetWaybillInWaybillWaybillLine
                {
                    LineBth = line.LineBreadth.ToString(),
                    LineHgt = line.LineHeight.ToString(),
                    LineLen = line.LineLength.ToString(),
                    LineItemQty = line.LineQty.ToString(),
                    LineItemMass = line.LineMass.ToString(),
                    LineNo = lineCounter.ToString(),
                    LineLabelPrinted = model.WaybillNo + " " + lineCounter.ToString("00#"),
                    LineItemDesc = line.Description

                });

                //Add to the parcel collection
                if (line.Parcels == null || line.Parcels.Count == 0) //Create the parcel objects if default
                {
                    for (var i = 1; i <= line.LineQty; i++)
                    {
                        //Create a parcel for every item in the line qty
                        inParcels.Add(new Vendor.Services.Freightware.PROD.SetParcel.SetParcelInParcel
                        {
                            ParcelNo = model.WaybillNo.PadRight(line.TotalParcelNoLength - 7, ' ') + " " + parcelCounter.ToString("00#"),
                            WaybillItemNo = lineCounter.ToString(),
                            WaybillNo = model.WaybillNo
                        });
                        parcelCounter++;
                    }
                }
                else
                {
                    foreach (var parcel in line.Parcels)
                    {
                        inParcels.Add(new Vendor.Services.Freightware.PROD.SetParcel.SetParcelInParcel
                        {
                            ParcelNo = parcel.ParcelNo,
                            WaybillNo = model.WaybillNo,
                            WaybillItemNo = lineCounter.ToString(),
                            ParcelBth = parcel.ParcelBreadth.ToString(),
                            ParcelHgt = parcel.ParcelHeight.ToString(),
                            ParcelLen = parcel.ParcelLength.ToString(),
                            ParcelMass = parcel.ParcelMass.ToString()
                        });
                    }
                }
                lineCounter++;
            }
            request.InWaybill.WaybillLines = inLines.ToArray();

            //We need to check if the parcel numbers are deemed valid first
            var parcelClient = new Vendor.Services.Freightware.PROD.SetParcel.FWV6WEBPortClient(Vendor.Services.Freightware.PROD.SetParcel.FWV6WEBPortClient.EndpointConfiguration.FWV6WEBSOAP11Port);

            foreach (var parcel in inParcels)
            {
                if (model.Lines[Convert.ToInt32(parcel.WaybillItemNo) - 1].Parcels == null)
                    parcel.ParcelNo = (await GetPostalListByCode(request.InWaybill.RecAdd5))[0].Branch + parcel.ParcelNo;

                var parcelRequest = new Vendor.Services.Freightware.PROD.SetParcel.SetParcelRequest
                {
                    InAction = new Vendor.Services.Freightware.PROD.SetParcel.SetParcelInAction
                    {
                        InGetSpecified = true,
                        InGet = true
                    },
                    InUserid = inUserid,
                    InPassword = inPassword,
                    InRequestCtr = inRequestCtr,
                    InParcel = parcel,
                    IoSessionData = spIOSessionData
                };

                var parcelOutput = await parcelClient.SetParcelAsync(parcelRequest);
                switch (parcelOutput.ReturnCode)
                {
                    case "0000":
                        return new FWResponsePacket
                        {
                            ReturnCode = "0000",
                            ReturnMessage = "Save parcel information",
                            DataObject = null
                        };
                    case "9998":
                        if (!parcelOutput.ReturnMessage.Contains("does not exist"))
                            return new FWResponsePacket
                            {
                                ReturnCode = "9998",
                                ReturnMessage = "A parcel with this numeric sequence already exists, not able to add",
                                DataObject = null
                            };
                        break;
                    default:
                        return new FWResponsePacket
                        {
                            ReturnCode = parcelOutput.ReturnCode,
                            ReturnMessage = parcelOutput.ReturnMessage,
                            DataObject = null
                        };
                }
            }

            //We can assume if it reaches here that the parcel numbers have passed validation.
            var waybillResponse = await client.SetWaybillAsync(request);

            var parcelError = "";

            if (waybillResponse.ReturnCode == "0000")
            {
               
                //Now that the waybill exists, we can create the parcels
                foreach (var parcel in inParcels)
                {
                    var parcelRequest = new Vendor.Services.Freightware.PROD.SetParcel.SetParcelRequest
                    {
                        InAction = new Vendor.Services.Freightware.PROD.SetParcel.SetParcelInAction
                        {
                            InAddSpecified = true,
                            InAdd = true
                        },
                        InUserid = inUserid,
                        InPassword = inPassword,
                        InRequestCtr = inRequestCtr,
                        InParcel = parcel,
                        IoSessionData = spIOSessionData
                    };
                    var parcelOutput = await parcelClient.SetParcelAsync(parcelRequest);

                    if (parcelOutput.ReturnCode != "0000")
                    {
                        parcelError = parcelError + " Parcel Error :" + parcelOutput.ReturnCode + " - " + parcelOutput.ReturnMessage + ". ";
                    }

                }
            }

            return new FWResponsePacket
            {
                ReturnCode = waybillResponse.ReturnCode,
                ReturnMessage = waybillResponse.ReturnMessage + parcelError,
                DataObject = waybillResponse.ReturnCode == "0000" ? waybillResponse.Waybill : null
            };
        }
        
        [HttpGet("PostalList/{searchPhrase}")]
        [SwaggerOperation(Summary = "Get postal code list by phrase", Description = "Get the postal codes with names matching a phrase")]
        public async Task<List<GetPcodeListResponsePcodeOutput>> GetPostalList(string searchPhrase)
        {
            GetPcodeListRequest postalRequest = new GetPcodeListRequest
            {
                InUserid = inUserid,
                InPassword = inPassword,
                Sequence = new GetPcodeListSequence
                {
                    ByNamePcode = true,
                    ByNamePcodeSpecified = true,
                    ByBranchPcode = false,
                    ByBranchPcodeSpecified = false,
                    ByHubBranchAreaPcode = false,
                    ByHubBranchAreaPcodeSpecified = false,
                    ByPcode = false,
                    ByPcodeSpecified = false,
                    ByPcodeName = false,
                    ByPcodeNameSpecified = false
                },
                StartValues = new GetPcodeListStartValues
                {
                    PcodeName = searchPhrase + "*"
                }
            };

            Vendor.Services.Freightware.PROD.GetPcodeList.FWV6WEBPortClient client = new Vendor.Services.Freightware.PROD.GetPcodeList.FWV6WEBPortClient(Vendor.Services.Freightware.PROD.GetPcodeList.FWV6WEBPortClient.EndpointConfiguration.FWV6WEBSOAP11Port);
            var pc = await client.GetPcodeListAsync(postalRequest);


            List<GetPcodeListResponsePcodeOutput> filtered = new List<GetPcodeListResponsePcodeOutput>();

            if (pc.PcodeOutputs != null && pc.PcodeOutputs.Count() > 0)
                filtered = (from c in pc.PcodeOutputs
                            select c
                            ).ToList();

            //Store the IOSession Data to retrieve more data
            string strNewIOSessionData = pc.IoSessionData;

            //Get all codes
            while (pc.ReturnCode == "0130")
            {
                postalRequest.IoSessionData = pc.IoSessionData;
                pc = await client.GetPcodeListAsync(postalRequest);

                foreach (var v in pc.PcodeOutputs)
                {
                    filtered.Add(v);
                }
            }
            return filtered;
        }
        
        [HttpGet("GetPostalListByCode/{code}")]
        [SwaggerOperation(Summary = "Get postal code list by code", Description = "Get the postal codes with code matching")]
        public async Task<List<GetPcodeListResponsePcodeOutput>> GetPostalListByCode(string code)
        {
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
                    PostCode = code + "*"
                }
            };

            var client = new Vendor.Services.Freightware.PROD.GetPcodeList.FWV6WEBPortClient(Vendor.Services.Freightware.PROD.GetPcodeList.FWV6WEBPortClient.EndpointConfiguration.FWV6WEBSOAP11Port);
            var pc = await client.GetPcodeListAsync(postalRequest);


            var filtered = new List<GetPcodeListResponsePcodeOutput>();

            if (pc.PcodeOutputs != null && pc.PcodeOutputs.Any())
                filtered = (from c in pc.PcodeOutputs select c).ToList();

            //Get all codes
            while (pc.ReturnCode == "0130")
            {
                postalRequest.IoSessionData = pc.IoSessionData;
                pc = await client.GetPcodeListAsync(postalRequest);

                filtered.AddRange(pc.PcodeOutputs);
            }
            return filtered;
        }

        [HttpPost("UAT/Collection/")]
        [SwaggerOperation(Summary = "Post UAT collection", Description = "Post a UAT collection.")]
        public async Task<ActionResult<FWResponsePacket>> PostCollectionUAT(CollectionRequests model)
        {
            var spIOSessionData = "";

            var client = new Vendor.Services.Freightware.UAT.SetCollection.FWV6WEBPortClient(Vendor.Services.Freightware.UAT.SetCollection.FWV6WEBPortClient.EndpointConfiguration.FWV6WEBSOAP11Port);
            
            var request = new SetCollectionRequest
            {
                InAction = new SetCollectionInAction
                {
                    InAdd=true,
                    InAddSpecified=true
                },
                InUserId=inUATUserid,
                InPassword=inUATPassword,
                InCollection = new SetCollectionInCollection
                {
                    CallerContact=model.CallerContactInfo,
                    CallerName=model.CallerName,
                    CallerSurname=model.CallerSurname,
                    CallerRelationship=model.CallerRelationship,
                    CollectFromSecurity=model.CollFromSecurity?"Y":"N",
                    CustCode=(await _customer.GetCrmCustomerById(model.CustomerID)).AccountCode,
                    //CustRefCode=model.CustomerXref,
                    DateCollection=model.DateCollReq.ToString("yyyyMMdd"),
                    ItemMass=model.EstMass.ToString(),
                    ItemQty=model.EstQty.ToString(),
                    ItemVol=model.EstVolume.ToString(),
                    ServiceType=(await _transportType.GetAllTransportTypes()).Find(x=>x.TransportTypeID==model.ServiceTypeID).Description,
                    Remarks=model.Remarks,
                    SpecialInstructions=model.SpecialInstructions,
                    TimeCollBefore=model.TimeCollBefore,
                    TimeCollAfter=model.TimeCollAfter,
                    XrefNo=model.CustomerXref,
                    ReceiverInformation = new SetCollectionInCollectionReceiverInformation
                    {
                        RecAdd1=model.RecAdd1,
                        RecAdd2=model.RecAdd2,
                        RecAdd3=model.RecAdd3,
                        RecAdd4=model.RecAdd4,
                        RecAdd5=model.RecAddPostalCode,
                        RecContact=model.RecContact,
                        RecFaxNo=model.RecTelNo,
                        RecTelNo=model.RecTelNo,
                        RecName=model.RecName,
                    },
                    RecInstructions=!string.IsNullOrEmpty(model.RecInstructions1)?new [] {model.RecInstructions1,model.RecInstructions2}:new [] {"","" },
                    RecSite=model.RecSite,
                    SenderInformation = new SetCollectionInCollectionSenderInformation
                    {
                        SenAdd1=model.SendAdd1,
                        SenAdd2=model.SendAdd2,
                        SenAdd3=model.SendAdd3,
                        SenAdd4=model.SendAdd4,
                        SenAdd5=model.SendAddPostalCode,
                        SenContact=model.SendContact,
                        SenFaxNo=model.SendTelNo,
                        SenTelNo=model.SendTelNo,
                        SenName=model.SendName
                    },
                    SendInstructions = !string.IsNullOrEmpty(model.SendInstructions1)?new [] {model.SendInstructions1,model.SendInstructions2}:new [] {"","" },
                    SendSite=model.SendSite
                },
                InRequestCtr=inRequestCtr,
                IOSessionData=spIOSessionData
            };

            var collectionResponse = await client.SetCollectionAsync(request);

            return new FWResponsePacket
            {
                ReturnCode=collectionResponse.ReturnCode,
                ReturnMessage=collectionResponse.ReturnMessage,
                DataObject=collectionResponse.Collection
            };
        }

        [HttpPost("Production/Collection/")]
        [SwaggerOperation(Summary = "Post Production collection", Description = "Post a Production collection.")]
        public async Task<ActionResult<FWResponsePacket>> PostCollectionProduction(CollectionRequests model)
        {
            var spIOSessionData = "";

            var client = new Vendor.Services.Freightware.PROD.SetCollection.FWV6WEBPortClient(Vendor.Services.Freightware.PROD.SetCollection.FWV6WEBPortClient.EndpointConfiguration.FWV6WEBSOAP12Port);

            var request = new Vendor.Services.Freightware.PROD.SetCollection.SetCollectionRequest
            {
                InAction = new Vendor.Services.Freightware.PROD.SetCollection.SetCollectionInAction
                {
                    InAdd = true,
                    InAddSpecified = true
                },
                InUserId = inUATUserid,
                InPassword = inUATPassword,
                InCollection = new Vendor.Services.Freightware.PROD.SetCollection.SetCollectionInCollection
                {
                    CallerContact = model.CallerContactInfo,
                    CallerName = model.CallerName,
                    CallerSurname = model.CallerSurname,
                    CallerRelationship = model.CallerRelationship,
                    CollectFromSecurity = model.CollFromSecurity ? "Y" : "N",
                    CustCode = (await _customer.GetCrmCustomerById(model.CustomerID)).AccountCode,
                    //CustRefCode=model.CustomerXref,
                    DateCollection = model.DateCollReq.ToString("yyyyMMdd"),
                    ItemMass = model.EstMass.ToString(),
                    ItemQty = model.EstQty.ToString(),
                    ItemVol = model.EstVolume.ToString(),
                    ServiceType = (await _transportType.GetAllTransportTypes()).Find(x => x.TransportTypeID == model.ServiceTypeID).Description,
                    Remarks = model.Remarks,
                    SpecialInstructions = model.SpecialInstructions,
                    TimeCollBefore = model.TimeCollBefore,
                    TimeCollAfter = model.TimeCollAfter,
                    XrefNo = model.CustomerXref,
                    ReceiverInformation = new Vendor.Services.Freightware.PROD.SetCollection.SetCollectionInCollectionReceiverInformation
                    {
                        RecAdd1 = model.RecAdd1,
                        RecAdd2 = model.RecAdd2,
                        RecAdd3 = model.RecAdd3,
                        RecAdd4 = model.RecAdd4,
                        RecAdd5 = model.RecAddPostalCode,
                        RecContact = model.RecContact,
                        RecFaxNo = model.RecTelNo,
                        RecTelNo = model.RecTelNo,
                        RecName = model.RecName,
                    },
                    RecInstructions = !string.IsNullOrEmpty(model.RecInstructions1) ? new[] { model.RecInstructions1, model.RecInstructions2 } : new[] { "", "" },
                    RecSite = model.RecSite,
                    SenderInformation = new Vendor.Services.Freightware.PROD.SetCollection.SetCollectionInCollectionSenderInformation
                    {
                        SenAdd1 = model.SendAdd1,
                        SenAdd2 = model.SendAdd2,
                        SenAdd3 = model.SendAdd3,
                        SenAdd4 = model.SendAdd4,
                        SenAdd5 = model.SendAddPostalCode,
                        SenContact = model.SendContact,
                        SenFaxNo = model.SendTelNo,
                        SenTelNo = model.SendTelNo,
                        SenName = model.SendName
                    },
                    SendInstructions = !string.IsNullOrEmpty(model.SendInstructions1) ? new[] { model.SendInstructions1, model.SendInstructions2 } : new[] { "", "" },
                    SendSite = model.SendSite
                },
                InRequestCtr = inRequestCtr,
                IOSessionData = spIOSessionData
            };

            var collectionResponse = await client.SetCollectionAsync(request);

            return new FWResponsePacket
            {
                ReturnCode = collectionResponse.ReturnCode,
                ReturnMessage = collectionResponse.ReturnMessage,
                DataObject = collectionResponse.Collection,
                Reference = collectionResponse.Collection?.OrdNo
            };
        }

        [HttpGet("Customer/{accountCode}/Statement/{period}")]
        [SwaggerOperation(Summary = "Get customer statement for period", Description = "Get a customers statement for a period.")]
        public async Task<ActionResult<FWResponsePacket>> GetCustomerStatement(string accountCode,DateTime period)
        {
            var request = new Vendor.Services.Freightware.PROD.GetStatement.GetStatementRequest
            {
                InUserId = inUserid,
                InPassword = inPassword,
                InRequestCtr = inRequestCtr,
                Sequence=new Vendor.Services.Freightware.PROD.GetStatement.GetStatementSequence
                {
                    ByCustPostedType=true,
                    ByCustPostedTypeSpecified=true
                },
                StartValues = new Vendor.Services.Freightware.PROD.GetStatement.GetStatementStartValues
                {
                    CustCode=accountCode,
                    PostedPeriod=period.ToString("yyyyMMdd")
                }
            };

            var client = new Vendor.Services.Freightware.PROD.GetStatement.FWV6WEBPortClient(Vendor.Services.Freightware.PROD.GetStatement.FWV6WEBPortClient.EndpointConfiguration.FWV6WEBSOAP11Port);
            var response = new FWResponsePacket();
            try
            {
                var x = await client.GetStatementAsync(request);

                response.ReturnCode = x.ReturnCode;
                response.ReturnMessage = x.ReturnMessage;
                response.DataObject = x.StatementOutput;
            }
            catch (Exception x)
            {
                response.ReturnCode = "Internal Error";
                response.ReturnMessage = x.Message + " - " + x.InnerException;
            }
            return response;
        }

        [HttpPost("SetSiteUAT")]
        [SwaggerOperation(Summary = "Post UAT sitecode", Description = "Post a UAT sitecode.")]
        public async Task<ActionResult<FWResponsePacket>> PostSiteCodeUAT(SiteCodeModel model, string dbName = "CRM")
        {
            return await _sites.SetSiteUAT(model, dbName);
        }

        [HttpPost("SetSiteProduction")]
        [SwaggerOperation(Summary = "Post PROD sitecode", Description = "Post a PROD sitecode.")]
        public async Task<ActionResult<FWResponsePacket>> PostSiteCodeProduction(SiteCodeModel model)
        {
            return await _sites.SetSiteProduction(model);
        }
    }
}