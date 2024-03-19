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
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;

public interface ICachedOuterApiService
{
    Task<bool> HasPermission(long? ukprn, long? accountLegalEntityId, Operation operation);
    Task<bool> CanAccessCohort(Party party, long partyId, long cohortId);
    Task<bool> CanAccessApprenticeship(Party party, long partyId, long apprenticeshipId);
}

public class CachedOuterApiService(ICacheStorageService cacheStorage, IOuterApiService outerApiService) : ICachedOuterApiService
{
    public const int CacheExpirationMinutes = 5;

    public async Task<bool> HasPermission(long? ukprn, long? accountLegalEntityId, Operation operation)
    {
        var cacheKey = $"{nameof(HasPermission)}.{ukprn}.{accountLegalEntityId}.{operation}";
        var cachedResponse = await cacheStorage.SafeRetrieveFromCache<bool?>(cacheKey);

        if (cachedResponse != null)
        {
            return cachedResponse.Value;
        }

        var hasPermission = await outerApiService.HasPermission(ukprn, accountLegalEntityId, operation);

        await cacheStorage.SaveToCache(cacheKey, hasPermission, TimeSpan.FromMinutes(CacheExpirationMinutes));

        return hasPermission;
    }

    public async Task<bool> CanAccessCohort(Party party, long partyId, long cohortId)
    {
        var cacheKey = $"{nameof(CanAccessCohort)}.{party}.{partyId}.{cohortId}";
        var cachedResponse = await cacheStorage.SafeRetrieveFromCache<bool?>(cacheKey);

        if (cachedResponse != null)
        {
            return cachedResponse.Value;
        }

        var canAccessCohort = await outerApiService.CanAccessCohort(party, partyId, cohortId);

        await cacheStorage.SaveToCache(cacheKey, canAccessCohort, TimeSpan.FromMinutes(CacheExpirationMinutes));

        return canAccessCohort;
    }

    public async Task<bool> CanAccessApprenticeship(Party party, long partyId, long apprenticeshipId)
    {
        var cacheKey = $"{nameof(CanAccessApprenticeship)}.{party}.{partyId}.{apprenticeshipId}";
        var cachedResponse = await cacheStorage.SafeRetrieveFromCache<bool?>(cacheKey);

        if (cachedResponse != null)
        {
            return cachedResponse.Value;
        }

        var canAccessApprenticeship = await outerApiService.CanAccessApprenticeship(party, partyId, apprenticeshipId);

        await cacheStorage.SaveToCache(cacheKey, canAccessApprenticeship, TimeSpan.FromMinutes(CacheExpirationMinutes));

        return canAccessApprenticeship;
    }
}