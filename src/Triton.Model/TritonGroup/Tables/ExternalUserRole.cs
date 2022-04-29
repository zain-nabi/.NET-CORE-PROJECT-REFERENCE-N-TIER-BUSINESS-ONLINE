using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Triton.Model.TritonGroup.Tables
{
    [Table("ExternalUserRole")]
    public class ExternalUserRole
    {
        [Dapper.Contrib.Extensions.Key]
        public int ExternalUserRoleID { get; set; }
        public int ExternalUserID { get; set; }
        public int RoleID { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedByUserID { get; set; }
        public DateTime? DeletedOn { get; set; }
        public int? DeletedByUserID { get; set; }
    }
}
