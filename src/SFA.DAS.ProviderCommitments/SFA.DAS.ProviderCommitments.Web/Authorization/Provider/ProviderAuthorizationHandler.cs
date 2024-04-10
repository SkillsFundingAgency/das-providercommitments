using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Provider;

public interface IProviderAuthorizationHandler
{
    Task<bool> CanCreateCohort();
}

public class ProviderAuthorizationHandler(
    IOuterApiService outerApiService,
    IAuthorizationValueProvider authorizationValueProvider,
    IOperationPermissionsProvider operationPermissionsProvider)
    : IProviderAuthorizationHandler
{
    public async Task<bool> CanCreateCohort()
    {
        var providerId = authorizationValueProvider.GetProviderId();
        var accountLegalEntityId = authorizationValueProvider.GetAccountLegalEntityId();

        const Operation operation = Operation.CreateCohort;

        if (operationPermissionsProvider.TryGetPermission(accountLegalEntityId, operation, out var hasPermission))
        {
            return hasPermission;
        }

        hasPermission = await outerApiService.HasPermission(providerId, accountLegalEntityId, operation);
        
        operationPermissionsProvider.Save(new OperationPermission
        {
            AccountLegalEntityId = accountLegalEntityId,
            Operation = operation,
            HasPermission = hasPermission,
        });

        return hasPermission;
    }
}