using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.Validation.Exceptions;
using SFA.DAS.Validation.Mvc.Extensions;
using SFA.DAS.Validation.Mvc.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Filters;

public class NewValidateModelStateFilter : ActionFilterAttribute //todo move this
{
    private static readonly string ModelStateKey = typeof(SerializableModelStateDictionary).FullName;
    private readonly ICacheStorageService _cacheStorageService;

    public NewValidateModelStateFilter(ICacheStorageService cacheStorageService)
    {
        _cacheStorageService = cacheStorageService;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (filterContext.Filters.Any(x => x.GetType() == typeof(UseCacheForValidationAttribute)))
        {
            if (!Guid.TryParse(filterContext.HttpContext.Request.Query["CachedModelStateGuid"].ToString(), out var modelStateId))
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            var serializableModelState = _cacheStorageService.RetrieveFromCache<SerializableModelStateDictionary>(modelStateId).Result;
            var dictionary = serializableModelState?.ToModelState();
            filterContext.ModelState.Merge(dictionary);

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
                    filterContext.Result = new BadRequestObjectResult(filterContext.ModelState);
                }
                else
                {
                    SerializableModelStateDictionary serializableModelState = filterContext.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>().GetTempData(filterContext.HttpContext).Get<SerializableModelStateDictionary>();
                    ModelStateDictionary dictionary = serializableModelState != null ? serializableModelState.ToModelState() : null;
                    filterContext.ModelState.Merge(dictionary);
                }
            }
            else
            {
                if (filterContext.ModelState.IsValid)
                    return;
                filterContext.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>().GetTempData(filterContext.HttpContext).Set(filterContext.ModelState.ToSerializable());
                filterContext.RouteData.Values.Merge(filterContext.HttpContext.Request.Query);
                filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);
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
            filterContext.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>().GetTempData(filterContext.HttpContext).Set(filterContext.ModelState.ToSerializable());
            filterContext.RouteData.Values.Merge(filterContext.HttpContext.Request.Query);
            filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);
            filterContext.ExceptionHandled = true;
        }
        else
        {
            if (filterContext.ModelState.IsValid)
                return;
            filterContext.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>().GetTempData(filterContext.HttpContext).Set(filterContext.ModelState.ToSerializable());
        }
    }
}