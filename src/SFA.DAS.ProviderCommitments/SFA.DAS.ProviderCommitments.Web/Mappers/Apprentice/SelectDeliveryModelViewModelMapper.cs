using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class SelectDeliveryModelViewModelMapper : IMapper<SelectDeliveryModelRequest, SelectDeliveryModelViewModel>
    {
        private readonly IOuterApiClient _outerApiClient;
        private readonly ICacheStorageService _cacheStorage;

        public SelectDeliveryModelViewModelMapper(IOuterApiClient approvalsOuterApiClient,
            ICacheStorageService cacheStorage)
        {
            _outerApiClient = approvalsOuterApiClient;
            _cacheStorage = cacheStorage;
        }

        public async Task<SelectDeliveryModelViewModel> Map(SelectDeliveryModelRequest source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);
            var apiRequest = new GetSelectDeliveryModelRequest(source.ProviderId, source.ApprenticeshipId,
                cacheItem.AccountLegalEntityId);
            var apiResponse = await _outerApiClient.Get<GetSelectDeliveryModelResponse>(apiRequest);

            if (apiResponse.DeliveryModels.Count == 1)
            {
                cacheItem.DeliveryModel = apiResponse.DeliveryModels.Single();
                cacheItem.SkippedDeliveryModelSelection = true;
                await _cacheStorage.SaveToCache(cacheItem.Key, cacheItem, 1);
            }

            return new SelectDeliveryModelViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                LegalEntityName = apiResponse.LegalEntityName,
                DeliveryModels = apiResponse.DeliveryModels,
                DeliveryModel = cacheItem.DeliveryModel,
                CacheKey = source.CacheKey,
                IsEdit = source.IsEdit,
                ApprenticeshipStatus = apiResponse.Status
            };
        }
    }
}