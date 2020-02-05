using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("v2/{providerId}/apprentices/manage")]
    public class ManageApprenticesController : Controller
    {
        [Route("all")]
        public IActionResult Index(long providerId)
        {
            var model = new ManageApprenticesViewModel {ProviderId = providerId};
            return View(model);
        }
    }
}