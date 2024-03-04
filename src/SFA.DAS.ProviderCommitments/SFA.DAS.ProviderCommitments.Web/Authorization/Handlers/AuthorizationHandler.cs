using SFA.DAS.ProviderCommitments.Authorization;
using SFA.DAS.ProviderCommitments.Extensions;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;

public class AuthorizationHandler : IAuthorizationHandler
{
    public string Prefix => "ProviderOperation.";
        
    private readonly IProviderRelationshipsApiClient _providerRelationshipsApiClient;

    public AuthorizationHandler(IProviderRelationshipsApiClient providerRelationshipsApiClient)
    {
        _providerRelationshipsApiClient = providerRelationshipsApiClient;
    }
        
    public async Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, Interfaces.IAuthorizationContext authorizationContext)
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

        var hasPermissionRequest = new HasPermissionRequest
        {
            Ukprn = values.Ukprn,
            AccountLegalEntityId = values.AccountLegalEntityId,
            Operation = operation
        };

        var hasPermission = await _providerRelationshipsApiClient.HasPermission(hasPermissionRequest).ConfigureAwait(false);
                
        if (!hasPermission)
        {
            authorizationResult.AddError(new ProviderPermissionNotGranted());
        }

        return authorizationResult;
    }
}