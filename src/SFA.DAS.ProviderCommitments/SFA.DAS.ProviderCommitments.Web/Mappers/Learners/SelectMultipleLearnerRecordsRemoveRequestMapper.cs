using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Learners;

public class SelectMultipleLearnerRecordsRemoveRequestMapper(ICacheStorageService cacheStorage)
    : IMapper<SelectMultipleLearnerRecordsRemoveRequest, SelectMultipleLearnerRecordsRequest>
{
    public async Task<SelectMultipleLearnerRecordsRequest> Map(SelectMultipleLearnerRecordsRemoveRequest source)
    {
        var cacheItem = await cacheStorage.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(source.CacheKey.Value);

        if (cacheItem.SelectedLearnersIds.Contains(source.LearnerId))
        {
            cacheItem.SelectedLearnersIds.Remove(source.LearnerId);
            await cacheStorage.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        var request = new SelectMultipleLearnerRecordsRequest
        {
            ProviderId = cacheItem.ProviderId,
            CacheKey = source.CacheKey
        };

        return request;
    }
}