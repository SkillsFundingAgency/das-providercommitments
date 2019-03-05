using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using SFA.DAS.Authorization;

namespace SFA.DAS.ProviderCommitments.Web.RouteValues
{
    /// <summary>
    ///     Returns a model provider that can provide un-hashed values to a model.
    /// </summary>
    public class UnhashingModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (typeof(IAuthorizationContextModel).IsAssignableFrom(context.Metadata.ModelType))
            {
                return new BinderTypeModelBinder(typeof(UnhashingModelBinder));
            }
            else
            {
                return null;
            }
        }
    }
}