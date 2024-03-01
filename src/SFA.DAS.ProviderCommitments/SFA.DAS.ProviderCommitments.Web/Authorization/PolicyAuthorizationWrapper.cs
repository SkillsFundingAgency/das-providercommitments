using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.ProviderCommitments.Web.Authorization;

public interface IPolicyAuthorizationWrapper
{
    Task<bool> IsAuthorized(ClaimsPrincipal user, string policyName);
}

public class PolicyAuthorizationWrapper : IPolicyAuthorizationWrapper
{
    private readonly Microsoft.AspNetCore.Authorization.IAuthorizationService _authorizationService;

    public PolicyAuthorizationWrapper(Microsoft.AspNetCore.Authorization.IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task<bool> IsAuthorized(ClaimsPrincipal user, string policyName)
    {
        var result = await _authorizationService.AuthorizeAsync(user, policyName);
        return result.Succeeded;
    }
}
