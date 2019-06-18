using Microsoft.AspNetCore.Authorization;
using SFA.DAS.ProviderCommitments.Web.Authorization;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class AuthorizationPolicyBuilderExtensions
    {
        public static AuthorizationPolicyBuilder RequireProviderInRouteMatchesProviderInClaims(this AuthorizationPolicyBuilder builder)
        {
            builder.Requirements.Add(new ProviderRequirement());
            return builder;
        }
    }
}