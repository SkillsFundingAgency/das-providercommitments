using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.Authorization;

namespace SFA.DAS.ProviderCommitments.Web.RouteValues
{
    public class UnhashingModelBinder : IModelBinder
    {
        private readonly IAuthorizationContextProvider _authorizationContextProvider;

        public UnhashingModelBinder(IAuthorizationContextProvider authorizationContextProvider)
        {
            _authorizationContextProvider = authorizationContextProvider;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var authorizationContext = _authorizationContextProvider.GetAuthorizationContext();

            var model = Activator.CreateInstance(bindingContext.ModelType);

            foreach (var property in bindingContext.ModelType.GetProperties())
            {
                if (authorizationContext.TryGet(property.Name, out object value))
                {
                    property.SetValue(model, value);
                }
            }

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}