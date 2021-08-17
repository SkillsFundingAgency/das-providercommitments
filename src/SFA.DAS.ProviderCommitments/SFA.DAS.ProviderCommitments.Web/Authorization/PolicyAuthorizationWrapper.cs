using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Authorization
{
    public class PolicyAuthorizationWrapper : IPolicyAuthorizationWrapper
    {
        private IAuthorizationService _authorizationService;

        public PolicyAuthorizationWrapper(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task<bool> IsAuthorized(ClaimsPrincipal user, string policyName)
        {
            var result = await _authorizationService.AuthorizeAsync(user, policyName);
            return result.Succeeded;
        }
    }

    public interface IPolicyAuthorizationWrapper
    {
        Task<bool> IsAuthorized(ClaimsPrincipal user, string policyName);
    }
}
