using Microsoft.AspNetCore.Routing;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Authorization;
using SFA.DAS.ProviderCommitments.Extensions;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;

public class ProviderAuthorizationHandler(IOuterApiService outerApiService,
    IAuthenticationService authenticationService, 
    IHttpContextAccessor httpContextAccessor, 
    IEncodingService encodingService) : IAuthorizationHandler
{
    public string Prefix => "ProviderOperation.";

    public async Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
    {
        var authorizationResult = new AuthorizationResult();

        if (options.Count <= 0)
        {
            return authorizationResult;
        }

        options.EnsureNoAndOptions();
        options.EnsureNoOrOptions();

        var ukPrn = GetUkrpn();
        var accountLegalEntityId = GetAccountLegalEntityId();
        var operation = options.Select(o => o.ToEnum<Operation>()).Single();

        var hasPermission = await outerApiService.HasPermission(ukPrn, accountLegalEntityId, operation.ToString());

        if (!hasPermission)
        {
            authorizationResult.AddError(new ProviderPermissionNotGranted());
        }

        return authorizationResult;
    }
    
    private long? GetAccountLegalEntityId()
    {
        return FindAndDecodeValue(RouteValueKeys.AccountLegalEntityPublicHashedId, EncodingType.PublicAccountLegalEntityId);
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
    
    private long? GetUkrpn()
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