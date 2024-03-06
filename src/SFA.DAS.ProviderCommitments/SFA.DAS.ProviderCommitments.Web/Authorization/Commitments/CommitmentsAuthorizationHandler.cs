using Microsoft.AspNetCore.Routing;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Commitments;

public class CommitmentsAuthorisationHandler(
    IHttpContextAccessor httpContextAccessor,
    IEncodingService encodingService,
    IOuterApiService cachedOuterApiService)
    : ICommitmentsAuthorisationHandler
{
    public Task<bool> CanAccessCohort()
    {
        var permissionValues = GetPermissionValues();

        return cachedOuterApiService.CanAccessCohort(Party.Employer, permissionValues.PartyId, permissionValues.CohortId);
    }
    
    public Task<bool> CanAccessApprenticeship()
    {
        var permissionValues = GetPermissionValues();
        
        return cachedOuterApiService.CanAccessApprenticeship(Party.Employer, permissionValues.PartyId, permissionValues.ApprenticeshipId);
    }

    private (long CohortId, long ApprenticeshipId, long PartyId) GetPermissionValues()
    {
        var cohortId = GetCohortId();
        var apprenticeshipId = GetApprenticeshipId();
        var accountId = GetAccountId();

        if (cohortId == 0 && apprenticeshipId == 0 && accountId == 0)
        {
            throw new KeyNotFoundException("At least one key of 'AccountId', 'CohortId' or 'ApprenticeshipId' should be present in the authorization context");
        }

        return (cohortId, apprenticeshipId, accountId);
    }

    private long GetAccountId()
    {
        return GetAndDecodeValueIfExists(RouteValueKeys.AccountHashedId, EncodingType.AccountId);
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
        if (!TryGetValueFromHttpContext(keyName, out var encodedValue))
        {
            return 0;
        }

        if (!encodingService.TryDecode(encodedValue, encodedType, out var id))
        {
            throw new UnauthorizedAccessException($"Failed to decode '{keyName}' value '{encodedValue}' using encoding type '{encodedType}'");
        }

        return id;
    }

    private bool TryGetValueFromHttpContext(string key, out string value)
    {
        value = null;

        if (httpContextAccessor.HttpContext.GetRouteData().Values.TryGetValue(key, out var routeValue))
        {
            value = (string)routeValue;
        }
        else if (httpContextAccessor.HttpContext.Request.Query.TryGetValue(key, out var queryStringValue))
        {
            value = queryStringValue;
        }
        else if (httpContextAccessor.HttpContext.Request.HasFormContentType && httpContextAccessor.HttpContext.Request.Form.TryGetValue(key, out var formValue))
        {
            value = formValue;
        }

        return !string.IsNullOrWhiteSpace(value);
    }
}