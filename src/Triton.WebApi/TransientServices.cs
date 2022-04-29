using Microsoft.Extensions.DependencyInjection;
using Triton.Interface.BusinessOnline;
using Triton.Interface.Collection;
using Triton.Interface.CRM;
using Triton.Interface.HR;
using Triton.Interface.Mobile;
using Triton.Interface.TritonGroup;
using Triton.Interface.TritonSecurity;
using Triton.Interface.Waybill;
using Triton.Repository.BusinessOnline;
using Triton.Repository.Collection;
using Triton.Repository.CRM;
using Triton.Repository.HR;
using Triton.Repository.Mobile;
using Triton.Repository.TritonExpress;
using Triton.Repository.TritonGroup;
using Triton.Repository.TritonSecurity;
using Triton.Repository.TritonStaging;

namespace Triton.WebApi
{
    public static class TransientServices
    {
        public static void AddTransientService(IServiceCollection services)
        {
            services.AddTransient<IBusinessOnline, BusinessOnlineRepository>();
            services.AddTransient<IBranches,BranchesRespository>();
            services.AddTransient<ICollection, CollectionRepository>();
            services.AddTransient<ICollectionRequest, CollectionRequestRepository>();
            services.AddTransient<ICollectionRequestTrackAndTraces, CollectionRequestTrackAndTracesRepository>();
            services.AddTransient<ICollectionManifestLineItems, CollectionManifestLineItemsRepository>();
            services.AddTransient<ICollectionManifests, CollectionManifestRepository>();
            services.AddTransient<ICollectionManifestLineItemStatuss, CollectionManifestLineItemStatusRepository>();
            services.AddTransient<ICountryCurrencySpots, CountryCurrencySpotsRepository>();
            services.AddTransient<ICustomers, CustomerRepository>();
            services.AddTransient<ICustomerSiteMaps, CustomerSiteMapsRepository>();
            services.AddTransient<IFuelSurchargeClass, FuelSurchargeClassRepository>();
            services.AddTransient<IEmployee, EmployeeRepository>();
            services.AddTransient<IInsuranceTypes, InsuranceTypesRepository>();
            services.AddTransient<IPostalCodes, PostalCodesRepository>();
            services.AddTransient<IQuestionnaire, QuestionnaireRepository>();
            services.AddTransient<IQuotes,QuotesRepository>();
            services.AddTransient<IRole, RoleRepository>();
            services.AddTransient<ISites, SitesRepository>();
            services.AddTransient<ITransportTypes, LookupServiceRepository>();
            services.AddTransient<ITritonScanner, TritonScannerRepository>();
            services.AddTransient<ITritonStagingStoredProcs, TritonStagingStoredProcsRepository>();
            services.AddTransient<IUser, UsersRepository>();
            services.AddTransient<IUserCustomerMap, UserCustomerMapRepository>();
            services.AddTransient<IUserMap, UserMapRepository>();
            services.AddTransient<IUserRole, UserRoleRepository>();
            services.AddTransient<IWaybill, WaybillsRepository>();
            services.AddTransient<IWaybillQueryMaster, WaybillQueryMasterRepository>();
            services.AddTransient<IInvoices, InvoiceRepository>();
            services.AddTransient<IReportManager, ReportManagerRepository>();
            services.AddTransient<IExternalUser, ExternalUserRepository>();
            services.AddTransient<IExternalUserRole, ExternalUserRoleRepository>();
            services.AddTransient<IExternalUserMap, ExternalUserMapRepository>();
        }
    }
}
