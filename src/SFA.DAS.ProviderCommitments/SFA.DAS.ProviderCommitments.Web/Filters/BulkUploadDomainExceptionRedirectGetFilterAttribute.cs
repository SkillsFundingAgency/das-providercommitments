using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.Validation.Mvc.Extensions;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Controllers;

namespace SFA.DAS.ProviderCommitments.Web.Filters
{
    public class BulkUploadDomainExceptionRedirectGetFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (!(context.Exception is CommitmentsApiBulkUploadModelException exception)) return;

            var tempDataFactory = context.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
            var tempData = tempDataFactory.GetTempData(context.HttpContext);

            var response = new BulkUploadValidateApiResponse() { BulkUploadValidationErrors = exception.Errors };

            tempData.Put(Constants.BulkUpload.BulkUploadErrors, response);

            context.RouteData.Values["action"] = nameof(CohortController.FileUploadValidationErrors);
                //"FileUploadValidationErrors";

            //context.RouteData.Values.Merge(context.HttpContext.Request.Query);
            context.Result = new RedirectToRouteResult(context.RouteData.Values);
        }
    }
}
