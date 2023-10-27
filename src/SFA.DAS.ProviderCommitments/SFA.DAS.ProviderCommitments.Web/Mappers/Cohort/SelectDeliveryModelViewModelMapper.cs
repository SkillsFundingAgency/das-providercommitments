using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using SelectDeliveryModelViewModel = SFA.DAS.ProviderCommitments.Web.Models.Cohort.SelectDeliveryModelViewModel;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class SelectDeliveryModelViewModelMapper : IMapper<CreateCohortWithDraftApprenticeshipRequest, SelectDeliveryModelViewModel>
    {
        private readonly IOuterApiClient _apiClient;
        private readonly ICacheStorageService _cacheStorage;

        public SelectDeliveryModelViewModelMapper(IOuterApiClient outerApiClient, ICacheStorageService cacheStorage)
        {
            _apiClient = outerApiClient;
            _cacheStorage = cacheStorage;
        }

        public async Task<SelectDeliveryModelViewModel> Map(CreateCohortWithDraftApprenticeshipRequest source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<CreateCohortCacheItem>(source.CacheKey);

            var apiRequest = new GetAddDraftApprenticeshipDeliveryModelRequest(source.ProviderId, cacheItem.AccountLegalEntityId, cacheItem.CourseCode);
            var apiResponse = await _apiClient.Get<GetAddDraftApprenticeshipDeliveryModelResponse>(apiRequest);

            var result = new SelectDeliveryModelViewModel
            {
                ProviderId = source.ProviderId,
                EmployerName = apiResponse.EmployerName,
                DeliveryModels = apiResponse.DeliveryModels,
                DeliveryModel = cacheItem.DeliveryModel
            };

            if (apiResponse.DeliveryModels.Count == 1)
            {
                cacheItem.DeliveryModel = apiResponse.DeliveryModels.Single();
                await _cacheStorage.SaveToCache(cacheItem, 1);
            }

            return result;
        }
    }
}
