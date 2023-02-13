using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.Validation.Mvc.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.Filters;

public class CommitmentsApiModelExceptionHandleFilter : ExceptionFilterAttribute
{
    private readonly ICacheStorageService _cacheStorageService;

    public CommitmentsApiModelExceptionHandleFilter(ICacheStorageService cacheStorageService)
    {
        _cacheStorageService = cacheStorageService;
    }
    public override void OnException(ExceptionContext context)
    {
        if (!(context.Exception is CommitmentsApiModelException exception)) return;

        if (context.Filters.Any(x => x.GetType() == typeof(UseCacheForValidationAttribute)))
        {
            //cache logic
            var cachedErrorId = Guid.NewGuid();
            var modelStateId = Guid.NewGuid();
            _cacheStorageService.SaveToCache(cachedErrorId, exception.Errors, 1);
            _cacheStorageService.SaveToCache(modelStateId, context.ModelState.ToSerializable(), 1);
            //var cachedData = _cacheService.SetCache(exception.Errors, nameof(HandleValidationErrorsAttribute)).Result;

            context.RouteData.Values["CachedErrorGuid"] = cachedErrorId;
            context.RouteData.Values["CachedModelStateGuid"] = modelStateId;
            //context.HttpContext.Items.Add("CachedErrorGuid", cachedErrorId);

            context.RouteData.Values.Merge(context.HttpContext.Request.Query);
            context.Result = new RedirectToRouteResult(context.RouteData.Values);
        }
        else
        {
            //existing temp data logic
            var tempDataFactory = context.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
            var tempData = tempDataFactory.GetTempData(context.HttpContext);

            var modelState = context.ModelState;
            modelState.AddModelExceptionErrors(exception);
            var serializableModelState = modelState.ToSerializable();

            tempData.Set(serializableModelState);

            context.RouteData.Values.Merge(context.HttpContext.Request.Query);
            context.Result = new RedirectToRouteResult(context.RouteData.Values);
        }
        base.OnException(context);
    }
}