using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Learners;

public class SelectMultipleLearnerRecordsFilterRequestMapper(ICacheStorageService cacheStorage)
    : IMapper<SelectMultipleLearnerRecordsFilterRequest, SelectMultipleLearnerRecordsRequest>
{
    public async Task<SelectMultipleLearnerRecordsRequest> Map(SelectMultipleLearnerRecordsFilterRequest source)
    {
        var cacheItem = await cacheStorage.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(source.CacheKey.Value);

        if (source.ClearFilter)
        {
            cacheItem.Filter.SearchTerm = "";
            cacheItem.Filter.StartMonth = null;
            cacheItem.Filter.StartYear = DateTime.UtcNow.Year.ToString();
        }
        else
        {
            cacheItem.Filter.SearchTerm = source.SearchTerm;
            cacheItem.Filter.StartMonth = source.StartMonth.ToString();
            cacheItem.Filter.StartYear = source.StartYear.ToString();
        }

        await cacheStorage.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);

        var request = new SelectMultipleLearnerRecordsRequest
        {
            ProviderId = cacheItem.ProviderId,
            CacheKey = source.CacheKey,
        };

        return request;
    }
}