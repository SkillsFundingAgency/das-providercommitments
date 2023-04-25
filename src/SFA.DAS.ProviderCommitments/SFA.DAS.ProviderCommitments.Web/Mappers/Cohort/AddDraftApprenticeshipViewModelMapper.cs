using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class AddDraftApprenticeshipViewModelMapper : IMapper<CreateCohortWithDraftApprenticeshipRequest, AddDraftApprenticeshipViewModel>
    {
        private readonly IOuterApiClient _outerApiClient;
        private readonly ITempDataStorageService _tempData;
        private readonly ICacheStorageService _cacheStorage;

        public AddDraftApprenticeshipViewModelMapper(IOuterApiClient outerApiClient, ITempDataStorageService tempDataStorageService, ICacheStorageService cacheStorage)
        {
            _outerApiClient = outerApiClient;
            _tempData = tempDataStorageService;
            _cacheStorage = cacheStorage;
        }

        public async Task<AddDraftApprenticeshipViewModel> Map(CreateCohortWithDraftApprenticeshipRequest source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<CreateCohortCacheItem>(source.CacheKey);

            var apiRequest = new GetAddDraftApprenticeshipDetailsRequest(source.ProviderId, cacheItem.AccountLegalEntityId, cacheItem.CourseCode);
            var apiResponse = await _outerApiClient.Get<GetAddDraftApprenticeshipDetailsResponse>(apiRequest);

            var result = _tempData.RetrieveFromCache<AddDraftApprenticeshipViewModel>();
            _tempData.RemoveFromCache<AddDraftApprenticeshipViewModel>();

            if (result == null)
            {
                result = new AddDraftApprenticeshipViewModel
                {
                    CacheKey = source.CacheKey,
                    ProviderId = source.ProviderId,
                    EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                    Courses = null
                };
            }

            result.ReservationId = cacheItem.ReservationId;
            result.StartDate = new MonthYearModel(cacheItem.StartMonthYear);
            result.CourseCode = cacheItem.CourseCode;
            result.DeliveryModel = (DeliveryModel) cacheItem.DeliveryModel.Value;
            result.HasMultipleDeliveryModelOptions = apiResponse.HasMultipleDeliveryModelOptions;
            result.Employer = apiResponse.LegalEntityName;
            result.AccountLegalEntityId = cacheItem.AccountLegalEntityId;
            result.IsOnFlexiPaymentPilot = cacheItem.IsOnFlexiPaymentPilot;

            result.FirstName = cacheItem.FirstName;
            result.LastName = cacheItem.LastName;
            result.Email = cacheItem.Email;
            result.Uln = cacheItem.Uln;

            return result;
        }
    }
}