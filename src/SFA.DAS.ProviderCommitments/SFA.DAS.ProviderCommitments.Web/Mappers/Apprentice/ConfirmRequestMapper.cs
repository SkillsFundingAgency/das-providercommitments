using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ConfirmRequestMapper : IMapper<PriceViewModel, ConfirmRequest>
    {
        private readonly ILogger<ConfirmRequestMapper> _logger;
        private readonly ICacheStorageService _cacheStorage;

        public ConfirmRequestMapper(ILogger<ConfirmRequestMapper> logger, ICacheStorageService cacheStorage)
        {
            _logger = logger;
            _cacheStorage = cacheStorage;
        }

        public async Task<ConfirmRequest> Map(PriceViewModel source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);
            cacheItem.Price = source.Price;
            cacheItem.EmploymentPrice = source.EmploymentPrice;
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
