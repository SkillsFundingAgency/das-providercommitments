using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Provider;

public interface IProviderAuthorizationHandler
{
    Task<bool> CanCreateCohort();
}

public class ProviderAuthorizationHandler(
    ICachedOuterApiService cachedOuterApiService,
    IAuthorizationValueProvider authorizationValueProvider)
    : IProviderAuthorizationHandler
{
    public async Task<bool> CanCreateCohort()
    {
        var providerId = authorizationValueProvider.GetProviderId();
        var accountLegalEntityId = authorizationValueProvider.GetAccountLegalEntityId();

        return await cachedOuterApiService.HasPermission(providerId, accountLegalEntityId, Operation.CreateCohort);
    }
}