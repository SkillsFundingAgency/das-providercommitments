using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling;

namespace SFA.DAS.ProviderCommitments.Web.Filters
{
    public class ShowBulkUploadValidationErrorsAttribute : ExceptionFilterAttribute
    {
        public ShowBulkUploadValidationErrorsAttribute() 
        {
            Order = int.MaxValue;
        }

        public override void OnException(ExceptionContext context)
        {
            if (!(context.Exception is CommitmentsApiBulkUploadModelException exception)) return;

            var tempDataFactory = context.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
            var tempData = tempDataFactory.GetTempData(context.HttpContext);
            tempData.Put(Constants.BulkUpload.BulkUploadErrors, exception.Errors);
            context.RouteData.Values["action"] = nameof(CohortController.FileUploadValidationErrors);
            context.Result = new RedirectToRouteResult(context.RouteData.Values);
        }
    }
}
