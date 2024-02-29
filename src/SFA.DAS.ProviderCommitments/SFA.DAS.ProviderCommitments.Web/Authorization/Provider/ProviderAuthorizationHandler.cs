using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Provider;

public interface IProviderAuthorizationHandler
{
    Task<bool> CanCreateCohort();
}

public class ProviderAuthorizationHandler : IProviderAuthorizationHandler
{
    private readonly ProviderRelationshipsApiClient _providerRelationshipsApiClient;
    private readonly IAuthorizationContext _authorizationContext;

    public ProviderAuthorizationHandler(ProviderRelationshipsApiClient providerRelationshipsApiClient, IAuthorizationContext authorizationContext)
    {
        _providerRelationshipsApiClient = providerRelationshipsApiClient;
        _authorizationContext = authorizationContext;
    }
   
    public Task<bool> CanCreateCohort()
    {
        var values = GetProviderPermissionValues();

        var hasPermissionRequest = new HasPermissionRequest
        {
            Ukprn = values.Ukprn,
            AccountLegalEntityId = values.AccountLegalEntityId,
            Operation = Operation.CreateCohort
        };

        return _providerRelationshipsApiClient.HasPermission(hasPermissionRequest);
    }
    
    private (long Ukprn, long AccountLegalEntityId) GetProviderPermissionValues()
    {
        return (_authorizationContext.Get<long>(AuthorizationContextKeys.Ukprn),
            _authorizationContext.Get<long>(AuthorizationContextKeys.AccountLegalEntityId));
    }
}