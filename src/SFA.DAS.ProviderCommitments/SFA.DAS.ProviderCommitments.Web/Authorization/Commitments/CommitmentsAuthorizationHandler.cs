using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Commitments;

public class CommitmentsAuthorisationHandler(
    ICachedOuterApiService cachedOuterApiService,
    IAuthorizationValueProvider authorizationValueProvider)
    : ICommitmentsAuthorisationHandler
{
    public Task<bool> CanAccessCohort()
    {
        var permissionValues = GetPermissionValues();

        return cachedOuterApiService.CanAccessCohort(Party.Provider, permissionValues.PartyId, permissionValues.CohortId);
    }

    public Task<bool> CanAccessApprenticeship()
    {
        var permissionValues = GetPermissionValues();

        return cachedOuterApiService.CanAccessApprenticeship(Party.Provider, permissionValues.PartyId, permissionValues.ApprenticeshipId);
    }

    private (long CohortId, long ApprenticeshipId, long PartyId) GetPermissionValues()
    {
        var cohortId = authorizationValueProvider.GetCohortId();
        var apprenticeshipId = authorizationValueProvider.GetApprenticeshipId();
        var providerId = authorizationValueProvider.GetProviderId();
        
        if (cohortId == 0 && apprenticeshipId == 0 && providerId == 0)
        {
            throw new KeyNotFoundException("At least one key of 'ProviderId', 'CohortId' or 'ApprenticeshipId' should be present in the authorization context");
        }

        return (cohortId, apprenticeshipId, providerId);
    }
}