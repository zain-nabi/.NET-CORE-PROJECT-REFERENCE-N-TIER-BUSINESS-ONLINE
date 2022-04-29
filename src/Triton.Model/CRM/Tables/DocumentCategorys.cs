using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace Triton.Model.CRM.Tables
{
    [Table("DocumentCategorys")]
    public class DocumentCategorys
    {
        public int DocumentCategoryID { get; set; }
        public string Description { get; set; }
        public int WorkFlowID { get; set; }
    }
}
