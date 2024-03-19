using Microsoft.AspNetCore.Authorization;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Authorization.Commitments;

public class AccessApprenticeshipAuthorizationHandler : AuthorizationHandler<AccessApprenticeshipRequirement>
{
    private readonly ICommitmentsAuthorisationHandler _handler;

    public AccessApprenticeshipAuthorizationHandler(ICommitmentsAuthorisationHandler handler)
    {
        _handler = handler;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessApprenticeshipRequirement requirement)
    {
        if (!await _handler.CanAccessApprenticeship())
        {
            return;
        }

        context.Succeed(requirement);
    }
}