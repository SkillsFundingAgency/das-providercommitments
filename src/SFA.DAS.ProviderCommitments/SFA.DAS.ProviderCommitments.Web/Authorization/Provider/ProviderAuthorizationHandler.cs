using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Provider;

public interface IProviderAuthorizationHandler
{
    Task<bool> CanCreateCohort();
}

public class ProviderAuthorizationHandler(
    IOuterApiService outerApiService,
    IAuthorizationValueProvider authorizationValueProvider,
    IOperationPermissionClaimsProvider operationPermissionClaimsProvider)
    : IProviderAuthorizationHandler
{
    public async Task<bool> CanCreateCohort()
    {
        var providerId = authorizationValueProvider.GetProviderId();
        var accountLegalEntityId = authorizationValueProvider.GetAccountLegalEntityId();

        const Operation operation = Operation.CreateCohort;

        if (operationPermissionClaimsProvider.TryGetPermission(accountLegalEntityId, operation, out var hasRelationshipWithPermission))
        {
            return hasRelationshipWithPermission;
        }

        hasRelationshipWithPermission = await outerApiService.HasRelationshipWithPermission(providerId, operation);

        operationPermissionClaimsProvider.Save(new OperationPermission
        {
            AccountLegalEntityId = accountLegalEntityId,
            Operation = operation,
            HasPermission = hasRelationshipWithPermission,
        });

        return hasRelationshipWithPermission;
    }
}