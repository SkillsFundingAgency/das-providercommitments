using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using SFA.DAS.DfESignIn.Auth.Extensions;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Commitments;

public class CommitmentsAuthorisationHandler(
    IHttpContextAccessor httpContextAccessor,
    IOuterApiService outerApiService,
    IAuthorizationValueProvider authorizationValueProvider)
    : ICommitmentsAuthorisationHandler
{
    public async Task<bool> CanAccessCohort()
    {
        var permissionValues = GetPermissionValues();
        
        return await CheckForAccess(
            ProviderClaims.AccessibleCohorts, 
            permissionValues.CohortId, 
            () => outerApiService.CanAccessCohort(permissionValues.PartyId, permissionValues.CohortId));
    }

    public async Task<bool> CanAccessApprenticeship()
    {
        var permissionValues = GetPermissionValues();

        return await CheckForAccess(
            ProviderClaims.AccessibleApprenticeships, 
            permissionValues.ApprenticeshipId, 
            () => outerApiService.CanAccessApprenticeship(permissionValues.PartyId, permissionValues.ApprenticeshipId));
    }

    private async Task<bool> CheckForAccess(string claimType, long entityId, Func<Task<bool>> accessChecker)
    {
        var user = httpContextAccessor.HttpContext?.User;

        var accessibleEntities = JsonConvert.DeserializeObject<Dictionary<long, bool>>(user.GetClaimValue(claimType));

        if (AccessibleEntityExistsOnClaims(accessibleEntities, entityId))
        {
            return accessibleEntities[entityId];
        }

        var canAccess = await accessChecker();

        AddResultToAccessibleEntities(accessibleEntities, entityId, canAccess);

        if (!user.HasClaim(x => x.Type.Equals(claimType)))
        {
            AddClaim(user, claimType, entityId, canAccess);
        }
        else
        {
            UpdateClaim(user, claimType, accessibleEntities);
        }

        return canAccess;
    }

    private static void AddResultToAccessibleEntities(Dictionary<long, bool> accessibleCohorts, long cohortId, bool canAccessCohort)
    {
        if (accessibleCohorts == null)
        {
            accessibleCohorts = new Dictionary<long, bool>();
        }

        accessibleCohorts.Add(cohortId, canAccessCohort);
    }

    private static void AddClaim(ClaimsPrincipal user, string claimType, long id, bool result)
    {
        user.Identities
            .First()
            .AddClaim(new Claim(claimType, JsonConvert.SerializeObject(new Dictionary<long, bool> { { id, result } }), JsonClaimValueTypes.Json));
    }

    private static void UpdateClaim(ClaimsPrincipal user, string claimType, IReadOnlyDictionary<long, bool> values)
    {
        var claimsIdentity = user.Identities.First();

        claimsIdentity.RemoveClaim(claimsIdentity.FindFirst(x => x.Type.Equals(claimType)));

        claimsIdentity.AddClaim(new Claim(claimType, JsonConvert.SerializeObject(values), JsonClaimValueTypes.Json));
    }

    private static bool AccessibleEntityExistsOnClaims(IReadOnlyDictionary<long, bool> accessibleEntities, long entityId)
    {
        return accessibleEntities != null && accessibleEntities.Any() && accessibleEntities.ContainsKey(entityId);
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