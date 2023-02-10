using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.Validation.Mvc.Extensions;
using SFA.DAS.CommitmentsV2.Shared.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.Filters
{
    public class HandleBulkUploadValidationErrorsAttribute : ExceptionFilterAttribute
    {
        private ICacheService _cacheService;

        public HandleBulkUploadValidationErrorsAttribute(ICacheService cacheService) 
        {
            Order = int.MaxValue;
            _cacheService = cacheService;
        }

        public override void OnException(ExceptionContext context)
        {
            // This was using TempData before. Reading from TempData failed as size of the response increased.
            // Now instead of using TempData using BlobStorage.
            if (!(context.Exception is CommitmentsApiBulkUploadModelException exception)) return;
            var cachedData = _cacheService.SetCache(exception.Errors, nameof(HandleBulkUploadValidationErrorsAttribute)).Result;
            context.RouteData.Values["action"] = nameof(CohortController.FileUploadValidationErrors);
            context.RouteData.Values["CachedErrorGuid"] = cachedData.ToString();
            context.Result = new RedirectToRouteResult(context.RouteData.Values);
        }
    }

    public class HandleValidationErrorsAttribute : ExceptionFilterAttribute
    {
        private readonly ICacheStorageService _cacheStorageService;

        public HandleValidationErrorsAttribute(ICacheStorageService cacheStorageService)
        {
            _cacheStorageService = cacheStorageService;
            Order = 2;
        }

        public override void OnException(ExceptionContext context)
        {
            // This was using TempData before. Reading from TempData failed as size of the response increased.
            // Now instead of using TempData using BlobStorage.
            if (!(context.Exception is CommitmentsApiModelException exception)) return;
            var cachedErrorId = Guid.NewGuid();
            _cacheStorageService.SaveToCache(cachedErrorId, exception.Errors, 1);
            //var cachedData = _cacheService.SetCache(exception.Errors, nameof(HandleValidationErrorsAttribute)).Result;
            
            context.RouteData.Values["CachedErrorGuid"] = cachedErrorId;
            //context.HttpContext.Items.Add("CachedErrorGuid", cachedErrorId);

            context.RouteData.Values.Merge(context.HttpContext.Request.Query);
            context.Result = new RedirectToRouteResult(context.RouteData.Values);
        }
    }

    public class PopulateValidationErrorsAttribute : ActionFilterAttribute
    {
        private readonly ICacheStorageService _cacheStorageService;

        public PopulateValidationErrorsAttribute(ICacheStorageService cacheStorageService)
        {
            _cacheStorageService = cacheStorageService;
            Order = int.MaxValue;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!Guid.TryParse(context.HttpContext.Request.Query["CachedErrorGuid"].ToString(), out var cachedErrorId))
            {
                base.OnActionExecuting(context);
                return;
            }
                

            var errors = _cacheStorageService.RetrieveFromCache<List<ErrorDetail>>(cachedErrorId).Result;
            //var errors = _cacheStorageService.GetFromCache<List<ErrorDetail>>(nameof(HandleValidationErrorsAttribute)).Result;

            if (errors != null && errors.Any())
            {
                var controller = (Controller)context.Controller;
                controller.ModelState.AddModelExceptionErrors(errors);
            }

            base.OnActionExecuting(context);
        }
    }

    public static class ModelStateExtensions
    {
        public static void AddModelExceptionErrors(
            this ModelStateDictionary modelState,
            List<ErrorDetail> errors,
            Func<string, string> fieldNameMapper = null)
        {
            if (errors == null)
                return;
            foreach (ErrorDetail error in errors)
            {
                string key = fieldNameMapper == null ? error.Field : fieldNameMapper(error.Field);
                modelState.AddModelError(key, error.Message);
            }
        }
    }
}
