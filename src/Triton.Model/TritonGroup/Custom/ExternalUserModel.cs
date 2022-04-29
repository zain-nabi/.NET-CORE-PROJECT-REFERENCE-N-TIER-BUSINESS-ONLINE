using System;
using System.Collections.Generic;
using System.Text;

namespace Triton.Model.TritonGroup.Custom
{
    public class ExternalUserModel
    {
        public int ExternalUserID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserRole { get; set; }
        public int RoleID { get; set; }
        public string CustomerNames { get; set; }
    }
}
