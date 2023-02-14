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

public class CacheFriendlyValidateModelStateFilter : ValidateModelStateFilter
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

            if (errors != null && errors.Any())
            {
                var controller = (Controller)filterContext.Controller;
                controller.ModelState.AddModelExceptionErrors(errors);
            }
        }
        else
        {
            _validateModelStateFilter.OnActionExecuting(filterContext);
        }
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        if (filterContext.Filters.Any(x => x.GetType() == typeof(UseCacheForValidationAttribute)))
            return;

        _validateModelStateFilter.OnActionExecuted(filterContext);
    }
}