using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class SelectDeliveryModelViewModelMapper : IMapper<SelectDeliveryModelRequest, SelectDeliveryModelViewModel>
    {
        private readonly IOuterApiClient _outerApiClient;
        private readonly ICacheStorageService _cacheStorage;

        public SelectDeliveryModelViewModelMapper(IOuterApiClient approvalsOuterApiClient, ICacheStorageService cacheStorage)
        {
            _outerApiClient = approvalsOuterApiClient;
            _cacheStorage = cacheStorage;
        }

        public async Task<SelectDeliveryModelViewModel> Map(SelectDeliveryModelRequest source)
        {
            var apiRequest = new GetSelectDeliveryModelRequest(source.ProviderId, source.ApprenticeshipId);

            var apiResponse = await _outerApiClient.Get<GetSelectDeliveryModelResponse>(apiRequest);
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);
           
            return new SelectDeliveryModelViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                LegalEntityName = apiResponse.LegalEntityName,
                DeliveryModels = apiResponse.DeliveryModels,
                DeliveryModel = cacheItem.DeliveryModel,
                CacheKey = source.CacheKey
            };
        }
    }
}
