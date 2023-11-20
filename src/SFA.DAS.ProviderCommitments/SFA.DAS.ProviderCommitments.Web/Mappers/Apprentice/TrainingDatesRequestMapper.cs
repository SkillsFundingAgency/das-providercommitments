using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class TrainingDatesRequestMapper :IMapper<SelectDeliveryModelViewModel, TrainingDatesRequest>
    {
        private readonly ICacheStorageService _cacheStorageService;

        public TrainingDatesRequestMapper(ICacheStorageService cacheStorageService)
        {
            _cacheStorageService = cacheStorageService;
        }

        public async Task<TrainingDatesRequest> Map(SelectDeliveryModelViewModel source)
        {
            var cacheItem = await _cacheStorageService.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);
            cacheItem.DeliveryModel = source.DeliveryModel;
            await _cacheStorageService.SaveToCache(source.CacheKey, cacheItem, 1);

            return new TrainingDatesRequest
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                IsEdit = source.IsEdit,
                CacheKey = source.CacheKey
            };
        }
    }
}
