using SFA.DAS.Authorization;
using SFA.DAS.Authorization.CommitmentPermissions;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Authorization
{
    public static class AuthorizationContextExtensions
    {
        public static void AddCommitmentPermissionValues(this IAuthorizationContext authorizationContext, long? cohortId, Party? partyType, long partyId)
        {
            authorizationContext.Set(AuthorizationContextKey.CohortId, cohortId);
            authorizationContext.Set(AuthorizationContextKey.Party, partyType);
            authorizationContext.Set(AuthorizationContextKey.PartyId, partyId);
        }
    }
}