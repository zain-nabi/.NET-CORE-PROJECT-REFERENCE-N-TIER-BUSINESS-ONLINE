using System;
using System.Collections.Generic;
using System.Text;
using Triton.Model.TritonGroup.Tables;
using Users = Triton.Model.TritonSecurity.Tables.Users;

namespace Triton.Model.TritonGroup.Custom
{
    public class LookupCodeCategoriesModel
    {
        public LookUpCodes LookUpCodes { get; set; }
        public LookupCodeCategories LookUpCodeCategories { get; set; }
        public Users Users { get; set; }
    }
}
