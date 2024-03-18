using Microsoft.AspNetCore.Routing;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.Authorization;

public interface IAuthorizationValueProvider
{
    long GetApprenticeshipId();
    long GetCohortId();
    long? GetAccountLegalEntityId();
    long? GetUkrpn();
    long GetProviderId();
}

public class AuthorizationValueProvider(
    IHttpContextAccessor httpContextAccessor,
    IEncodingService encodingService,
    IAuthenticationService authenticationService) : IAuthorizationValueProvider
{
    public long GetApprenticeshipId()
    {
        return GetAndDecodeValueIfExists(RouteValueKeys.ApprenticeshipId, EncodingType.ApprenticeshipId);
    }

    public long GetCohortId()
    {
        return GetAndDecodeValueIfExists(RouteValueKeys.CohortReference, EncodingType.CohortReference);
    }
    
    public long GetProviderId()
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
    
    public long? GetAccountLegalEntityId()
    {
        return FindAndDecodeValue(RouteValueKeys.AccountLegalEntityPublicHashedId, EncodingType.PublicAccountLegalEntityId);
    }
    
    public long? GetUkrpn()
    {
        if (!authenticationService.IsUserAuthenticated())
        {
            return null;
        }

        if (!authenticationService.TryGetUserClaimValue(ProviderClaims.Ukprn, out var ukprnClaimValue))
        {
            throw new UnauthorizedAccessException($"Failed to get value for claim '{ProviderClaims.Ukprn}'");
        }

        if (!long.TryParse(ukprnClaimValue, out var ukprn))
        {
            throw new UnauthorizedAccessException($"Failed to parse value '{ukprnClaimValue}' for claim '{ProviderClaims.Ukprn}'");
        }

        return ukprn;
    }
    
    private long? FindAndDecodeValue(string key, EncodingType encodingType)
    {
        if (!TryGetValueFromHttpContext(key, out var encodedValue))
        {
            return null;
        }

        if (!encodingService.TryDecode(encodedValue, encodingType, out var value))
        {
            throw new UnauthorizedAccessException($"Failed to decode '{key}' value '{encodedValue}' using encoding type '{encodingType}'");
        }

        return value;
    }
    
    private bool TryGetValueFromHttpContext(string key, out string value)
    {
        value = null;

        // for testing
        if (httpContextAccessor.HttpContext == null)
        {
            return false;
        }

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