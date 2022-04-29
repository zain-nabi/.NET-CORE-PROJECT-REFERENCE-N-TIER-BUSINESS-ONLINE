using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triton.Model.LeaveManagement.Tables
{
    public class Uploader
    {
        public int UploadID { get; set; }
        public byte[] FileData { get; set; }
        public int? EmployeeID { get; set; }
        public int? LeaveID { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long? Length { get; set; }
        public DateTime? TimeUploaded { get; set; }
        public int? UserUploadedID { get; set; }
        public int? NumPages { get; set; }
    }
}
