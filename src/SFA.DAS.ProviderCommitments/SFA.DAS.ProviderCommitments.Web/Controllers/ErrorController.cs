using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.Shared.UI.Attributes;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [AllowAnonymous]
    [HideNavigationBar(hideAccountHeader: false, hideNavigationLinks: true)]
    public class ErrorController : Controller
    {
        [Route("error/{statuscode?}")]
        public IActionResult Error(int? statusCode)
        {
            switch (statusCode)
            {
                case 403:
                case 404:
                case 500:
                    return View(statusCode.ToString());
                default:
                    return View();
            }
        }
    }
}