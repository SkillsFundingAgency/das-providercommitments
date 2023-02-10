using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.Validation.Mvc.Extensions;
using SFA.DAS.Validation.Mvc.Filters;
using SFA.DAS.Validation.Mvc.ModelBinding;
using System;
using System.Linq;
using SFA.DAS.ProviderCommitments.Web.Filters;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web;

public static class ProviderCommitmentsMvcOptionsExtensions
{
    //public static void AddValidationWithLogging(this MvcOptions mvcOptions, IServiceCollection services)
    //{
    //    mvcOptions.Filters.Add(new DomainExceptionRedirectGetFilterWithLoggingAttribute(services));
    //    mvcOptions.Filters.Add<ValidateModelStateFilter>(int.MaxValue);
    //}

    //public static void AddValidationWithoutTempData(this MvcOptions mvcOptions, IServiceCollection services)
    //{
    //    mvcOptions.Filters.Add(new HandleValidationErrorsAttribute());
    //    mvcOptions.Filters.Add<ValidateModelStateFilter>(int.MaxValue);
    //}

    public static void AddValidation(this MvcOptions mvcOptions)
    {
        mvcOptions.Filters.Add<DomainExceptionRedirectGetFilterAttribute>(int.MaxValue);
        mvcOptions.Filters.Add<ValidateModelStateFilter>(int.MaxValue);
    }
}

public class StoreValidationErrorsInCacheAttribute : ActionFilterAttribute {}

public class DomainExceptionRedirectGetFilterAttribute : ExceptionFilterAttribute
{
    private readonly ICacheStorageService _cacheStorageService;

    public DomainExceptionRedirectGetFilterAttribute(ICacheStorageService cacheStorageService)
    {
        _cacheStorageService = cacheStorageService;
    }
    public override void OnException(ExceptionContext context)
    {
        if (!(context.Exception is CommitmentsApiModelException exception)) return;

        if (context.Filters.Any(x => x.GetType() == typeof(StoreValidationErrorsInCacheAttribute)))
        {
            //cache logic
            var cachedErrorId = Guid.NewGuid();
            _cacheStorageService.SaveToCache(cachedErrorId, exception.Errors, 1);
            //var cachedData = _cacheService.SetCache(exception.Errors, nameof(HandleValidationErrorsAttribute)).Result;

            context.RouteData.Values["CachedErrorGuid"] = cachedErrorId;
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
    }
}

//public class DomainExceptionRedirectGetFilterWithLoggingAttribute : ExceptionFilterAttribute
//{
//    private readonly IServiceCollection _services;

//    public DomainExceptionRedirectGetFilterWithLoggingAttribute(IServiceCollection services)
//    {
//        _services = services;
//    }

//    public override void OnException(ExceptionContext context)
//    {
//        WriteLog(x =>  x.Log(LogLevel.Warning, context.Exception, "FLP-202 OnException Triggered."));
//        if (!(context.Exception is CommitmentsApiModelException exception))
//            return;
//        ITempDataDictionary tempData = context.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>().GetTempData(context.HttpContext);
//        ModelStateDictionary modelState = context.ModelState;
//        modelState.AddModelExceptionErrors(exception);
//        WriteLog(x => x.Log(LogLevel.Warning, $"FLP-202 Exception errors added to model state: {context.ModelState}"));
//        SerializableModelStateDictionary serializable = modelState.ToSerializable();
//        tempData.Set<SerializableModelStateDictionary>(serializable);
//        context.RouteData.Values.Merge(context.HttpContext.Request.Query);
//        WriteLog(x => x.Log(LogLevel.Warning, $"FLP-202 Route Data Constructed: {context.RouteData}"));
//        context.Result = (IActionResult)new RedirectToRouteResult((object)context.RouteData.Values);
//        WriteLog(x => x.Log(LogLevel.Warning, $"FLP-202 Result Constructed: {context.Result}"));
//    }

//    private void WriteLog(Action<ILogger> action)
//    {
//        using var scope = _services.BuildServiceProvider().CreateScope();
//        var logger = scope.ServiceProvider.GetRequiredService <ILogger<DomainExceptionRedirectGetFilterWithLoggingAttribute>>();
//        action(logger);
//    }
//}
