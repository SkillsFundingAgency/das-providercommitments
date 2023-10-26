using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class EndDateViewModelToConfirmRequestMapper : IMapper<EndDateViewModel, ConfirmRequest>
    {
        private readonly ILogger<EndDateViewModelToConfirmRequestMapper> _logger;
        private readonly ICacheStorageService _cacheStorage;

        public EndDateViewModelToConfirmRequestMapper(ILogger<EndDateViewModelToConfirmRequestMapper> logger, ICacheStorageService cacheStorage)
        {
            _logger = logger;
            _cacheStorage = cacheStorage;
        }

        public async Task<ConfirmRequest> Map(EndDateViewModel source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);
            cacheItem.EndDate = source.EndDate.MonthYear;
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
