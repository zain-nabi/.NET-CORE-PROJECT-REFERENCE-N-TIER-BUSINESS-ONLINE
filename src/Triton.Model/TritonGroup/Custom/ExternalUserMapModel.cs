using System;
using System.Collections.Generic;
using System.Text;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Model.TritonGroup.Custom
{
    public class ExternalUserMapModel
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public ExternalUser ExternalUser { get; set; }
        public List<ExternalUserMap> ExternalUserMapList { get; set; }

       
    }
}

