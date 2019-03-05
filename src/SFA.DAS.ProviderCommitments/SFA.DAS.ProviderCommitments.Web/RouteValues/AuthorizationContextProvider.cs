using Microsoft.AspNetCore.Mvc.Infrastructure;
using SFA.DAS.Authorization;

namespace SFA.DAS.ProviderCommitments.Web.RouteValues
{
    /// <summary>
    ///     For each model property that 
    /// </summary>
    public class AuthorizationContextProvider : IAuthorizationContextProvider
    {
        private readonly IActionContextAccessor _contextAccessor;
        private readonly IHashedPropertyModelBinder[] _hashedPropertyModelBinders;

        public AuthorizationContextProvider(
            IActionContextAccessor contextAccessor,
            IHashedPropertyModelBinder[] hashedPropertyModelBinders)
        {
            _contextAccessor = contextAccessor;
            _hashedPropertyModelBinders = hashedPropertyModelBinders;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            var authorizationContext = new AuthorizationContext();

            foreach (var accountInfoExtractor in _hashedPropertyModelBinders)
            {
                accountInfoExtractor.BindModel(_contextAccessor.ActionContext, authorizationContext);
            }

            return authorizationContext;
        }
    }
}
