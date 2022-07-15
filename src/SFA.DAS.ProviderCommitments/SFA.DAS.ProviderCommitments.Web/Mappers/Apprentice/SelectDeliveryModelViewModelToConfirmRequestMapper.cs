using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class SelectDeliveryModelViewModelToConfirmRequestMapper : IMapper<SelectDeliveryModelViewModel, ConfirmRequest>
    {
        private readonly ILogger<SelectDeliveryModelViewModelToConfirmRequestMapper> _logger;
        private readonly ICacheStorageService _cacheStorage;

        public SelectDeliveryModelViewModelToConfirmRequestMapper(ICacheStorageService cacheStorage, ILogger<SelectDeliveryModelViewModelToConfirmRequestMapper> logger)
        {
            _cacheStorage = cacheStorage;
            _logger = logger;
        }

        public async Task<ConfirmRequest> Map(SelectDeliveryModelViewModel source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);
            cacheItem.DeliveryModel = source.DeliveryModel;
            await _cacheStorage.SaveToCache(cacheItem.Key, cacheItem, 1);

            return new ConfirmRequest
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                CacheKey = source.CacheKey
            };
        }
    }
}
