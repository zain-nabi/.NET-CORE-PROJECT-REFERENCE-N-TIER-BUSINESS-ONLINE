using System;
using System.Collections.Generic;
using System.Text;
using Triton.Model.Applications.Tables;
using Triton.Model.LeaveManagement.Tables;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Model.TritonGroup.Custom
{
    public class ExternalUserMapCustomerModels
    {
        public List<ExternalUserMap> UserMap { get; set; }
        public List<CRM.Tables.Customers> Customers { get; set; }
        public List<tblMasterSuppliers> Suppliers { get; set; }
        public Employees Employees { get; set; }
    }
}
