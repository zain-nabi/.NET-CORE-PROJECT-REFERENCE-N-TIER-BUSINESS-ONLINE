using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.Tables;
using Vendor.Services.Freightware.PROD.GetChargesList;
using Vendor.Services.Freightware.PROD.GetStatement;

namespace Vendor.Services.CustomModels
{
    public class SundryList
    {
        public string Value { get; set; }
        public string Heading { get; set; }
        public string Description { get; set; }
        public bool Selected { get; set; }
        public decimal ChargeAmount { get; set; }
    }

    public class VendorQuoteModel : QuoteModels
    {
        public List<GetChargesListResponseChargesOutput> AllowedSundries { get; set; }

        public List<QuoteChargeItem> AllowedSundriesDropDown
        {
            get
            {
                try
                {
                    if (AllowedSundries != null)
                        return (from sundry in AllowedSundries
                                select new QuoteChargeItem
                                {
                                    Value = sundry.OutChargeCode,
                                    Description = sundry.Heading + " - " + sundry.Description
                                }
                            ).OrderBy(x => x.Description).ToList();
                    return null;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
               
            }
        }

        public List<SundryList> SundryDropDownList { get; set; }
    }

    public class VendorQuoteSearchModel
    {

        public DateTime? DateFrom { get; set; } = DateTime.Now.AddDays(-14);
        public DateTime? DateTo { get; set; } = DateTime.Now;
        public string QuoteRef { get; set; }
        public int CustomerID { get; set; }
        public string AccountCode { get; set; }
        public List<Customers> AllowedCustomerList { get; set; }
        public List<Quotes> Quotes { get; set; }
        public List<QuoteSundrys> QuoteSundrys { get; set; }
        public bool ShowReport { get; set; }
        public string FilterDate { get; set; }
    }

    public class VendorStatementSearch
    {
        public int customerId { get; set; }
        public List<Customers> AllowedCustomers { get; set; }
        public Customers Customers { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = false)]
        public DateTime Period { get; set; } = DateTime.Now.AddDays(-28);
        public GetStatementResponseStatementOutput GetStatementResponseStatementOutput { get; set; }
        public string Url { get; set; }
        public bool ShowReport { get; set; }
        public string FilterDate { get; set; }
    }
}
