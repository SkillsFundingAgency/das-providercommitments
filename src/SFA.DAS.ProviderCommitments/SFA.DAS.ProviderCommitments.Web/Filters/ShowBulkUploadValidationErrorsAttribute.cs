using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling;
using SFA.DAS.ProviderCommitments.Interfaces;

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
            if (!(context.Exception is CommitmentsApiBulkUploadModelException exception)) return;
            var cachedData = _cacheService.SetCache(exception.Errors).Result;
            context.RouteData.Values["action"] = nameof(CohortController.FileUploadValidationErrors);
            context.RouteData.Values["CachedErrorGuid"] = cachedData.ToString();
            context.Result = new RedirectToRouteResult(context.RouteData.Values);
        }
    }
}
