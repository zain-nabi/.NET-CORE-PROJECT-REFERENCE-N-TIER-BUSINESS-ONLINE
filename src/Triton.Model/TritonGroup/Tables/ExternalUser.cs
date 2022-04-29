using Dapper.Contrib.Extensions;
using System;
using Microsoft.AspNetCore.Identity;

namespace Triton.Model.TritonGroup.Tables
{
    [Table("ExternalUser")]
    public class ExternalUser : IdentityUser
    {
        [Dapper.Contrib.Extensions.Key]
        public int ExternalUserID { get; set; }
        public new string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public new string PasswordHash { get; set; }
        public new string SecurityStamp { get; set; }
        public new string PhoneNumber { get; set; }
        public new bool PhoneNumberConfirmed { get; set; }
        public new string Email { get; set; }
        public new bool EmailConfirmed { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public new bool LockoutEnabled { get; set; }
        public new int AccessFailedCount { get; set; }

        [Write(false)] public override string Id { get; set; }
        [Write(false)] public override string NormalizedUserName { get; set; }
        [Write(false)] public override string NormalizedEmail { get; set; }
        [Write(false)] public override string ConcurrencyStamp { get; set; }
        [Write(false)] public override bool TwoFactorEnabled { get; set; }
        [Write(false)] public override DateTimeOffset? LockoutEnd { get; set; }
    }
}
