using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using SFA.DAS.Authorization;

public class AuthorizationModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (typeof(IAuthorizationContextModel).IsAssignableFrom(context.Metadata.ContainerType))
        {
            return new BinderTypeModelBinder(typeof(AuthorizationModelBinder));
        }
        else
        {
            return null;
        }
    }
}

public class AuthorizationModelBinder : IModelBinder
{
    private readonly IAuthorizationContextProvider _authorizationContextProvider;

    public AuthorizationModelBinder(IAuthorizationContextProvider authorizationContextProvider)
    {
        _authorizationContextProvider = authorizationContextProvider;
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var authorizationContext = _authorizationContextProvider.GetAuthorizationContext();

        if (authorizationContext.TryGet(bindingContext.ModelMetadata.PropertyName, out object value))
        {
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value, value.ToString());
            bindingContext.Result = ModelBindingResult.Success(value);
        }

        return Task.CompletedTask;
    }
}