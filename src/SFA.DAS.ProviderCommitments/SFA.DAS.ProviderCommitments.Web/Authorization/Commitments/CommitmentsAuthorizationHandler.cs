using Microsoft.AspNetCore.Routing;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Commitments;

public class CommitmentsAuthorisationHandler(
    IHttpContextAccessor httpContextAccessor,
    IEncodingService encodingService,
    ICachedOuterApiService cachedOuterApiService)
    : ICommitmentsAuthorisationHandler
{
    public Task<bool> CanAccessCohort()
    {
        var permissionValues = GetPermissionValues();

        return cachedOuterApiService.CanAccessCohort(Party.Provider, permissionValues.PartyId, permissionValues.CohortId);
    }

    public Task<bool> CanAccessApprenticeship()
    {
        var permissionValues = GetPermissionValues();

        return cachedOuterApiService.CanAccessApprenticeship(Party.Provider, permissionValues.PartyId, permissionValues.ApprenticeshipId);
    }

    private (long CohortId, long ApprenticeshipId, long PartyId) GetPermissionValues()
    {
        var cohortId = GetCohortId();
        var apprenticeshipId = GetApprenticeshipId();
        var providerId = GetProviderId();
        
        if (cohortId == 0 && apprenticeshipId == 0 && providerId == 0)
        {
            throw new KeyNotFoundException("At least one key of 'ProviderId', 'CohortId' or 'ApprenticeshipId' should be present in the authorization context");
        }

        return (cohortId, apprenticeshipId, providerId);
    }

    private long GetProviderId()
    {
        if (!httpContextAccessor.HttpContext.TryGetValueFromHttpContext(RouteValueKeys.ProviderId, out var value))
        {
            return 0;
        }

        if (!int.TryParse(value, out var providerId))
        {
            providerId = 0;
        }

        return providerId;
    }

    private long GetApprenticeshipId()
    {
        return GetAndDecodeValueIfExists(RouteValueKeys.ApprenticeshipId, EncodingType.ApprenticeshipId);
    }

    private long GetCohortId()
    {
        return GetAndDecodeValueIfExists(RouteValueKeys.CohortReference, EncodingType.CohortReference);
    }

    private long GetAndDecodeValueIfExists(string keyName, EncodingType encodedType)
    {
        if (!httpContextAccessor.HttpContext.TryGetValueFromHttpContext(keyName, out var encodedValue))
        {
            return 0;
        }

        if (!encodingService.TryDecode(encodedValue, encodedType, out var id))
        {
            throw new UnauthorizedAccessException($"Failed to decode '{keyName}' value '{encodedValue}' using encoding type '{encodedType}'");
        }

        return id;
    }
}