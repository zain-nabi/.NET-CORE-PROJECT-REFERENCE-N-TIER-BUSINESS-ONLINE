using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Triton.Core;
using Triton.Interface.Mobile;
using Triton.Model.CRM.Custom;

namespace Triton.Repository.Mobile
{
    public class TritonScannerRepository : ITritonScanner
    {
        private static IConfiguration _config;
        public TritonScannerRepository(IConfiguration config) { _config = config; }

        public async Task<List<DeviceDeliveryManifest>> GetDeviceManifest(string route)
        {
            const string sql = @"SELECT DeliveryManifestID, DeliveryRoute, DeliveryVehicleRegistration, DelManifestNumber, TotalQty
                        FROM Deliveries D
                        INNER JOIN DeliveryManifests DM ON DM.DelManifestNumber = D.DeliveryNo
                        WHERE DeliveryRoute = @route
                        AND CONVERT(DATE, DeliverySheetDate) = CONVERT(DATE, GETDATE())";

            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            return connection.Query<DeviceDeliveryManifest>(sql, new { route }).ToList();
        }
    }
}
