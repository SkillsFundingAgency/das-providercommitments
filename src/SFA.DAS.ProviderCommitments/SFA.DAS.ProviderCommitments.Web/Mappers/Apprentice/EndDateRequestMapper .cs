using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class EndDateRequestMapper : IMapper<StartDateViewModel, EndDateRequest>
    {
        private readonly ICacheStorageService _cacheStorage;

        public EndDateRequestMapper(ICacheStorageService cacheStorage)
        {
            _cacheStorage = cacheStorage;
        }

        public async Task<EndDateRequest> Map(StartDateViewModel source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);
            cacheItem.StartDate = source.StartDate.Date.Value.ToString("MMyyyy");
            await _cacheStorage.SaveToCache(cacheItem.Key, cacheItem,1);

            return new EndDateRequest
            {
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                ProviderId = source.ProviderId,
                CacheKey = source.CacheKey
            };
        }
    }
}