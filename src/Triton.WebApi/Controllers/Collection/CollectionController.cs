using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Swagger;
using Triton.Interface.Collection;
using Triton.Interface.CRM;
using Triton.Interface.Waybill;
using Triton.Model.CRM.Custom;
using Triton.Model.CRM.StoredProcs;
using Triton.Model.CRM.Tables;
using Triton.Model.FWWebservice.Custom;
using Triton.Model.TritonOps.StoredProcs;
using Triton.Model.TritonSecurity.Custom;
using Triton.WebApi.Controllers.Freightware;
using Vendor.Services.Edocs.VirtualPostman;
using Vendor.Services.Helpers;
using CollectionRequestTrackandTracesModel = Triton.Model.CRM.Custom.CollectionRequestTrackandTracesModel;

namespace Triton.WebApi.Controllers.Collection
{
    [Route("api/[controller]")]
    [SwaggerTag("Collection Request / Collections / CollectionManifestLineItems / CollectionRequestTrackAndTrace / CollectionManifests")]
    [ApiController]
    public class CollectionController : ControllerBase
    {
        private readonly ICollectionRequest _collectionRequests;
        private readonly ICollection _collection;
        private readonly ICollectionManifestLineItems _collectionManifestLineItems;
        private readonly ICollectionRequestTrackAndTraces _collectionRequestTrackAndTraces;
        private readonly ICollectionManifests _collectionManifests;
        private readonly IWaybill _waybillsRepo;
        private readonly IVendorServices _converter;
        private readonly ICustomers _customer;
        private readonly ITransportTypes _transportType;
        private readonly IConfiguration _configuration;
        private readonly ISites _sites;

        public CollectionController(ICollectionRequest collectionRequest, ICollection collection, ICollectionManifestLineItems collectionManifestLineItems, ICollectionRequestTrackAndTraces collectionRequestTrackAndTraces, ICollectionManifests collectionManifests,
                                     ICustomers customer,IVendorServices converter,IWaybill waybill,ITransportTypes transportTypes,IConfiguration configuration, ISites sites)
        {
            _collectionRequests = collectionRequest;
            _collection = collection;
            _collectionManifestLineItems = collectionManifestLineItems;
            _collectionRequestTrackAndTraces = collectionRequestTrackAndTraces;
            _collectionManifests = collectionManifests;
            _converter=converter;
            _waybillsRepo=waybill;
            _customer=customer;
            _transportType=transportTypes;
            _configuration=configuration;
            _sites = sites;
        }
        #region Collection Request
        [HttpGet]
        [SwaggerOperation(Summary = "Get collection Request by customerXRef and Date", Description = "Get Collection By Date or CustomerXRef")]
        public async Task<ActionResult<CollectionRequestsModel>> GetCollectionRequest(string customerXRef, DateTime? startDate, DateTime? endDate, string CollectionRequestNumber, int customerId)
        {
            return await _collectionRequests.FindCollectionRequest(customerXRef, startDate, endDate, CollectionRequestNumber, customerId);
        }
        [HttpGet("CollectionRequest/{collectionRequestId}")]
        [SwaggerOperation(Summary = "GetCollectionRequest", Description = "Get Collection Request BY CollectionrequestID")]
        public async Task<ActionResult<CollectionRequests>> GetCollectionRequestById(int collectionRequestId)
        {
            return await _collectionRequests.GetCollectionRequest(collectionRequestId);
        }
        [HttpGet("GetComplexCollectionRequest/{collectionRequestId}")]
        [SwaggerOperation("Returns a complex object for collection Request", "Returns a complex object for collection Request")]

        public async Task<ActionResult<CollectionRequestsModel>> GetComplex(int collectionRequestId)
        {
            return await _collectionRequests.GetComplex(collectionRequestId);
        }

        [HttpPut("CollectionRequest")]
        [SwaggerOperation(Summary = "Update an existing Collection Request", Description = "Update exiting Collection Request")]
        public async Task<ActionResult<bool>> PutCollectionRequest(CollectionRequests collectionRequests)
        {
            return await _collectionRequests.Put(collectionRequests);
        }

        [HttpPost("CollectionRequest/PostCollectionRequestUat")]
        [SwaggerOperation(Summary = "Create a new Collection Request", Description = "Create a Collection Request")]
        public async Task<ActionResult<FWResponsePacket>> PostCollectionRequestUat(CollectionRequests collectionRequests)
        {
            var fc = new FreightwareController(_configuration,_waybillsRepo,_converter,_customer,_transportType, _sites);
            var response = await fc.PostCollectionUAT(collectionRequests);
            if (response.Value.ReturnCode == "0000")
            {
                //collectionRequests.CollectionrequestNumber = response.Value.DataObject.OrdNo;
                //return await _collectionRequests.Post(collectionRequests, "CRM");
                return response;
            }

            return BadRequest($"There was an error saving - {response.Value.ReturnCode} : {response.Value.ReturnMessage}");
        }

        [HttpPost("CollectionRequest/Production")]
        [SwaggerOperation(Summary = "Create a new Collection Request", Description = "Create a Collection Request")]
        public async Task<ActionResult<FWResponsePacket>> PostCollectionRequestProduction(CollectionRequests collectionRequests)
        {
            var fc = new FreightwareController(_configuration, _waybillsRepo, _converter, _customer, _transportType, _sites);
            var response = await fc.PostCollectionProduction(collectionRequests);
            if (response.Value.ReturnCode == "0000")
            {
                //collectionRequests.CollectionrequestNumber = response.Value.DataObject.OrdNo;
                //return await _collectionRequests.Post(collectionRequests, "CRM");
                return response;
            }

            return BadRequest($"There was an error saving - {response.Value.ReturnCode} : {response.Value.ReturnMessage}");
        }
        #endregion
        #region Collections


        [HttpGet("Collection/{collectionId}")]
        [SwaggerOperation(Summary = "Collection by ID", Description = "Get Collection By collectionId")]
        public async Task<ActionResult<Collections>> GetCollectionByID(int collectionId)
        {
            return await _collection.GetCollection(collectionId);
        }
      
        [HttpGet("Collections/{CollectionNo}")]
        [SwaggerOperation(Summary = "Get Collection By Number", Description = "Get Collection By CollectionNo")]
        public async Task<ActionResult<Collections>> GetCollectionByNo(string CollectionNo)
        {
            return await _collection.GetCollectionByNo(CollectionNo);
        }
        [HttpPost("PostCollection")]
        [SwaggerOperation(Summary = "Create a new Collection", Description = "add A new Collection")]
        public async Task<ActionResult<long>> PostCollection(Collections collections)
        {
            return await _collection.Post(collections);
        }

        [HttpPut("PutCollection")]
        [SwaggerOperation(Summary = "Updating of a Collection", Description = "Update a Collection")]
        public async Task<ActionResult<bool>> PutCollection(Collections collections)
        {
            return await _collection.Put(collections);
        }

        #endregion

        #region CollectionManifestLineItems
        [HttpGet("CollectionManifestLineItem/{collectionManifestLineItemId}")]
        [SwaggerOperation(Summary = "Get CollectionManifestLineItem By ColletionManifestbyitemID", Description = "Get CollectionManifestLineItem By ColletionManifestbyitemID")]
        public async Task<ActionResult<CollectionManifestLineItems>> GetCollectionManifestLineItemById(int collectionManifestLineItemId)
        {
            return await _collectionManifestLineItems.GetCollectionManifestLineItems(collectionManifestLineItemId);
        }

        [HttpGet("GetComplexCollectionManifestLineItem/{collectionmanifestitemitemsId}")]
        [SwaggerOperation(Summary = "Get Complex CollectionManifestlineItem", Description = "Get Complex CollectionManifestlineItem")]
        public async Task<ActionResult<CollectionManifestLineItemsModel>> GetComplexbyId(int collectionmanifestitemitemsId)
        {
            return await _collectionManifestLineItems.GetComplex(collectionmanifestitemitemsId);
        }

        [HttpPost("PostCollectionManifestLineItems"),
         SwaggerOperation(Summary = "Insert a new collectionManifestLineITem", Description = "insert a collectionmanifestlineitem")]
        public async Task<ActionResult<long>> Post(CollectionManifestLineItems collectionManifestLineItems)
        {
            return await _collectionManifestLineItems.Post(collectionManifestLineItems);
        }
        [HttpPut("PutCollectionManifestLineItems"),
         SwaggerOperation(Summary = "update a collectionManifestLineITem", Description = "updatea collectionmanifestlineitem")]
        public async Task<ActionResult<long>> PostCollectionManifestLineitem(CollectionManifestLineItems collectionManifestLineItems)
        {
            return await _collectionManifestLineItems.Post(collectionManifestLineItems);
        }

        #endregion
        #region CollectionRequestTrackAndTrace
        [HttpGet("FindCollectionRequestTrackAndtrace/{collectionRequestId}")]
        [SwaggerOperation(Summary = "Return a list of track and Trace by CollectionRequestId", Description = "Return a list of track and Trace by CollectionRequestId")]
        public async Task<ActionResult<List<CollectionRequestTrackandTracesModel>>> FindCollectionRequestTrackAndTrace(int collectionRequestId)
        {
            return await _collectionRequestTrackAndTraces.FindCollectionRequest(collectionRequestId);
        }

        [HttpGet("GetCollectionRequestTraceAndTrace/{collectionRequestTrackAndTraceId}"), SwaggerOperation(Summary = "Get CollectionRequestTrackAndTrace by ID", Description = "Get CollectionRequestTrackAndTrace by CollectionRequestTrackAndTraceID")]
        public async Task<ActionResult<CollectionRequestTrackAndTraces>> GetCollectionRequestTraceAndTracebyId(int collectionRequestTrackAndTraceId)
        {
            return await _collectionRequestTrackAndTraces.GetCollectionRequestTrackAndTraces(collectionRequestTrackAndTraceId);
        }

        [HttpGet("GetComplexCollectionRequestTrackAndTrace/{collectionRequestTrackAndTraceId}"),
         SwaggerOperation(Summary = "Get complex CollectionRequestTrackAndTrace by id", Description = "get Complex CollectionRequestTrackAndTrace by CollectionRequestTrackAndTraceID")]
        public async Task<CollectionRequestTrackandTracesModel> GetComplexId(int collectionRequestTrackAndTraceId)
        {
            return await _collectionRequestTrackAndTraces.GetComplex(collectionRequestTrackAndTraceId);
        }

        [HttpPost("PostCollectionRequestTrackAndTraces"),
         SwaggerOperation(Summary = "Post CollectionRequestTrackAndTraces", Description = "Post CollectionRequestTrackAndTraces")]
        public async Task<ActionResult<long>> PostCollectionRequestTrackAndTraces(CollectionRequestTrackAndTraces collectionRequestTrackAndTraces)
        {
            return await _collectionRequestTrackAndTraces.Post(collectionRequestTrackAndTraces);
        }
        [HttpPost("PutCollectionRequestTrackAndTraces"), 
         SwaggerOperation(Summary = "Put CollectionRequestTrackAndTraces", Description = "Put CollectionRequestTrackAndTraces")]
        public async Task<ActionResult<bool>> PutlectionRequestTrackAndTraces(CollectionRequestTrackAndTraces collectionRequestTrackAndTraces)
        {
            return await _collectionRequestTrackAndTraces.Put(collectionRequestTrackAndTraces);
        }

        #endregion
        #region CollectionManifest
       [HttpGet("GetCollectionManifest/{collectionManifestId}"), SwaggerOperation("Get CollectionManifest By id", Description = "Get CollectionManifest by collectionManifestId")]
        public async Task<ActionResult<CollectionManifests>> GetCollectionManifestById(int collectionManifestId)
        {
            return await _collectionManifests.GetCollectionManifest(collectionManifestId);
        }

        [HttpGet("GetComplexCollectionManifest/{CollectionManifestId}"), SwaggerOperation(Summary = "Get complex CollectionManifest by Id", Description = "Get complex CollectionManifest by CollectionManifestId")]
        public async Task<ActionResult<CollectionManifestsModel>> GetComplexById(int CollectionManifestId)
        {
            return await _collectionManifests.GetComplex(CollectionManifestId);
        }

        [HttpPost("PostCollectioManifest"), SwaggerOperation(Summary = "Post CollectioManifest", Description = "Post CollectioManifest")]
        public async Task<ActionResult<long>> PostCollectionManifest(CollectionManifests collectionManifests)
        {
            return await _collectionManifests.Post(collectionManifests);
        }

        [HttpPut("PutCollectionManIfest"), SwaggerOperation(Summary = "Put CollectionManIfest", Description = "Put CollectionManIfest")]
        public async Task<ActionResult<bool>> PutCollectionManifest(CollectionManifests collectionManifests)
        {
            return await _collectionManifests.Put(collectionManifests);
        }

        #endregion

    }
}