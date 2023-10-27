using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class PriceRequestMapper : IMapper<EndDateViewModel, PriceRequest>
    {
        private readonly ICacheStorageService _cacheStorage;

        public PriceRequestMapper(ICacheStorageService cacheStorage)
        {
            _cacheStorage = cacheStorage;
        }

        public async Task<PriceRequest> Map(EndDateViewModel source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);
            cacheItem.EndDate = source.EndDate.MonthYear;
            cacheItem.EmploymentEndDate = source.EmploymentEndDate.MonthYear;
            await _cacheStorage.SaveToCache(cacheItem.Key, cacheItem, 1);

            return new PriceRequest
            {
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                ProviderId = source.ProviderId,
                CacheKey = source.CacheKey
            };
        }
    }
}