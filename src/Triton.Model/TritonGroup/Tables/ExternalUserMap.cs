using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Triton.Model.TritonGroup.Tables
{
    [Table("ExternalUserMap")]
    public class ExternalUserMap
    {
        [Dapper.Contrib.Extensions.Key]
        public int ExternalUserMapID { get; set; }

        [Required]
        public int ExternalUserID { get; set; }

        public int? CustomerID { get; set; }

        [Required]
        public int UserTypeLCID { get; set; }

        [Required]
        public int CreatedByUserID { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public int? DeletedByUserID { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
