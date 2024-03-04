using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Authorization;

public static class AuthorizationContextExtensions
{
    public static void AddProviderPermissionValues(this IAuthorizationContext authorizationContext, long accountLegalEntityId, long ukprn)
    {
        authorizationContext.Set(AuthorizationContextKeys.Ukprn, ukprn);
        authorizationContext.Set(AuthorizationContextKeys.AccountLegalEntityId, accountLegalEntityId);
    }

    public static void AddCommitmentPermissionValues(this IAuthorizationContext authorizationContext, long cohortId, Party party, long partyId)
    {
        authorizationContext.Set(AuthorizationContextKeys.CohortId, cohortId);
        authorizationContext.Set(AuthorizationContextKeys.Party, party);
        authorizationContext.Set(AuthorizationContextKeys.PartyId, partyId);
    }
    
    public static void AddApprenticeshipPermissionValues(this IAuthorizationContext authorizationContext, long apprenticeshipId, Party party, long partyId)
    {
        authorizationContext.Set(AuthorizationContextKeys.ApprenticeshipId, apprenticeshipId);
        authorizationContext.Set(AuthorizationContextKeys.Party, party);
        authorizationContext.Set(AuthorizationContextKeys.PartyId, partyId);
    }
    
    public static void AddProviderFeatureValues(this IAuthorizationContext authorizationContext, long ukprn, string userEmail)
    {
        authorizationContext.Set(AuthorizationContextKeys.Ukprn, ukprn);
        authorizationContext.Set(AuthorizationContextKeys.UserEmail, userEmail);
    }
    
    public static (long Ukprn, long AccountLegalEntityId) GetProviderPermissionValues(this IAuthorizationContext authorizationContext)
    {
        return (authorizationContext.Get<long>(AuthorizationContextKeys.Ukprn),
            authorizationContext.Get<long>(AuthorizationContextKeys.AccountLegalEntityId));
    }
    
    public static (long Ukprn, string UserEmail) GetProviderFeatureValues(this IAuthorizationContext authorizationContext)
    {
        return (authorizationContext.Get<long>(AuthorizationContextKeys.Ukprn),
            authorizationContext.Get<string>(AuthorizationContextKeys.UserEmail));
    }
    
    internal static bool TryGetPermissionValues(this IAuthorizationContext authorizationContext, out long cohortId, out long apprenticeshipId, out Party party, out long partyId)
    {
        return (authorizationContext.TryGet(AuthorizationContextKeys.CohortId, out cohortId) | authorizationContext.TryGet(AuthorizationContextKeys.ApprenticeshipId, out apprenticeshipId)) &
               authorizationContext.TryGet(AuthorizationContextKeys.Party, out party) &
               authorizationContext.TryGet(AuthorizationContextKeys.PartyId, out partyId);
    }
}