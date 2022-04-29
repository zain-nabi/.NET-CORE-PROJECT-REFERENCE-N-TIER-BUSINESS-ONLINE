using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triton.Model.CRM.Tables;

namespace Triton.Interface.CRM
{
        public interface ITransportTypes
    {
        Task<List<TransportTypes>> GetAllTransportTypes();
        Task<List<TransportTypes>> GetTransportTypes();
    }
}
