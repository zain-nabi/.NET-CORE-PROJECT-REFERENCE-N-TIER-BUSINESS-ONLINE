using System;
using System.Collections.Generic;
using System.Text;
using Triton.Model.CRM.Tables;
using Triton.Model.TritonExpress.Tables;

namespace Triton.Model.CRM.Custom
{
    public class SiteCodeModel: Sites
    {
        public string AccountCode { get; set; }
        public List<Customers> Customers { get; set; }
        public int CustomerID { get; set; }
        public List<PostalCodes> PostalCodes { get; set; }
    }
}
