using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Triton.Core;
using Triton.Interface.Collection;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.Tables;
using Triton.Model.Notifications.Tables;
namespace Triton.Repository.Collection
{
    public class CollectionRequestTrackAndTracesRepository : ICollectionRequestTrackAndTraces
    {
        private readonly IConfiguration _config;

        public CollectionRequestTrackAndTracesRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

     
        public async Task<CollectionRequestTrackAndTraces> GetCollectionRequestTrackAndTraces(int collectionRequestTrackAndTraceId, string dbname = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                return connection.QueryFirstAsync<CollectionRequestTrackAndTraces>($"SELECT * FROM CollectionRequestTrackandTraces WHERE CollectionRequestTrackandTraceID={collectionRequestTrackAndTraceId}").Result;
            }
        }

        public async Task<CollectionRequestTrackandTracesModel> GetComplex(int CollectionRequestTrackAndTraceID, string dbname = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                var sql = $@"SELECT * FROM CRM..CollectionRequestTrackAndTraces CRTT
                                  LEFT OUTER JOIN Notifications..Notifications  N ON N.NotificationID = CRTT.NotificationID
                                  LEFT OUTER JOIN Notifications..NotificationTypes NT ON NT.NotificationTypeID = CRTT.NotificationTypeID 
                                  INNER JOIN CRM..FWEventCodes FWE ON FWE.FWEventCodeID = CRTT.FWEventCodeID
                                  INNER JOIN CollectionRequests CR ON CRTT.CollectionRequestID = CR.CollectionRequestID
                                WHERE CollectionRequestTrackAndTraceID = {CollectionRequestTrackAndTraceID}";
                {
                    var data = connection.Query<CollectionRequestTrackAndTraces, Notification, NotificationTypes, FWEventCodes, CollectionRequests, CollectionRequestTrackandTracesModel>(sql,(CollectionRequestTrackAndTraceS, notification, notificationtypeS, fwevemtcodeS, collectionrequestS) =>
                            {
                                var model = new CollectionRequestTrackandTracesModel
                                {
                                    CollectionRequestTrackAndTraces = CollectionRequestTrackAndTraceS,
                                    Notification = notification,
                                    NotificationTypes = notificationtypeS,
                                    FwEventCodes = fwevemtcodeS,
                                    CollectionRequests = collectionrequestS
                                };
                                return model;
                            }, new {CollectionRequestTrackAndTraceID},
                            splitOn:"CollectionRequestTrackAndTraceID, NotificationID, NotificationtypeID, FWEventCodeID, CollectionRequestID").FirstOrDefault();
                    return data;
                }
            }
        }

           public async Task<List<CollectionRequestTrackandTracesModel>> FindCollectionRequest(int collectionRequestId, string dbname="CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                var sql = $@"SELECT * FROM CRM..CollectionRequestTrackAndTraces CRTT
                                  LEFT OUTER JOIN Notifications..Notifications  N ON N.NotificationID = CRTT.NotificationID
                                  LEFT OUTER JOIN Notifications..NotificationTypes NT ON NT.NotificationTypeID = CRTT.NotificationTypeID 
                                  INNER JOIN CRM..FWEventCodes FWE ON FWE.FWEventCodeID = CRTT.FWEventCodeID
                                  INNER JOIN CollectionRequests CR ON CRTT.CollectionRequestID = CR.CollectionRequestID
                                  WHERE CRTT.CollectionRequestID = {collectionRequestId}";
                {
                    var data = connection.Query<CollectionRequestTrackAndTraces, Notification, NotificationTypes, FWEventCodes, CollectionRequests, CollectionRequestTrackandTracesModel>(sql,(CollectionRequestTrackAndTraceS, notification, notificationtypeS, fwevemtcodeS, collectionrequestS) =>
                        {
                            var model = new CollectionRequestTrackandTracesModel
                            {
                                CollectionRequestTrackAndTraces = CollectionRequestTrackAndTraceS,
                                Notification = notification,
                                NotificationTypes = notificationtypeS,
                                FwEventCodes = fwevemtcodeS,
                                CollectionRequests = collectionrequestS
                            };
                            return model;
                        }, new {collectionRequestId}, splitOn:"CollectionRequestTrackAndTraceID, NotificationID, NotificationtypeID, FWEventCodeID, CollectionRequestID").ToList();
                    return data;
                }
            }
        }

        public async Task<long> Post(CollectionRequestTrackAndTraces collectionRequestTrackAndTraces, string dbname = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                return connection.Insert(collectionRequestTrackAndTraces);
            }
        }

        public async Task<bool> Put(CollectionRequestTrackAndTraces collectionRequestTrackAndTraces, string dbname = "CRM")
        {
            await using var connection = Connection.GetOpenConnection(_config.GetConnectionString(StringHelpers.Database.Crm));
            {
                return connection.Update(collectionRequestTrackAndTraces);
            }
        }
    }
}