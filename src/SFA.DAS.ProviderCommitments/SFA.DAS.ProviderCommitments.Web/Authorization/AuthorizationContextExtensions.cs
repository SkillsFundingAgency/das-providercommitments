using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Authorization;

public static class AuthorizationContextExtensions
{
    internal static void AddProviderPermissionValues(this IAuthorizationContext authorizationContext, long accountLegalEntityId, long ukprn)
    {
        authorizationContext.Set(AuthorizationContextKeys.Ukprn, ukprn);
        authorizationContext.Set(AuthorizationContextKeys.AccountLegalEntityId, accountLegalEntityId);
    }

    internal static void AddCommitmentPermissionValues(this IAuthorizationContext authorizationContext, long cohortId, Party party, long partyId)
    {
        authorizationContext.Set(AuthorizationContextKeys.CohortId, cohortId);
        authorizationContext.Set(AuthorizationContextKeys.Party, party);
        authorizationContext.Set(AuthorizationContextKeys.PartyId, partyId);
    }
    
    internal static void AddApprenticeshipPermissionValues(this IAuthorizationContext authorizationContext, long apprenticeshipId, Party party, long partyId)
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
    
    internal static (long Ukprn, long AccountLegalEntityId) GetProviderPermissionValues(this IAuthorizationContext authorizationContext)
    {
        return (authorizationContext.Get<long>(AuthorizationContextKeys.Ukprn),
            authorizationContext.Get<long>(AuthorizationContextKeys.AccountLegalEntityId));
    }
}