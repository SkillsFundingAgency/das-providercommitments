using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class StartDateViewModelToConfirmRequestMapper : IMapper<StartDateViewModel, ConfirmRequest>
    {
        private readonly ILogger<StartDateViewModelToConfirmRequestMapper> _logger;
        private readonly ICacheStorageService _cacheStorage;

        public StartDateViewModelToConfirmRequestMapper(ILogger<StartDateViewModelToConfirmRequestMapper> logger, ICacheStorageService cacheStorage)
        {
            _logger = logger;
            _cacheStorage = cacheStorage;
        }

        public async Task<ConfirmRequest> Map(StartDateViewModel source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);
            cacheItem.StartDate = source.StartDate.Date.Value.ToString("MMyyyy");
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
