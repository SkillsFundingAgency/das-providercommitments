using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.Authorization
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

            if (!HasServiceAuthorization(context))
            {
                context.Fail();
            }
            else if (actionContext.RouteData.Values.TryGetValue("ProviderId", out var providerIdSpecifiedInRoute))
            {
                var currentUsersProviderId = context.User.Claims.FirstOrDefault(claim => claim.Type == ProviderClaims.Ukprn);

                if (currentUsersProviderId == null)
                {
                    context.Fail();
                }
                else if (currentUsersProviderId.Value != providerIdSpecifiedInRoute.ToString())
                {
                    context.Fail();
                }
                else
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }

        private static bool HasServiceAuthorization(AuthorizationHandlerContext context)
        {
            var serviceClaims = context.User.FindAll(c => c.Type.Equals(ProviderClaims.Service));
            return serviceClaims.Any(claim => claim.Value.IsServiceClaim());
        }
    }
}