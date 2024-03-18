using SFA.DAS.ProviderCommitments.Authorization;
using SFA.DAS.ProviderCommitments.Extensions;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;

public class ProviderAuthorizationHandler(ICachedOuterApiService cachedOuterApiService, IAuthorizationValueProvider authorizationValueProvider) : IAuthorizationHandler
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

        var ukPrn = authorizationValueProvider.GetUkrpn();
        var accountLegalEntityId = authorizationValueProvider.GetAccountLegalEntityId();
        var operation = options.Select(o => o.ToEnum<Operation>()).Single();

        var hasPermission = await cachedOuterApiService.HasPermission(ukPrn, accountLegalEntityId, operation);

        if (!hasPermission)
        {
            authorizationResult.AddError(new ProviderPermissionNotGranted());
        }

        return authorizationResult;
    }
}