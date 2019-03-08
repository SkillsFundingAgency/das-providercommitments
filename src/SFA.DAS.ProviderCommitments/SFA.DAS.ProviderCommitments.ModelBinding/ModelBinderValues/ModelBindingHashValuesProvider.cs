using Microsoft.AspNetCore.Mvc.Infrastructure;
using SFA.DAS.ProviderCommitments.ModelBinding.Interfaces;

namespace SFA.DAS.ProviderCommitments.ModelBinding.ModelBinderValues
{
    /// <summary>
    ///     Provide access to the current <see cref="IHashingValues"/> which
    ///     will provides access to the "un-hashed" values that can be un-hashed from
    ///     the current request.
    /// </summary>
    public class ModelBindingHashValuesProvider : IHashingContextProvider
    {
        private readonly IActionContextAccessor _contextAccessor;
        private readonly IHashedPropertyModelBinder[] _hashedPropertyModelBinders;

        public ModelBindingHashValuesProvider(
            IActionContextAccessor contextAccessor,
            IHashedPropertyModelBinder[] hashedPropertyModelBinders)
        {
            _contextAccessor = contextAccessor;
            _hashedPropertyModelBinders = hashedPropertyModelBinders;
        }

        public IHashingValues GetAuthorizationContext()
        {
            var authorizationContext = new ModelBindingHashValues();

            foreach (var accountInfoExtractor in _hashedPropertyModelBinders)
            {
                accountInfoExtractor.BindModel(_contextAccessor.ActionContext, authorizationContext);
            }

            return authorizationContext;
        }
    }
}
