using System;
using System.Collections.Generic;
using System.Text;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Model.TritonGroup.Custom
{
    public class ExternalUserWithRoles
    {
        public ExternalUser Users { get; set; }

        public UserRoles UserRoles { get; set; }

        public List<Roles> Roles { get; set; }
    }
}
