using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;

public class SelectEmployerRedirectRequestMapper(ICacheStorageService cacheStorage)
    : IMapper<SelectEmployerRedirectRequest, SelectMultipleLearnerRecordsRequest>
{
    public async Task<SelectMultipleLearnerRecordsRequest> Map(SelectEmployerRedirectRequest source)
    {
        var cacheItem = new SelectMultipleLearnerRecordsCacheItem(Guid.NewGuid())
        {
            ProviderId = source.ProviderId,
            EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
            UseLearnerData = source.UseLearnerData,
            EmployerAccountName = source.EmployerAccountName
        };

        await cacheStorage.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);

        return new SelectMultipleLearnerRecordsRequest
        {
            ProviderId = source.ProviderId,
            CacheKey = cacheItem.CacheKey
        };
    }
}
