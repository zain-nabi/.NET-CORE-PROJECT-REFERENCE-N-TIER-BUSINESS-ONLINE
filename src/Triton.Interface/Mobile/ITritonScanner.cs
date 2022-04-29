using System.Collections.Generic;
using System.Threading.Tasks;
using Triton.Model.CRM.Custom;
using Triton.Model.TritonStaging.Tables;

namespace Triton.Interface.Mobile
{
    public interface ITritonScanner
    {
        Task<List<DeviceDeliveryManifest>> GetDeviceManifest(string route);
    }
}
