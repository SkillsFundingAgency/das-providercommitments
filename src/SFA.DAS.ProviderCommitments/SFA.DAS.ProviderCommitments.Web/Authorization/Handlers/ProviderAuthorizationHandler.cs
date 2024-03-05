using SFA.DAS.ProviderCommitments.Authorization;
using SFA.DAS.ProviderCommitments.Extensions;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization.Context;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;

public class ProviderAuthorizationHandler(IOuterApiService outerApiService) : IAuthorizationHandler
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

        var values = authorizationContext.GetProviderPermissionValues();
        var operation = options.Select(o => o.ToEnum<Operation>()).Single();

        var hasPermission = await outerApiService.GetHasPermission(values.Ukprn, values.AccountLegalEntityId, operation.ToString());

        if (!hasPermission)
        {
            authorizationResult.AddError(new ProviderPermissionNotGranted());
        }

        return authorizationResult;
    }
}