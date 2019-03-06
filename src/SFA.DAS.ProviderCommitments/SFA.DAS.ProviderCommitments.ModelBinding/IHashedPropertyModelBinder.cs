using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderCommitments.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.RouteValues
{
    /// <summary>
    ///     Used by <see cref="AuthorizationContextProvider"/> to set model properties
    ///     from route data and query params on the incoming request.
    /// </summary>
    public interface IHashedPropertyModelBinder
    {
        void BindModel(ActionContext context, IAuthorizationContext authorizationContext);
    }
}