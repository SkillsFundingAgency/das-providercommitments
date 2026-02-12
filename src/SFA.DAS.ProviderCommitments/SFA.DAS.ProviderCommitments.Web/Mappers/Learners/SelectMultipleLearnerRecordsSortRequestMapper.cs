using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Learners;

public class SelectMultipleLearnerRecordsSortRequestMapper(ICacheStorageService cacheStorage)
    : IMapper<SelectMultipleLearnerRecordsSortRequest, SelectMultipleLearnerRecordsRequest>
{
    public async Task<SelectMultipleLearnerRecordsRequest> Map(SelectMultipleLearnerRecordsSortRequest source)
    {
        var cacheItem = await cacheStorage.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(source.CacheKey.Value);

        var reverseSort = !string.IsNullOrEmpty(cacheItem.SortField)
                           && cacheItem.SortField.ToLower() == source.SortField.ToLower()
                           && !cacheItem.ReverseSort;

        cacheItem.SortField = source.SortField;
        cacheItem.ReverseSort = reverseSort;

        await cacheStorage.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);

        var request = new SelectMultipleLearnerRecordsRequest
        {
            ProviderId = cacheItem.ProviderId,
            CacheKey = source.CacheKey
        };

        return request;
    }
}