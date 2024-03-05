using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization.Context;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Provider;

public interface IProviderAuthorizationHandler
{
    Task<bool> CanCreateCohort();
}

public class ProviderAuthorizationHandler(IAuthorizationContext authorizationContext, IOuterApiService outerApiService)
    : IProviderAuthorizationHandler
{
    public Task<bool> CanCreateCohort()
    {
        var values = GetProviderPermissionValues();
        
        return outerApiService.GetHasPermission(values.Ukprn, values.AccountLegalEntityId, Operation.CreateCohort.ToString());
    }
    
    private (long Ukprn, long AccountLegalEntityId) GetProviderPermissionValues()
    {
        return (authorizationContext.Get<long>(AuthorizationContextKeys.Ukprn),
            authorizationContext.Get<long>(AuthorizationContextKeys.AccountLegalEntityId));
    }
}