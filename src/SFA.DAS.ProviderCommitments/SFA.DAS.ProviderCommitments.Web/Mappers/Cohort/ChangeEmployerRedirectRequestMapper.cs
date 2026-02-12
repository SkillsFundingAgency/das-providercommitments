using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;

public class ChangeEmployerRedirectRequestMapper(ICacheStorageService cacheStorage)
    : IMapper<ChangeEmployerRedirectRequest, SelectMultipleLearnerRecordsRequest>
{
    public async Task<SelectMultipleLearnerRecordsRequest> Map(ChangeEmployerRedirectRequest source)
    {
        var cacheItem = await cacheStorage.RetrieveFromCache<SelectMultipleLearnerRecordsCacheItem>(source.CacheKey.Value);

        cacheItem.ProviderId = source.ProviderId;
        cacheItem.EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId;
        cacheItem.EmployerAccountName = source.EmployerAccountName;

        await cacheStorage.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);

        return new SelectMultipleLearnerRecordsRequest
        {
            ProviderId = source.ProviderId,
            CacheKey = cacheItem.CacheKey
        };
    }
}
