using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Authorization;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;

public interface ICachedOuterApiService
{
    Task<bool> HasPermission(long? ukprn, long? accountLegalEntityId, string operation);
    Task<bool> CanAccessCohort(Party party, long partyId, long cohortId);
    Task<bool> CanAccessApprenticeship(Party party, long partyId, long apprenticeshipId);
}

public class CachedOuterApiService(ICacheStorageService cacheStorage, IOuterApiService outerApiService) : ICachedOuterApiService
{
    private const int CacheExpirationMinutes = 5;
    
    public async Task<bool> HasPermission(long? ukprn, long? accountLegalEntityId, string operation)
    {
        var cacheKey = $"{nameof(GetHasPermissionResponse)}.{ukprn}.{accountLegalEntityId}.{operation}";
        var cachedResponse = await cacheStorage.RetrieveFromCache<GetHasPermissionResponse>(cacheKey);

        if (cachedResponse != null)
        {
            return cachedResponse.HasPermission;
        }

        var hasPermission = await outerApiService.HasPermission(ukprn, accountLegalEntityId, operation);

        await cacheStorage.SaveToCache(cacheKey, hasPermission, TimeSpan.FromMinutes(CacheExpirationMinutes));

        return hasPermission;
    }

    public async Task<bool> CanAccessCohort(Party party, long partyId, long cohortId)
    {
        var cacheKey = $"{nameof(GetCohortAccessResponse)}.{party}.{partyId}.{cohortId}";
        var cachedResponse = await cacheStorage.RetrieveFromCache<GetCohortAccessResponse>(cacheKey);

        if (cachedResponse != null)
        {
            return cachedResponse.HasCohortAccess;
        }

        var canAccessCohort = await outerApiService.CanAccessCohort(party, partyId, cohortId);

        await cacheStorage.SaveToCache(cacheKey, canAccessCohort, TimeSpan.FromMinutes(CacheExpirationMinutes));

        return canAccessCohort;
    }

    public async Task<bool> CanAccessApprenticeship(Party party, long partyId, long apprenticeshipId)
    {
        var cacheKey = $"{nameof(GetApprenticeshipAccessResponse)}.{party}.{partyId}.{apprenticeshipId}";
        var cachedResponse = await cacheStorage.RetrieveFromCache<GetApprenticeshipAccessResponse>(cacheKey);

        if (cachedResponse != null)
        {
            return cachedResponse.HasApprenticeshipAccess;
        }

        var canAccessApprenticeship = await outerApiService.CanAccessApprenticeship(party, partyId, apprenticeshipId);

        await cacheStorage.SaveToCache(cacheKey, canAccessApprenticeship, TimeSpan.FromMinutes(CacheExpirationMinutes));

        return canAccessApprenticeship;
    }
}