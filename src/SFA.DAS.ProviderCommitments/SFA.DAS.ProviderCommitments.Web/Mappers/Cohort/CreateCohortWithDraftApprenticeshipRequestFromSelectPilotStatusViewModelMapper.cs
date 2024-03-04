using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class SelectPilotStatusRedirectModelMapper : IMapper<SelectPilotStatusViewModel, SelectPilotStatusRedirectModel>
    {
        private readonly ICacheStorageService _cacheStorage;

        public SelectPilotStatusRedirectModelMapper(ICacheStorageService cacheStorage)
        {
            _cacheStorage = cacheStorage;
        }

        public async Task<SelectPilotStatusRedirectModel> Map(SelectPilotStatusViewModel source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<CreateCohortCacheItem>(source.CacheKey);
            cacheItem.IsOnFlexiPaymentPilot = source.Selection == ChoosePilotStatusOptions.Pilot;
            await _cacheStorage.SaveToCache(cacheItem.CacheKey, cacheItem, 1);

            return new SelectPilotStatusRedirectModel
            {
                CacheKey = source.CacheKey,
                ProviderId = source.ProviderId
            };
        }
    }
}
