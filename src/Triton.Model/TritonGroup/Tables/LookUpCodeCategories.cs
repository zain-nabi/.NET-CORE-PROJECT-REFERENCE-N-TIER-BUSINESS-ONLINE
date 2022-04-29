using System;
using System.ComponentModel.DataAnnotations;

namespace Triton.Model.TritonGroup.Tables
{
    public class LookupCodeCategories
    {
        [Key]
        public long LookUpCodeCategoryID { get; set; }
        public string Category { get; set; }
        public int CreatedByUserID { get; set; }
        public DateTime CreatedOn { get; set; }
        public int DeletedByUserID { get; set; }
        public DateTime DeletedOn { get; set; }

        public  Tables.LookUpCodes LookUpCodes { get; set; }
        public Tables.LookUpCodeCategorySystemMaps LookUpCodeCategorySystemMaps { get; set; }
    }
}
