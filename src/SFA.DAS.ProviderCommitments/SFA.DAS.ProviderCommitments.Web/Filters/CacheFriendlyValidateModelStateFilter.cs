using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.Validation.Mvc.Extensions;
using SFA.DAS.Validation.Mvc.Filters;
using SFA.DAS.Validation.Mvc.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Filters;

public class CacheFriendlyValidateModelStateFilter : ActionFilterAttribute
{
    private readonly ICacheStorageService _cacheStorageService;
    private readonly ValidateModelStateFilter _validateModelStateFilter;

    public CacheFriendlyValidateModelStateFilter(ICacheStorageService cacheStorageService, ValidateModelStateFilter validateModelStateFilter)
    {
        _cacheStorageService = cacheStorageService;
        _validateModelStateFilter = validateModelStateFilter;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (filterContext.Filters.Any(x => x.GetType() == typeof(UseCacheForValidationAttribute)))
        {
            AddErrorsFromCache(filterContext);
            PopulateErrorsFromModelState(filterContext);
        }
        else
        {
            _validateModelStateFilter.OnActionExecuting(filterContext);
        }
    }

    private bool TryGetFromCache<T>(ActionExecutingContext filterContext, string key, out T cacheResult)
    {
        cacheResult = default;

        try
        {
            if (!filterContext.HttpContext.Request.Query.TryGetValue(key, out var queryValue))
            {
                return false;
            }

            if (Guid.TryParse(queryValue.ToString(), out var guidValue))
            {
                cacheResult = _cacheStorageService.RetrieveFromCache<T>(guidValue).Result;
            }
            else
            {
                cacheResult = _cacheStorageService.RetrieveFromCache<T>(queryValue.ToString()).Result;
            }
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        if (filterContext.Filters.Any(x => x.GetType() == typeof(UseCacheForValidationAttribute)))
        {
            return;
        }

        _validateModelStateFilter.OnActionExecuted(filterContext);
    }

    private void AddErrorsFromCache(ActionExecutingContext filterContext)
    {
        if (!TryGetFromCache<SerializableModelStateDictionary>(filterContext, "CachedModelStateGuid", out var serializableModelState))
        {
            base.OnActionExecuting(filterContext);
            return;
        }

        var dictionary = serializableModelState?.ToModelState();
        filterContext.ModelState.Merge(dictionary);
        _cacheStorageService.DeleteFromCache(filterContext.HttpContext.Request.Query["CachedModelStateGuid"].ToString());

        if (!TryGetFromCache<List<ErrorDetail>>(filterContext, "CachedErrorGuid", out var errors))
        {
            base.OnActionExecuting(filterContext);
            return;
        }

        if (errors != null && errors.Any())
        {
            var controller = (Controller)filterContext.Controller;
            controller.ModelState.AddModelExceptionErrors(errors);
            _cacheStorageService.DeleteFromCache(filterContext.HttpContext.Request.Query["CachedErrorGuid"].ToString());
        }
    }

    private void PopulateErrorsFromModelState(ActionExecutingContext filterContext)
    {
        if (filterContext.HttpContext.Request.Method != "GET" && !filterContext.ModelState.IsValid)
        {
            var modelStateErrorGuid = Guid.NewGuid();
            _cacheStorageService.SaveToCache(modelStateErrorGuid, filterContext.ModelState.ToSerializable(), 1);
            filterContext.RouteData.Values.Merge(filterContext.HttpContext.Request.Query);
            filterContext.RouteData.Values["CachedModelStateGuid"] = modelStateErrorGuid;
            filterContext.Result = (IActionResult)new RedirectToRouteResult((object)filterContext.RouteData.Values);
        }
    }
}