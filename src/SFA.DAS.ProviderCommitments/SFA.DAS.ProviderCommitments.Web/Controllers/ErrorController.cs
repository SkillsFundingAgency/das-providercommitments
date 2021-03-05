using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.Shared.UI.Attributes;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [HideNavigationBar(hideAccountHeader: false, hideNavigationLinks: true)]
    public class ErrorController : Controller
    {
        [Route("error")]
        public IActionResult Error(int? statusCode)
        {
            switch (statusCode)
            {
                case 403:
                case 404:
                    return View(statusCode.ToString());
                default:
                    return View();
            }
        }
    }
}