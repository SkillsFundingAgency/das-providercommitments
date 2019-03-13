using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace SFA.DAS.ProviderCommitments.Web.Authorisation
{
    /// <summary>
    ///     If the current route contains a {ProviderId} parameter (case is not sensitive) then this handler
    ///     will block access if the UkPrn value for the current user does not equal the provider id specified in
    ///     the route. 
    /// </summary>
    /// <remarks>
    ///     This only checks route parameters. Query parameters are not inspected.
    /// </remarks>
    public class ProviderHandler : AuthorizationHandler<ProviderRequirement>
    {
        private readonly IActionContextAccessor _actionContextAccessor;

        public ProviderHandler(IActionContextAccessor actionContextAccessor)
        {
            _actionContextAccessor = actionContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProviderRequirement requirement)
        {
            var actionContext = _actionContextAccessor.ActionContext;

            if (actionContext.RouteData.Values.TryGetValue("ProviderId", out var providerIdSpecifiedInRoute))
            {
                var currentUsersProviderId = context.User.Claims.FirstOrDefault(claim => string.Equals(claim.Type,Constants.ClaimTypes.UKPrn, StringComparison.OrdinalIgnoreCase));

                if (currentUsersProviderId == null)
                {
                    context.Fail();
                    return Task.CompletedTask;
                }

                if (currentUsersProviderId.Value != providerIdSpecifiedInRoute.ToString())
                {
                    context.Fail();
                    return Task.CompletedTask;
                }
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}