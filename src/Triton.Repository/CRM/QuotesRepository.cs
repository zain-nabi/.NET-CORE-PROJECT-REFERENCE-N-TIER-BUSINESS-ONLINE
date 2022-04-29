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
using Vendor.Services.CustomModels;

namespace Triton.Repository.CRM
{
    public class QuotesRepository : IQuotes
    {
        private static IConfiguration _config;
        public QuotesRepository(IConfiguration config) { _config = config; }
        public async Task<VendorQuoteModel> GetQuoteByID(long quoteId, string DBName = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(DBName));
            VendorQuoteModel qm = new VendorQuoteModel();
            var sql = string.Format(@"  SELECT * FROM Quotes WHERE QuoteID={0}
                                            SELECT * FROM QuoteLines QL WHERE QuoteID={0}
                                            SELECT * FROM QuoteAdditionals QA WHERE QuoteID={0}
                                            SELECT * FROM QuoteSundrys QA WHERE QuoteID={0}
                                            SELECT * FROM TransportTypes TT
                                            SELECT C.* FROM Customers C INNER JOIN Quotes Q ON Q.CustCode = C.AccountCode WHERE Q.QuoteID={0}
                                            ", quoteId);

            using (var multi = connection.QueryMultiple(sql))
            {
                qm.Quote = multi.ReadFirst<Triton.Model.CRM.Tables.Quotes>();
                qm.QuoteLines = multi.Read<Triton.Model.CRM.Tables.QuoteLines>().ToList();
                qm.QuoteAdditionals = multi.Read<Triton.Model.CRM.Tables.QuoteAdditionals>().ToList();
                qm.QuoteSundrys = multi.Read<Triton.Model.CRM.Tables.QuoteSundrys>().ToList();
                qm.TransportTypes = multi.Read<Triton.Model.CRM.Tables.TransportTypes>().ToList();
                qm.Customers = multi.ReadFirst<Triton.Model.CRM.Tables.Customers>();
            }

            return qm;
        }

        public async Task<VendorQuoteModel> GetQuoteByQuoteNumber(string quoteNumber, string DBName = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(DBName));
            VendorQuoteModel qm = new VendorQuoteModel();
            var sql = string.Format(@"  SELECT * FROM Quotes WHERE QuoteNo='{0}'
                                            SELECT QL.* FROM QuoteLines QL inner join Quotes Q on Q.QuoteID=QL.QuoteID WHERE QuoteNo='{0}'
                                            SELECT QA.* FROM QuoteAdditionals QA inner join Quotes Q on Q.QuoteID=QA.QuoteID WHERE QuoteNo='{0}'
                                            SELECT QS.* FROM QuoteSundrys QS inner join Quotes Q on Q.QuoteID=QS.QuoteID WHERE QuoteNo='{0}'
                                            SELECT * FROM TransportTypes TT 
                                            ", quoteNumber);

            using (var multi = connection.QueryMultiple(sql))
            {
                qm.Quote = multi.ReadFirstOrDefault<Triton.Model.CRM.Tables.Quotes>();
                qm.QuoteLines = multi.Read<Triton.Model.CRM.Tables.QuoteLines>().ToList();
                qm.QuoteAdditionals = multi.Read<Triton.Model.CRM.Tables.QuoteAdditionals>().ToList();
                qm.QuoteSundrys = multi.Read<Triton.Model.CRM.Tables.QuoteSundrys>().ToList();
                qm.TransportTypes = multi.Read<Triton.Model.CRM.Tables.TransportTypes>().ToList();
            }

            return qm;
        }

        //public Task<TransportPriceResultModels> GetTransportPrice(TransportPriceSubmitModels quote)
        //{
        //     await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
        //}

        public async Task<long> PostQuote(VendorQuoteModel quoteModel, string dbName = "CRM")
        {
            using var scope = new TransactionScope();
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            var origQuoteModel = quoteModel;
            try
            {
                quoteModel.Quote.CreatedByTSUserID = origQuoteModel.Quote.CreatedByTSUserID;
                quoteModel.Quote.CreatedOn = origQuoteModel.Quote.CreatedOn;
                var quoteId = connection.Insert(quoteModel.Quote);



                //Save Lines
                foreach (var line in quoteModel.QuoteLines)
                {
                    line.QuoteID = Convert.ToInt32(quoteId);
                    connection.Insert(line);
                }

                //Save Sundries
                foreach (var sundry in quoteModel.QuoteSundrys)
                {
                    sundry.QuoteID = Convert.ToInt32(quoteId);
                    connection.Insert(sundry);
                }
                foreach (var additional in quoteModel.QuoteAdditionals)
                {
                    additional.QuoteID = Convert.ToInt32(quoteId);
                    connection.Insert(additional);
                }
                scope.Complete();
            }
            catch (Exception)
            {
                return 0;
            }

            return quoteModel.Quote.QuoteID;
        }

        public async Task<VendorQuoteSearchModel> GetQuotesbyCustomerIdOptRefandDates(int customerId, string referance, DateTime? from, DateTime? to, string dbName = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            var additionalWhereClause = "";
            if (!string.IsNullOrEmpty(referance))
                additionalWhereClause = $" and (QuoteNo like '%{referance}%' or CustRefCode like '%{referance}%' or XRefNo like '%{referance}%' ";
            if (from.HasValue && to.HasValue)
                additionalWhereClause = $" and DateQuote between '{from.Value:yyyy-MM-dd}' and '{to.Value:yyyy-MM-dd}' ";
            var qm = new VendorQuoteSearchModel();
            qm.Quotes = connection.Query<Quotes>($"  SELECT * FROM Quotes WHERE CustomerId=@customerId {additionalWhereClause}", new { customerId }).ToList();
            return qm;

        }

        public async Task<long> PostQuotationEmail(QuotationTracker quotationTracker, string dbName = "CRM")
        {
            using var scope = new TransactionScope();
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(dbName));
            var _ = connection.Insert(quotationTracker);
            scope.Complete();
            return _;
        }
    }
}



//public TransportPriceResultModels GetTransportPrice(TransportPriceSubmitModels quote)
//{
//    string username = "twebserv";//Properties.Resources.FWWebServiceUser;        //These paramaters are set in the resources tab of the project or the resources file
//    string password = "tw3b53rv";//Properties.Resources.FWWebServicePassword;    //If added to the app.config, it will not pickup as the calling projects config will be used over the reference dll.


//    string IOSessionData = "";

//    Customers Customer = custRepo.GetCRMCustomerByAccountCode(quote.CustCode);

//    SetQuote.SetQuote sq = new SetQuote.SetQuote();
//    SetQuote.SetQuoteInAction sqAction = new SetQuote.SetQuoteInAction();
//    if (Customer.FWPriceCheckQuoteNo==null || Customer.FWPriceCheckQuoteNo.Length==0){
//        sqAction.InAddSpecified = true;
//        sqAction.InAdd = true;
//    }
//    else {
//        sqAction.InModifySpecified = true;
//        sqAction.InModify = true;
//    }

//    SetQuoteInQuote inQuoteObj = new SetQuoteInQuote
//    {
//        DateQuote = DateTime.Now.ToString("yyyyMMdd"),
//        CustCode = quote.CustCode,
//        ServiceType = quote.ServiceType,
//        ReceiverInformation = new SetQuoteInQuoteReceiverInformation
//        {
//            RecAdd4 = quote.ReceiverSuburb,
//            RecAdd5 = quote.ReceiverPostalCode
//        },
//        SenderInformation = new SetQuoteInQuoteSenderInformation
//        {
//            SenAdd4 = quote.SenderSuburb,
//            SenAdd5 = quote.SenderPostalCode
//        },

//        InQuoteLines = ConvertTransportRateLines(quote.Lines)
//    };
//    if (sqAction.InModify)
//        inQuoteObj.QuoteNo = Customer.FWPriceCheckQuoteNo;
//    sq.CallSetQuote(username, password, "ZZ 10", "1", ref IOSessionData, sqAction, inQuoteObj, out string returnCode, out string returnMessage, out SetQuoteResponseQuote outQuoteObj);
//    if ((Customer.FWPriceCheckQuoteNo == null || Customer.FWPriceCheckQuoteNo.Length == 0) && returnCode == "0000")
//    {
//        Customer.FWPriceCheckQuoteNo = outQuoteObj.QuoteNo;
//        custRepo.PutCustomer(Customer);
//    }
//    sq.Dispose();
//    return new TransportPriceResultModels
//    {
//        wasSuccesfull = returnCode == "0000" ? true : false,
//        PriceExVat = returnCode == "0000" ? outQuoteObj.TotalValue - outQuoteObj.Vat: (Decimal?)null ,
//        PriceIncVat = returnCode == "0000" ? outQuoteObj.TotalValue : (Decimal?)null,
//        ErrorMessage = returnCode == "0000" ? "" : returnCode + " - " + returnMessage
//    };
//}

//private SetQuoteInQuoteInQuoteLine[] ConvertTransportRateLines(List<TransportPriceLineItemModels> lines)
//{
//    List<SetQuoteInQuoteInQuoteLine> returnLines = new List<SetQuoteInQuoteInQuoteLine>();
//    foreach(TransportPriceLineItemModels line in lines)
//    {
//        returnLines.Add(new SetQuoteInQuoteInQuoteLine
//        {
//            InItemQty = line.LineQty.ToString(),
//            InRateType = "G",
//            InItemMass = line.LineMass.ToString(),
//            InItemVol = line.LineVol.ToString(),
//            Dimensions = new SetQuoteInQuoteInQuoteLineDimensions
//            {
//                InBreadth = line.LineBreadth.ToString(),
//                InHeight = line.LineHeight.ToString(),
//                InLength = line.LineLength.ToString()
//            }
//        });
//    }
//    return returnLines.ToArray();
//}

