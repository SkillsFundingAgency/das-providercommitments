using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Provider;

public class CreateCohortAuthorizationHandler(IProviderAuthorizationHandler handler) : AuthorizationHandler<CreateCohortRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CreateCohortRequirement requirement)
    {
        if (!await handler.CanCreateCohort())
        {
            return;
        }

        context.Succeed(requirement); 
    }
}