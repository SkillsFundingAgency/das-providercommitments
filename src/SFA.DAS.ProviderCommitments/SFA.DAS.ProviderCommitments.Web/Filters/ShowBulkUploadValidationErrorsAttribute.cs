using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Filters
{
    public class HandleBulkUploadValidationErrorsAttribute : ExceptionFilterAttribute
    {
        private readonly ICacheService _cacheService;

        public HandleBulkUploadValidationErrorsAttribute(ICacheService cacheService) 
        {
            Order = int.MaxValue;
            _cacheService = cacheService;
        }

        public override void OnException(ExceptionContext context)
        {
            // This was using TempData before. Reading from TempData failed as size of the response increased.
            // Now instead of using TempData using BlobStorage.
            var bulkUploadException = context.Exception as CommitmentsApiBulkUploadModelException;
            if (bulkUploadException == null && context.Exception is CommitmentsApiModelException modelException)
            {
                bulkUploadException = new CommitmentsApiBulkUploadModelException(
                    BulkUploadDomainExceptionMapper.ToBulkUploadValidationErrors(modelException));
            }

            if (bulkUploadException == null)
            {
                return;
            }
            
            var cachedData = _cacheService.SetCache(bulkUploadException.Errors, nameof(HandleBulkUploadValidationErrorsAttribute)).Result;
            context.RouteData.Values["action"] = nameof(CohortController.FileUploadValidationErrors);
            context.RouteData.Values["CachedErrorGuid"] = cachedData.ToString();
            context.Result = new RedirectToRouteResult(context.RouteData.Values);
        }
    }
}
