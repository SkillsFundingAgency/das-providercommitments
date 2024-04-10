using SFA.DAS.ProviderCommitments.Authorization;
using SFA.DAS.ProviderCommitments.Extensions;
using SFA.DAS.ProviderCommitments.Interfaces;
using Operation = SFA.DAS.ProviderRelationships.Types.Models.Operation;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;

public class ProviderAuthorizationHandler(
    IOuterApiService outerApiService,
    IAuthorizationValueProvider authorizationValueProvider,
    IOperationPermissionClaimsProvider operationPermissionClaimsProvider) : IAuthorizationHandler
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

        var providerId = authorizationValueProvider.GetProviderId();
        var accountLegalEntityId = authorizationValueProvider.GetAccountLegalEntityId();
        var operation = options.Select(o => o.ToEnum<Operation>()).Single();
        
        if (operationPermissionClaimsProvider.TryGetPermission(accountLegalEntityId, operation, out var hasPermission))
        {
            if (!hasPermission)
            {
                authorizationResult.AddError(new ProviderPermissionNotGranted());
            }

            return authorizationResult;
        }

        hasPermission = await outerApiService.HasPermission(providerId, accountLegalEntityId, operation);

        operationPermissionClaimsProvider.Save(new OperationPermission
        {
            AccountLegalEntityId = accountLegalEntityId,
            Operation = operation,
            HasPermission = hasPermission
        });

        if (!hasPermission)
        {
            authorizationResult.AddError(new ProviderPermissionNotGranted());
        }

        return authorizationResult;
    }
}