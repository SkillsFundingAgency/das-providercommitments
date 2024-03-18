using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Provider;

public interface IProviderAuthorizationHandler
{
    Task<bool> CanCreateCohort();
}

public class ProviderAuthorizationHandler(IHttpContextAccessor httpContextAccessor,
    ICachedOuterApiService cachedOuterApiService,
    IAuthenticationService authenticationService, 
    IEncodingService encodingService)
    : IProviderAuthorizationHandler
{
    public async Task<bool> CanCreateCohort()
    {
        var ukPrn = GetUkrpn();
        var accountLegalEntityId = GetAccountLegalEntityId();

        return await cachedOuterApiService.HasPermission(ukPrn, accountLegalEntityId, Operation.CreateCohort.ToString());
    }

    private long? GetAccountLegalEntityId()
    {
        return FindAndDecodeValue(RouteValueKeys.AccountLegalEntityPublicHashedId, EncodingType.PublicAccountLegalEntityId);
    }

    private long? FindAndDecodeValue(string key, EncodingType encodingType)
    {
        if (!httpContextAccessor.HttpContext.TryGetValueFromHttpContext(key, out var encodedValue))
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
}