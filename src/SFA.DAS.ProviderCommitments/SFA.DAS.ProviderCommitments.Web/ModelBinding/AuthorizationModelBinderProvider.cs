﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.ModelBinding;

public class AuthorizationModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (!typeof(IAuthorizationContextModel).IsAssignableFrom(context.Metadata.ContainerType) || context.Metadata.IsComplexType)
        {
            return null;
        }

        var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
        var simpleTypeModelBinder = new SimpleTypeModelBinder(context.Metadata.ModelType, loggerFactory);
        var authorizationModelBinder = new AuthorizationModelBinder(simpleTypeModelBinder);

        return authorizationModelBinder;

    }
}