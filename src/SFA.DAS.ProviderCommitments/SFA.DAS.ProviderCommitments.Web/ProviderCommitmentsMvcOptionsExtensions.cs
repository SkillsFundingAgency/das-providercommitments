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
using SFA.DAS.CommitmentsV2.Shared.Filters;
using SFA.DAS.ProviderCommitments.Web.Filters;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.Validation.Exceptions;
using SFA.DAS.ProviderCommitments.Infrastructure.CacheStorageService;
using System.Collections.Generic;

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
        mvcOptions.Filters.Add<NewDomainExceptionRedirectGetFilterAttribute>(int.MaxValue);
        mvcOptions.Filters.Add<NewValidateModelStateFilter>(int.MaxValue);
    }
}

public class StoreValidationErrorsInCacheAttribute : ActionFilterAttribute {}

public class NewDomainExceptionRedirectGetFilterAttribute : ExceptionFilterAttribute
{
    private readonly ICacheStorageService _cacheStorageService;

    public NewDomainExceptionRedirectGetFilterAttribute(ICacheStorageService cacheStorageService)
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
        base.OnException(context);
    }
}

public class NewValidateModelStateFilter : ActionFilterAttribute
{
    private static readonly string ModelStateKey = typeof(SerializableModelStateDictionary).FullName;
    private readonly ICacheStorageService _cacheStorageService;

    public NewValidateModelStateFilter(ICacheStorageService cacheStorageService)
    {
        _cacheStorageService = cacheStorageService;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (filterContext.Filters.Any(x => x.GetType() == typeof(StoreValidationErrorsInCacheAttribute)))
        {
            if (!Guid.TryParse(filterContext.HttpContext.Request.Query["CachedErrorGuid"].ToString(), out var cachedErrorId))
            {
                base.OnActionExecuting(filterContext);
                return;
            }


            var errors = _cacheStorageService.RetrieveFromCache<List<ErrorDetail>>(cachedErrorId).Result;
            //var errors = _cacheStorageService.GetFromCache<List<ErrorDetail>>(nameof(HandleValidationErrorsAttribute)).Result;

            if (errors != null && errors.Any())
            {
                var controller = (Controller)filterContext.Controller;
                controller.ModelState.AddModelExceptionErrors(errors);
            }
        }
        else
        {
            if (filterContext.HttpContext.Request.Method == "GET")
            {
                if (!filterContext.ModelState.IsValid)
                {
                    filterContext.Result = (IActionResult)new BadRequestObjectResult(filterContext.ModelState);
                }
                else
                {
                    SerializableModelStateDictionary serializableModelState = filterContext.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>().GetTempData(filterContext.HttpContext).Get<SerializableModelStateDictionary>();
                    ModelStateDictionary dictionary = serializableModelState != null ? serializableModelState.ToModelState() : (ModelStateDictionary)null;
                    filterContext.ModelState.Merge(dictionary);
                }
            }
            else
            {
                if (filterContext.ModelState.IsValid)
                    return;
                filterContext.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>().GetTempData(filterContext.HttpContext).Set<SerializableModelStateDictionary>(filterContext.ModelState.ToSerializable());
                filterContext.RouteData.Values.Merge(filterContext.HttpContext.Request.Query);
                filterContext.Result = (IActionResult)new RedirectToRouteResult((object)filterContext.RouteData.Values);
            }
        }
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        if (!(filterContext.HttpContext.Request.Method != "GET"))
            return;
        if (filterContext.Exception is ValidationException exception)
        {
            filterContext.ModelState.AddModelError(exception);
            filterContext.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>().GetTempData(filterContext.HttpContext).Set<SerializableModelStateDictionary>(filterContext.ModelState.ToSerializable());
            filterContext.RouteData.Values.Merge(filterContext.HttpContext.Request.Query);
            filterContext.Result = (IActionResult)new RedirectToRouteResult((object)filterContext.RouteData.Values);
            filterContext.ExceptionHandled = true;
        }
        else
        {
            if (filterContext.ModelState.IsValid)
                return;
            filterContext.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>().GetTempData(filterContext.HttpContext).Set<SerializableModelStateDictionary>(filterContext.ModelState.ToSerializable());
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
