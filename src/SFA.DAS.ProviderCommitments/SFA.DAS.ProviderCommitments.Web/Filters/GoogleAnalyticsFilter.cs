using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.ProviderCommitments.Web.Authentication;

namespace SFA.DAS.ProviderCommitments.Web.Filters
{
    public class GoogleAnalyticsFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller))
            {
                return;
            }

            controller.ViewBag.GaData = PopulateGaData(context);

            base.OnActionExecuting(context);
        }

        private GaData PopulateGaData(ActionExecutingContext context)
        {
            var ukPrn = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(ProviderClaims.Ukprn))?.Value;

            return new GaData
            {
                UkPrn = ukPrn
            };
        }
    }
}