using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Shared.Filters;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.Validation.Mvc.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.Filters;

public class CacheFriendlyCommitmentsValidationFilter : ExceptionFilterAttribute
{
    private readonly ICacheStorageService _cacheStorageService;
    private readonly DomainExceptionRedirectGetFilterAttribute _domainExceptionRedirectGetFilterAttribute;

    public CacheFriendlyCommitmentsValidationFilter(ICacheStorageService cacheStorageService, DomainExceptionRedirectGetFilterAttribute domainExceptionRedirectGetFilterAttribute)
    {
        _cacheStorageService = cacheStorageService;
        _domainExceptionRedirectGetFilterAttribute = domainExceptionRedirectGetFilterAttribute;
    }
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is not CommitmentsApiModelException exception) return;

        if (context.Filters.Any(x => x.GetType() == typeof(UseCacheForValidationAttribute)))
        {
            var cachedErrorId = Guid.NewGuid();
            var modelStateId = Guid.NewGuid();
            _cacheStorageService.SaveToCache(cachedErrorId, exception.Errors, 1);
            _cacheStorageService.SaveToCache(modelStateId, context.ModelState.ToSerializable(), 1);

            context.RouteData.Values["CachedErrorGuid"] = cachedErrorId;
            context.RouteData.Values["CachedModelStateGuid"] = modelStateId;

            context.RouteData.Values.AddQueryValuesToRoute(context.HttpContext.Request.Query);
            context.Result = new RedirectToRouteResult(context.RouteData.Values);
        }
        else
        {
            _domainExceptionRedirectGetFilterAttribute.OnException(context);
        }
        base.OnException(context);
    }
}