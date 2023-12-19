using Microsoft.AspNetCore.Authorization;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.Authorization;

public class MinimumServiceClaimRequirementHandler : AuthorizationHandler<MinimumServiceClaimRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumServiceClaimRequirement requirement)
    {
        if(context.User.HasPermission(requirement.MinimumServiceClaim)) context.Succeed(requirement);
        else context.Fail();

        return Task.CompletedTask;
    }
}