using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triton.Model.TritonExpress.Tables;

namespace Triton.Interface.TritonGroup
{
    public interface IPostalCodes
    {
        Task<List<PostalCodes>> GetAllPostalCodes(string BranchCode);

        Task<bool> PutPostalCodes(PostalCodes PostalCodes);

        Task<List<PostalCodes>> GetPostalCodes(string Name);

        Task<PostalCodes> GetSenderPostCodeName(string Date, DateTime CollectionDate, string SenderPostCodeName);

        Task<List<PostalCodes>> GetPostalCodesByCode(string Code);
    }
}
