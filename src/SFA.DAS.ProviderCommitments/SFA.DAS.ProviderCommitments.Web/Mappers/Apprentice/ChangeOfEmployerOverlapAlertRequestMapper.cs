using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class
        ChangeOfEmployerOverlapAlertRequestMapper : IMapper<PriceViewModel, ChangeOfEmployerOverlapAlertRequest>
    {
        private readonly ICacheStorageService _cacheStorage;

        public ChangeOfEmployerOverlapAlertRequestMapper(ICacheStorageService cacheStorage)
        {
            _cacheStorage = cacheStorage;
        }

        public async Task<ChangeOfEmployerOverlapAlertRequest> Map(PriceViewModel source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);
            cacheItem.Price = source.Price;
            cacheItem.EmploymentPrice = source.EmploymentPrice;
            await _cacheStorage.SaveToCache(cacheItem.Key, cacheItem, 1);

            return new ChangeOfEmployerOverlapAlertRequest
            {
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                ProviderId = source.ProviderId,
                CacheKey = source.CacheKey
            };
        }
    }
}