using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Triton.Model.CRM.Tables
{
    [Table("QuotationTracker")]
    public class QuotationTracker
    {
        [Dapper.Contrib.Extensions.Key]
        public int QuotationEmailID { get; set; }
        [Required]
        [Display(Name = "Quote ID")]
        public int QuoteID { get; set; }
        [Required]
        [Display(Name = "From")]
        public string From { get; set; }
        [Required]
        [Display(Name = "To")]
        public string To { get; set; }
        [Required]
        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }
        [Required]
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }
    }
}
