using Triton.Model.LeaveManagement.Tables;
using Triton.Model.TritonSecurity.Tables;

namespace Triton.Model.LeaveManagement.Custom
{
    public class EmployeesModels
    {
        public Employees Employees { get; set; }

        public orgOrganogram orgOrganogram { get; set; }

        public JobProfiles JobProfiles { get; set; }

        public Branches Branches { get; set; }
    }

     public class IDCheckModel
    {
        public string EmployeeCode { get; set; }
        public string DisplayName { get; set; }

        public string LastName { get; set; }
        public string KnownAsName { get; set; }
        public string FirstNames { get; set; }
        public string Initials { get; set; }
        public string IDNumber { get; set; }
        public string EmailAddress { get; set; }
        public string HomeTelNo { get; set; }
        public string WorkTelNo { get; set; }
        public string FaxNo { get; set; }
        public string CellNo { get; set; }
        public string SecondName { get; set; }
        public string OtherNames { get; set; }
        public string EmployeeStatusCode { get; set; }
        public string CompanyDisplayName { get; set; }

        public string CompanyEntityCode { get; set; }

    }
}
